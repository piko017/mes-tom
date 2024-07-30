<template>
  <tm-app>
    <tm-form
      @submit="handleUpdate"
      ref="form"
      v-model="formData"
      :label-width="260"
      :min-height="400"
    >
      <tm-form-item
        :label="language('user.uploadAvatar')"
        field="upload"
        :rules="{ required: false, message: '请上传' }"
      >
        <tm-upload
          :rows="3"
          :width="420"
          :max-file="1"
          form-name="files"
          :default-value="formData.uploadAvatar"
          :url="`${getBaseUrl()}/SysUser/UploadAvatar`"
          :header="authStore.getAuthorization"
          :on-success-after="onUploadCb"
          v-model="formData.uploadAvatar"
        ></tm-upload>
      </tm-form-item>
      <tm-form-item
        :label="language('user.phone')"
        field="phoneNo"
        :rules="[
          {
            required: false,
            message: '请输入正确的手机号',
            validator: val => isPhone(val)
          }
        ]"
      >
        <tm-input
          :input-padding="[0, 0]"
          type="number"
          v-model.lazy="formData.phoneNo"
          :transprent="true"
          :show-bottom-botder="false"
        >
        </tm-input>
      </tm-form-item>
      <tm-form-item
        :label="language('user.email')"
        :rules="[{ message: '请输入正确的邮箱', validator: val => isEmail(val) }]"
      >
        <tm-input
          :input-padding="[0, 0]"
          v-model.lazy="formData.email"
          :transprent="true"
          :show-bottom-botder="false"
        >
        </tm-input>
      </tm-form-item>
      <tm-form-item :label="language('user.desc')">
        <tm-input
          auto-height
          type="textarea"
          :maxlength="100"
          :input-padding="[0, 0]"
          v-model.lazy="formData.remark"
          :transprent="true"
          :show-bottom-botder="false"
        >
        </tm-input>
      </tm-form-item>
      <view class="_u_mx-4 _u_my-6">
        <tm-button block :label="language('common.update')" @click="handleUpdate"></tm-button>
      </view>
    </tm-form>
  </tm-app>
</template>
<script lang="ts" setup>
  import { useAuthStore } from '@/state/modules/auth'
  import { getBaseUrl } from '@/utils/env'
  import tmForm from '@/tmui/components/tm-form/tm-form.vue'
  import { Toast } from '@/utils/uniapi/prompt'
  import { updateUserInfo } from '@/api/auth'
  import { language } from '@/tmui/tool/lib/language'
  import { useRequest } from 'alova'
  import { isEmail, isPhone } from '@/tmui/tool/function/util'
  const authStore = useAuthStore()

  const userInfo = computed(() => authStore.getUserInfo)
  const form = ref<InstanceType<typeof tmForm> | null>(null)
  const formData = ref({
    uploadAvatar: [],
    phoneNo: userInfo.value?.phoneNo,
    email: userInfo.value?.email,
    remark: userInfo.value?.remark
  })

  const handleUpdate = _ => {
    const isPass = form.value?.validate().isPass
    console.log(form.value?.validate(), isPass)
    if (!isPass) return
    const params: Partial<UserModel> = { ...userInfo.value, ...formData.value }
    useRequest(updateUserInfo(params))
    authStore.setUserInfo(params)
  }

  const onUploadCb = res => {
    const response = JSON.parse(res.response)
    if (response.status !== 200 || !response.success) {
      Toast(response.msg ?? res.status, { duration: 1500 })
      return false
    }
    const { data: newUrl } = response
    const user: Partial<UserModel> = { ...userInfo.value, avatar: newUrl }
    authStore.setUserInfo(user)
    return true
  }
</script>
