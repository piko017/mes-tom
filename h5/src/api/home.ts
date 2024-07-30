import { request } from '@/utils/http'

/**
 * 获取当前用户拥有权限的应用菜单列表
 * @returns
 */
export const getAuthMenus = () => request.Get<GridCardType[]>('/Home/GetAuthMenus')
