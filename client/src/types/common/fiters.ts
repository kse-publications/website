export interface IFilterItem {
  id: number
  value: string
  matchedPublicationsCount: number
}

export interface IFilter {
  id: number
  name: string
  filters: IFilterItem[]
}
