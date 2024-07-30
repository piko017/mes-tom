import { render, createVNode } from 'vue'
import H5Scan from '@/components/Scan/index.vue'

/**
 * 扫码hooks, 兼容h5和app
 * @param cb 扫描成功后回调函数
 * @returns
 */
export const useScan = (cb: (val: string) => void) => {
  const isScaning = ref(false)

  const onSuccessCb = (val: string) => {
    cb(val)
    isScaning.value = false
  }

  const startScan = async () => {
    if (navigator) {
      // #ifdef H5
      render(<H5Scan open={isScaning.value} onSuccess={onSuccessCb} />, document.body)
      await nextTick()
      // #endif
    } else {
      uni.scanCode({
        success: res => {
          cb(res.result)
        },
        fail: err => {
          // 扫码失败
          uni.showToast({
            title: `识别二维码失败: ${err}`,
            duration: 2000,
            icon: 'none'
          })
        }
      })
    }
  }

  watch(
    () => isScaning.value,
    async val => {
      val && (await startScan())
    }
  )

  return { isScaning }
}
