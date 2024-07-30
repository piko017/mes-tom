import { EquipmentInspectionEnum } from '@/enums/commonEnum'
import { getBaseUrl } from '@/utils/env'
import { request, UploadFile } from '@/utils/http'

/**
 * 根据设备sn获取设备信息
 * @param sn 设备序列号
 * @returns
 */
export const getBySn = (sn: string) => request.Get<Equipment>(`/Equipment/GetBySn?sn=${sn}`)

/**
 * 根据设备sn获取当前用户拥有的待点检/保养记录单信息列表
 * @param sn 设备序列号
 * @param type 类型
 * @returns
 */
export const getToDoTrackNo = (sn: string, type: keyof typeof EquipmentInspectionEnum) =>
  request.Get<EquipmentInspectionTrackDto[]>(`/Equipment/GetToDoTrackNo?sn=${sn}&type=${type}`)

/**
 * 根据计划表单id获取待点检/保养的明细项列表
 * @param formId 计划表单id
 * @returns
 */
export const getItemListByFormId = (formId: number) =>
  request.Get<EquipmentItemDto[]>(`/Equipment/GetItemListByFormId?formId=${formId}`)

/**
 * 取消计划
 * @param trackId 记录id
 * @param reason 取消原因
 * @returns
 */
export const cancelPlan = (trackId: number, reason: string) =>
  request.Patch<boolean>(`/Equipment/CancelPlan?trackId=${trackId}&reason=${reason}`)

/** 提交表单项目数据 */
export const submitPlanForm = (data: any) =>
  request.Post<boolean>(`/Equipment/SubmitPlanForm`, data)

/** 上传图片接口地址 */
export const uploadImgUrl = `${getBaseUrl()}/Equipment/UploadImg`
