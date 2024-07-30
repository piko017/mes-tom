/* eslint-disable prettier/prettier */
// eslint-disable-next-line prettier/prettier
module.exports = {
  root: true,
  /**环境提供预定义的全局变量 */
  env: {
    /**Node.js全局变量和Node.js范围 */
    node: true,
    /**浏览器全局变量 */
    browser: true
  },
  /**定义ESLint的解析器 */
  parser: 'vue-eslint-parser',
  parserOptions: {
    parser: '@typescript-eslint/parser',
    ecmaVersion: 2020,
    sourceType: 'module',
    jsxPragma: 'React',
    ecmaFeatures: {
      jsx: true,
      tsx: true
    }
  },
  /**定义文件继承的子规范 */
  extends: [
    'eslint:recommended',
    'plugin:vue/vue3-essential',
    // 'plugin:@typescript-eslint/recommended',
    './.eslintrc-auto-import.json',
    'plugin:vue/vue3-recommended',
    'prettier'
  ],
  plugins: ['vue', '@typescript-eslint', 'prettier', 'import'],

  rules: {
    // js/ts
    // 'no-console': ['warn', { allow: ['error'] }],
    'no-restricted-syntax': ['error', 'LabeledStatement', 'WithStatement'],
    camelcase: ['error', { properties: 'never' }],

    'no-undef': 'off',
    'no-var': 'error',
    'no-empty': ['error', { allowEmptyCatch: true }],
    'no-void': 'error',
    'prefer-const': ['warn', { destructuring: 'all', ignoreReadBeforeAssign: true }],
    'prefer-template': 'error',
    'object-shorthand': ['error', 'always', { ignoreConstructors: false, avoidQuotes: true }],
    'block-scoped-var': 'error',
    'no-constant-condition': ['error', { checkLoops: false }],

    'no-redeclare': 'off',
    '@typescript-eslint/no-redeclare': 'error',
    '@typescript-eslint/ban-ts-comment': 'off',
    '@typescript-eslint/ban-types': 'off',
    '@typescript-eslint/explicit-module-boundary-types': 'off',
    '@typescript-eslint/no-empty-function': 'off',
    '@typescript-eslint/no-explicit-any': 'off',
    '@typescript-eslint/no-non-null-assertion': 'off',
    '@typescript-eslint/no-non-null-asserted-optional-chain': 'off',
    // '@typescript-eslint/consistent-type-imports': ['error', { disallowTypeAnnotations: false }],
    '@typescript-eslint/no-var-requires': 'off',
    '@typescript-eslint/no-unused-vars': [
      'off',
      {
        argsIgnorePattern: '^_',
        varsIgnorePattern: '^_'
      }
    ],
    'no-unused-vars': [
      'off',
      {
        argsIgnorePattern: '^_',
        varsIgnorePattern: '^_'
      }
    ],
    'no-unsafe-optional-chaining': ['off', { disallowArithmeticOperators: true }],

    // vue
    'vue/no-v-html': 'off',
    'vue/require-default-prop': 'off',
    'vue/require-explicit-emits': 'off',
    'vue/multi-word-component-names': 'off',
    'vue/attributes-order': 'off', // 属性顺序
    'vue/no-reserved-component-names': 'off',

    // prettier
    'prettier/prettier': 'error',

    // import
    'import/first': 'error',
    'import/no-duplicates': 'error',
    'import/order': [
      'off',
      {
        groups: ['builtin', 'external', 'internal', 'parent', 'sibling', 'index', 'object', 'type'],

        pathGroups: [
          {
            pattern: 'vue',
            group: 'external',
            position: 'before'
          },
          {
            pattern: '@vue/**',
            group: 'external',
            position: 'before'
          }
        ],
        pathGroupsExcludedImportTypes: ['type']
      }
    ]
  },
  globals: {
    //可以定义全局中的变量的权限（只读，可读可写）
    defineProps: 'readonly',
    defineEmits: 'readonly',
    defineExpose: 'readonly',
    withDefaults: 'readonly',
    uni: 'readonly'
  },
  ignorePatterns: [
    // # 忽略目录
    '/dist',
    '/public',
    '/src/public',
    '/src/static',
    '/node_modules',
    // # 忽略文件
    '**/*-min.js',
    '**/*.min.js',
    '**/*-min.css',
    '**/*.min.css',
    '**/*.tsbuildinfo',
    '**/*.config.js',
    '**/*.config.ts',
    '/src/tmui/**/*',
    '/src/manifest.json'
  ]
}
