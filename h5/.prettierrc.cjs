//////////////////////////////////////////////////////////////////
// 官网参考：https://prettier.io/docs/en/options.html#tab-width //
////////////////////////////////////////////////////////////////
module.exports = {
  /**.pellerrc 的架构 */
  $schema: 'https://json.schemastore.org/prettierrc',
  /**在所有代码语句的末尾添加分号 */
  semi: false,
  vueIndentScriptAndStyle: true,
  /**使用 4 个空格缩进 */
  // tabWidth: 4,
  /**每行最多 100 字符 */
  printWidth: 100,
  /**指定文件的结尾换行符 */
  endOfLine: 'auto',
  /**使用单引号代替双引号 */
  singleQuote: true,
  // prettier-自动格式化代码
  formatOnSave: true,
  htmlWhitespaceSensitivity: 'strict',
  trailingComma: 'none',
  // prettier- (x) => {} 箭头函数参数只有一个时是否要有小括号。avoid：省略括号
  arrowParens: 'avoid'
}
