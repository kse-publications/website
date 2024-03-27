export interface PaginatedCollection<T> {
  totalCount: number
  resultCount: number
  items: T[]
}
