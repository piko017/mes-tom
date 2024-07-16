namespace TMom.Domain.Model
{
    /// <summary>
    /// 用来判断是否为系统基础信息表, 多库时, 基础表会作为公共表使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class UtilsTableAttribute : Attribute
    {
    }
}