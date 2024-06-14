import { http } from '@/api/http'

/**
 * 工厂管理接口
 */
export default {
  /**
   * 获取分页列表
   * @param query
   * @returns
   */
  getWithPage: (query: API.PageParams) =>
    http.get<API.TableListResult>('/SysFactory/GetWithPage', query),

  /**
   * 根据Id获取单条
   * @param id
   * @returns
   */
  getById: (id: number) => http.get(`/SysFactory/Get/${id}`),

  /**
   * 新增一条数据
   * @param data
   * @returns
   */
  add: (data: any) => http.post(`/SysFactory/Add`, data),

  /**
   * 删除数据
   * @param ids 主键Ids
   * @returns
   */
  delete: (ids: number[]) => http.delete(`/SysFactory/Delete`, { ids: ids.join(',') }),

  /**
   * 更新数据
   * @param data
   * @returns
   */
  update: (data: any) => http.put(`/SysFactory/Update`, data),

  /**
   * 获取所有工厂列表
   * @returns
   */
  getFactory: () => http.get(`/SysFactory/GetFactory`),
}
