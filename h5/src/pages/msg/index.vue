<template>
  <view>
    <tm-app>
      <custom-nav-bar :title="language('common.msg')"></custom-nav-bar>
      <tm-sheet :margin="[0]">
        <tm-input
          :search-width="120"
          :height="70"
          @search="search"
          prefix="tmicon-search"
          :placeholder="language('common.searchTips')"
          :search-label="language('common.search')"
          :margin="[10, 0]"
        ></tm-input>
        <tm-divider></tm-divider>

        <view
          v-if="!state && dataList.length > 0"
          class="flex _u_justify-center"
          @click="handleConn"
        >
          <Iconify icon="i-icon-park-outline-error-prompt" :use-theme-color="false" color="red" />
          <tm-text>消息服务断开~~ 点击重试!</tm-text>
        </view>
        <view
          v-if="dataList.length == 0"
          class="flex _u_justify-center _u_items-center _u_flex-col"
        >
          <tm-text class="_u_mt-8" label="您还未订阅任何消息!"></tm-text>
          <!-- <tm-button @click="handleGoSub" label="去订阅"></tm-button> -->
        </view>
        <view v-else class="flex flex-col flex-col-top-center">
          <tm-virtual-list
            :scroll-view-in-to="position"
            :load="load"
            :width="700"
            :height="1200"
            :data="dataList"
            :item-height="100"
            class="list"
          >
            <template #default="{ data }">
              <tm-sheet
                :border="1"
                border-direction="bottom"
                :height="140"
                :width="700"
                _class="flex flex-row flex-row-center-start"
                :padding="[0, 0]"
                :margin="[0, 0]"
                v-for="(item, index) in data as MsgGroupModel[]"
                :key="index"
                @click="handleGoDetail(item)"
              >
                <view class="flex flex-row flex-row-center-between flex-1">
                  <view class="left flex">
                    <tm-badge :max-count="99" color="red" :count="item.unReadCount">
                      <Iconify
                        :size="80"
                        :icon="item.imgUrl || 'i-icon-park-outline-message-one'"
                      />
                    </tm-badge>
                    <view class="_u_ml-3">
                      <tm-text _class="text-weight-b" class="_u_mb-1" :label="item.title"></tm-text>
                      <tm-text
                        :font-size="18"
                        _class="text-overflow-1"
                        :label="item.lastContent"
                      ></tm-text>
                    </view>
                  </view>
                  <view class="right">
                    <tm-text :label="formatMsgDate(item.lastTime)"></tm-text>
                  </view>
                </view>
              </tm-sheet>
            </template>
          </tm-virtual-list>
        </view>
      </tm-sheet>
    </tm-app>
  </view>
</template>

<script setup lang="ts">
  import CustomTabBar from '@/components/CustomTabBar/index.vue'
  import CustomNavBar from '@/components/CustomNavBar/index.vue'
  import { language } from '@/tmui/tool/lib/language'
  import { Iconify } from '@/components/Iconify'
  import { getWebSocketUrl } from '@/utils/env'
  import { useUniWebSocket } from '@/hooks/app/useUniWebSocket.hook'
  import { useAuthStore } from '@/state/modules/auth'
  import { getMsgGroup } from '@/api/msg'
  import { useRequest } from 'alova'
  import { formatMsgDate } from '@/utils/date'

  const router = useRouter()
  const authStore = useAuthStore()
  const WS_URL = getWebSocketUrl()
  /** 滚动到的位置 */
  const position = ref<string>('') as unknown as '' | 'top' | 'bottom'
  const dataList = ref<MsgGroupModel[]>([])
  /** 消息服务器连接状态 */
  const state = ref(false)

  const { onSuccess } = useRequest(getMsgGroup)
  onSuccess(res => {
    dataList.value = res.data
  })

  const search = () => {
    console.log('搜索')
  }
  const load = (): boolean => {
    // console.log('加载...')
    return true
  }

  // 处理接收到的最新消息
  const calcMsg = data => {
    console.log('计算消息...', data)
  }

  const handleConn = () => {
    // TODO: 重新链接ws
    setTimeout(() => {
      state.value = true
      console.log('重新链接ws')
    }, 1000)
  }

  const handleGoSub = () => router.push({ path: 'pages_sub/sys/msgList/index' })

  /** 跳转到消息明细页面 */
  const handleGoDetail = (item: MsgGroupModel) =>
    router.push({
      path: `pages_sub/msg/msgDetail`,
      query: { id: `${item.id}`, name: `${item.title}` }
    })

  onMounted(() => {
    if (!authStore.isLogin || !authStore.getUserInfo?.id) return

    const { isConnected, send } = useUniWebSocket(
      `${WS_URL}?userId=${authStore.getUserInfo?.id}`,
      calcMsg
    )

    // 监听连接状态
    watch(
      isConnected,
      newVal => {
        state.value = newVal
      },
      {
        immediate: true
      }
    )
  })
</script>
<style scoped lang="scss">
  .list {
    height: calc(100vh - 260rpx);
    overflow-y: scroll !important;

    uni-scroll-view {
      height: calc(100vh - 260rpx);
    }
  }
</style>
