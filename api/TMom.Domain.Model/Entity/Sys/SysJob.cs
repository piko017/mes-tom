using SqlSugar;

namespace TMom.Domain.Model.Entity.Sys
{
    /// <summary>
    /// 定时任务表
    /// </summary>
    [UtilsTable]
    [SugarTable("sys_job")]
    public class SysJob : RootEntity<int>
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        [SugarColumn(Length = 200)]
        public string JobName { get; set; }

        /// <summary>
        /// 执行传参
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? JobParams { get; set; }

        /// <summary>
        /// 任务分组
        /// </summary>
        [SugarColumn(Length = 200)]
        public string JobGroup { get; set; }

        /// <summary>
        /// 任务运行时间表达式
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string Cron { get; set; }

        /// <summary>
        /// 任务所在DLL对应的程序集名称
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string AssemblyName { get; set; }

        /// <summary>
        /// 任务类名称
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string ClassName { get; set; }

        /// <summary>
        /// 邮件接收人
        /// </summary>
        public string EmailReceiver { get; set; } = "";

        /// <summary>
        /// 任务描述
        /// </summary>
        [SugarColumn(Length = 1000, IsNullable = true)]
        public string? Description { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 触发器类型（0、simple 1、cron）
        /// </summary>
        public int TriggerType { get; set; } = 1;

        /// <summary>
        /// 执行间隔时间, 单位秒
        /// </summary>
        public int IntervalSecond { get; set; } = 0;

        /// <summary>
        /// 执行次数
        /// </summary>
        public int RunTimes { get; set; } = 0;

        /// <summary>
        /// 循环执行次数
        /// </summary>
        public int CycleRunTimes { get; set; } = 0;

        /// <summary>
        /// 是否启动
        /// </summary>
        public bool IsStart { get; set; } = true;

        /// <summary>
        /// 上次运行时间
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string PrevTime { get; set; } = "";

        /// <summary>
        /// 下次执行时间
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string NextTime { get; set; } = "";

        /// <summary>
        /// 最新一次执行的结果状态
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool LastStatus { get; set; } = true;

        /// <summary>
        /// 执行的工厂id(多个以逗号分隔, 为空表示不涉及多工厂)
        /// </summary>
        public string FactoryIds { get; set; } = "";
    }
}