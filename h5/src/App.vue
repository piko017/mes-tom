<script setup lang="ts">
  import { onLaunch, onShow, onHide } from '@dcloudio/uni-app'
  import { useAuthStore } from '@/state/modules/auth'
  onLaunch(() => {
    console.log('App Launch')
  })
  onShow(() => {
    const authStore = useAuthStore()
    authStore.initToken()
    console.log('App Show')
  })
  onHide(() => {
    console.log('App Hide')
  })
</script>
<style lang="scss">
  /* #ifdef APP-NVUE */
  @import './tmui/scss/nvue.css';
  /* #endif */
  /* #ifndef APP-NVUE */
  @import './tmui/scss/noNvue.css';
  /* #endif */
  @import '@/assets/main.scss';

  //引入iconfont
  @import '@/static/myFont/iconfont.css';
</style>
