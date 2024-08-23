<template>
  <tm-app ref="app">
    <tm-sheet :shadow="4" :round="2" :height="200">
      <view class="flex">
        <view v-if="authStore.isLogin" class="flex _u_flex-row _u_mt-4 _u_ml-5">
          <tm-avatar v-if="img" :font-size="140" :round="16" :img="img"></tm-avatar>
          <tm-avatar v-else :font-size="140" :round="16" img="/static/logo.png"></tm-avatar>
          <view class="_u_ml-5 _u_mt-3 _u_my-auto">
            <view class="flex flex-row">
              <tm-text
                class="font-weight-b _u_font-bold _u_mb-2"
                :font-size="35"
                :label="userInfo?.realName"
              >
              </tm-text>
            </view>
            <tm-text class="font-weight-b" :label="userInfo?.loginAccount"> </tm-text>
          </view>
          <view>
            <tm-text
              class="_u_ml-27 _u_mt-7"
              color="#9E9E9E"
              :font-size="24"
              :label="`${language('user.edit')} >`"
              @click="goEdit"
            ></tm-text>
          </view>
        </view>
        <view v-else class="flex _u_flex-row _u_mt-5 _u_ml-5">
          <tm-avatar :font-size="140" :round="16" img="/static/images/no-user.png"></tm-avatar>
          <view class="_u_ml-5 _u_mt-6 _u_my-auto">
            <tm-text
              class="font-weight-b _u_font-bold _u_mb-2"
              color="#333"
              :font-size="35"
              label="您还未登录~"
            ></tm-text>
          </view>
        </view>
      </view>
    </tm-sheet>
    <view class="mb-32 mx-32 round-3 overflow">
      <tm-cell
        bottom-border
        :margin="[0, 0]"
        :title-font-size="30"
        :right-text="userInfo?.factoryName"
        @click="handleChangeFactory"
      >
        <template #title>
          <view class="flex">
            <Iconify icon="i-icon-park-outline-tree-diagram" />
            <tm-text class="_u_ml-3" :label="language('user.factory')"></tm-text>
          </view>
        </template>
      </tm-cell>
      <tm-cell bottom-border :margin="[0, 0]" :title-font-size="30">
        <template #title>
          <view class="flex">
            <Iconify icon="i-icon-park-outline-iphone"></Iconify>
            <tm-text class="_u_ml-3" :label="language('user.phone')"></tm-text>
          </view>
        </template>
        <template #right>
          <tm-text color="#9E9E9E" :font-size="24" :label="userInfo?.phoneNo"></tm-text>
        </template>
      </tm-cell>
      <tm-cell bottom-border :margin="[0, 0]" :title-font-size="30">
        <template #title>
          <view class="flex">
            <Iconify icon="i-icon-park-outline-mail"></Iconify>
            <tm-text class="_u_ml-3" :label="language('user.email')"></tm-text>
          </view>
        </template>
        <template #right>
          <tm-text color="#9E9E9E" :font-size="24" :label="userInfo?.email"></tm-text>
        </template>
      </tm-cell>
      <tm-cell
        bottom-border
        :margin="[0, 0]"
        :title-font-size="30"
        :right-text="localeName"
        @click="() => (showLanguage = true)"
      >
        <template #title>
          <view class="flex">
            <Iconify icon="i-icon-park-outline-translate"></Iconify>
            <tm-text class="_u_ml-3" :label="language('user.language')"></tm-text>
          </view>
        </template>
      </tm-cell>
      <tm-cell
        bottom-border
        :margin="[0, 0]"
        :title-font-size="30"
        :right-text="themeName"
        @click="() => (showTheme = true)"
      >
        <template #title>
          <view class="flex">
            <Iconify icon="i-icon-park-outline-platte"></Iconify>
            <tm-text class="_u_ml-3" :label="language('user.theme')"></tm-text>
          </view>
        </template>
      </tm-cell>
      <tm-cell bottom-border :margin="[0, 0]" :title-font-size="30">
        <template #title>
          <view class="flex">
            <Iconify icon="i-icon-park-outline-dark-mode"></Iconify>
            <tm-text class="_u_ml-3" :label="language('user.dark')"></tm-text>
          </view>
        </template>
        <template #right>
          <tm-text color="#9E9E9E" :font-size="24" :label="language('user.autoDark')"></tm-text>
          <tm-checkbox :round="12" v-model="autoDark" @change="handleDarkChange"></tm-checkbox>
        </template>
      </tm-cell>
    </view>
    <view class="_u_mx-4">
      <tm-button
        v-if="authStore.isLogin"
        block
        :label="language('common.logout')"
        @click="handleLogout"
      ></tm-button>
      <tm-button
        v-else
        block
        :label="language('common.login')"
        @click="handleJump('/pages/login/index')"
      ></tm-button>
    </view>

    <tm-float-button :btn="{ icon: '' }">
      <template #default>
        <view @click="onChangeDark" class="fixBtn">
          <Iconify v-if="!isDark" icon="i-icon-park-outline-dark-mode" :size="80"></Iconify>
          <Iconify v-else icon="i-icon-park-outline-sun-one" :size="80"></Iconify>
        </view>
      </template>
    </tm-float-button>
    <tm-picker
      v-model:show="showFactory"
      v-model:model-str="data.factoryName"
      v-model="data.factoryId"
      :columns="factoryList"
      selected-model="id"
    ></tm-picker>
    <tm-action-menu
      @change="handleLanguageChange"
      v-model="showLanguage"
      :list="languageList"
    ></tm-action-menu>
    <tm-action-menu
      @change="handleThemeChange"
      v-model="showTheme"
      :list="themeList"
    ></tm-action-menu>
    <view class="py-32 flex flex-row flex-row-center-center">
      <view class="w-40">
        <tm-divider color="red" :label="`v${version}`"></tm-divider>
      </view>
    </view>
    <custom-tab-bar :active-index="3"> </custom-tab-bar>
  </tm-app>
