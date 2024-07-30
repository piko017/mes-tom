import { useTmpiniaStore } from '@/tmui/tool/lib/tmpinia'
import { themeColors } from '@/utils/constant'

/**
 * 主题颜色hook
 * @returns 主题颜色
 */
export const useThemeColor = () => {
  const tmStore = useTmpiniaStore()
  const color = computed(
    () =>
      themeColors.find(item => item.key === tmStore.tmStore.color)?.value || themeColors[0].value
  )
  return { color }
}
