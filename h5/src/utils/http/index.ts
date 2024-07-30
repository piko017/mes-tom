import { createAlova } from 'alova'
import AdapterUniapp from '@alova/adapter-uniapp'
import { getBaseUrl, isUseMock } from '@/utils/env'
import { assign } from 'lodash-es'
import { useAuthStore } from '@/state/modules/auth'
import { checkStatus } from '@/utils/http/checkStatus'
import { ContentTypeEnum, ResultEnum } from '@/enums/httpEnum'
import { Toast } from '@/utils/uniapi/prompt'
import { BaseResponse } from '@/@type/global'

const BASE_URL = getBaseUrl()

const HEADER = {
  'Content-Type': ContentTypeEnum.JSON,
  Accept: 'application/json, text/plain, */*'
}

/** 获取上传文件token头 */
export const getUploadTokenHeader = () => {
  const authStore = useAuthStore()
  return authStore.getAuthorization
}

/**
 * alova 请求实例
 * @link https://github.com/alovajs/alova
 */
const alovaInstance = createAlova({
  baseURL: BASE_URL,
  localCache: null,
  ...AdapterUniapp({
    /* #ifndef APP-PLUS */
    mockRequest: undefined // APP 平台无法使用mock
    /* #endif */
  }),
  timeout: 5000,
  beforeRequest: method => {
    const authStore = useAuthStore()
    method.config.headers = assign(HEADER, method.config.headers, authStore.getAuthorization)
  },
  responsed: {
    /**
     * 请求成功的拦截器
     * 第二个参数为当前请求的method实例，你可以用它同步请求前后的配置信息
     * @param response
     * @param method
     */
    onSuccess: async (response, method) => {
      const { config, type } = method
      const { enableDownload, enableUpload } = config
      // @ts-ignore
      const { statusCode, data: rawData } = response
      const { success, status, msg, data } = rawData as BaseResponse
      if (success) {
        if (enableDownload) {
          // 下载处理
          return rawData
        }
        if (enableUpload) {
          // 上传处理
          return rawData
        }
        if (status === ResultEnum.SUCCESS) {
          ;['DELETE', 'POST', 'PUT'].includes(type) && Toast(msg || '操作成功!', { duration: 2000 })
          return data as any
        }
        msg && Toast(msg)
        return Promise.reject(rawData)
      }
      checkStatus(status, msg || '未知错误!')
      return Promise.reject(rawData)
    },

    /**
     * 请求失败的拦截器，请求错误时将会进入该拦截器。
     * 第二个参数为当前请求的method实例，你可以用它同步请求前后的配置信息
     * @param err
     * @param method
     */
    onError: (err, method) => {
      uni.$emit('z-paging-error-emit')
      // error('Request Error!');
      return Promise.reject({ err, method })
    }
  }
})

export const request = alovaInstance

/**
 * 上传文件
 * @param url url
 * @param params 文件参数
 * @returns
 */
export const UploadFile = async <T = any>(url: string, params: any) => {
  const formData = new window.FormData()
  const customFilename = params.name || 'file'

  if (params.filename) {
    formData.append(customFilename, params.file, params.filename)
  } else {
    formData.append(customFilename, params.file)
  }

  if (params.data) {
    Object.keys(params.data).forEach(key => {
      const value = params.data![key]
      if (Array.isArray(value)) {
        value.forEach(item => {
          formData.append(`${key}[]`, item)
        })
        return
      }

      formData.append(key, params.data![key])
    })
  }
  return alovaInstance.Post<T>(url, formData, {
    headers: {
      'Content-Type': ContentTypeEnum.FORM_DATA
    }
  })
}
