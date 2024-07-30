/**
 * 将路径中重复的正斜杆替换成单个斜杆隔开的字符串
 * @param path 要处理的路径
 * @returns {string} 将/去重后的结果
 */
export const uniqueSlash = (path: string) => {
  // path.replace(/(?<!:)\/{2,}/g, '/')
  // 兼容 mac系统
  return path.replace(/[^://]\/{2,}/g, function (match) {
    return `${match.substring(0, 1)}/`
  })
}
/**
 * 处理图片地址
 * @param imgUrl 图片地址
 */
export const renderImg = (imgUrl?: string | null) => {
  if (!imgUrl || imgUrl === 'undefined') return ''
  return imgUrl.startsWith('http')
    ? imgUrl
    : uniqueSlash(`${import.meta.env.VITE_STATIC_URL}${imgUrl}`).trim()
}
