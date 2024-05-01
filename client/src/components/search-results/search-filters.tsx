import React, { useState } from 'react'
import { useSearchContext } from '@/contexts/search-context'
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from '../ui/select'

interface Filter {
  id: string
  name: string
  filters: { id: string; value: string }[]
}

interface SelectItem {
  value: string
  label: string
}

export const SearchFilters = () => {
  const { filters, selectedFilters, setSelectedFilters } = useSearchContext()

  const handleFilterChange = (value: string, siblingsIds: number[]) => {
    setSelectedFilters((prev) => {
      if (value === 'reset') {
        return prev.filter((item) => !siblingsIds.includes(item))
      }
      return prev.includes(+value)
        ? prev.filter((item) => item !== +value)
        : [...prev.filter((item) => !siblingsIds.includes(item)), +value]
    })
  }

  return (
    <div className="flex flex-wrap justify-center gap-6">
      {(filters || []).map((filter) => {
        const selectedFilter = filter.filters.find(({ id }) => selectedFilters.includes(id))

        return (
          <Select
            key={filter.id}
            onValueChange={(value) =>
              handleFilterChange(
                value,
                filter.filters.map(({ id }) => +id)
              )
            }
            value={selectedFilter ? selectedFilter.id.toString() : ''}
          >
            <SelectTrigger className="w-full xs:w-[171px]">
              <SelectValue placeholder={`Filter by ${filter.name.toLowerCase()}`} />
            </SelectTrigger>
            <SelectContent>
              <SelectGroup>
                <SelectLabel>Filter by {filter.name.toLowerCase()}</SelectLabel>
                <SelectItem value={'reset'}>None</SelectItem>
                {filter.filters.map(({ id, value }) => (
                  <SelectItem key={id} value={id.toString()}>
                    {value}
                  </SelectItem>
                ))}
              </SelectGroup>
            </SelectContent>
          </Select>
        )
      })}
    </div>
  )
}
