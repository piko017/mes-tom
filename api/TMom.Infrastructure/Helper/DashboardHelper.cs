using Microsoft.AspNetCore.Http;
using TMom.Infrastructure.Helper;

namespace TMom.Infrastructure
{
    public static class DashboardHelper
    {
        /// <summary>
        /// 根据header中信息返回工厂代码(如果有自定义header内容)
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>当前操作的工厂代码</returns>
        public static string GetFactoryCodeByHeader(HttpContext context)
        {
            var header = context?.Request?.Headers;
            if (header == null) return "";
            header.TryGetValue("FactoryCode", out var value);
            if (string.IsNullOrEmpty(value)) return "";
            string factoryCode = MD5Helper.DesDecrypt(value.ToString());
            return factoryCode;
        }
    }
}