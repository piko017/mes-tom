using System.Security.Claims;

namespace TMom.Domain.Model
{
    public interface IUser
    {
        /// <summary>
        /// 当前登录用户账号
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 当前登录用户Id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 当前登录用户工厂Id(没有登陆会从header中获取)
        /// <para>内部用到了异步方法, 不要直接用在参数中, eg: where(x => x.FactoryId == _user.FactoryId)中, 会影响查询效率</para>
        /// <para>正确方式: 先赋值到一个变量再用到where中</para>
        /// </summary>
        int FactoryId { get; }

        /// <summary>
        /// 当前登录用户工厂编码(没有登陆会从header中获取)
        /// </summary>
        string FactoryCode { get; }

        /// <summary>
        /// 当前登录用户工厂名称
        /// <para>内部用到了异步方法, 不要直接用在参数中, eg: where(x => x.FactoryName == _user.FactoryName)中, 会影响查询效率</para>
        /// <para>正确方式: 先赋值到一个变量再用到where中</para>
        /// </summary>
        string FactoryName { get; }

        bool IsAuthenticated();

        IEnumerable<Claim> GetClaimsIdentity();

        List<string> GetClaimValueByType(string ClaimType);

        string GetToken();

        List<string> GetUserInfoFromToken(string ClaimType = "jti");
    }
}