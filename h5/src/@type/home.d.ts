declare interface MsgModel {
  id: number
  /** 图标 */
  icon: string
  /** 消息标题(类型) */
  title: string
  /** 未读总数 */
  total: number
  /** 消息 */
  msg: string
  /** 时间 */
  time: string
}

declare interface MsgItemModel {
  id: number
  /** 消息内容 */
  content: string
  /** 消息时间 */
  time: string
  /** 头像地址 */
  avatar?: string
}

interface GridCardType {
  id: number
  label: string
  icon?: string
  /** 自定义的图标颜色(为空默认使用主题色) */
  color?: string
  items: GridItemType[]
}

interface GridItemType {
  id: number
  label: string
  icon: string
  /** 自定义的图标颜色(为空默认使用主题色) */
  color?: string
  // 自定义函数(单击事件)
  fnStr?: string
  /** 权限角色id集合 */
  sysRoleIds?: string[]
}
