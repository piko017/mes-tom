<template>
  <tm-app>
    <tm-sheet :margin="[0]" class="main">
      <tm-sheet :transprent="true" :margin="[0]" :height="1">
        <!-- <tm-text color="#fff" :font-size="80" class="_u_pt-10 title" label="欢迎~"></tm-text> -->
        <!-- <tm-image
          :width="650"
          :height="300"
          src="http://10.0.0.138:311/static/ly_login.da08e639.png"
        ></tm-image> -->
      </tm-sheet>
      <tm-sheet :margin="[0]" class="login" :shadow="6" :round="8">
        <view class="flex _u_justify-center _u_mt-3 _u_mb-6">
          <tm-avatar img="/static/logo.png"></tm-avatar>
          <!-- <tm-text :font-size="60" class="_u_ml-2" _class="text-weight-b" label="登录"> </tm-text> -->
        </view>
        <tm-input
          v-model.lazy="form.username"
          :height="100"
          :round="5"
          :font-size="60"
          prefix="tmicon-account"
        ></tm-input>
        <tm-input
          password
          v-model="form.password"
          :margin="[0, 50]"
          :height="100"
          :round="5"
          :font-size="60"
          prefix="tmicon-lock"
        ></tm-input>
        <tm-button
          block
          :label="language('common.login')"
          :font-size="50"
          :height="100"
          :round="8"
          @click="submit"
        ></tm-button>
        <view class="flex _u_justify-between _u_mt-4">
          <tm-text label=""></tm-text>
          <tm-text color="#777" label="忘记密码"></tm-text>
        </view>
      </tm-sheet>
      <view class="py-50 flex flex-row flex-row-center-center">
        <tm-divider label="---- TMom ----"></tm-divider>
      </view>
    </tm-sheet>
  </tm-app>
</template>
<script setup lang="ts">
  import CustomNavBar from '@/components/CustomNavBar/index.vue'
  import { useAuthStore } from '@/state/modules/auth'
  import { Toast } from '@/utils/uniapi/prompt'
  import { login } from '@/api/auth'
  import { language } from '@/tmui/tool/lib/language'
  import { themeColors } from '@/utils/constant'
  import { useTmpiniaStore } from '@/tmui/tool/lib/tmpinia'
  import { omit } from 'lodash-es'
  import { useAutoUpdate } from '@/hooks/app/useAutoUpdate'

  const pageQuery = ref<Record<string, any> | undefined>(undefined)
  onLoad(query => {
    pageQuery.value = query
    const themeColor = tmStore.tmStore.color
    if (!themeColor) {
      tmStore.setTmVuetifyAddTheme(themeColors[0].key, themeColors[0].value)
    }
    useAutoUpdate()
  })

  const router = useRouter()

  const form = reactive({
    username: '',
    password: '',
    factoryName: '',
    factoryId: [] as number[]
  })
  const authStore = useAuthStore()
  const tmStore = useTmpiniaStore()

  const submit = () => {
    login({
      username: form.username,
      password: form.password,
      factoryId: form.factoryId[0]
    }).then(res => {
      Toast('登录成功', { duration: 1500 })
      authStore.afterLogin(res.token, res.expires_in)
      setTimeout(() => {
        if (unref(pageQuery)?.redirect) {
          // 如果有存在redirect(重定向)参数，登录成功后直接跳转
          const params = omit(unref(pageQuery), ['redirect', 'tabBar'])
          if (unref(pageQuery)?.tabBar) {
            // 这里replace方法无法跳转tabbar页面故改为replaceAll
            router.replaceAll({ name: unref(pageQuery)?.redirect, params })
          } else {
            router.replace({ name: unref(pageQuery)?.redirect, params })
          }
        } else {
          // 不存在则返回上一页
          // router.back()
          router.replaceAll({ name: 'Home' })
        }
      }, 200)
    })
  }
</script>

<style lang="scss" scoped>
  .main {
    height: 100vh;
    overflow: hidden;
    background-image: linear-gradient(to bottom, #13c2c2, #ffffff);
  }

  .login {
    margin-top: auto !important;
    margin-bottom: auto !important;
  }

  .title {
    font-family: 'Courier New', Courier, monospace;
  }
</style>
