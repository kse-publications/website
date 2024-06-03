import { defineConfig } from 'astro/config'
import tailwind from '@astrojs/tailwind'
import react from '@astrojs/react'

import vercel from '@astrojs/vercel/serverless'
import node from '@astrojs/node'

const adapter = process.env.ASTRO_ADAPTER === 'node' ? node({ mode: 'standalone' }) : vercel()

// https://astro.build/config
export default defineConfig({
  output: 'server',
  integrations: [
    tailwind({
      applyBaseStyles: true,
    }),
    react(),
  ],
  adapter: adapter,
})
