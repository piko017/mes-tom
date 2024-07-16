using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMom.Application;
using TMom.Application.Dto;
using TMom.Application.Dto.Sys;
using TMom.Application.Service;
using TMom.Application.Service.IService;
using TMom.Domain.Model;
using TMom.Domain.Model.Entity;
using TMom.Infrastructure.Common;
using TMom.Infrastructure.Helper;
using TMom.Infrastructure.Repository;
using SqlSugar;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static TMom.Domain.Model.GlobalVars;

namespace TMom.Api.Controllers.Home
{
    /// <summary>
    /// 登录
    /// </summary>
    [Produces("application/json")]
    [Route("api/Login")]
    [AllowAnonymous]
    public class LoginController : BaseApiController<SysUser, int>
    {
        private readonly ISysUserService _sysUserService;
        private readonly ISysUserRoleService _sysUserRoleServic;
        private readonly ISysRoleService _sysRoleService;
        private readonly PermissionRequirement _requirement;
        private readonly ISysRoleMenuService _sysRoleMenuService;
        private readonly IRedisRepository _redis;
        private readonly ISysFactoryService _sysFactoryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysUserFactoryService _sysUserFactoryService;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="sysUserService"></param>
        /// <param name="sysUserRoleServic"></param>
        /// <param name="sysRoleService"></param>
        /// <param name="requirement"></param>
        /// <param name="sysRoleMenuService"></param>
        /// <param name="redis"></param>
        /// <param name="sysFactoryService"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="sysUserFactoryService"></param>
        public LoginController(ISysUserService sysUserService, ISysUserRoleService sysUserRoleServic
            , ISysRoleService sysRoleService
            , PermissionRequirement requirement
            , ISysRoleMenuService sysRoleMenuService
            , IRedisRepository redis
            , ISysFactoryService sysFactoryService
            , IUnitOfWork unitOfWork
            , ISysUserFactoryService sysUserFactoryService
            )
        {
            this._sysUserService = sysUserService;
            this._sysUserRoleServic = sysUserRoleServic;
            this._sysRoleService = sysRoleService;
            _requirement = requirement;
            _sysRoleMenuService = sysRoleMenuService;
            _redis = redis;
            _sysFactoryService = sysFactoryService;
            _unitOfWork = unitOfWork;
            _sysUserFactoryService = sysUserFactoryService;
        }

        /// <summary>
        /// 获取JWT的方法：整个系统主要方法
        /// </summary>
        /// <param name="loginParams">登录参数</param>
        /// <returns></returns>
        [HttpPost]
        [Route("JWTToken")]
        public async Task<MessageModel<TokenInfoViewModel>> GetJwtToken([FromBody] loginParams loginParams)
        {
            string jwtStr = string.Empty;
            string username = loginParams.username;
            string password = loginParams.password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return Failed<TokenInfoViewModel>("用户名或密码不能为空!");
            string code = await _redis.GetValue(loginParams.captchaId);
            if (string.IsNullOrEmpty(code))
                return Failed<TokenInfoViewModel>("验证码已过期!");
            if (code.ToLower() != loginParams.verifyCode?.ToLower())
                return Failed<TokenInfoViewModel>("输入的验证码有误!");

            var factoryList = await _sysFactoryService.Query();

            password = MD5Helper.MD5Encrypt32(password);

            var whereExpression = Expressionable.Create<SysUser>()
                                                .And(x => x.LoginAccount == username)
                                                .And(x => x.Password == password)
                                                .And(x => x.IsEnabled && !x.IsDeleted)
                                                .ToExpression();

            UserLoginDto user = await _sysUserService.LoginIn(whereExpression);
            if (user != null && user.FactoryCode.IsNotEmptyOrNull())
            {
                (var userRoles, string userId) = await _sysUserService.GetUserRoleNameStr(username);
                var now = DateTime.Now;
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.GroupSid, user.FactoryId.ToString()),
                    new Claim(ClaimTypes.PostalCode, user.FactoryCode),
                    new Claim(JwtRegisteredClaimNames.Jti, userId),
                    new Claim(ClaimTypes.Expiration, now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };
                claims.AddRange(userRoles.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));

                // jwt
                if (!Permissions.IsUseIds4)
                {
                    var data = await _sysRoleMenuService.RoleMenuMaps();
                    var list = (from item in data
                                where item.IsDeleted == false && item.SysMenu?.Type == 2
                                orderby item.Id
                                select new PermissionItem
                                {
                                    Url = item.SysMenu?.LinkUrl,
                                    Role = item.SysRole?.RoleName,
                                })?.ToList();

                    _requirement.Permissions = list;
                }

                var token = JwtToken.BuildJwtToken(claims.ToArray(), _requirement, now);
                await _redis.Remove(loginParams.captchaId);
                return Success(token, "token获取成功");
            }
            else
            {
                return Failed<TokenInfoViewModel>("认证失败，如果启用多工厂模式，请检查用户是否已分配该工厂权限!");
            }
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="len">字符个数</param>
        /// <param name="bgColor">背景颜色</param>
        /// <returns></returns>
        [HttpGet]
        [Route("InitVerifyCode")]
        public async Task<MessageModel<verifyCodeDto>> InitVerifyCode(int width = 100, int height = 50, int len = 4, string bgColor = "light")
        {
            string id = Guid.NewGuid().ToString().Replace("-", "");
            var str = ImageHelper.GetVerifyCode(out string code, width, height, len, bgColor);
            await _redis.Set(id, code, new TimeSpan(0, 3, 0));
            return Success(new verifyCodeDto { id = id, img = str });
        }

