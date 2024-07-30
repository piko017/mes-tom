<template>
  <div>
    <tm-app>
      <tm-sheet>
        <tm-text :font-size="24" _class="text-weight-b" label="项目列表"></tm-text>
        <tm-divider></tm-divider>
        <view v-if="loading">
          <tm-skeleton-line :height="50"></tm-skeleton-line>
          <tm-skeleton-line :height="50"></tm-skeleton-line>
        </view>
        <tm-cell
          v-for="(item, ind) in items"
          :key="item.id"
          :margin="[0]"
          :padding="[24, 5]"
          :title-font-size="25"
          :title="`${ind + 1}. ${item.name}`"
          @click="openModal(item)"
        >
          <template #right>
            <tm-checkbox
              v-model:model-value="item.check"
              class="_u_pointer-events-none"
              :color="getColor(item)"
              :follow-theme="false"
              @click="handleCheckbox(item)"
            ></tm-checkbox>
          </template>
        </tm-cell>

        <view class="_u_flex _u_justify-between">
          <tm-modal title="取消任务" ok-text="确定" @ok="handleCancelPlan">
            <template #trigger>
              <tm-button
                label="取消任务"
                :follow-theme="false"
                color="red"
                outlined
                size="small"
                :margin="[10, 50, 10, 10]"
              >
              </tm-button>
            </template>
            <tm-input
              type="textarea"
              v-model.lazy="cancelModalInfo.reason"
              placeholder="请输入取消原因"
              :input-padding="[24]"
              :height="100"
              :margin="[10, 40]"
              :maxlength="100"
              :border="1"
              font-color="grey-4"
            ></tm-input>
          </tm-modal>
          <tm-button
            label="一键全检"
            :follow-theme="false"
            color="green"
            outlined
            size="small"
            :margin="[10, 50, 10, 10]"
            @click="handleCheckAll"
          ></tm-button>
        </view>
        <tm-button label="提交" :height="70" block :margin="[10]" @click="submit"></tm-button>
      </tm-sheet>
      <tm-modal
        ref="modal"
        color="grey-5"
        :height="720"
        :border="0"
        text
        ok-color="black"
        cancel-color="white"
        linear="bottom"
        :overlay-click="false"
        ok-text="确定"
        :title="modalInfo.title"
        v-model:show="modalInfo.show"
        @ok="handleOk"
      >
        <tm-form
          ref="form"
          :label-width="150"
          :border="false"
          transprent
          :margin="[0]"
          :height="550"
        >
          <tm-form-item label="检验方式:" field="modeName" :margin="[12, 0]" class="_u_h-70rpx">
            <tm-text :font-size="30" :label="modalInfo.form.modeName"> </tm-text>
          </tm-form-item>
          <tm-form-item label="判定基准:" field="datum" :margin="[12, 0]" class="_u_h-70rpx">
            <tm-text :font-size="30" :label="modalInfo.form.datum"> </tm-text>
          </tm-form-item>
          <tm-form-item
            required
            label="检验结果:"
            field="result"
            :margin="[12, 0]"
            class="_u_h-70rpx"
          >
            <tm-radio-group v-model="modalInfo.form.result">
              <tm-radio label="OK" value="OK"></tm-radio>
              <tm-radio label="NG" value="NG"></tm-radio>
            </tm-radio-group>
          </tm-form-item>
          <tm-form-item label="上传截图:" field="upload" :margin="[12, 0]" class="_u_h-150rpx">
            <tm-upload
              :rows="3"
              :width="420"
              :default-value="modalInfo.form.imgList"
              :header="getUploadTokenHeader()"
              :url="uploadImgUrl"
              v-model="modalInfo.form.imgList"
            ></tm-upload>
          </tm-form-item>
          <tm-form-item label="备注:" field="remark" :margin="[12, 0]" class="_u_h-70rpx">
            <tm-input
              v-model.lazy="modalInfo.form.remark"
              :input-padding="[12]"
              :maxlength="100"
              :border="1"
              font-color="grey-4"
              type="textarea"
            >
            </tm-input>
          </tm-form-item>
        </tm-form>
      </tm-modal>
    </tm-app>
  </div>
