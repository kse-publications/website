import { DEFAULT_PAGE, DEFAULT_PAGE_SIZE } from '@/config/search-params'
import type { PaginatedCollection } from '@/types/common/paginated-collection'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import type { FilterTypes } from '@/types/common/filter-types'
import { SortOrders } from '@/types/common/sort-orders'

export const getInitialPublications = async (
  page = DEFAULT_PAGE
): Promise<PaginatedCollection<PublicationSummary>> => {
  return fetch(
    `${import.meta.env.PUBLIC_API_URL}/publications?Page=${page}&PageSize=${DEFAULT_PAGE_SIZE}`
  )
    .then((response) => response.json())
    .catch((error) => console.log(error))
}

interface SearchPublications {
  searchText: string
  page: number
  filterType: FilterTypes
  sortOrder: SortOrders
}

export const searchPublications = async ({
  searchText,
  page,
  filterType,
  sortOrder,
}: SearchPublications): Promise<PaginatedCollection<PublicationSummary>> => {
  const isAscending = sortOrder === SortOrders.ASC

  return fetch(
    `${import.meta.env.PUBLIC_API_URL}/publications/search?Page=${page}&SearchTerm=${searchText}&PageSize=${DEFAULT_PAGE_SIZE}&SortBy=${filterType}&Ascending=${isAscending}`
  ).then((response) => response.json())
}
