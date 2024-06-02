import { DEFAULT_PAGE_SIZE } from '@/config/search-params'
import type { IFilter } from '@/types/common/fiters'
import type { SelectedFilters } from '@/types/common/selected-filters'
import { getFiltersString } from '@/utils/parse-filters'

const BASE_URL = import.meta.env.PUBLIC_API_URL

interface IGetFiltersArgs {
  filters: SelectedFilters
  searchText: string
}

export const getFilters = async ({ filters, searchText }: IGetFiltersArgs): Promise<IFilter[]> => {
  let url = `${BASE_URL}/publications/filters?PageSize=${DEFAULT_PAGE_SIZE}&SearchTerm=${searchText}`

  if (filters?.length) {
    url += `&Filters=${getFiltersString(filters)};`
  }

  return fetch(url)
    .then((response) => response.json())
    .catch((error) => console.log(error))
}
