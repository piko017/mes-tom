using TMom.Domain.Model.Entity;
using TMom.Infrastructure;
using SqlSugar;
using static TMom.Domain.Model.GlobalVars;

namespace TMom.Domain.Model
{
    /// <summary>
    /// 公共缓存类
    /// </summary>
    public static class CommonCache
    {
        private static readonly IRedisRepository _redis = AutofacContainer.Resolve<IRedisRepository>();
        private static readonly ISqlSugarClient _sqlSugarClient = AutofacContainer.Resolve<ISqlSugarClient>();
        private static readonly IUser _user = AutofacContainer.Resolve<IUser>();

        /// <summary>
        ///
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="cb"></param>
        /// <returns></returns>
        private static async Task SetNxKey(string redisKey, Action cb)
        {
            if (await _redis.Db.StringSetAsync($"{redisKey}_mutex", true, TimeSpan.FromSeconds(10), StackExchange.Redis.When.NotExists))
            {
                cb();
                await _redis.Remove($"{redisKey}_mutex");
            }
        }

        #region 基础信息缓存(不分工厂)

        /// <summary>
        /// 获取所有缓存的用户列表
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SysUserCacheDto>> GetAllUserWithCache()
        {
            var cacheUsers = await _redis.HashGet<List<SysUserCacheDto>>(RedisGlobalKey.AllUserList, RedisHashField.UserInfo);
            if (cacheUsers != null && cacheUsers.Any()) return cacheUsers;
            var list = await _sqlSugarClient.Queryable<SysUser>()
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
            await _redis.HashSet(RedisGlobalKey.AllUserList, RedisHashField.UserInfo, list, TimeSpan.FromHours(12));
            return list;
        }

        /// <summary>
        /// 获取所有缓存的工厂列表
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SysFactory>> GetAllFactoryWithCache()
        {
            var cacheDBs = await _redis.Get<List<SysFactory>>(RedisGlobalKey.AllDbList);
            if (cacheDBs != null && cacheDBs.Any()) return cacheDBs;
            await SetNxKey(RedisGlobalKey.AllDbList, async () =>
            {
                var list = await _sqlSugarClient.Queryable<SysFactory>().Where(x => !x.IsDeleted).ToListAsync();
                await _redis.Set(RedisGlobalKey.AllDbList, list, TimeSpan.FromHours(12));
                cacheDBs = list;
            });
            if (cacheDBs != null && cacheDBs.Any()) return cacheDBs;
            await Task.Delay(100);
            return await GetAllFactoryWithCache();
        }

        /// <summary>
        /// 解析当前登录用户token获取工厂实体信息（缓存）
        /// </summary>
        /// <returns></returns>
        public static async Task<SysFactory?> GetSysFactoryByTokenWithCache()
        {
            int factoryId = _user.FactoryId;
            var list = await GetAllFactoryWithCache();
            return list.FirstOrDefault(x => x.Id == factoryId);
        }

        #endregion 基础信息缓存(不分工厂)
    }
}