using TMom.Domain.IRepository;
using TMom.Domain.Model.Entity;

namespace TMom.Infrastructure.Repository
{
    /// <summary>
    /// SysJobLogRepository
    /// </summary>
    public class SysJobLogRepository : BaseRepository<SysJobLog, long>, ISysJobLogRepository
    {
        public SysJobLogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}