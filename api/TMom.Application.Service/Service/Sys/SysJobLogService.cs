using TMom.Application.Service.IService;
using TMom.Domain.IRepository;
using TMom.Domain.Model.Entity;

namespace TMom.Application.Service.Service
{
    public class SysJobLogService : BaseService<SysJobLog, long>, ISysJobLogService
    {
        private readonly IBaseRepository<SysJobLog, long> _dal;
        private readonly ISysJobLogRepository _sysJobLogRepository;

        public SysJobLogService(IBaseRepository<SysJobLog, long> dal, ISysJobLogRepository sysJobLogRepository)
        {
            this._dal = dal;
            base.BaseRepo = dal;
            _sysJobLogRepository = sysJobLogRepository;
        }
    }
}