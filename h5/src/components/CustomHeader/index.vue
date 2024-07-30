<template>
  <tm-sticky>
    <template #sticky>
      <tm-sheet
        :text="false"
        :margin="[0, 0]"
        :padding="[24, 0]"
        :style="{ height: `${(statusBarHeight || 0) + 125}rpx` }"
      >
        <view :style="{ height: statusBarHeight + 'px' }"></view>
        <view class="flex-row flex-row-center-start">
          <view class="flex-col flex-center">
            <tm-avatar
              v-if="img"
              :font-size="70"
              :round="16"
              :img="img"
              :margin="[0, 0]"
            ></tm-avatar>
            <tm-avatar
              v-else
              :font-size="70"
              :round="16"
              img="/static/logo.png"
              :margin="[0, 0]"
            ></tm-avatar>
            <tm-text _class="text-weight-b" :font-size="16" :label="userInfo?.realName"></tm-text>
          </view>
          <view class="flex-1 _u_overflow-hidden" style="width: 0px">
            <tm-sticky :offset="stop">
              <template #sticky>
                <tm-filterMenu
                  ref="filter"
                  :width="showMoreFilterItem ? 1200 : 750"
                  :fixed="true"
                  @click="handleFilterMenu"
                  @close="handleFilterMenuClose"
                >
                  <tm-filterMenu-item :footer-height="100" :height="450" :title="factoryName">
                    <tm-cascader
                      ref="textCascader"
                      @cell-click="handleClick"
                      v-model="factory"
                      :default-value="factory"
                      :data="dataList"
                    ></tm-cascader>
                    <template #footer>
                      <view class="flex flex-row flex-row-between">
                        <view class="pl-24 pr-12 flex-1">
                          <tm-button
                            @click="filter?.close()"
                            :shadow="0"
                            text
                            :outlined="true"
                            block
                            label="取消"
                          ></tm-button>
                        </view>
                        <view class="pr-24 pl-12 flex-1">
                          <tm-button
                            @click="handleConfirm"
                            block
                            :border="1"
                            label="确认"
                          ></tm-button>
                        </view>
                      </view>
                    </template>
                  </tm-filterMenu-item>
                  <!-- 占位符 -->
                  <tm-filterMenu-item v-if="showMoreFilterItem"></tm-filterMenu-item>
                </tm-filterMenu>
              </template>
            </tm-sticky>
          </view>
          <slot name="right">
            <view class="flex flex-row-center-end pb-10"></view>
          </slot>
        </view>
      </tm-sheet>
    </template>
  </tm-sticky>
</template>

<script lang="ts" setup name="CustomHeader">
  import { useAuthStore } from '@/state/modules/auth'
  import { renderImg } from '@/utils/file'
  import { Iconify } from '@/components/Iconify'
  import { useTmpiniaStore } from '@/tmui/tool/lib/tmpinia'
  import { language } from '@/tmui/tool/lib/language'
  import { changeFactory } from '@/api/auth'
  import tmFilterMenu from '@/tmui/components/tm-filterMenu/tm-filterMenu.vue'
  import { useRequest } from 'alova'

  const props = defineProps({
    /** 是否首页, 默认false */
    isHome: {
      type: Boolean,
      default: false
    }
  })

  const { send: sendChangeFactory } = useRequest(changeFactory, { immediate: false })
  const store = useTmpiniaStore()
  const authStore = useAuthStore()

  const filter = ref<InstanceType<typeof tmFilterMenu>>()
  const stop = ref(0)
  // #ifdef H5
  stop.value = uni.$tm.u.torpx(44)
  // #endif
  /** 当前确定选择的工厂 */
  const currentFactory = ref<string[]>([''])
  const factory = ref<string[]>([authStore.getUserInfo?.factoryCode || ''])
  const factoryName = computed(() => {
    const name = authStore.getUserInfo?.hasAuthFactoryList?.find(
      item => item.factoryCode === factory.value[0]
    )?.factoryName
    return name || ''
  })

  /** fix: 弹窗组织选择样式BUG */
  const showMoreFilterItem = ref(true)
  const isConfirm = ref(false)

  const dataList = computed(() => {
    return (authStore.getUserInfo?.hasAuthFactoryList || []).map((item: any) => ({
      text: item.factoryName,
      id: item.factoryCode
    }))
  })

  const userInfo = computed(() => authStore.getUserInfo)

  const { statusBarHeight } = uni.getSystemInfoSync()

  const img = renderImg(userInfo.value?.avatar)

  /** 确认切换工厂 */
  const handleConfirm = () => {
    isConfirm.value = true
    filter.value?.close()
    if (currentFactory.value[0] === factory.value[0]) return
    const factoryId = authStore.getUserInfo?.hasAuthFactoryList?.find(
      item => item.factoryCode === factory.value[0]
    )?.factoryId
    if (!factoryId) return
    sendChangeFactory(factoryId, authStore.getToken).then(data => {
      authStore.setToken(data.token, data.expires_in)
      authStore.updateUserFactory(factoryId)
    })
  }

  const handleClick = item => {
    //console.log(item)
  }

  const handleFilterMenu = () => {
    currentFactory.value = factory.value
    showMoreFilterItem.value = false
  }

  const handleFilterMenuClose = () => {
    if (!isConfirm.value) {
      factory.value = currentFactory.value
    }
    isConfirm.value = false
    showMoreFilterItem.value = true
  }
</script>

<style lang="scss" scoped></style>
