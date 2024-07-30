<template>
  <tm-app>
    <tm-sheet>
      <view class="flex flex-col flex-col-top-center">
        <tm-virtual-list
          :scroll-view-in-to="position"
          :load="load"
          :width="626"
          :height="900"
          :data="dataList"
          :item-height="100"
          class="list"
        >
          <template #default="{ data }">
            <tm-sheet
              :border="1"
              border-direction="bottom"
              :height="140"
              :width="626"
              _class="flex flex-row flex-row-center-start"
              :padding="[0, 0]"
              :margin="[0, 0]"
              v-for="(item, index) in data"
              :key="index"
            >
              <view class="flex flex-row flex-row-center-between flex-1">
                <view class="left flex">
                  <tm-text _class="text-weight-b" class="_u_mb-1" :label="item.name"></tm-text>
                </view>
                <view class="right">
                  <Iconify
                    v-if="!item.isSub"
                    icon="i-icon-park-outline-plus-cross"
                    @click="handleAddSub(item.subId)"
                  ></Iconify>
                  <Iconify
                    v-else
                    icon="i-icon-park-outline-minus"
                    @click="handleRemoveSub(item.subId)"
                  ></Iconify>
                </view>
              </view>
            </tm-sheet>
          </template>
        </tm-virtual-list>
      </view>
    </tm-sheet>
  </tm-app>
</template>
<script lang="ts" setup>
  import { Iconify } from '@/components/Iconify'
  import { useRequest } from 'alova'
  import { getUserSubList, addSub, removeSub } from '@/api/auth'

  const { send: getMsgList } = useRequest(getUserSubList('Message'), { immediate: false })
  const { send: sendAddSub } = useRequest(addSub, { immediate: false })
  const { send: sendRemoveSub } = useRequest(removeSub, { immediate: false })

  /** 滚动到的位置 */
  const position = ref<string>('') as unknown as '' | 'top' | 'bottom'
  const dataList = ref<HasSubscribeReport[]>([])

  const load = async () => {
    const data = await getMsgList()
    dataList.value = data
    return true
  }

  /** 添加消息订阅 */
  const handleAddSub = async (id: number) => {
    sendAddSub(id, 'Message').then(_ => {
      const item = dataList.value.find(item => item.subId === id)
      if (item) item.isSub = true
    })
  }

  /** 移除消息订阅 */
  const handleRemoveSub = async (id: number) => {
    sendRemoveSub(id, 'Message').then(_ => {
      const item = dataList.value.find(item => item.subId === id)
      if (item) item.isSub = false
    })
  }
</script>

<style lang="scss" scoped>
  .list {
    height: calc(100vh - 200rpx);
  }
</style>
