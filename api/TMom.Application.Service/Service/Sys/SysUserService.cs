﻿using TMom.Application.Dto.Sys;
using TMom.Application.Service.IService;
using TMom.Domain.IRepository;
using TMom.Domain.Model;
using TMom.Domain.Model.Entity;
using TMom.Infrastructure;
using TMom.Infrastructure.Helper;
using SqlSugar;
using System.Linq.Expressions;
using static TMom.Domain.Model.GlobalVars;

namespace TMom.Application.Service.Service
{
    public class SysUserService : BaseService<SysUser, int>, ISysUserService
    {
        private readonly IBaseRepository<SysUser, int> _dal;
        private readonly ISysRoleRepository _sysRoleRepository;
        private readonly ISysUserRoleRepository _sysUserRoleRepository;
        private readonly ISysOrgRepository _sysOrgRepository;
        private readonly ISysUserRoleService _sysUserRoleService;
        private readonly ISysUserFactoryService _sysUserFactoryService;
        private readonly IRedisRepository _redisRepository;

        public SysUserService(IBaseRepository<SysUser, int> dal, ISysUserRoleRepository sysUserRoleRepository
            , ISysRoleRepository sysRoleRepository, IRedisRepository redis, ISysOrgRepository sysOrgRepository
            , ISysUserRoleService sysUserRoleService, ISysUserFactoryService sysUserFactoryService)
        {
            this._dal = dal;
            base.BaseRepo = dal;
            _sysRoleRepository = sysRoleRepository;
            _sysUserRoleRepository = sysUserRoleRepository;
            _sysOrgRepository = sysOrgRepository;
            _sysUserRoleService = sysUserRoleService;
            _sysUserFactoryService = sysUserFactoryService;
            _redisRepository = redis;
        }

