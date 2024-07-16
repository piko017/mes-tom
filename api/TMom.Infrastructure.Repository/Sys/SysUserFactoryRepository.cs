using TMom.Domain.IRepository;
using TMom.Domain.Model.Entity;

namespace TMom.Infrastructure.Repository
{
    /// <summary>
    /// SysUserFactoryRepository
    /// </summary>
    public class SysUserFactoryRepository : BaseRepository<SysUserFactory, int>, ISysUserFactoryRepository
    {
        public SysUserFactoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}