declare interface Equipment {
  id: number
  sn: string
  name: string
  /** 规格 */
  spec: string
  /** 型号 */
  model: string
  /** 品牌 */
  brand: string
  inDate: string
}

interface EquipmentInspectionTrackDto {
  /** 点检/保养记录主表id */
  trackId: number
  /** 记录单号 */
  no: string
  /** 计划名称 */
  planName: string
  /** 配置的表单id */
  formId: number
  createTime: string
}

/** 检验项类 */
interface EquipmentItemDto {
  /** 检验项id */
  id: number
  code: string
  name: string
  /** 检验方式名称 */
  modeName: string
  /** 判断基准 */
  datum: string
}
