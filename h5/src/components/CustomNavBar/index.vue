<template>
  <view
    :style="{ height: navHeight, fontSize: navFontSize }"
    class="_u_flex _u_items-center"
    :class="{ '_u_justify-center': !showLeft, nav: showLeft }"
  >
    <view class="_u_flex _u_justify-start left" @click="handleLeft">
      <slot name="left" v-if="showLeft">
        <tm-icon :font-size="25" color="#799" class="_u_mt-0.2" :name="leftIcon"></tm-icon>
        <tm-text :font-size="28" _class="ml-10" v-if="leftText">{{ leftText }}</tm-text>
      </slot>
    </view>
    <view class="title">
      <slot name="center">
        <tm-text :font-size="30" _class="text-weight-b">{{ title }}</tm-text>
      </slot>
    </view>
    <view class="_u_flex _u_justify-end right" @click="emit('click-right')">
      <slot name="right">
        <view></view>
      </slot>
    </view>
  </view>
</template>
<script lang="ts" setup>
  import { language } from '@/tmui/tool/lib/language'

  defineProps({
    title: {
      type: String,
      default: '标题'
    },
    leftIcon: {
      type: String,
      default: 'tmicon-angle-left'
    },
    leftText: {
      type: String,
      default: language('common.back')
    },
    /** 是否显示左侧内容, 默认显示 */
    showLeft: {
      type: Boolean,
      default: true
    }
  })

  const navHeight = ref('')
  const navFontSize = ref('')

  onLoad(() => {
    const { platform, statusBarHeight } = uni.getSystemInfoSync()
    // console.log(platform, statusBarHeight)
    if (platform == 'android') {
      navHeight.value = `${(statusBarHeight ?? 0) + 48}px`
      navFontSize.value = '16px'
    } else {
      navHeight.value = `${(statusBarHeight ?? 0) + 44}px`
      navFontSize.value = '18px'
    }
  })

  const emit = defineEmits(['click-left', 'click-right'])

  const handleLeft = () => uni.navigateBack()
</script>

<style lang="scss" scoped>
  .nav {
    .left,
    .right {
      flex-grow: 1;
      padding: 0 20rpx;
    }
    .title {
      flex-grow: 1;
      padding-right: 70rpx;
      display: flex;
      justify-content: center;
    }
  }
</style>
