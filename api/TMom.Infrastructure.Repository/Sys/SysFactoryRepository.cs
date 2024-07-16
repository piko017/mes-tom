using TMom.Domain.IRepository;
using TMom.Domain.Model.Entity;

namespace TMom.Infrastructure.Repository
{
    /// <summary>
    /// SysFactoryRepository
    /// </summary>
    public class SysFactoryRepository : BaseRepository<SysFactory, int>, ISysFactoryRepository
    {
        public SysFactoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}