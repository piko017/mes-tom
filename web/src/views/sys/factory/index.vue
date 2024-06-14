<template>
  <div>
    <DynamicTable
      ref="dynamicTableRef"
      row-key="id"
      :data-request="loadData"
      :columns="columns"
      bordered
      size="small"
      :row-selection="rowSelection"
    >
      <template v-if="isCheckRows" #title>
        <Alert class="w-full" type="info" show-icon>
          <template #message>
            {{ $t('common.selected', [isCheckRows]) }}
            <a-button type="link" @click="rowSelection.selectedRowKeys = []">{{
              $t('common.cancelChoose')
            }}</a-button>
          </template>
        </Alert>
      </template>
      <template #toolbar>
        <a-button v-if="$auth('/api/SysFactory/Add')" type="primary" @click="openModal({})">
          {{ $t('common.add') }}
        </a-button>
        <a-button type="primary" @click="aoaToExcel"> {{ $t('common.export') }} </a-button>
        <a-button
          v-if="$auth('/api/SysFactory/Delete')"
          type="danger"
          :disabled="!isCheckRows"
          @click="delRowConfirm(rowSelection.selectedRowKeys)"
        >
          {{ $t('common.delete') }}
        </a-button>
      </template>
    </DynamicTable>
  </div>
</template>

