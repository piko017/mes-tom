declare interface LoginParams {
  username: string
  password: string
  factoryId?: number
}

/** 登录返回类型 */
declare interface LoginModel {
  token_type: string
  token: string
  /** 多少秒之后过期 */
  expires_in: number
  /** 最终过期的时间戳 */
  expires_timestamp: number
}

/** 选择项类型 */
declare interface SelectModel {
  text: string
  id: string | number
}

declare interface SelectOptionModel {
  label: string
  value: string | number
}

/** APP更新实体 */
declare interface AppUpdateInfo {
  id: number
  versionNo: string
  downloadUrl: string
  remark: string
}