        /// <summary>
        /// 请求刷新Token（以旧换新）/ 解锁屏幕
        /// </summary>
        /// <param name="token"></param>
        /// <param name="pwd">密码, 解锁屏幕需要传递该参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("RefreshToken")]
        public async Task<MessageModel<TokenInfoViewModel>> RefreshToken(string token = "", string pwd = "")
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(token))
                return Failed<TokenInfoViewModel>("token无效，请重新登录！");
            var tokenModel = JwtHelper.SerializeJwt(token);
            if (tokenModel != null && JwtHelper.customSafeVerify(token) && tokenModel.Uid > 0)
            {
                var user = await _sysUserService.QueryById(tokenModel.Uid);
                if (!string.IsNullOrWhiteSpace(pwd) && MD5Helper.MD5Encrypt32(pwd) != user.Password)
                {
                    return Failed<TokenInfoViewModel>("密码输入有误, 请重新输入!");
                }
                if (user != null)
                {
                    (var userRoles, string userId) = await _sysUserService.GetUserRoleNameStr(user.LoginAccount);
                    var now = DateTime.Now;
                    var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.LoginAccount),
                    new Claim(ClaimTypes.GroupSid, tokenModel.FactoryId),
                    new Claim(ClaimTypes.PostalCode, tokenModel.FactoryCode),
                    new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid.ObjToString()),
                    new Claim(ClaimTypes.Expiration, now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };
                    claims.AddRange(userRoles.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));

                    var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                    identity.AddClaims(claims);

                    // 重新赋予权限
                    if (!Permissions.IsUseIds4)
                    {
                        var data = await _sysRoleMenuService.RoleMenuMaps();
                        var list = (from item in data
                                    where item.IsDeleted == false && item.SysMenu?.Type == 2
                                    orderby item.Id
                                    select new PermissionItem
                                    {
                                        Url = item.SysMenu?.LinkUrl,
                                        Role = item.SysRole?.RoleName,
                                    }).ToList();

                        _requirement.Permissions = list;
                    }
                    var refreshToken = JwtToken.BuildJwtToken(claims.ToArray(), _requirement, now);
                    return Success(refreshToken, "获取成功");
                }
            }
            return Failed<TokenInfoViewModel>("认证失败！");
        }

        /// <summary>
        /// 切换工厂
        /// </summary>
        /// <param name="newFactoryId">新工厂id</param>
        /// <param name="token">旧token</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ChangeFactory")]
        [Authorize(Permissions.Name)]
        public async Task<MessageModel<TokenInfoViewModel>> ChangeFactory(int newFactoryId, string token)
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(token))
                return Failed<TokenInfoViewModel>("token无效，请重新登录！");
            var tokenModel = JwtHelper.SerializeJwt(token);
            if (tokenModel != null && JwtHelper.customSafeVerify(token) && tokenModel.Uid > 0)
            {
                var user = await _sysUserService.QueryById(tokenModel.Uid);
                if (user != null)
                {
                    var sysUserFactory = await _sysUserFactoryService.QuerySingle(x => x.SysUserId == user.Id && x.SysFactoryId == newFactoryId);
                    if (sysUserFactory == null) return Failed<TokenInfoViewModel>("当前用户没有新工厂的权限!");
                    var newFactory = (await CommonCache.GetAllFactoryWithCache()).FirstOrDefault(x => x.Id == newFactoryId);
                    (var userRoles, string userId) = await _sysUserService.GetUserRoleNameStr(user.LoginAccount);
                    var now = DateTime.Now;
                    var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.LoginAccount),
                    new Claim(ClaimTypes.GroupSid, newFactoryId.ObjToString()),
                    new Claim(ClaimTypes.PostalCode, newFactory?.Code),
                    new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid.ObjToString()),
                    new Claim(ClaimTypes.Expiration, now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };
                    claims.AddRange(userRoles.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));

                    var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                    identity.AddClaims(claims);

                    // 重新赋予权限
                    if (!Permissions.IsUseIds4)
                    {
                        var data = await _sysRoleMenuService.RoleMenuMaps();
                        var list = (from item in data
                                    where item.IsDeleted == false && item.SysMenu?.Type == 2
                                    orderby item.Id
                                    select new PermissionItem
                                    {
                                        Url = item.SysMenu?.LinkUrl,
                                        Role = item.SysRole?.RoleName,
                                    }).ToList();

                        _requirement.Permissions = list;
                    }
                    var refreshToken = JwtToken.BuildJwtToken(claims.ToArray(), _requirement, now);
                    return Success(refreshToken, "获取成功");
                }
            }
            return Failed<TokenInfoViewModel>("认证失败！");
        }
    }
}