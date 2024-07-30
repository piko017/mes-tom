<template>
  <tm-app>
    <custom-header>
      <!-- <template #right>
        <Iconify :size="50" :use-theme-color="false" icon="i-icon-park-outline-share-two" />
      </template> -->
    </custom-header>
    <tm-tabs
      @change="tabsChange"
      :show-tabs-line-ani="true"
      :list="tabsTitle"
      :item-width="180"
      :width="750"
      class="_u_mt-2"
      default-name="1"
    ></tm-tabs>
    <view v-if="tabKey == '1'">
      <tm-sheet :height="550">
        <tm-text
          :font-size="24"
          _class="text-weight-b _u_ml-2 _u_my-auto"
          label="工单完成率"
        ></tm-text>
        <tm-divider></tm-divider>
        <tm-chart ref="chartDom" :width="630" :height="530" @on-init="chartInit"></tm-chart>
      </tm-sheet>

      <tm-sheet :height="550">
        <tm-text
          :font-size="24"
          _class="text-weight-b _u_ml-2 _u_my-auto"
          label="产量达成率"
        ></tm-text>
        <tm-divider></tm-divider>
        <tm-chart ref="chartDom" :width="630" :height="530" @on-init="chartInit"></tm-chart>
      </tm-sheet>

      <tm-sheet :height="550">
        <tm-text :font-size="24" _class="text-weight-b _u_ml-2 _u_my-auto" label="Yield"></tm-text>
        <tm-divider></tm-divider>
        <tm-chart ref="chartDom" :width="630" :height="530" @on-init="chartInit"></tm-chart>
      </tm-sheet>
    </view>
    <!-- <tm-float-button v-if="!showWin" position="tl" :btn="{ icon: '' }">
      <template #default>
        <view @click="() => (showWin = true)" class="fixBtn">
          <Iconify icon="i-icon-park-outline-double-right" :size="80"></Iconify>
        </view>
      </template>
    </tm-float-button>
    <tm-drawer ref="calendarView" placement="left" v-model:show="showWin"></tm-drawer> -->
    <view class="_u_mt-15"></view>
    <custom-tab-bar :active-index="1"> </custom-tab-bar>
  </tm-app>
</template>
<script lang="ts" setup>
  import CustomHeader from '@/components/CustomHeader/index.vue'
  import CustomTabBar from '@/components/CustomTabBar/index.vue'
  import { language } from '@/tmui/tool/lib/language'
  import { Iconify } from '@/components/Iconify'
  import FrameBlank from '@/components/FrameBlank/FrameBlank.vue'
  import { ECharts } from 'echarts'

  const showWin = ref(false)
  const tabKey = ref('1')

  const tabsTitle = ref([
    { key: '1', title: '生产指标' },
    { key: '2', title: '质量指标' },
    { key: '3', title: '设备指标' },
    { key: '4', title: '物料指标' },
    { key: '5', title: '实时指标' },
    { key: '6', title: '其他指标' }
  ])

  const tabsChange = (key: string) => {
    console.log(key)
    tabKey.value = key
  }

  const chartInit = (chart: ECharts) => {
    chart.setOption(option)
  }

  const option = {
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'cross',
        label: {
          backgroundColor: '#283b56'
        }
      }
    },
    legend: {},
    // toolbox: {
    //   show: true,
    //   feature: {
    //     dataView: { readOnly: false },
    //     restore: {},
    //     saveAsImage: {}
    //   }
    // },
    // dataZoom: {
    //   show: false,
    //   start: 0,
    //   end: 100
    // },
    xAxis: {
      type: 'category',
      // boundaryGap: true,
      data: ['1月', '2月', '3月', '4月', '5月']
    },
    yAxis: [
      {
        type: 'value'
        // scale: true,
        // name: 'Price',
      },
      {
        type: 'value'
        // scale: true,
        // name: 'Order',
      }
    ],
    series: [
      {
        name: 'Dynamic Bar',
        type: 'bar',
        yAxisIndex: 1,
        data: [21, 45, 12, 83, 56]
      },
      {
        name: 'Dynamic Line',
        type: 'line',
        data: [52, 33, 112, 90, 56]
      }
    ]
  }
</script>

<style lang="scss" scoped>
  .fixBtn {
    position: fixed;
    top: 50%;
    left: 5rpx;
  }

  iframe {
    width: 100vw;
    height: 100vh;
  }
</style>
