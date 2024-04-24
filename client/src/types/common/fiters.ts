export interface IFilterItem {
  id: number
  value: string
}

export interface IFilter {
  id: number
  name: string
  filters: IFilterItem[]
}
