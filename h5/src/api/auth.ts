import { BaseResponse } from '@/@type/global'
import { request } from '@/utils/http'

const LOGIN_OUT = '/logout'
const REFRESH_TOKEN = '/refresh/token'

/**
 * 登录
 * @param params
 */
export function login(params: LoginParams) {
  return request.Post<LoginModel>(`/login/JWTToken`, params)
}

/**
 * 登出
 */
export function logout() {
  return request.Post(LOGIN_OUT, {})
}

/**
 * 刷新token
 */
export function refreshToken() {
  return request.Post<LoginModel>(REFRESH_TOKEN, {})
}

/** 获取最新APP版本号信息 */
export const getAppLastVersion = () => request.Get<AppUpdateInfo>('/login/GetAppLastVersion')

/** 下载apk安装包 */
export const downLastAppApk = () => request.Get<AppUpdateInfo>('/login/DownLastAppApk')

/** 获取所有工厂列表 */
export const getFactoryList = () => request.Get<SelectModel[]>('/login/GetFactoryList')

/** 获取当前登录用户信息 */
export const getUserInfo = () => request.Get<UserModel>('/SysUser/GetUserInfo')

/** 切换工厂 */
export const changeFactory = (newFactoryId: number, token: string) =>
  request.Get<LoginModel>(`/login/ChangeFactory?newFactoryId=${newFactoryId}&token=${token}`)

/**
 * 更新用户基本信息
 * @param params 参数
 * @returns
 */
export const updateUserInfo = (params: Partial<UserModel>) =>
  request.Put<BaseResponse>('/SysUser/Update', params)

/** 获取用户订阅的报表/消息列表 */
export const getUserSubList = (subType: 'Report' | 'Message') =>
  request.Get<HasSubscribeReport[]>(`/SysUser/GetUserSubList?subType=${subType}`)

/**
 * 添加订阅
 * @param subId 订阅配置表id
 * @param subType 类型
 * @returns
 */
export const addSub = (subId: number, subType: 'Report' | 'Message') =>
  request.Post<BaseResponse>(`/SysUser/AddSub?subId=${subId}&subType=${subType}`)

/**
 * 移除订阅
 * @param subId 订阅配置表id
 * @param subType 类型
 * @returns
 */
export const removeSub = (subId: number, subType: 'Report' | 'Message') =>
  request.Delete<BaseResponse>(`/SysUser/RemoveSub?subId=${subId}&subType=${subType}`)
