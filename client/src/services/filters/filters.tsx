import type { IFilter } from '@/types/common/fiters'

const BASE_URL = import.meta.env.PUBLIC_API_URL

export const getFilters = async (): Promise<IFilter[]> => {
  const url = `${BASE_URL}/publications/filters`

  console.log({ url })

  return fetch(url)
    .then((response) => response.json())
    .catch((error) => console.log(error))
}
