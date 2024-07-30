import { useRequest } from 'alova'
import { getAppLastVersion } from '@/api/auth'
import { getBaseUrl } from '@/utils/env'

/** APP自动更新 */
export const useAutoUpdate = () => {
  /** 当前版本号 */
  const version = ref('1.0.0')
  const downloadUrl = ref('')
  /** 下载进度 */
  const downloadNum = ref(0)

  const { send: sendGetLastVersion } = useRequest(getAppLastVersion, { immediate: false })

  const androidUpdate = async () => {
    // #ifdef APP-PLUS
    plus.runtime.getProperty(plus.runtime.appid || '', async wgtinfo => {
      version.value = wgtinfo.version || '1.0.0'
      console.log(`当前app版本信息: ${version.value}`)
      await getLastVersion()
    })
    // #endif
  }

  const getLastVersion = async () => {
    const data = await sendGetLastVersion()
    if (!data.id) return
    downloadUrl.value = data.downloadUrl
    if (data.versionNo > version.value) {
      uni.showModal({
        title: `发现新版本${data.versionNo}, 是否更新`,
        success: res => {
          if (res.confirm) {
            downApk()
          }
        }
      })
    }
  }

  const downApk = () => {
    uni.showLoading({
      title: '更新中...'
    })
    const downloadTask = uni.downloadFile({
      url: `${getBaseUrl()}${downloadUrl.value}`,
      timeout: 1000 * 60, // 1分钟
      success: downloadResult => {
        uni.hideLoading()
        if (downloadResult.statusCode == 200) {
          uni.showModal({
            title: '',
            content: '更新成功，确定现在重启吗?',
            confirmText: '重启',
            confirmColor: '#EE8F57',
            success: res => {
              if (res.confirm) {
                plus.runtime.install(
                  //安装
                  downloadResult.tempFilePath,
                  {
                    force: true
                  },
                  () => {
                    uni.showToast({
                      title: '更新成功，重启中'
                    })
                    plus.runtime.restart()
                  }
                )
              }
            }
          })
        }
      },
      fail: err => {
        uni.hideLoading()
        uni.showToast({
          title: err.errMsg || '更新失败!'
        })
        console.log(err)
      }
    })
    // 下载进度
    downloadTask.onProgressUpdate(res => {
      downloadNum.value = res.progress
    })
  }

  const init = () => {
    uni.getSystemInfo({
      success: res => {
        if (res.platform === 'android') {
          androidUpdate()
        }
      }
    })
  }
  init()
}
