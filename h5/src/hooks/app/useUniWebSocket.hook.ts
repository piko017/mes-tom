import { ref } from 'vue'

/** webSocket 连接池, key是url */
const connectionMap = new Map<string, UniApp.SocketTask>()

/**
 * useUniWebSocket
 * @param url ws地址
 * @param callback 数据回调
 */
export const useUniWebSocket = (url: string, callback?: (data: any) => void) => {
  const socket = ref<UniApp.SocketTask>()
  const isConnected = ref(connectionMap.has(url))
  const data = ref<any>()

  /** 初始化WebSocket连接 */
  const init = () => {
    socket.value =
      connectionMap.get(url) ??
      uni.connectSocket({
        url,
        success: () => {
          console.log(`WebSocket连接已初始化:${url}`)
        }
      })
  }

  init()

  socket.value?.onOpen(() => {
    isConnected.value = true
    connectionMap.set(url, socket.value!)
    console.log('WebSocket连接已打开')
  })

  /** 监听最新消息 */
  socket.value?.onMessage(res => {
    data.value = res.data
    if (callback) {
      callback(res.data)
    }
  })

  socket.value?.onClose(() => {
    isConnected.value = false
    connectionMap.delete(url)
    console.log('WebSocket连接已关闭')
  })

  /** 关闭连接 */
  const close = () => socket.value?.close({ code: 1000 })

  /** 向服务器发送数据 */
  const send = (msg: string) =>
    socket.value?.send({
      data: msg
    })

  return {
    isConnected,
    data,
    send,
    close
  }
}
