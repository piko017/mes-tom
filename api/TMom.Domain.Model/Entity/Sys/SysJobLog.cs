using SqlSugar;

namespace TMom.Domain.Model.Entity
{
    /// <summary>
    /// 定时任务日志表(按年分表)
    /// </summary>
    [UtilsTable]
    [SplitTable(SplitType.Year)]
    [SugarTable("sys_job_log_{year}{month}{day}")]
    public class SysJobLog : RootEntity<long>
    {
        public int SysJobId { get; set; }

        /// <summary>
        /// 详细内容
        /// </summary>
        [SugarColumn(Length = 3000)]
        public string? Content { get; set; }

        /// <summary>
        /// 是否成功执行
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 执行总时间(秒)
        /// </summary>
        public float ExecSeconds { get; set; }

        /// <summary>
        /// 创建时间(分表字段)
        /// </summary>
        [SplitField]
        public override DateTime CreateTime { get; set; } = DateTime.Now;
    }
}