import { PropType } from 'vue'

export const gridItemProps = {
  /** 图标名称 , 格式eg: i-icon-park-outline-iphone*/
  icon: { type: String as PropType<string>, default: 'user' },
  /** 图标颜色 */
  color: { type: String as PropType<string>, default: '#13c2c2' },
  /** 显示的文本 */
  label: { type: String, default: '默认文本' },
  fn: { type: Function, default: () => {} }
}

export type GridItemProps = ExtractPropTypes<typeof gridItemProps>
