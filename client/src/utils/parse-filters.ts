import type { SelectedFilters } from '@/types/common/selected-filters'

export const getFiltersString = (filters: SelectedFilters): string => {
  return filters
    .filter((filter) => filter.values.length > 0)
    .map((filter) => {
      return `${filter.id}:${filter.values.join('-')}`
    })
    .join(';')
}

export const getFiltersFromString = (filtersString: string | null): SelectedFilters => {
  if (!filtersString) {
    return []
  }

  const parsedFilters = filtersString.split(';').map((filter) => {
    const [id, values] = filter.split(':')
    return {
      id: Number(id),
      values: values.split('-').map(Number),
    }
  })

  return parsedFilters
}
