namespace TMom.Application.Dto
{
    public class loginParams
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 验证码id
        /// </summary>
        public string? captchaId { get; set; }

        /// <summary>
        /// 验证码code
        /// </summary>
        public string? verifyCode { get; set; }

        /// <summary>
        /// 工厂Id
        /// </summary>
        //public int? factoryId { get; set; }
    }

    public class verifyCodeDto
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 验证码图片base64字符串
        /// </summary>
        public string img { get; set; }
    }

    public class ssoLoginParams
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 工厂代码
        /// </summary>
        public string factoryCode { get; set; }
    }
}