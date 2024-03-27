import { DEFAULT_PAGE, DEFAULT_PAGE_SIZE } from 'public/config/search-params'
import type { PaginatedCollection } from '@/types/common/paginated-collection'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'

export const getInitialPublications = async (): Promise<
  PaginatedCollection<PublicationSummary>
> => {
  return fetch(
    `${import.meta.env.PUBLIC_API_URL}/publications?Page=${DEFAULT_PAGE}&PageSize=${DEFAULT_PAGE_SIZE}`
  ).then((response) => response.json())
}

export const searchPublications = async (
  searchText: string,
  page: number
): Promise<PaginatedCollection<PublicationSummary>> => {
  return fetch(
    `${import.meta.env.PUBLIC_API_URL}/publications/search?Page=${page}&SearchTerm=${searchText}&PageSize=${DEFAULT_PAGE_SIZE}`
  ).then((response) => response.json())
}
