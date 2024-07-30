<template>
  <tm-app>
    <view class="header-container">
      <view class="left-box">
        <tm-avatar :img="img" :round="3"></tm-avatar>
        <view class="info-box">
          <h5>{{ userInfo?.realName }}</h5>
          <p>{{ userInfo?.factoryName }}</p>
        </view>
      </view>
      <IconPark name="addOne"></IconPark>
    </view>
  </tm-app>
</template>

<script lang="ts" setup name="Header">
  import { useAuthStore } from '@/state/modules/auth'
  import { renderImg } from '@/utils/file'
  import IconPark from '@/components/IconPark/index.vue'
  const authStore = useAuthStore()

  const userInfo = computed(() => authStore.getUserInfo)

  const img = renderImg(userInfo.value?.avatar ?? 'https://picsum.photos/200/300?id=66')
</script>

<style lang="scss" scoped>
  .header-container {
    width: 100%;
    height: 120rpx;
    display: flex;
    align-items: center;
    padding: 20rpx 20rpx 0;
    justify-content: space-between;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;

    .left-box {
      display: flex;
      align-items: center;

      .info-box {
        padding-left: 50rpx;

        h3 {
          line-height: 20rpx;
        }

        p {
          color: #bbbbbb;
          font-size: x-small;
        }
      }
    }
  }
</style>
