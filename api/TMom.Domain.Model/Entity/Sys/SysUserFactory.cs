using SqlSugar;

namespace TMom.Domain.Model.Entity
{
    /// <summary>
    /// 用户和工厂关系表
    /// </summary>
    [UtilsTable]
    [SugarTable("sys_user_factory")]
    public class SysUserFactory : RootEntity<int>
    {
        public int SysUserId { get; set; }

        public int SysFactoryId { get; set; }

        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(SysUserId))]
        public SysUser? SysUser { get; set; }

        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(SysFactoryId))]
        public SysFactory? SysFactory { get; set; }
    }
}