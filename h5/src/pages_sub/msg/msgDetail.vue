<template>
  <view>
    <z-paging
      ref="paging"
      use-chat-record-mode
      safe-area-inset-bottom
      v-model="dataList"
      @query="queryList"
    >
      <template #top>
        <custom-nav-bar :title="title" :show-left="true" />
      </template>
      <tm-app>
        <view v-for="(item, ind) in dataList" :key="ind" style="transform: scaleY(-1)">
          <view v-if="item.isShowTime">
            <tm-text
              class="flex _u_justify-center"
              :font-size="20"
              color="grey"
              :label="formatMsgTime(item.time)"
            ></tm-text>
            <msg-item :content="item.content"></msg-item>
          </view>
          <msg-item v-else :content="item.content"></msg-item>
        </view>
      </tm-app>
    </z-paging>
  </view>
</template>
<script lang="ts" setup>
  import CustomNavBar from '@/components/CustomNavBar/index.vue'
  import MsgItem from '@/components/MsgItem/index.vue'
  import zPaging from 'z-paging/components/z-paging/z-paging.vue'
  import { getMsgItemList } from '@/api/msg'
  import { useRequest } from 'alova'
  import { formatMsgTime } from '@/utils/date'

  onLoad((query: { id: string; name: string }) => {
    titleId.value = +query.id
    title.value = query.name
  })

  const { send: sendGetMsgItemList } = useRequest(getMsgItemList, {
    immediate: false
  })

  const paging = ref<any>(null)
  const titleId = ref(0)
  const title = ref('')
  /** 注意这个值不要手动赋值 */
  const dataList = ref<MsgItemModel[]>([])

  // @query所绑定的方法不要自己调用！！需要刷新列表数据时，只需要调用paging.value.reload()即可
  const queryList = (pageNo, pageSize) => {
    sendGetMsgItemList({ titleId: titleId.value, pageIndex: pageNo, pageSize }).then(data => {
      paging.value?.complete(data)
    })
  }
</script>

<style lang="scss" scoped></style>
