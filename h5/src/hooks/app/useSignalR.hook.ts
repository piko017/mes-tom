import { ref } from 'vue'
import { HubConnectionBuilder, HubConnection, HubConnectionState } from '@microsoft/signalr'

/** signalR 连接池, key是url */
const connectionMap = new Map<string, HubConnection | undefined>()

/** 提前初始化signalR连接, 减少相同的url重复连接的问题 */
export const initSignalRConnection = (url: string) => {
  if (connectionMap.get(url)) return
  const connection = new HubConnectionBuilder().withUrl(url).build()
  connectionMap.set(url, connection)
}

/**
 * useSignalR
 * @param url 地址
 * @param channel 频道
 * @param callback 回调函数
 * @returns
 */
export const useSignalR = (url: string, channel: string, callback?: (...args: any) => any) => {
  const connection = ref<HubConnection>()
  const isConnected = ref(false)

  /** 最新数据 */
  const data = ref()

  /** 初始化连接并启动监听 */
  const init = () => {
    connection.value =
      connectionMap.get(url) ??
      new HubConnectionBuilder().withUrl(url).withAutomaticReconnect().build()

    if (connection.value.state === HubConnectionState.Disconnected) {
      connection.value
        .start()
        .then(() => {
          isConnected.value = true
          connectionMap.set(url, connection.value)
          console.log(`${url}「${channel} SignalR connected`)
        })
        .catch((error: any) => {
          console.error(`${url}「${channel} SignalR connection error:`, error)
        })
    }

    // 监听SignalR消息
    connection.value.on(channel, res => {
      try {
        data.value = JSON.parse(res)
      } catch {
        data.value = res
      }
      if (callback) callback(data.value)
    })
  }

  /** 断开SignalR连接 */
  const close = (url: string) => {
    if (connection.value) {
      connection.value.stop().then(_ => (isConnected.value = false))
    }
    connectionMap.delete(url)
    console.log(`${url}「${channel} SignalR disconnected`)
  }

  init()

  return {
    isConnected,
    data,
    close
  }
}
