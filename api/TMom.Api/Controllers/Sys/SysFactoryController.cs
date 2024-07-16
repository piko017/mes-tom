using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMom.Application;
using TMom.Application.Service.IService;
using TMom.Domain.Model;
using TMom.Domain.Model.Entity;
using TMom.Infrastructure.Common;
using TMom.Infrastructure.Repository;
using SqlSugar;
using static TMom.Domain.Model.GlobalVars;

namespace TMom.Api.Controllers
{
    /// <summary>
    /// 工厂
    /// </summary>
	[Route("api/[controller]/[action]")]
    [ApiController]
    public class SysFactoryController : BaseApiController<SysFactory, int>
    {
        private readonly SqlSugarScope _sqlSugarClient;
        private readonly ISysFactoryService _sysFactoryService;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        private readonly IRedisRepository _redis;
        private readonly ISysUserFactoryService _sysUserFactoryService;
        private readonly IUnitOfWork _unitOfWork;

        public SysFactoryController(ISqlSugarClient sqlSugarClient, ISysFactoryService sysFactoryService
            , IUser user, IMapper mapper, IRedisRepository redis
            , ISysUserFactoryService sysUserFactoryService
            , IUnitOfWork unitOfWork)
        {
            _sqlSugarClient = sqlSugarClient as SqlSugarScope;
            _sysFactoryService = sysFactoryService;
            _user = user;
            _mapper = mapper;
            _redis = redis;
            _sysUserFactoryService = sysUserFactoryService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageIndex">页标, 默认1</param>
        /// <param name="pageSize">页数, 默认10</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Permissions.Name)]
        public async Task<MessageModel<PageModel<SysFactory>>> GetWithPage(int pageIndex = 1, int pageSize = 10)
        {
            var data = await _sysFactoryService.QueryPage(DynamicFilterExpress().Item1, pageIndex, pageSize);
            data.list.ForEach(p =>
            {
                if (!p.IsCustom)
                {
                    p.DBConn = "-";
                }
            });
            return Success(data);
        }

        /// <summary>
        /// 获取所有工厂列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<dynamic>> GetFactory()
        {
            var list = await _sysFactoryService.Query();
            var res = list.Select(x => new { factoryId = x.Id, factoryName = x.Name }).ToList();
            return Success<dynamic>(res);
        }

        /// <summary>
        /// 根据主键Id获取单条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<MessageModel<SysFactory>> Get(int id)
        {
            var entity = await _sysFactoryService.QueryById(id);
            if (entity == null || entity.Id <= 0)
            {
                return Failed<SysFactory>($"主键:{id}不存在或已被删除!");
            }
            if (!entity.IsCustom)
            {
                entity.DBConn = "-";
            }
            return Success(entity);
        }

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Permissions.Name)]
        public async Task<MessageModel<string>> Add([FromBody] SysFactory request)
        {
            var res = new MessageModel<string>();
            var isSameCode = (await _sysFactoryService.Query(x => x.Code == request.Code)).Any();
            if (isSameCode) return Failed("已存在相同的工厂编码!");
            // 检查数据库链接是否正确
            if (request.IsCustom && !DBHelper.TestDBConn(request.DBConn, request.DBType.Value)) return Failed("数据库连接失败, 请检查链接字符串!");
            RenderColumn(request);
            request.UpdateCommonFields(_user.Id);
            var Id = await _sysFactoryService.Add(request);
            if (Id > 0)
            {
                await RefreshCache();
                BaseDBConfig.InitBusinessDatabaseTable(request.DBConn, (DbType)request.DBType, request.Code);
                // 给默认管理员增加工厂权限
                var sysUserFactory = new SysUserFactory()
                {
                    SysUserId = 1,
                    SysFactoryId = Id
                };
                sysUserFactory.UpdateCommonFields(_user.Id);
                await _sysUserFactoryService.Add(sysUserFactory);
                res.success = true;
                res.data = Id.ObjToString();
                res.msg = "添加成功";
            }

            return res;
        }

        private void RenderColumn(SysFactory request)
        {
            if (!request.IsCustom)
            {
                var dbType = _sqlSugarClient.CurrentConnectionConfig.DbType;
                var database = _sqlSugarClient.Ado.Connection.Database;
                var conn = _sqlSugarClient.CurrentConnectionConfig.ConnectionString;
                request.DBType = EnumExtensions.ToEnumInt(dbType);
                request.DBConn = conn.Replace(database, $"{database}_{request.Code}");
            }
        }

        /// <summary>
        /// 刷新工厂列表缓存
        /// </summary>
        /// <returns></returns>
        private async Task RefreshCache()
        {
            var cacheDBList = await _redis.Get<List<SysFactory>>(RedisGlobalKey.AllDbList);
            var list = await _sysFactoryService.Query();
            if (cacheDBList != null)
            {
                await _redis.Remove(RedisGlobalKey.AllDbList);
            }
            await _redis.Set(RedisGlobalKey.AllDbList, list, TimeSpan.FromHours(1));
        }

        /// <summary>
        /// 修改全部数据(默认根据主键更新)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>修改的主键Id</returns>
        [HttpPut]
        [Authorize(Permissions.Name)]
        public async Task<MessageModel<string>> Update([FromBody] SysFactory request)
        {
            var res = new MessageModel<string>();
            var isSameCode = (await _sysFactoryService.Query(x => x.Code == request.Code && x.Id != request.Id)).Any();
            if (isSameCode) return Failed("已存在相同的工厂编码!");
            // 检查数据库链接是否正确
            if (request.IsCustom && !DBHelper.TestDBConn(request.DBConn, request.DBType.Value)) return Failed("数据库连接失败, 请检查链接字符串!");
            RenderColumn(request);
            var entity = await _sysFactoryService.QueryById(request.Id);
            if (entity.DBConn != request.DBConn)
            {
                BaseDBConfig.InitBusinessDatabaseTable(request.DBConn, (DbType)request.DBType, entity.Code);
            }
            request.UpdateCommonFields(_user.Id, false);
            res.success = await _sysFactoryService.Update(request);
            if (res.success)
            {
                await RefreshCache();
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
            var entityList = await _sysFactoryService.Query(x => _ids.Contains(x.Id.ToString()));
            if (entityList == null || !entityList.Any())
            {
                return Failed($"主键: {ids}均不存在或已被删除!");
            }
            entityList.MarkedAsDeleted(_user.Id);
            // 工厂关联的用户数据也删除
            var userAndFactoryList = await _sysUserFactoryService.Query(x => _ids.Contains(x.SysFactoryId.ToString()));
            userAndFactoryList.MarkedAsDeleted(_user.Id);

            _unitOfWork.BeginTran();
            bool isSuccess = await _sysFactoryService.Update(entityList);
            await _sysUserFactoryService.Update(userAndFactoryList);
            _unitOfWork.CommitTran();
            if (isSuccess) await RefreshCache();
            return isSuccess ? Success() : Failed();
        }
    }
}