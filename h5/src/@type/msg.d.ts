declare interface MsgGroupModel {
  id: number
  title: string
  imgUrl: string
  lastContent: string
  lastTime: string
  unReadCount: number
}

declare interface MsgItemModel {
  content: string
  time: string
  isShowTime: boolean
}
