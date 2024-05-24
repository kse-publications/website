import type { SelectedFilters } from './selected-filters'

export interface QueryParams {
  page?: number
  filters?: SelectedFilters
}

export interface SearchPublicationsQueryParams extends QueryParams {
  searchText: string
}
