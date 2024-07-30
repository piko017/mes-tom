<template>
  <view>
    <tm-app>
      <custom-header :is-home="true" />
      <tm-roll-notice
        :follow-theme="true"
        icon="tmicon-tongzhi"
        list="测试滚动测试滚动测试滚动测试滚动测试滚动"
      ></tm-roll-notice>
      <custom-tab-bar :active-index="0"> </custom-tab-bar>
    </tm-app>
  </view>
</template>

<script setup lang="ts">
  import CustomTabBar from '@/components/CustomTabBar/index.vue'
  import CustomHeader from '@/components/CustomHeader/index.vue'
  import { language } from '@/tmui/tool/lib/language'
  import { Iconify } from '@/components/Iconify'
  import { useWindowInfo } from '@/tmui/tool/useFun/useWindowInfo'
  import { useRequest } from 'alova'
  import { useAuthStore } from '@/state/modules/auth'
  import { useThemeColor } from '@/hooks/theme/useThemeColor'
  import { formatMsgDate } from '@/utils/date'

  const router = useRouter()
  const authStore = useAuthStore()
  const winSize = useWindowInfo()
  const { width, height } = winSize
  const left = ref(uni.$tm.u.torpx(width) - 120)
  const top = ref(uni.$tm.u.torpx(height) - 400)
  const { color: themeColor } = useThemeColor()
  const dataList = ref<MsgGroupModel[]>([])
</script>
<style scoped lang="scss">
  .content {
    height: 100vh;
    overflow-y: scroll;
  }

  .adsorb {
    width: 120rpx;
    height: 120rpx;
    border-radius: 120rpx;
  }

  .gItem {
    position: relative;
  }

  .icon {
    position: absolute;
    right: -10rpx;
    top: 7rpx;
    z-index: 10;
  }

  #msg-sheet {
    margin-top: -20rpx !important;
  }
</style>
