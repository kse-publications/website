import { DEFAULT_PAGE, DEFAULT_PAGE_SIZE } from '@/config/search-params'
import type { PaginatedCollection } from '@/types/common/paginated-collection'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import type { QueryParams, SearchPublicationsQueryParams } from '@/types/common/query-params'
import { getFiltersString } from '@/utils/parse-filters'


const getBaseUrl = () => {
  const isServer = typeof window === 'undefined'
  if (isServer && import.meta.env.PUBLIC_SSR_API_URL !== undefined) {
    return import.meta.env.PUBLIC_SSR_API_URL
  }
  
  return import.meta.env.PUBLIC_API_URL
}

export const getInitialPublications = async ({
  page = DEFAULT_PAGE,
  filters,
}: QueryParams): Promise<PaginatedCollection<PublicationSummary>> => {
  const BASE_URL = getBaseUrl()
  let url = `${BASE_URL}/publications?Page=${page}&PageSize=${DEFAULT_PAGE_SIZE}`

  if (filters?.length) {
    url += `&Filters=${getFiltersString(filters)}`
  }

  return fetch(url)
    .then((response) => response.json())
    .catch((error) => console.log(error))
}

export const searchPublications = async ({
  page,
  searchText,
  filters,
}: SearchPublicationsQueryParams): Promise<PaginatedCollection<PublicationSummary>> => {
  const BASE_URL = getBaseUrl()
  let url = `${BASE_URL}/publications/search?Page=${page}&SearchTerm=${searchText}&PageSize=${DEFAULT_PAGE_SIZE}`

  if (filters?.length) {
    url += `&Filters=${getFiltersString(filters)}`
  }

  return fetch(url).then((response) => response.json())
}

export const getPublication = async (id: string, clientUuid?: string): Promise<any> => {
  const BASE_URL = getBaseUrl()
  return fetch(`${BASE_URL}/publications/${id}`, {
    headers: clientUuid
      ? {
          'Client-Uuid': clientUuid,
        }
      : {},
  }).then((response) => response.json())
}
