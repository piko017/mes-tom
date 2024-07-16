namespace TMom.Application.Dto
{
    /// <summary>
    /// 前端Select组件选择项
    /// </summary>
    public class SelectDto
    {
        public string label { get; set; }
        public dynamic value { get; set; }

        public string remark { get; set; }
    }

    /// <summary>
    /// App选择项
    /// </summary>
    public class AppSelectDto
    {
        /// <summary>
        /// 显示文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public dynamic Id { get; set; }
    }
}