declare interface UserModel {
  id: number
  loginAccount: string
  realName: string
  avatar: string
  factoryId?: number
  factoryCode?: string
  /** 加密后的工厂代码 */
  factoryCodeEncrypt?: string
  factoryName: string
  orgId?: number
  isSuper: boolean
  phoneNo: string
  email: string
  loginIp: string
  addr: string
  remark: string
  /** 当前用户所拥有的工厂列表 */
  hasAuthFactoryList: hasAuthFactory[]
}

declare interface hasAuthFactory {
  factoryId: number
  factoryCode: string
  factoryName: string
  factoryCodeEncrypt: string
}

declare interface HasSubscribeReport {
  subType: 'Report' | 'Message'
  subId: number
  isSub: boolean
  name: string
  orderSort: number
  remark?: string
}
