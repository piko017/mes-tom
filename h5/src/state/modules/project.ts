import { defineStore } from 'pinia'

interface ProjectState {
  /** 跟随系统开启暗黑模式 */
  autoDark: boolean
}
export const useProjectStore = defineStore({
  id: 'project',
  state: (): ProjectState => ({
    autoDark: false
  }),
  getters: {
    getAutoDark: state => state.autoDark
  },
  actions: {
    setAutoDark(autoDark: boolean) {
      this.autoDark = autoDark
    }
  }
})
