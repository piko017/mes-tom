import type { App } from 'vue'
import IconPark from '@/components/IconPark/index.vue'
import {
  CheckOne,
  Check,
  Iphone,
  Mail,
  Translate,
  Platte,
  DarkMode,
  User,
  Brain,
  RobotTwo,
  TreeDiagram,
  AddOne,
  Garage,
  AdProduct,
  Box,
  Devices,
  Zijinyunying,
  Currency,
  GreenNewEnergy,
  Control,
  Worker,
  Spanner,
  SunOne,
  TableReport,
  MessageOne,
  MessageUnread
} from '@icon-park/vue-next'

/** 注册IconPark图标库 */
export function setupIconPark(app: App<Element>) {
  app.component('IconPark', IconPark)
  for (const [key, component] of Object.entries(iconParks)) {
    const prefix = 'IconPark' // 注册时使用大写
    app.component(prefix + key, component)
  }
}

export const iconParks = {
  CheckOne,
  Check,
  Iphone,
  Mail,
  Translate,
  Platte,
  DarkMode,
  User,
  Brain,
  RobotTwo,
  TreeDiagram,
  AddOne,
  Garage,
  AdProduct,
  Box,
  Devices,
  Zijinyunying,
  Currency,
  GreenNewEnergy,
  Control,
  Worker,
  Spanner,
  SunOne,
  TableReport,
  MessageOne,
  MessageUnread
}
