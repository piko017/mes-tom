using TMom.Application.Service.IService;
using TMom.Domain.IRepository;
using TMom.Domain.Model.Entity;

namespace TMom.Application.Service.Service
{
    public class SysUserFactoryService : BaseService<SysUserFactory, int>, ISysUserFactoryService
    {
        private readonly IBaseRepository<SysUserFactory, int> _dal;
        private readonly ISysUserFactoryRepository _sysUserFactoryRepository;

        public SysUserFactoryService(IBaseRepository<SysUserFactory, int> dal, ISysUserFactoryRepository sysUserFactoryRepository)
        {
            this._dal = dal;
            base.BaseRepo = dal;
            _sysUserFactoryRepository = sysUserFactoryRepository;
        }
    }
}