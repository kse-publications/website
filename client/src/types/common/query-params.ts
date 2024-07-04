import type { SelectedFilters } from './selected-filters'

export interface QueryParams {
  page?: number
  filters?: SelectedFilters
}

export interface SearchPublicationsQueryParams extends QueryParams {
  searchText: string
}

export interface RelatedPublicationsQueryParams {
  id: string
  authors: string
  page?: number
}

export interface SimilarPublicationsQueryParams {
  id: string
  page?: number
}
