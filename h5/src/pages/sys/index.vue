<template>
  <tm-app>
    <custom-header>
      <!-- <template #right>
        <Iconify :size="50" :use-theme-color="false" icon="i-icon-park-outline-hamburger-button" />
      </template> -->
    </custom-header>
    <view class="main">
      <tm-sheet :margin="[32, 24, 32, 0]" v-for="group in menus" :key="group.id">
        <view class="flex _u_gap-1">
          <Icon
            v-if="group.icon"
            :icon="group.icon"
            :width="16"
            :color="group.color"
            class="_u_pt-0.2"
          ></Icon>
          <tm-text :font-size="24" _class="text-weight-b" :label="group.label"></tm-text>
        </view>
        <tm-divider></tm-divider>
        <tm-grid :width="638" :col="4">
          <BasicGridItem
            v-for="item in group.items"
            :key="item.id"
            v-bind="item"
            @click="() => renderFn(item.fnStr)"
          ></BasicGridItem>
        </tm-grid>
      </tm-sheet>
      <custom-tab-bar :active-index="2"> </custom-tab-bar>
    </view>
  </tm-app>
</template>
<script lang="ts" setup>
  import CustomHeader from '@/components/CustomHeader/index.vue'
  import CustomTabBar from '@/components/CustomTabBar/index.vue'
  import BasicGridItem from '@/components/BasicGridItem/index.vue'
  import { language } from '@/tmui/tool/lib/language'
  import { Icon } from '@iconify/vue'
  import { useAuthStore } from '@/state/modules/auth'
  import { getAuthMenus } from '@/api/home'

  const router = useRouter()
  const authStore = useAuthStore()

  const menus = ref<GridCardType[]>([])

  watch(
    () => authStore.getUserMenus,
    async val => {
      if (!val || val.length === 0) {
        menus.value = await getAuthMenus()
      } else {
        menus.value = val
      }
    },
    { immediate: true }
  )

  /**
   * 生成点击事件函数
   * @param fnStr 函数体字符串
   */
  const renderFn = (fnStr?: string) => {
    if (!fnStr) return
    try {
      return new Function(`
        return (
          function(router) {
            ${fnStr}
          }
        )
      `)().bind(undefined, router)()
    } catch (error) {
      console.log(error)
    }
  }
</script>

<!-- <style lang="scss" scoped>
  .main {
    height: calc(100vh - 60px);
    overflow-y: auto;
    background-color: #666;
  }
</style> -->
