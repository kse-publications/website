const getBaseUrl = () => {
  const isServer = typeof window === 'undefined'
  const SSR_URL = import.meta.env.PUBLIC_SSR_API_URL

  if (isServer && SSR_URL) {
    return SSR_URL
  }

  return import.meta.env.PUBLIC_API_URL
}

export const getOverallStatistics = async () => {
  const BASE_URL = getBaseUrl()
  return fetch(`${BASE_URL}/stats/overall`).then((response) => response.json())
}

export const getRecentStatistics = async () => {
  const BASE_URL = getBaseUrl()
  return fetch(`${BASE_URL}/stats/recent`).then((response) => response.json())
}
