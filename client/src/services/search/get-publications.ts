import { DEFAULT_PAGE, DEFAULT_PAGE_SIZE } from '@/config/search-params'
import type { PaginatedCollection } from '@/types/common/paginated-collection'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import type { QueryParams, SearchPublicationsQueryParams } from '@/types/common/query-params'

const BASE_URL = import.meta.env.PUBLIC_API_URL

console.log(BASE_URL)

const getFiltersString = (filters: number[]): string => {
  return filters.join('-')
}

export const getInitialPublications = async ({
  page = DEFAULT_PAGE,
  filters,
}: QueryParams): Promise<PaginatedCollection<PublicationSummary>> => {
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
  let url = `${BASE_URL}/publications/search?Page=${page}&SearchTerm=${searchText}&PageSize=${DEFAULT_PAGE_SIZE}`

  if (filters?.length) {
    url += `&Filters=${getFiltersString(filters)}`
  }

  return fetch(url).then((response) => response.json())
}

export const getPublication = async (id: string): Promise<any> => {
  return fetch(`${BASE_URL}/publications/${id}`).then((response) => response.json())
}
