export interface QueryParams {
  page?: number
  filters?: number[]
}

export interface SearchPublicationsQueryParams extends QueryParams {
  searchText: string
}
