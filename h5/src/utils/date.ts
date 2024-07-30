/**
 * 格式化消息日期(当天: 显示时分, 昨天: 昨天, 今年: 月日, 其他: 显示年月日), 时分都按照2位字符显示
 * @param dateStr 日期字符串
 * @returns
 */
export const formatMsgDate = (dateStr: string) => {
  const date = new Date(dateStr)
  if (date.toDateString() === new Date().toDateString())
    return `${date.getHours().toString().padStart(2, '0')}:${date
      .getMinutes()
      .toString()
      .padStart(2, '0')}`
  else if (date.toDateString() === new Date(Date.now() - 24 * 60 * 60 * 1000).toDateString())
    return '昨天'
  else if (date.getFullYear() === new Date().getFullYear())
    return `${date.getMonth() + 1}月${date.getDate()}日`
  else return `${date.getFullYear()}年${date.getMonth() + 1}月${date.getDate()}日`
}

/**
 * 格式化消息明细的时间(当天: 显示时分, 昨天: 昨天 时分, 今年: 月日 时分, 其他: 年月日 时分), 时分都按照2位字符显示
 * @param dateStr 日期字符串
 */
export const formatMsgTime = (dateStr: string) => {
  const date = new Date(dateStr)
  if (date.toDateString() === new Date().toDateString())
    return `${date.getHours().toString().padStart(2, '0')}:${date
      .getMinutes()
      .toString()
      .padStart(2, '0')}`
  else if (date.toDateString() === new Date(Date.now() - 24 * 60 * 60 * 1000).toDateString())
    return (
      '昨天 ' +
      `${date.getHours().toString().padStart(2, '0')}:${date
        .getMinutes()
        .toString()
        .padStart(2, '0')}`
    )
  else if (date.getFullYear() === new Date().getFullYear())
    return `${date.getMonth() + 1}月${date.getDate()}日 ${date
      .getHours()
      .toString()
      .padStart(2, '0')}:${date.getMinutes().toString().padStart(2, '0')}`
  else
    return `${date.getFullYear()}年${date.getMonth() + 1}月${date.getDate()}日 ${date
      .getHours()
      .toString()
      .padStart(2, '0')}:${date.getMinutes().toString().padStart(2, '0')}`
}
