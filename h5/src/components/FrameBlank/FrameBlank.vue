<template>
  <iframe
    ref="frameRef"
    :src="src"
    frameborder="0"
    class="iframe_main"
    @load="hideLoading"
  ></iframe>
</template>
<script lang="ts" setup name="FrameBlank">
  defineProps({
    src: {
      type: String,
      default: ''
    }
  })

  const frameRef = ref<HTMLIFrameElement>()

  /**
   * 隐藏报表的header和tab元素
   * @param iframe iframe
   */
  const hideReportCss = (iframe: HTMLIFrameElement) => {
    const head = iframe.contentWindow?.document.getElementsByTagName('head')
    const styleTag = document.createElement('style')
    styleTag.id = 'kpiReportStyle'
    styleTag.innerHTML = `
          #mom-home{
            display: none !important;
          }
        `
    styleTag.setAttribute('type', 'text/css')
    head?.[0]?.appendChild(styleTag)
  }

  function init() {
    const iframe = unref(frameRef)
    if (!iframe) {
      return
    }

    hideReportCss(iframe)
  }

  const hideLoading = () => {
    init()
  }
</script>