</template>
<script lang="ts" setup>
  import { useRequest } from 'alova'
  import { getItemListByFormId, cancelPlan, submitPlanForm, uploadImgUrl } from '@/api/equipment'
  import { ResultStatusEnum } from '@/enums/commonEnum'
  import { Toast } from '@/utils/uniapi/prompt'
  import { getUploadTokenHeader } from '@/utils/http'
  import { isUnDef } from '@/utils/is'
  import { file } from '@/tmui/components/tm-upload/upload'

  onLoad((query: { trackId: string; formId: string }) => {
    trackId.value = +query.trackId
    formId.value = +query.formId
  })

  type FormItemType = {
    result?: ResultStatusEnum
    imgList?: Array<any>
    remark?: string
    check?: boolean
  } & EquipmentItemDto

  const trackId = ref(0)
  const formId = ref(0)
  const loading = ref(true)
  const cancelModalInfo = reactive({
    reason: ''
  })
  const modalInfo = reactive({
    id: 0,
    show: false,
    title: '',
    form: {
      /** 检验方式 */
      modeName: '',
      /** 判定基准 */
      datum: '',
      result: '',
      imgList: [] as Array<any>,
      remark: ''
    }
  })
  const items = ref<FormItemType[]>([])

  const { send: sendGetItemList } = useRequest(getItemListByFormId, { immediate: false })
  const { send: sendCancelPlan } = useRequest(cancelPlan, { immediate: false })
  const { send: sendSubmitPlanForm } = useRequest(submitPlanForm, { immediate: false })

  const init = async () => {
    if (!formId.value) return
    const data = await sendGetItemList(formId.value).finally(() => (loading.value = false))
    items.value = data
  }

  watch(
    () => formId.value,
    async () => {
      await init()
    }
  )

  const getColor = (item: FormItemType) => {
    if (isUnDef(item.check)) return 'primary'
    return item.result === ResultStatusEnum.OK ? 'primary' : 'red'
  }

  const handleCheckbox = (item: FormItemType) => {
    openModal(item)
  }

  const openModal = (item: FormItemType) => {
    modalInfo.show = true
    modalInfo.id = item.id
    modalInfo.title = item.name
    modalInfo.form.modeName = item.modeName
    modalInfo.form.datum = item.datum
    modalInfo.form.result = item.result || ''
    modalInfo.form.imgList = item.imgList || []
    modalInfo.form.remark = item.remark || ''
  }

  const handleOk = () => {
    if (!modalInfo.form.result) {
      Toast('请选择检验结果!', { duration: 2000 })
      return
    }
    const item = items.value.find(item => item.id === modalInfo.id)
    if (item) {
      item.result = modalInfo.form.result as ResultStatusEnum
      item.imgList = modalInfo.form.imgList
      item.remark = modalInfo.form.remark
      item.check = true
    }
    modalInfo.show = false
  }

  const handleCheckAll = () => {
    const unCheckItems = items.value.filter(item => !item.check)
    if (unCheckItems.length === 0) return
    unCheckItems.forEach(item => {
      item.result = ResultStatusEnum.OK
      item.check = true
    })
  }

  const handleCancelPlan = async () => {
    if (!cancelModalInfo.reason) {
      Toast('请填写取消原因!')
      return
    }
    await sendCancelPlan(trackId.value, cancelModalInfo.reason)
    Toast('取消成功!')
    uni.navigateBack({ delta: 2 })
  }

  const submit = async () => {
    const unCheckItems = items.value.filter(item => !item.check)
    if (unCheckItems.length > 0) {
      Toast('请完成所有检查项!')
      return
    }
    const data = {
      trackId: trackId.value,
      checkItemList: items.value.map(item => {
        const imgUrls = (item.imgList || []).map((res: file) => {
          const { data: url } = JSON.parse(res.response)
          return url as string
        })
        return {
          id: item.id,
          result: item.result,
          imgUrls: imgUrls.join(','),
          remark: item.remark
        }
      })
    }
    await sendSubmitPlanForm(data)
    Toast('提交成功!')
    uni.navigateBack({ delta: 2 })
  }
</script>

<style lang="scss" scoped></style>
