import globals from 'globals'
import js from '@eslint/js'
import typescript from '@typescript-eslint/eslint-plugin'
import astro from 'eslint-plugin-astro'
import astroJsxA11y from 'eslint-plugin-jsx-a11y'

export default [
  {
    files: ['*.js', '*.jsx'],
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.node,
      },
      ecmaVersion: 'latest',
      sourceType: 'module',
    },
    rules: {
      ...js.configs.recommended.rules,
    },
  },
  {
    files: ['*.ts', '*.tsx'],
    languageOptions: {
      parser: '@typescript-eslint/parser',
      globals: {
        ...globals.browser,
        ...globals.node,
      },
      ecmaVersion: 'latest',
      sourceType: 'module',
    },
    plugins: {
      '@typescript-eslint': typescript,
    },
    rules: {
      ...typescript.configs.recommended.rules,
    },
  },
  {
    files: ['*.astro'],
    languageOptions: {
      parser: 'astro-eslint-parser',
      globals: {
        ...globals.browser,
        ...globals.node,
      },
      parserOptions: {
        parser: '@typescript-eslint/parser',
        extraFileExtensions: ['.astro'],
      },
    },
    plugins: {
      astro,
      'jsx-a11y': astroJsxA11y,
    },
    rules: {
      ...astro.configs.recommended.rules,
      ...astroJsxA11y.configs.recommended.rules,
    },
  },
  {
    ignores: ['node_modules/', 'build/', 'package/', '.env', '.env.*', 'src/env.d.ts'],
  },
]
