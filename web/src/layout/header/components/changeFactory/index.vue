<template>
  <Select
    v-if="selectFactoryId && showSelect"
    ref="selectRef"
    v-model:value="selectFactoryId"
    style="width: 130px"
    size="small"
    :options="options"
    @change="handleChangeFactory"
    @dropdown-visible-change="handleDropdownVisibleChange"
  ></Select>
  <span v-if="selectFactoryId && !showSelect && factoryName"
    >{{ factoryName }}
    <Tooltip :title="$t('common.changeFactory')" placement="bottom"
      ><SwapOutlined @click="handleIcon" /></Tooltip
  ></span>
</template>

<script lang="ts" setup>
  import { ref, computed, watch } from 'vue'
  import { useRouter } from 'vue-router'
  import { Select, Tooltip } from 'ant-design-vue'
  import { useUserStore } from '@/store/modules/user'
  import { ACCESS_TOKEN_KEY } from '@/enums/cacheEnum'
  import { Storage } from '@/utils/Storage'
  import { SwapOutlined } from '@ant-design/icons-vue'
  import { onClickOutside } from '@vueuse/core'
  import { changeFactory } from '@/api/login'
  import action from '@/api/sys/user'

  const userStore = useUserStore()
  const router = useRouter()
  const selectRef = ref(null)
  const selectFactoryId = ref<number>(0)
  const showSelect = ref<boolean>(false)
  const isOpenSelect = ref<boolean>(false)

  watch(
    () => userStore.userInfo.factoryId,
    newVal => {
      if (newVal) {
        selectFactoryId.value = newVal
      }
    },
    { immediate: true },
  )

  onClickOutside(selectRef, () => {
    if (!isOpenSelect.value) showSelect.value = false
  })

  const factoryName = computed(() => userStore.userInfo.factoryName)

  const options = computed(() =>
    userStore.userInfo.hasAuthFactoryList?.map(n => ({
      label: n.factoryName,
      value: n.factoryId,
    })),
  )

  const handleIcon = () => (showSelect.value = true)

  const handleDropdownVisibleChange = (open: boolean) => {
    isOpenSelect.value = open
  }

  /** 切换工厂 */
  const handleChangeFactory = async () => {
    const oldToken = Storage.get(ACCESS_TOKEN_KEY, null)
    const tokenInfo = await changeFactory(selectFactoryId.value, oldToken)
    // eslint-disable-next-line camelcase
    const { token, expires_timestamp } = tokenInfo
    userStore.setToken(token, expires_timestamp)
    const userInfo = await action.getInfo()
    userStore.setUserInfo(userInfo)
    // 刷新当前路由
    router.go(0)
  }
</script>
<style lang="less" scoped></style>
