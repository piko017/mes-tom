<template>
  <tm-app>
    <tm-sheet>
      <tm-input
        v-model.lazy="sn"
        :search-width="120"
        :focus="true"
        placeholder="设备序列号"
        search-label="扫描"
        @search="handleScan()"
        @confirm="handleConfirm()"
      ></tm-input>
    </tm-sheet>
    <tm-sheet v-if="list.length" :margin="[32, 0]">
      <tm-descriptions
        :data="list"
        title="设备信息"
        :label-width="150"
        :font-size="40"
        :margin="[50, 0]"
        :column="1"
      ></tm-descriptions>
    </tm-sheet>

    <tm-card v-for="item in trackList" :key="item.trackId" title="单号" :margin="[32, 24, 32, 0]">
      <template #status>
        <tm-text
          :label="item.no"
          _class="text-weight-b"
          color="primary"
          @click="goFormItem(item.trackId, item.formId)"
        ></tm-text>
      </template>
      <template #content>
        <tm-text :label="`计划名称: ${item.planName}`" _class="text-overflow-1"></tm-text>
        <tm-text
          _class="text-overflow-1"
          class="_u_ml-9"
          :label="`生成时间: ${item.createTime.substring(5)}`"
        ></tm-text>
      </template>
    </tm-card>
  </tm-app>
</template>
<script lang="ts" setup>
  import { PropType, ref } from 'vue'
  import { getBySn, getToDoTrackNo } from '@/api/equipment'
  import { useRequest } from 'alova'
  import { EquipmentInspectionEnum } from '@/enums/commonEnum'
  import { useScan } from '@/hooks/scan'

  const props = defineProps({
    type: {
      type: String as PropType<keyof typeof EquipmentInspectionEnum>,
      default: EquipmentInspectionEnum.Inspection
    }
  })

  const router = useRouter()
  const { send: sendGetBySn } = useRequest(getBySn, { immediate: false })
  const { send: sendGetToDoTrackNo } = useRequest(getToDoTrackNo, { immediate: false })

  const sn = ref('')
  const list = ref<SelectOptionModel[]>([])
  const trackList = ref<EquipmentInspectionTrackDto[]>([])
  const equipmentInfo = ref<Equipment>()

  watch(
    () => equipmentInfo.value,
    newVal => {
      if (!newVal) return
      list.value = [
        { label: '设备序列号', value: newVal.sn },
        { label: '设备名称', value: newVal.name },
        { label: '设备型号', value: newVal.model },
        { label: '设备规格', value: newVal.spec },
        { label: '设备品牌', value: newVal.brand },
        { label: '入厂日期', value: newVal.inDate?.substring(0, 10) }
      ]
    }
  )

  const { isScaning } = useScan(async val => {
    sn.value = val
    await handleConfirm()
  })

  const handleScan = () => {
    isScaning.value = true
  }

  const handleConfirm = async () => {
    if (!sn.value) return
    const [equipment, tracks] = await Promise.all([
      sendGetBySn(sn.value),
      sendGetToDoTrackNo(sn.value, props.type)
    ])
    trackList.value = tracks
    equipmentInfo.value = equipment
    sn.value = ''
  }

  /** 跳转到表单明细页面 */
  const goFormItem = (trackId: number, formId: number) => {
    router.push({
      path: 'pages_sub/sys/equipment/components/form',
      query: { trackId: `${trackId}`, formId: `${formId}` }
    })
  }
</script>

<style lang="scss" scoped></style>
