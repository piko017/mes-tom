import { BaseResponse } from '@/@type/global'
import { request } from '@/utils/http'

/**
 * 获取用户拥有的kpi列表
 * @returns
 */
export const getUserKpi = () => request.Get<UserKpiModel[]>('/AppKpi/GetUserKpi')
