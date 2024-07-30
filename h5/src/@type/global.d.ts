import { ResultEnum } from '@/enums/httpEnum'

declare interface BaseResponse<T = any> {
  status: number
  success: boolean
  msg: string
  msgDev: string
  data: T
}

/** 基础分页类 */
declare interface PageInfo {
  pageIndex: number
  pageSize: number
}
