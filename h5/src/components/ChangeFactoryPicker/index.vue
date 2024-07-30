<template>
  <tm-picker
    v-model:show="showFactorySelect"
    v-model:model-str="data.factoryName"
    v-model="data.factoryId"
    :columns="factoryList"
    selected-model="id"
  ></tm-picker>
</template>

<script lang="ts" setup name="ChangeFactoryPicker">
  import { useAuthStore } from '@/state/modules/auth'
  import { getFactoryList, changeFactory } from '@/api/auth'
  import { useRequest } from 'alova'

  // 外部传入只能生效一次, 后续再查找BUG
  const props = defineProps({
    show: {
      type: Boolean,
      default: false
    }
  })
  const { send: sendGetFactoryList } = useRequest(getFactoryList, { immediate: false })
  const { send: sendChangeFactory } = useRequest(changeFactory, { immediate: false })

  const authStore = useAuthStore()
  const showFactorySelect = ref(false)
  const factoryList = ref<SelectModel[]>([])

  console.log(showFactorySelect.value)
  const data = reactive({
    factoryName: '',
    factoryId: [] as number[]
  })

  watch(
    () => props.show,
    async newValue => {
      if (newValue) {
        const data = await sendGetFactoryList()
        factoryList.value = data
      }
      showFactorySelect.value = newValue
    }
  )

  watch(
    () => data.factoryId,
    async newVal => {
      if (newVal.length > 0) {
        const factoryId = newVal[0] as number
        const data = await sendChangeFactory(factoryId, authStore.getToken)
        authStore.afterLogin(data.token, data.expires_in)
      }
    }
  )
</script>
