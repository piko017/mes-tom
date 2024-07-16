using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMom.Application;
using TMom.Application.Service.IService;
using TMom.Domain.Model;
using TMom.Domain.Model.Entity;
using SqlSugar;
using static TMom.Domain.Model.GlobalVars;

namespace TMom.Api.Controllers
{
    /// <summary>
    /// 用户工厂关系
    /// </summary>
	[Route("api/[controller]/[action]")]
    [ApiController]
    public class SysUserFactoryController : BaseApiController<SysUserFactory, int>
    {
        private readonly ISysUserFactoryService _sysUserFactoryService;
        private readonly IUser _user;
        private readonly IMapper _mapper;

        public SysUserFactoryController(ISysUserFactoryService sysUserFactoryService, IUser user, IMapper mapper)
        {
            _sysUserFactoryService = sysUserFactoryService;
            _user = user;
            _mapper = mapper;
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageIndex">页标, 默认1</param>
        /// <param name="pageSize">页数, 默认10</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Permissions.Name)]
        public async Task<MessageModel<PageModel<SysUserFactory>>> GetWithPage(int pageIndex = 1, int pageSize = 10)
        {
            return new MessageModel<PageModel<SysUserFactory>>()
            {
                msg = "获取成功",
                success = true,
                data = await _sysUserFactoryService.QueryPage(DynamicFilterExpress().Item1, pageIndex, pageSize)
            };
        }

        /// <summary>
        /// 根据主键Id获取单条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<MessageModel<SysUserFactory>> Get(int id)
        {
            var entity = await _sysUserFactoryService.QueryById(id);
            if (entity == null || entity.Id <= 0)
            {
                return Failed<SysUserFactory>($"主键:{id}不存在或已被删除!");
            }
            return new MessageModel<SysUserFactory>()
            {
                msg = "获取成功",
                success = true,
                data = entity
            };
        }

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Permissions.Name)]
        public async Task<MessageModel<string>> Add([FromBody] SysUserFactory request)
        {
            var res = new MessageModel<string>();

            request.UpdateCommonFields(_user.Id);
            var Id = await _sysUserFactoryService.Add(request);
            if (Id > 0)
            {
                res.success = true;
                res.data = Id.ObjToString();
                res.msg = "添加成功";
            }

            return res;
        }

        /// <summary>
        /// 修改全部数据(默认根据主键更新)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>修改的主键Id</returns>
        [HttpPut]
        [Authorize(Permissions.Name)]
        public async Task<MessageModel<string>> Update([FromBody] SysUserFactory request)
        {
            var res = new MessageModel<string>();
            request.UpdateCommonFields(_user.Id, false);
            res.success = await _sysUserFactoryService.Update(request);
            if (res.success)
            {
                res.msg = "更新成功";
                res.data = request?.Id.ObjToString();
            }

            return res;
        }

        /// <summary>
        /// 根据主键Id删除数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Permissions.Name)]
        public async Task<MessageModel<string>> Delete(string ids)
        {
            string[] _ids = ids.Split(',');
            var entityList = await _sysUserFactoryService.Query(x => _ids.Contains(x.Id.ToString()));
            if (entityList == null || !entityList.Any())
            {
                return Failed($"主键: {ids}均不存在或已被删除!");
            }
            entityList.ForEach(entity => entity.MarkedAsDeleted(_user.Id));
            return await _sysUserFactoryService.Update(entityList) ? Success() : Failed();
        }
    }
}