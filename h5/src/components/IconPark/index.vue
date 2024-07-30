<template>
  <component
    class="z-icon"
    :is="`${prefix}${name}`"
    :size="size"
    :fill="useThemeColor ? themeColor : fill"
  ></component>

  <!-- 兼容小程序写法 -->
  <!-- <template v-if="name == 'CheckOne'">
    <CheckOne class="z-icon" :size="size" :fill="useThemeColor ? themeColor : fill" />
  </template>
  <template v-if="name == 'Check'">
    <Check class="z-icon" :size="size" :fill="useThemeColor ? themeColor : fill" />
  </template>
  <template v-if="name == 'Iphone'">
    <Iphone class="z-icon" :size="size" :fill="useThemeColor ? themeColor : fill" />
  </template>
  <template v-if="name == 'Platte'">
    <Platte class="z-icon" :size="size" :fill="useThemeColor ? themeColor : fill" />
  </template>
  <template v-else>
    <Iphone class="z-icon" :size="size" :fill="useThemeColor ? themeColor : fill" />
  </template> -->
</template>
<script lang="ts" setup name="IconPark">
  import { PropType } from 'vue'
  import { useThemeColor as _useThemeColor } from '@/hooks/theme/useThemeColor'
  // import { CheckOne, Check, Iphone, Platte } from '@icon-park/vue-next'

  const { color: themeColor } = _useThemeColor()

  defineProps({
    // iconpark图标名称
    // 参考：https://iconpark.oceanengine.com/official
    /** 注意格式要使用大驼峰命名(为了兼容小程序), 例如：UserFilled */
    name: {
      type: String
    },
    // iconpark图标前缀
    prefix: {
      type: String,
      default: 'icon-park-' // 默认前缀(小写)
    },
    size: {
      type: [Number, String],
      default: 18
    },
    // iconpark图标填充颜色
    // 例如：#333333
    fill: {
      type: [String, Array] as PropType<string | string[]>
    },
    // 是否使用主题色
    useThemeColor: {
      type: Boolean,
      default: true
    }
  })
</script>
<style lang="scss" scoped>
  .z-icon {
    margin-top: auto;
    margin-bottom: auto;
  }
</style>
