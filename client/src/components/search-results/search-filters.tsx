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
import { FilterTypes } from '@/types/common/filter-types'

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

  const handleFilterChange = (value: number) => {
    setSelectedFilters((prev) =>
      prev.includes(value) ? prev.filter((item) => item !== value) : [...prev, value]
    )
  }

  return (
    <div className="flex justify-center gap-6">
      {(filters || []).map((filter) => {
        const selectedFilter = filter.filters.find(({ id }) => selectedFilters.includes(id))

        return (
          <Select
            key={filter.id}
            onValueChange={(value) => handleFilterChange(+value)}
            value={selectedFilter ? selectedFilter.id.toString() : ''}
          >
            <SelectTrigger className="w-fit">
              <SelectValue placeholder={`Filter by ${filter.name.toLowerCase()}`} />
            </SelectTrigger>
            <SelectContent>
              <SelectGroup>
                <SelectLabel>Filter by {filter.name.toLowerCase()}</SelectLabel>
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
