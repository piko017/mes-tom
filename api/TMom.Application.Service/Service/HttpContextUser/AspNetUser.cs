using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TMom.Domain.Model;
using TMom.Domain.Model.Entity;
using TMom.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static TMom.Domain.Model.GlobalVars;

namespace TMom.Application.Service
{
    /// <summary>
    /// 处理token中的用户信息
    /// </summary>
    public class AspNetUser : IUser
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger<AspNetUser> _logger;

        public AspNetUser(IHttpContextAccessor accessor, ILogger<AspNetUser> logger)
        {
            _accessor = accessor;
            _logger = logger;
        }

        /// <summary>
        /// 当前登录用户账号
        /// </summary>
        public string Name => GetName();

        private string GetName()
        {
            if (IsAuthenticated() && _accessor.HttpContext.User.Identity.Name.IsNotEmptyOrNull())
            {
                return _accessor.HttpContext.User.Identity.Name;
            }
            else
            {
                if (!string.IsNullOrEmpty(GetToken()))
                {
                    var getNameType = Permissions.IsUseIds4 ? "name" : "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
                    return GetUserInfoFromToken(getNameType).FirstOrDefault().ObjToString();
                }
            }

            return "";
        }

        /// <summary>
        /// 当前登录用户Id
        /// </summary>
        public int Id => GetUserId();

        private int GetUserId()
        {
            return GetClaimValueByType("jti").FirstOrDefault()?.ObjToInt() ?? 0;
        }

        /// <summary>
        /// 当前登录用户工厂Id
        /// <para>内部用到了异步方法, 不要直接用在参数中, eg: where(x => x.FactoryId == _user.FactoryId)中, 会影响查询效率</para>
        /// <para>正确方式: 先赋值到一个变量再用到where中</para>
        /// </summary>
        public int FactoryId => GetSafeFactoryId();

        /// <summary>
        /// 当前登录用户工厂编码
        /// </summary>
        public string FactoryCode => GetSafeFactoryCode();

        /// <summary>
        /// 当前登录用户工厂名称
        /// <para>内部用到了异步方法, 不要直接用在参数中, eg: where(x => x.FactoryName == _user.FactoryName)中, 会影响查询效率</para>
        /// <para>正确方式: 先赋值到一个变量再用到where中</para>
        /// </summary>
        public string FactoryName => GetFactory().Name;

        private SysFactory GetFactory()
        {
            int factoryId = FactoryId;
            var list = CommonCache.GetAllFactoryWithCache().Result;
            var model = list.FirstOrDefault(x => x.Id == factoryId) ?? new SysFactory();
            return model;
        }

        /// <summary>
        /// 安全获取工厂代码
        /// </summary>
        /// <returns></returns>
        private string GetSafeFactoryCode()
        {
            var factoryCode = GetClaimValueByType(ClaimTypes.PostalCode).FirstOrDefault()?.ObjToString();
            if (!string.IsNullOrWhiteSpace(factoryCode)) return factoryCode;
            // 防止某些场景下确实不需要登陆，但是又需要知道是哪个工厂的数据, 主要是缓存中用到了当前操作的工厂, 从header中获取
            factoryCode = DashboardHelper.GetFactoryCodeByHeader(_accessor.HttpContext);
            return factoryCode ?? "";
        }

        /// <summary>
        /// 安全获取工厂Id
        /// </summary>
        /// <returns></returns>
        private int GetSafeFactoryId()
        {
            var factoryId = GetClaimValueByType(ClaimTypes.GroupSid).FirstOrDefault()?.ObjToInt();
            if (factoryId.HasValue) return factoryId.Value;
            // 防止某些场景下确实不需要登陆，但是又需要知道是哪个工厂的数据, 主要是缓存中用到了当前操作的工厂, 从header中获取
            var factoryCode = DashboardHelper.GetFactoryCodeByHeader(_accessor.HttpContext);
            if (string.IsNullOrWhiteSpace(factoryCode)) return 0;
            var list = CommonCache.GetAllFactoryWithCache().Result;
            factoryId = list.Where(x => x.Code == factoryCode).FirstOrDefault()?.Id ?? 0;
            return factoryId.Value;
        }

        public bool IsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        /// <summary>
        /// 获取Token信息, 请求头中格式必须是："Bearer " + token，否则出错
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            string tokenStr = _accessor.HttpContext?.Request?.Headers["Authorization"].ObjToString();
            if (tokenStr.IsNotEmptyOrNull() && !tokenStr.StartsWith("Bearer "))
            {
                throw new Exception("请求头token格式有误, 必须是：'Bearer ' + token");
            }
            string token = tokenStr.Replace("Bearer ", "");
            return token;
        }

        /// <summary>
        /// 根据Token获取登录用户信息
        /// </summary>
        /// <param name="ClaimType"></param>
        /// <returns></returns>
        public List<string> GetUserInfoFromToken(string ClaimType = "jti")
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var token = "";

            token = GetToken();
            // token校验
            if (token.IsNotEmptyOrNull() && jwtHandler.CanReadToken(token))
            {
                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(token);

                return (from item in jwtToken.Claims
                        where item.Type == ClaimType
                        select item.Value).ToList();
            }

            return new List<string>() { };
        }

        public IEnumerable<Claim>? GetClaimsIdentity()
        {
            return _accessor.HttpContext?.User.Claims;
        }

        public List<string> GetClaimValueByType(string ClaimType)
        {
            if (GetClaimsIdentity() == null) return new List<string> { };
            return (from item in GetClaimsIdentity()
                    where item.Type == ClaimType
                    select item.Value).ToList();
        }
    }
}