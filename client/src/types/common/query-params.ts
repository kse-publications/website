import type { FilterTypes } from './filter-types'

export interface QueryParams {
  page?: number
  filterType?: FilterTypes | null
}

export interface SearchPublicationsQueryParams extends QueryParams {
  searchText: string
}
