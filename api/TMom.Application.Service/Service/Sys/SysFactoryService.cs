using TMom.Application.Service.IService;
using TMom.Domain.IRepository;
using TMom.Domain.Model.Entity;
using TMom.Infrastructure;

namespace TMom.Application.Service.Service
{
    public class SysFactoryService : BaseService<SysFactory, int>, ISysFactoryService
    {
        private readonly IBaseRepository<SysFactory, int> _dal;
        private readonly ISysFactoryRepository _sysFactoryRepository;

        public SysFactoryService(IBaseRepository<SysFactory, int> dal, ISysFactoryRepository sysFactoryRepository)
        {
            this._dal = dal;
            base.BaseRepo = dal;
            _sysFactoryRepository = sysFactoryRepository;
        }
    }
}