        /// <summary>
        /// 获取用户所有的角色名及用户Id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 5)]
        public async Task<(string, string)> GetUserRoleNameStr(string name)
        {
            string roleName = "";
            string uid = "";
            var user = await base.QuerySingle(a => a.LoginAccount == name && !a.IsDeleted);
            var roleList = await _sysRoleRepository.Query(a => !a.IsDeleted);
            if (user != null)
            {
                var userRoles = await _sysUserRoleRepository.Query(ur => ur.SysUserId == user.Id && !ur.IsDeleted);
                if (userRoles.Count > 0)
                {
                    var arr = userRoles.Select(ur => ur.SysRoleId.ObjToString()).ToList();
                    var roles = roleList.Where(d => arr.Contains(d.Id.ObjToString()));

                    roleName = string.Join(',', roles.Select(r => r.RoleName).ToArray());
                }
                uid = user.Id.ObjToString();
            }
            return (roleName, uid);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="_whereExp"></param>
        /// <param name="orgId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PageModel<SysUser>> GetWithPage((Expression<Func<SysUser, bool>>, List<FormattableString>) _whereExp, int? orgId, int pageIndex, int pageSize)
        {
            var whereExp = _whereExp.Item1;
            whereExp = whereExp.And(x => x.IsDeleted == false);
            RefAsync<int> totalCount = 0;
            if (orgId.HasValue && orgId.Value > 0)
            {
                var orgData = await _sysOrgRepository.Query();
                List<int> allIds = new List<int>() { orgId.Value };
                RecursionHelper.LoopIdToList(orgData, orgId.Value, allIds);
                whereExp = whereExp.And(x => allIds.Contains(x.OrgId.Value));
            }

            // whereExp 中已筛选掉已删除的数据
            var data = await _dal.Db.Queryable<SysUser>()
                                    .Includes(x => x.SysOrg)
                                    .Where(whereExp)
                                    .ToPageListAsync(pageIndex, pageSize, totalCount)
                                    .RenderUserInfoList();
            var userFactoryList = await _dal.Db.Queryable<SysUserFactory>()
                                               .Includes(x => x.SysUser)
                                               .Includes(x => x.SysFactory)
                                               .Where(x => data.Select(t => t.Id).Contains(x.SysUserId))
                                               .ToListAsync();
            data.ForEach(x =>
            {
                x.FactoryName = string.Join(",", userFactoryList.Where(t => t.SysUserId == x.Id).Select(t => t.SysFactory?.Name));
                x.OrgName = x.SysOrg != null ? $"({x.SysOrg.OrgCode}){x.SysOrg.OrgName}" : null;
            });
            return new PageModel<SysUser>()
            {
                list = data,
                pagination = new pageInfo()
                {
                    pageIndex = pageIndex,
                    pageSize = pageSize,
                    total = totalCount
                }
            };
        }

        /// <summary>
        /// 用户登录, 成功时返回当前用户信息
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<UserLoginDto> LoginIn(Expression<Func<SysUser, bool>> whereExpression)
        {
            var user = await _dal.Db.Queryable<SysUser>()
                                    .LeftJoin<SysUserFactory>((x, suf) => x.Id == suf.SysUserId && !suf.IsDeleted)
                                    .LeftJoin<SysFactory>((x, suf, sf) => suf.SysFactoryId == sf.Id && !sf.IsDeleted)
                                    .Where(whereExpression)
                                    .OrderBy((x, suf) => suf.CreateTime)
                                    .Select((x, suf, sf) => new UserLoginDto()
                                    {
                                        UserId = x.Id,
                                        FactoryId = suf.SysFactoryId,
                                        FactoryCode = sf.Code
                                    })
                                    .ToListAsync();
            var userInfo = user.FirstOrDefault();
            return userInfo;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [UseTran]
        public async Task<bool> UpdateData(SysUser request, int userId)
        {
            #region 角色

            var sysUserRoles = await _sysUserRoleService.Query(x => x.SysUserId == request.Id);
            var sysUserRoleIds = sysUserRoles.Select(x => x.SysRoleId);

            var sysUserRoleList = new List<SysUserRole>();
            // 新增的角色: 表中不存在, 参数中有
            var addRoleIds = request.SysRoleIds.Where(x => !sysUserRoleIds.Contains(x)).ToList();
            foreach (var roleId in addRoleIds)
            {
                SysUserRole sysUserRole = new SysUserRole()
                {
                    SysUserId = request.Id,
                    SysRoleId = roleId
                };
                sysUserRole.UpdateCommonFields(userId);
                sysUserRoleList.Add(sysUserRole);
            }
            await _sysUserRoleService.Add(sysUserRoleList);

            // 删除的角色: 表中有, 参数中没有
            var delRoleIds = sysUserRoleIds.Where(x => !request.SysRoleIds.Contains(x)).ToList();
            var delUserRoleList = sysUserRoles.Where(x => delRoleIds.Contains(x.SysRoleId)).ToList();
            delUserRoleList.ForEach(x => x.MarkedAsDeleted(userId));
            await _sysUserRoleService.Update(delUserRoleList);

            #endregion 角色

            #region 工厂

            var sysUserFactorys = await _sysUserFactoryService.Query(x => x.SysUserId == request.Id);
            var sysUserFactoryIds = sysUserFactorys.Select(x => x.SysFactoryId);

            var sysUserFactoryList = new List<SysUserFactory>();
            // 新增的工厂: 表中不存在, 参数中有
            var addFactoryIds = request.SysFactoryIds.Where(x => !sysUserFactoryIds.Contains(x)).ToList();
            foreach (var FactoryId in addFactoryIds)
            {
                SysUserFactory sysUserFactory = new SysUserFactory()
                {
                    SysUserId = request.Id,
                    SysFactoryId = FactoryId
                };
                sysUserFactory.UpdateCommonFields(userId);
                sysUserFactoryList.Add(sysUserFactory);
            }
            await _sysUserFactoryService.Add(sysUserFactoryList);

            // 删除的工厂: 表中有, 参数中没有
            var delFactoryIds = sysUserFactoryIds.Where(x => !request.SysFactoryIds.Contains(x)).ToList();
            var delUserFactoryList = sysUserFactorys.Where(x => delFactoryIds.Contains(x.SysFactoryId)).ToList();
            delUserFactoryList.ForEach(x => x.MarkedAsDeleted(userId));
            await _sysUserFactoryService.Update(delUserFactoryList);

            #endregion 工厂

            // 用户表
            request.UpdateCommonFields(userId, false);
            bool success = await _dal.Update(request, null, new List<string>() { "Password", "Avatar" });
            return success;
        }

        /// <summary>
        /// 刷新缓存用户
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task RefreshAllUserCache()
        {
            await _redisRepository.HashDelete(RedisGlobalKey.AllUserList, RedisHashField.UserInfo);
            var userList = await _dal.Db.Queryable<SysUser>()
                                        .Where(x => !x.IsDeleted)
                                        .Select(x => new SysUserCacheDto()
                                        {
                                            id = x.Id,
                                            code = x.LoginAccount,
                                            name = x.RealName,
                                            email = x.Email ?? "",
                                            isSuper = x.IsSuper
                                        })
                                        .ToListAsync();
            await _redisRepository.HashSet(RedisGlobalKey.AllUserList, RedisHashField.UserInfo, userList, TimeSpan.FromHours(12));
        }
    }
}