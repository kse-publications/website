import type { APIRoute } from 'astro'

const APIurl = import.meta.env.PUBLIC_API_URL
const hostUrl = import.meta.env.PUBLIC_HOST_URL

export const GET: APIRoute = async () => {
  const sitemap = await fetch(`${APIurl}/sitemap.xml?baseUrl=${hostUrl}`).then((res) => res.text())
  return new Response(sitemap, {
    headers: {
      'Content-Type': 'application/xml',
    },
  })
}
