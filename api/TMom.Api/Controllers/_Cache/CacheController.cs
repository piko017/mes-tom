using Microsoft.AspNetCore.Mvc;
using TMom.Application;
using TMom.Domain.Model;

namespace TMom.Api.Controllers
{
    /// <summary>
    /// 公共缓存
    /// </summary>
	[Route("api/[controller]/[action]")]
    [ApiController]
    public class CacheController : BaseApiController<RootEntity<int>, int>
    {
        public CacheController()
        { }

        /// <summary>
        /// 获取工厂缓存数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<List<CacheCommonDto>>> GetFactoryWithCache()
        {
            var list = await CommonCache.GetAllFactoryWithCache();
            List<CacheCommonDto> data = list.Select(x => new CacheCommonDto()
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
            }).ToList();
            return Success(data);
        }
    }
}