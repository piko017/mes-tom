import { createSSRApp } from 'vue'
import App from './App.vue'
import 'uno.css'
import { setupStore } from '@/state'
import { setupRouter } from '@/router'
import tmui from './tmui'
import { config as tmuiConfig } from './tmui-config'

export function createApp() {
  const app = createSSRApp(App)

  // Configure router
  setupRouter(app)

  // Configure store
  setupStore(app)

  // 放在pinia后面
  app.use(tmui, { ...tmuiConfig } as Tmui.tmuiConfig)

  return {
    app
  }
}
