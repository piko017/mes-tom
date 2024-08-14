﻿import type { FormSchema } from '@/components/core/schema-form'
import cacheAction from '@/api/cache'

/**
 * 表单信息
 */
export const formSchemas: FormSchema[] = [
  {
    field: 'code',
    component: 'Input',
    label: '编码',
    required: true,
    colProps: {
      span: 12,
    },
  },
  {
    field: 'name',
    component: 'Input',
    label: '名称',
    required: true,
    colProps: {
      span: 12,
    },
  },
  {
    field: 'lineId',
    component: 'Select',
    label: '产线',
    required: true,
    componentProps: {
      request: async () => {
        const list = await cacheAction.getLineWithCache()
        return list.map(item => ({
          label: item.name,
          value: item.id,
        }))
      },
    },
  },
  {
    field: 'processOperationId',
    component: 'Select',
    label: '工序',
    required: true,
    componentProps: {
      request: async () => {
        const list = await cacheAction.getOperationWithCache()
        return list.map(item => ({
          label: item.name,
          value: item.id,
        }))
      },
    },
  },
  {
    field: 'description',
    component: 'InputTextArea',
    label: '描述',
  },
]
