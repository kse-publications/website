import type { APIRoute } from 'astro'

const robotsTxt = `
User-agent: *
Allow: /

Sitemap: ${new URL('sitemap.xml', import.meta.env.PUBLIC_HOST_URL).href}
`.trim()

export const GET: APIRoute = () => {
  return new Response(robotsTxt, {
    headers: {
      'Content-Type': 'text/plain; charset=utf-8',
    },
  })
}
