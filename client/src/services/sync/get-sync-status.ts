const BASE_URL = import.meta.env.PUBLIC_SSR_API_URL

export const getSyncStatus = async () => {
  return fetch(`${BASE_URL}/sync/status`)
    .then((response) => response.json())
    .then((data) => data.isRunning)
}
