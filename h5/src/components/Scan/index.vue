<template>
  <view class="reader-box" v-if="isScaning">
    <QrcodeStream @detect="onDetect">
      <view class="reader"></view>
    </QrcodeStream>
  </view>
</template>

<script setup lang="ts" name="Scan">
  import { QrcodeStream } from 'vue-qrcode-reader'

  const props = defineProps({
    open: {
      type: Boolean,
      default: false
    }
  })

  const emit = defineEmits(['success'])
  const isScaning = ref(props.open)

  const onDetect = detectedCodes => {
    const target = detectedCodes[0]
    if (target && target.rawValue) {
      isScaning.value = false
      emit('success', target.rawValue)
    }
  }

  watch(
    () => props.open,
    newVal => {
      isScaning.value = newVal
    }
  )
</script>

<style lang="scss" scoped>
  .reader-box {
    position: fixed;
    top: 0;
    bottom: 0;
    left: 0;
    right: 0;
    background-color: rgba(0, 0, 0, 0.5);
  }

  .reader {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    height: 100%;
  }
</style>
