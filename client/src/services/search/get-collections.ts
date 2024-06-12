import type { ICollection } from '@/types/common/collection'

const BASE_URL = import.meta.env.PUBLIC_API_URL

export const getAllCollections = async (): Promise<ICollection[]> => {
  let url = `${BASE_URL}/collections`

  return fetch(url)
    .then((response) => response.json())
    .catch((error) => console.log(error))
}
