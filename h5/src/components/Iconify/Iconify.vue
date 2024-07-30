<template>
  <view ref="elRef" @click="onClick" :class="['iconify', icon]" :style="style"></view>
</template>
<script lang="ts" setup name="Iconify">
  import { assign } from 'lodash-es'
  import { isBoolean } from '@/utils/is'
  import { useThemeColor as _useThemeColor } from '@/hooks/theme/useThemeColor'
  import { useTmpiniaStore } from '@/tmui/tool/lib/tmpinia'

  const props = defineProps({
    /** 图标名称,, 格式eg: i-icon-park-outline-iphone */
    icon: {
      type: String
    },
    size: {
      type: [Number, String],
      default: 36
    },
    color: {
      type: String
    },
    /** 是否跟随系统主题色 */
    useThemeColor: {
      type: Boolean,
      default: true
    }
  })

  const store = useTmpiniaStore()
  const { color: themeColor } = _useThemeColor()
  const iconSize = computed<string | boolean>(() => (props.size ? `${props.size}rpx` : false))
  const style = computed(() => {
    const ISize = unref(iconSize)
    let color = props.useThemeColor ? themeColor.value : props.color
    if (!color && store.tmStore.dark) {
      color = '#fff'
    }
    return assign(
      { width: isBoolean(ISize) ? '' : ISize, height: isBoolean(ISize) ? '' : ISize },
      { color }
    )
  })

  const emit = defineEmits(['click'])
  const onClick = () => {
    emit('click')
  }
</script>
<style lang="scss" scoped>
  .iconify {
    // display: inline-block;
    // vertical-align: middle;
    height: 44rpx;
    width: 44rpx;
    color: inherit;
    margin-top: auto;
    margin-bottom: auto;
    &:hover {
      cursor: pointer;
      opacity: 0.8;
    }
  }
</style>
