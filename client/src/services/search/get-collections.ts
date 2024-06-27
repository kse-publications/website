import { DEFAULT_PAGE_SIZE, DEFAULT_PAGE } from '@/config/search-params'
import type { ICollection, IDetailedCollection } from '@/types/common/collection'

const BASE_URL = import.meta.env.PUBLIC_API_URL

export const getAllCollections = async (): Promise<ICollection[]> => {
  let url = `${BASE_URL}/collections`

  return fetch(url)
    .then((response) => response.json())
    .catch((error) => console.log(error))
}

export const getDetailedCollection = async (
  slug: string,
  page = DEFAULT_PAGE
): Promise<IDetailedCollection> => {
  let url = `${BASE_URL}/collections/${slug}?Page=${page}&PageSize=${DEFAULT_PAGE_SIZE}`

  return fetch(url)
    .then((response) => response.json())
    .catch((error) => console.log(error))
}
