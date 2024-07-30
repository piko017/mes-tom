import { randomColor } from '@/utils/common'

/**
 * 获取枚举key
 * @param enumObj 枚举对象
 * @param value 枚举值
 * @returns Key
 */
export function getEnumKey<T extends object>(enumObj: T, value: T[keyof T]): keyof T {
  const key = Object.keys(enumObj).find(_key => enumObj[_key] === value) || ''
  return key as keyof T
}

/**
 * 获取key对应的枚举下标
 * @param enumObj 枚举
 * @param key key
 * @returns 下标
 */
export const getEmunIndex = (enumObj: Object, key: string): number => {
  return Object.keys(enumObj).findIndex(_key => _key === key)
}

/** 枚举转换成下拉框options */
export const transformEnumToOptions = (
  enumObj: Object
): Array<{ label: string; value: string }> => {
  return Object.keys(enumObj).map(key => ({
    label: enumObj[key] as string,
    value: key
  }))
}

/** 属性类型枚举 */
export enum AttributeTypeEnum {
  'Global' = '全局',
  'Workshop' = '车间',
  'Line' = '产线',
  'Workstation' = '工位',
  'ProcessOperation' = '工序',
  'Part' = '产品料号',
  'WorkOrder' = '工单',
  'Product' = '产品',
  'Equipment' = '设备'
}

/** 工单状态枚举 */
export enum WorkOrderStatusEnum {
  'Initial' = '初始化',
  'Open' = '已开工',
  'Pending' = '暂停',
  'Complete' = '完工',
  'Close' = '结案'
}

/** 设备基础状态 */
export enum EquipmentStatusEnum {
  'Normal' = '正常',
  'NG' = '故障',
  'Repair' = '维修',
  'Scrap' = '报废'
}

/** 设备点检保养类型 */
export enum EquipmentInspectionEnum {
  'Inspection' = '点检',
  'Maintenance' = '保养'
}

/** 设备点检保养执行状态枚举 */
export enum EquipmentMaintenanceStatusEnum {
  'Unexecuted' = '未作业',
  'Executed' = '已作业',
  'TimeOutUnexecuted' = '超时未作业',
  'TimeOutExecuted' = '超时已作业',
  'Canceled' = '已取消'
}

/** 设备维修状态 */
export enum EquipmentRepairStatusEnum {
  'UnRepair' = '未维修',
  'Repaired' = '已维修'
}

/** 设备点检保养结果状态 */
export enum ResultStatusEnum {
  'OK' = 'OK',
  'NG' = 'NG'
}
