using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TMom.Application;
using TMom.Application.Service.IService;
using TMom.Domain.Model;
using TMom.Domain.Model.Entity;
using TMom.Infrastructure.Helper;

namespace TMom.Api.Controllers
{
    /// <summary>
    /// 定时任务日志【按月分表】
    /// </summary>
	[Route("api/[controller]/[action]")]
    [ApiController]
    public class SysJobLogController : BaseApiController<SysJobLog, long>
    {
        private readonly ISysJobLogService _sysJobLogService;
        private readonly IUser _user;
        private readonly IMapper _mapper;

        public SysJobLogController(ISysJobLogService sysJobLogService, IUser user, IMapper mapper)
        {
            _sysJobLogService = sysJobLogService;
            _user = user;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取Job日志分页列表
        /// </summary>
        /// <param name="jobId">任务id</param>
        /// <param name="pageIndex">页标, 默认1</param>
        /// <param name="pageSize">页数, 默认10</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<PageModel<SysJobLog>>> GetWithPage(int jobId, int pageIndex = 1, int pageSize = 10)
        {
            var whereExp = DynamicFilterExpress().Item1;
            whereExp = whereExp.And(x => x.SysJobId == jobId);
            var data = await _sysJobLogService.QueryPage(whereExp, pageIndex, pageSize, null, "CreateTime DESC");
            return SuccessPage(data);
        }
    }
}