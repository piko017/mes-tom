import { defineStore } from 'pinia'
import { getCache, removeCache, setCache } from '@/utils/cache'
import { TOKEN_KEY, USER_INFO_KEY } from '@/enums/cacheEnum'
import { useRequest } from 'alova'
import { getUserInfo } from '@/api/auth'
import { getAuthMenus } from '@/api/home'

interface AuthState {
  token?: string
  userInfo?: Partial<UserModel>
  menuList: GridCardType[]
}

const { send: sendGetUser } = useRequest(getUserInfo, { immediate: false })
const { send: sendGetAuthMenus } = useRequest(getAuthMenus, { immediate: false })

export const useAuthStore = defineStore({
  id: 'auth',
  state: (): AuthState => ({
    token: undefined,
    userInfo: undefined,
    menuList: []
  }),
  getters: {
    getToken: state => state.token,
    isLogin: (state): boolean => !!state.token,
    getAuthorization: state => {
      return state.token ? { authorization: `Bearer ${state.token}` } : {}
    },
    /** 获取当前登录的用户信息 */
    getUserInfo: state => state.userInfo,
    /** 获取应用菜单 */
    getUserMenus: state => state.menuList
  },
  actions: {
    /** 初始化token */
    initToken() {
      this.token = getCache<string>(TOKEN_KEY) || undefined
      this.userInfo = getCache<UserModel>(USER_INFO_KEY) || undefined
      if (!this.token) {
        const router = useRouter()
        router.replaceAll({ name: 'Login' })
      }
    },
    afterLogin(token: string, expire?: number) {
      this.setToken(token, expire)
      sendGetUser().then(userData => {
        this.setUserInfo(userData, expire)
      })
      sendGetAuthMenus().then(menuList => {
        this.menuList = menuList
      })
    },
    setToken(token: string | undefined, expire?: number) {
      setCache(TOKEN_KEY, token, expire)
      this.token = token
    },
    /**
     * @description 登出
     */
    async loginOut(): Promise<any> {
      try {
        removeCache(TOKEN_KEY)
        this.clearUserInfo()
        this.setToken(undefined)
        return Promise.resolve(null)
      } catch (err: any) {
        return Promise.reject(err)
      }
    },
    setUserInfo(userInfo: Partial<UserModel>, expire?: number) {
      setCache(USER_INFO_KEY, userInfo, expire)
      this.userInfo = userInfo
    },
    /** 切换工厂后，更新缓存中用户信息 */
    updateUserFactory(newFactoryId: number) {
      const item = this.userInfo?.hasAuthFactoryList?.find(x => x.factoryId === newFactoryId)
      if (!item) return
      const { factoryId, factoryName, factoryCode, factoryCodeEncrypt } = item as hasAuthFactory
      this.userInfo = { ...this.userInfo, factoryId, factoryName, factoryCode, factoryCodeEncrypt }
    },
    /** 清除当前登录的用户信息 */
    clearUserInfo() {
      this.userInfo = {}
    }
    /**
     * @description 刷新token
     */
    // async refreshToken(): Promise<LoginModel> {
    //     try {
    //         const { data } = await refreshToken();
    //         this.setToken(data.token);
    //         return Promise.resolve(data);
    //     } catch (err: any) {
    //         return Promise.reject(err);
    //     }
    // },
  }
})