</template>
<script lang="ts" setup>
  import CustomTabBar from '@/components/CustomTabBar/index.vue'
  import tmApp from '@/tmui/components/tm-app/tm-app.vue'
  import { useAuthStore } from '@/state/modules/auth'
  import { changeFactory } from '@/api/auth'
  import { renderImg } from '@/utils/file'
  import { themeColors } from '@/utils/constant'
  import { useTmpiniaStore } from '@/tmui/tool/lib/tmpinia'
  import { language } from '@/tmui/tool/lib/language'
  import { Iconify } from '@/components/Iconify'

  const tmStore = useTmpiniaStore()
  const authStore = useAuthStore()
  const router = useRouter()
  const showFactory = ref(false)
  const showLanguage = ref(false)
  const showTheme = ref(false)
  const autoDark = ref(tmStore.tmuiConfig.autoDark)
  const themeName = ref<string>(themeColors[0].title)
  const localeName = ref<string>()
  const app = ref<InstanceType<typeof tmApp> | null>(null)
  const data = reactive({
    factoryName: '',
    factoryId: [] as number[]
  })
  const version = ref('1.0.0')
  /** 当前是否暗黑模式 */
  const isDark = computed(() => tmStore.tmStore.dark)

  onLoad(() => {
    themeName.value = themeColors.find(item => item.key === tmStore.tmStore.color)?.title || ''
    localeName.value = languageList.find(item => item.id === tmStore.tmStore.local)?.text || ''
  })

  const languageList: SelectModel[] = [
    { text: '简体中文', id: 'zh-Hans' },
    { text: language('common.en_US'), id: 'en' }
  ]

  const themeList: SelectModel[] = themeColors.map(item => ({ text: item.title, id: item.value }))

  const userInfo = computed(() => authStore.getUserInfo)

  const img = computed(() => renderImg(userInfo.value?.avatar))
 
  const factoryList = computed(() => {
    return (authStore.getUserInfo?.hasAuthFactoryList || []).map(item => ({
      text: item.factoryName,
      id: item.factoryId
    }))
  })

  /** 切换工厂 */
  const handleChangeFactory = () => {
    showFactory.value = true
  }

  watch(
    () => data.factoryId,
    async newVal => {
      if (newVal.length > 0) {
        const factoryId = newVal[0] as number
        const data = await changeFactory(factoryId, authStore.getToken)
        authStore.afterLogin(data.token, data.expires_in)
      }
    }
  )
  const handleJump = (url: string) => {
    router.push(url)
  }

  /** 设置暗黑主题跟随系统 */
  const handleDarkChange = (val: boolean) => {
    autoDark.value = val
    tmStore.setTmAutoDark(val)
  }

  /** 手动切换暗黑主题 */
  const onChangeDark = () => app.value?.setDark()

  const handleLogout = () => {
    authStore.loginOut()
    router.replaceAll({ name: 'Login' })
  }

  /** 语言改变 */
  const handleLanguageChange = (item: Tmui.tmActionMenu, index: number) => {
    localeName.value = item.text
    tmStore.setTmLocal(item.id)
    uni.setLocale(item.id)
  }

  /** 主题色改变 */
  const handleThemeChange = (item: Tmui.tmActionMenu, index: number) => {
    themeName.value = item.text as string
    tmStore.setTmVuetifyAddTheme(themeColors[index]!.key, themeColors[index]!.value)
  }

  const goEdit = () => {
    router.push({
      path: 'pages_sub/editUser/index'
    })
  }
  // #ifdef APP-PLUS
  plus.runtime.getProperty(plus.runtime.appid || '', wgtinfo => {
    version.value = wgtinfo.version || '1.0.0'
  })
  // #endif
</script>

<style lang="scss" scoped>
  .fixBtn {
    position: fixed;
    bottom: 140rpx;
    right: 40rpx;
  }
</style>