<script lang="tsx" setup>
  import { ref, computed } from 'vue'
  import { Avatar, Tag, Tooltip, Modal, Alert, Badge } from 'ant-design-vue'
  import { ExclamationCircleOutlined } from '@ant-design/icons-vue'
  import { useRoute } from 'vue-router'
  import { DynamicTable } from '@/components/core/dynamic-table'
  import { useFormModal } from '@/hooks/useModal/useFormModal'
  import type { FormSchema } from '@/components/core/schema-form/src/types/form'
  import type { TableColumn } from '@/components/core/dynamic-table'
  import { formatToDate, formatToDateTime } from '@/utils/dateUtil'
  import { aoaToSheetXlsx, exportExcelForTable } from '@/components/basic/excel'
  import action from '@/api/sys/factory'
  import { exportPageParam } from '@/utils/global'
  import { useI18n } from '@/hooks/useI18n'

  defineOptions({
    name: 'Factory',
  })

  const dynamicTableRef = ref<InstanceType<typeof DynamicTable>>()

  const { t } = useI18n()
  const [showModal] = useFormModal()
  const route = useRoute()
  let searchParams: any = {}

  /**
   * 选中的行数据
   */
  const rowSelection = ref({
    selectedRowKeys: [] as number[],
    onChange: (selectedRowKeys: number[], selectedRows) => {
      console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows)
      rowSelection.value.selectedRowKeys = selectedRowKeys
    },
  })

  /**
   * 是否勾选了表格行
   */
  const isCheckRows = computed(() => rowSelection.value.selectedRowKeys.length)

  /**
   * 导出
   */
  const aoaToExcel = async () => {
    Object.assign(searchParams, exportPageParam)
    const { list: exportData } = await action.getWithPage(searchParams)
    exportExcelForTable(exportData, columns, `${route.meta.title}${formatToDateTime()}.xlsx`)
  }

  /**
   * 加载数据
   */
  const loadData = async params => {
    if (params?.createTime) {
      params.createTimeS = formatToDate(params.createTime[0])
      params.createTimeE = formatToDate(params.createTime[1])
      delete params.createTime
    }
    searchParams = params
    const data = await action.getWithPage(params)
    rowSelection.value.selectedRowKeys = []
    return data
  }

  /**
   * 打开新增/编辑弹窗
   */
  const openModal = async record => {
    const [formRef] = await showModal({
      modalProps: {
        title: `${record.id ? t('common.edit') : t('common.add')}${t('common.data')}`,
        width: '40%',
        onFinish: async values => {
          values.id = record.id
          const params = { ...values }
          console.log('新增/编辑参数', params)
          await (record.id ? action.update : action.add)(params)
          dynamicTableRef.value?.reload()
        },
      },
      formProps: {
        labelWidth: 100,
        schemas: formSchemas,
      },
    })

    if (record.id) {
      const data = await action.getById(record.id)

      formRef?.setFieldsValue({
        ...data,
        dbType: data.dbType.toString(),
      })
    }
  }

  /**
   * 删除行数据
   */
  const delRowConfirm = async (id: number | number[]) => {
    if (Array.isArray(id)) {
      Modal.confirm({
        title: t('column.confirmDelSelectedData'),
        icon: <ExclamationCircleOutlined />,
        centered: true,
        onOk: async () => {
          await action.delete(id)
          rowSelection.value.selectedRowKeys = []
          dynamicTableRef.value?.reload()
        },
      })
    } else {
      await action.delete([id])
      rowSelection.value.selectedRowKeys = []
      dynamicTableRef.value?.reload()
    }
  }

  /**
   * 表格列
   */
  const columns: TableColumn<any>[] = [
    {
      title: 'id',
      dataIndex: 'id',
      width: 55,
      align: 'center',
      hideInTable: true,
      hideInSearch: true,
    },
    {
      title: '工厂代码',
      dataIndex: 'code',
      align: 'center',
    },
    {
      title: '工厂名称',
      dataIndex: 'name',
      align: 'center',
    },
    {
      title: '是否自定义配置',
      align: 'center',
      dataIndex: 'isCustom',
      hideInSearch: true,
      customRender: ({ record }) => (
        <Badge
          status={record.isCustom ? 'success' : 'default'}
          text={record.isCustom ? '是' : '否'}
        />
      ),
    },
    {
      title: '数据库类型',
      dataIndex: 'dbType',
      align: 'center',
      hideInSearch: true,
      customRender: ({ record }) => {
        return (
          <Tag color={'cyan'}>
            {options.options.filter(x => x.value == record.dbType)?.[0]?.label}
          </Tag>
        )
      },
    },
    {
      title: '数据库链接',
      dataIndex: 'dbConn',
      align: 'center',
      hideInSearch: true,
    },
    {
      title: '备注',
      dataIndex: 'remark',
      align: 'center',
      hideInSearch: true,
    },
    {
      title: '创建人',
      dataIndex: 'createUser',
      align: 'center',
    },
    {
      title: '创建时间',
      align: 'center',
      dataIndex: 'createTime',
      formItemProps: {
        component: 'RangePicker',
        componentProps: {
          class: 'w-full',
        },
      },
    },
    {
      title: '更新时间',
      align: 'center',
      dataIndex: 'updateTime',
      hideInSearch: true,
      hideInTable: true,
    },
    {
      title: '操作',
      width: 200,
      dataIndex: 'ACTION',
      hideInSearch: true,
      align: 'center',
      fixed: 'right',
      actions: ({ record }) => [
        {
          label: t('common.edit'),
          auth: {
            perm: '/api/SysFactory/Update',
            effect: 'disable',
          },
          onClick: () => openModal(record),
        },
        {
          label: t('common.delete'),
          danger: true,
          auth: {
            perm: '/api/SysFactory/Delete',
            effect: 'disable',
          },
          popConfirm: {
            title: t('column.confirmDel'),
            onConfirm: () => delRowConfirm(record.id),
          },
        },
      ],
    },
  ]

  const options = {
    options: [
      {
        label: 'MySql',
        value: '0',
      },
      {
        label: 'SqlServer',
        value: '1',
      },
      {
        label: 'Sqlite',
        value: '2',
      },
      {
        label: 'Oracle',
        value: '3',
      },
      {
        label: 'PostgreSQL',
        value: '4',
      },
      {
        label: 'Dm',
        value: '5',
      },
      {
        label: 'Kdbndp',
        value: '6',
      },
    ],
  }

  /**
   * 表单信息
   */
  const formSchemas: FormSchema[] = [
    {
      field: 'id',
      component: 'Input',
      label: 'id',
      vIf: false,
    },
    {
      field: 'code',
      component: 'Input',
      label: '工厂代码',
      dynamicDisabled: ({ formModel }) => Boolean(formModel['id']),
      rules: [
        {
          required: true,
          validator: async (_, value) => {
            if (!value) {
              return Promise.reject('工厂代码不能为空!')
            }
            if (!/^\w{1,20}$/.test(value)) {
              return Promise.reject('格式只能包含数字、英文字母以及_, 并且长度不能超过20位!')
            }
            return Promise.resolve()
          },
          trigger: 'change',
        },
      ],
    },
    {
      field: 'name',
      component: 'Input',
      label: '工厂名称',
      rules: [{ required: true, type: 'string' }],
    },
    {
      field: 'isCustom',
      component: 'Switch',
      label: '是否自定义配置',
      defaultValue: false,
    },
    {
      field: 'dbType',
      component: 'Select',
      label: '数据库类型',
      vIf: ({ formModel }) => formModel['isCustom'],
      componentProps: options,
      rules: [{ required: true, type: 'string' }],
    },
    {
      field: 'dbConn',
      component: 'InputTextArea',
      label: '数据库链接',
      vIf: ({ formModel }) => formModel['isCustom'],
      rules: [{ required: true, type: 'string' }],
    },
  ]
</script>
