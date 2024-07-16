﻿namespace TMom.Infrastructure
{
    /// <summary>
    /// 动态集成事件处理程序
    /// 接口
    /// </summary>
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}