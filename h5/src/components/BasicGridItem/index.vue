<template>
  <tm-grid-item :height="130">
    <!-- <Iconify :size="60" :icon="props.icon"></Iconify> -->
    <Icon :width="30" :icon="props.icon" :color="props.color" />
    <tm-text _class="pt-10" :font-size="22" :label="props.label"></tm-text>
  </tm-grid-item>
</template>

<script lang="ts" setup>
  import { gridItemProps } from './props'
  import { Icon } from '@iconify/vue'
  // import { Iconify } from '@/components/Iconify'

  const props = defineProps(gridItemProps)
</script>
