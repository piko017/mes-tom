使用 `pnpm` 管理依赖, 安装方式: npm install -g pnpm

> 安装依赖:

```shell

pnpm install

```

> 运行:

```shell

pnpm dev:h5

```

> 打包:

```shell

pnpm build:xxx

```

---

> 项目介绍:

1. 基于 uniapp 框架`cli`方式开发
2. 技术点: vue3 + ts + vite + pinia + scss + unocss
3. 请求库使用 `alova`
4. 图标库使用 `IconPark`,格式: `i-icon-park-outline-xxx` , 地址: [https://icones.js.org/collection/icon-park-outline](https://icones.js.org/collection/icon-park-outline)
5. UI 框架使用`TMUI`, 文档地址: [https://tmui.design/#/](https://tmui.design/#/), 已经全局引入
6. 安装 `eslint`、`prettier` 插件统一代码风格
7. 官方建议使用如下代码结构 (每个页面使用 `tm-app` 包裹):

```vue
<template>
  <tm-app>
    <!-- 这里是你的页面代码。 -->
  </tm-app>
</template>
```
