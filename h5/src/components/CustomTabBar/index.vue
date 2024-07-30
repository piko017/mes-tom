<template>
  <tm-tabbar :auto-select="false" v-model:active="activeIndex" height="100">
    <tm-tabbar-item
      open-type="reLaunch"
      :text="language('common.home')"
      icon="tmicon-md-home"
      url="/pages/home/index"
    ></tm-tabbar-item>
    <tm-tabbar-item
      @click="activeIndex = 1"
      :text="language('common.report')"
      icon="tmicon-rank"
      url="/pages/report/index"
    ></tm-tabbar-item>
    <tm-tabbar-item
      url="/pages/sys/index"
      :text="language('common.workbench')"
      icon="tmicon-all"
    ></tm-tabbar-item>
    <tm-tabbar-item
      url="/pages/user/index"
      :text="language('common.user')"
      icon="tmicon-account"
    ></tm-tabbar-item>
  </tm-tabbar>
</template>

<script lang="ts" setup>
  import { tabBarProps } from './props'
  import { language } from '@/tmui/tool/lib/language'

  const props = defineProps(tabBarProps)

  const activeIndex = ref(props.activeIndex)
</script>
