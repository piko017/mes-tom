<template>
  <div id="main">
    <div id="stars" />
    <div id="stars2" />
    <div id="stars3" />
    <div id="container">
      <div id="login" class="login-box">
        <div class="login-logo">
          <img src="~@/assets/images/logo.png" width="45" />
          <h1 class="mb-0 ml-2 text-3xl font-bold" style="color: #fff">TMom</h1>
        </div>
        <ConfigProvider :theme="{ algorithm: themeColor.realDark }">
          <a-form layout="horizontal" :model="state.formInline" @submit.prevent="handleSubmit">
            <a-form-item>
              <a-input
                v-model:value="state.formInline.username"
                size="large"
                :placeholder="$t('common.username')"
              >
                <template #prefix><user-outlined /></template>
              </a-input>
            </a-form-item>
            <a-form-item>
              <a-input
                v-model:value="state.formInline.password"
                size="large"
                type="password"
                :placeholder="$t('common.password')"
                autocomplete="new-password"
              >
                <template #prefix><lock-outlined /></template>
              </a-input>
            </a-form-item>
            <a-form-item>
              <a-input
                v-model:value="state.formInline.verifyCode"
                :placeholder="$t('common.verifyCode')"
                :maxlength="4"
                size="large"
              >
                <template #prefix><SafetyOutlined /></template>
                <template #suffix>
                  <img
                    :src="state.captcha"
                    class="absolute right-0 h-full cursor-pointer"
                    @click="updateCaptcha"
                  />
                </template>
              </a-input>
            </a-form-item>
            <a-form-item>
              <a-checkbox v-model:checked="state.formInline.remember">{{
                $t('common.rememberMe')
              }}</a-checkbox>
            </a-form-item>
            <a-form-item>
              <a-button
                type="primary"
                html-type="submit"
                size="large"
                :loading="state.loading"
                block
              >
                {{ $t('common.login') }}
              </a-button>
            </a-form-item>
          </a-form>
        </ConfigProvider>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { computed, onMounted, onUnmounted, reactive, ref } from 'vue'
  import { ConfigProvider, type SelectProps } from 'ant-design-vue'
  import {
    UserOutlined,
    LockOutlined,
    SafetyOutlined,
    ProfileOutlined,
  } from '@ant-design/icons-vue'
  import { useRoute, useRouter } from 'vue-router'
  import { message, Modal, Tooltip } from 'ant-design-vue'
  import { useUserStore } from '@/store/modules/user'
  import { getImageCaptcha } from '@/api/login'
  import { Storage } from '@/utils/Storage'
  import { REMEMBERUSER } from '@/enums/cacheEnum'
  import { useLayoutSettingStore } from '@/store/modules/layoutSetting'
  import { themeColor } from '@/layout/header/components/setting/constant'
  import { IFRAME_WELCOME_ROUTE } from '@/router/constant'
  import { useI18n } from '@/hooks/useI18n'

  const { t } = useI18n()
  const rememberUser = Storage.get(REMEMBERUSER)
  const state = reactive({
    loading: false,
    captcha: '',
    formInline: {
      username: rememberUser?.username ?? 'test',
      password: rememberUser?.password ?? '123456',
      verifyCode: '',
      captchaId: '',
      remember: rememberUser != null,
    },
  })

  const router = useRouter()

  const userStore = useUserStore()
  const themeStore = useLayoutSettingStore()
  themeStore.setColorPrimary(themeStore.getPrimaryColor ?? '#13C2C2')

  const updateCaptcha = async () => {
    const { id, img } = await getImageCaptcha({
      width: 100,
      height: 50,
      bgColor: 'realDark',
    })
    state.captcha = img
    state.formInline.captchaId = id
  }
  updateCaptcha()

  const handleSubmit = async () => {
    const { username, password, verifyCode, remember } = state.formInline
    if (username.trim() == '' || password.trim() == '') {
      return message.warning(t('common.usernameOrPwdNotNull'))
    }
    if (!verifyCode) {
      return message.warning(t('common.plsEnterVerifyCode'))
    }
    message.loading(t('common.logging'), 0)
    state.loading = true
    try {
      await userStore.login(state.formInline).finally(() => {
        state.loading = false
        message.destroy()
      })
      if (remember) {
        Storage.set(REMEMBERUSER, { username, password })
      } else {
        Storage.remove(REMEMBERUSER)
      }
      message.success(t('common.loginSuccess'))
      setTimeout(() => {
        const path = '/dashboard'
        router.replace(path)
        // router.push({ path })
      })
    } catch (error: any) {
      Modal.error({
        title: () => t('common.tip'),
        content: () => error.message,
        onOk: () => {
          state.formInline.verifyCode = ''
        },
      })

      updateCaptcha()
    }
  }
</script>

<style lang="less" scoped>
  @import '/src/styles/login.css';

  #main {
    height: 100vh;
    overflow: hidden;
    background: radial-gradient(ellipse at bottom, #1b2735 0%, #090a0f 100%);
  }

  #container {
    display: flex;
    justify-content: center;
    width: 100vw;
    height: 100vh;
  }

  #container.sm-screen {
    align-items: center;
    justify-content: center;

    .login-box {
      padding-left: 0;
    }
  }

  .carousel {
    width: 55%;
  }

  .login-box {
    display: flex;
    flex-direction: column;
    flex-grow: 1;
    align-items: center;
    justify-content: center;
    width: 100vw;
    // height: 100vh;
    margin: auto 10px;
    padding-left: 0;
    // padding-top: 240px;
    // background: url('~@/assets/login.svg');
    background-size: 100%;

    .login-logo {
      display: flex;
      align-items: center;
      margin-bottom: 30px;

      .svg-icon {
        font-size: 48px;
      }
    }

    :deep(.ant-form) {
      width: 23rem;

      .ant-col {
        position: relative;
        width: 100%;

        .anticon-partition {
          position: absolute;
          z-index: 1000;
          top: calc(50% - 8px);
          left: 11px;
        }
      }

      .ant-form-item-label {
        padding-right: 6px;
      }

      .ant-select-selection-item,
      .ant-select-selection-placeholder {
        margin-left: 20px;
      }
    }
  }
</style>
