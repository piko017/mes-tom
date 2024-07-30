<template>
  <view class="flex">
    <view class="left">
      <tm-sheet :padding="[0]" :margin="[32, 25, 0, 24]" :transprent="true">
        <tm-avatar :img="img"></tm-avatar>
      </tm-sheet>
    </view>
    <view class="right">
      <tm-sheet :round="3">
        <tm-text :label="content"></tm-text>
      </tm-sheet>
    </view>
  </view>
</template>

<script lang="ts" setup>
  import { language } from '@/tmui/tool/lib/language'

  defineProps({
    img: {
      type: String,
      default: '/static/logo.png'
    },
    content: {
      type: String,
      default: '欢迎使用Tmui'
    }
  })
</script>
