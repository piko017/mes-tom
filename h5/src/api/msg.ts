import { BaseResponse } from '@/@type/global'
import { request } from '@/utils/http'

/**
 * 获取用户消息列表
 * @returns
 */
export const getMsgGroup = () => request.Get<MsgGroupModel[]>('/AppMsg/GetMsgGroup')

/**
 * 分页获取消息明细列表
 * @param data
 * @returns
 */
export const getMsgItemList = ({ titleId, pageIndex, pageSize }) =>
  request.Get<MsgItemModel[]>(
    `/AppMsg/GetMsgItem?titleId=${titleId}&pageIndex=${pageIndex}&pageSize=${pageSize}`
  )

/** 获取未读消息数量 */
export const getUnReadCount = () => request.Get<number>('/AppMsg/GetUnReadCount')
