namespace TMom.Application.Dto.Sys
{
    public class userInfoDto
    {
        public int id { get; set; }

        public string? loginAccount { get; set; }

        public string realName { get; set; }

        public string? avatar { get; set; }

        public bool? isSuper { get; set; }

        public int? factoryId { get; set; }
        public string? factoryCode { get; set; }

        /// <summary>
        /// 加密的工厂代码
        /// </summary>
        public string? factoryCodeEncrypt { get; set; }

        public string? factoryName { get; set; }

        public int? orgId { get; set; }

        public string phoneNo { get; set; }

        public string? loginIp { get; set; }

        public string email { get; set; }

        public string addr { get; set; }

        public string remark { get; set; }

        /// <summary>
        /// 当前用户拥有的工厂列表
        /// </summary>
        public List<HasAuthFactoryDto> HasAuthFactoryList { get; set; } = new List<HasAuthFactoryDto>();
    }

    public class HasAuthFactoryDto
    {
        public int FactoryId { get; set; }

        public string FactoryCode { get; set; }

        public string FactoryName { get; set; }

        public string FactoryCodeEncrypt { get; set; }
    }

    public class UserLoginDto
    {
        public int UserId { get; set; }
        public int FactoryId { get; set; }

        public string FactoryCode { get; set; }
    }
}