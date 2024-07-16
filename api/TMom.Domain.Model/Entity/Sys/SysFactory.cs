using SqlSugar;

namespace TMom.Domain.Model.Entity
{
    /// <summary>
    /// 工厂表
    /// </summary>
    [UtilsTable]
    [SugarTable("sys_factory")]
    public class SysFactory : RootEntity<int>
    {
        /// <summary>
        /// 工厂代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 工厂名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否自定义配置
        /// </summary>
        public bool IsCustom { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public int? DBType { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public string DBConn { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";
    }
}