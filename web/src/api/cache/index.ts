import { http } from '@/api/http'
import { getEnumKey, AttributeTypeEnum } from '@/enums/commonEnum'

export default {
  /**
   * 获取缓存的用户信息列表
   * @returns 用户 id、code、name、email、isSuper 基本信息列表
   */
  getUserWithCache: () => http.get<API.UserInfoCache[]>(`/SysUser/GetAllUserWithCache`),

  /**
   * 获取工厂缓存数据
   * @returns eg: [{ id: 1, code: '1', name: '1' }, ...]
   */
  getFactoryWithCache: () => http.get<cacheCommonModel[]>('/Cache/GetFactoryWithCache'),

  /**
   * 获取产线缓存数据
   * @returns eg: [{ id: 1, code: '1', name: '1' }, ...]
   */
  getLineWithCache: () => http.get<cacheCommonModel[]>('/Cache/GetLineWithCache'),

  /**
   * 获取工序缓存数据
   * @returns eg: [{ id: 1, code: '1', name: '1' }, ...]
   */
  getOperationWithCache: () => http.get<cacheCommonModel[]>('/Cache/GetOperationWithCache'),

  /**
   * 获取工位缓存数据
   * @returns eg: [{ id: 1, code: '1', name: '1' }, ...]
   */
  getWorkstationWithCache: () => http.get<cacheCommonModel[]>('/Cache/GetWorkstationWithCache'),

  /**
   * 获取布尔型属性值
   * @param code 属性编码
   * @param type 属性分类
   * @param businessId 业务对象Id(和分类要一致, 为空则表示该属性为全局属性), eg: lineId、partId、workOrderId ...
   * @returns true / false
   */
  getBoolAttrWithCache: (code: string, type: AttributeTypeEnum, businessId?: number) =>
    http.get<boolean>('/Cache/GetBoolAttrWithCache', {
      code,
      type: getEnumKey(AttributeTypeEnum, type),
      businessId,
    }),
}
