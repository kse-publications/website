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
import { captureEvent } from '@/services/posthog/posthog'

interface SelectItem {
  value: string
  label: string
}

export const SearchFilters = () => {
  const { filters, selectedFilters, setSelectedFilters } = useSearchContext()

  const handleFilterChange = (id: number, inputValue: string, name: string) => {
    captureEvent('filter_change', { filter: name, value: inputValue })

    if (inputValue === 'reset') {
      setSelectedFilters((prev) => prev.filter((item) => item.id !== id))
      return
    }

    setSelectedFilters((prev) => {
      const index = prev.findIndex((item) => item.id === id)
      const clonedPrev = [...prev]
      const value = +inputValue

      if (index === -1) {
        return [...clonedPrev, { id, values: [value] }]
      }

      clonedPrev[index].values.includes(value)
        ? (clonedPrev[index].values = clonedPrev[index].values.filter((item) => item !== value))
        : (clonedPrev[index].values = [value])

      return clonedPrev
    })
  }

  return (
    <div className="flex flex-wrap justify-center gap-6">
      {(filters || []).map((filter) => {
        let selectedFilterValue = ''
        const selectedFilter = selectedFilters.find((item) => item.id === filter.id)

        if (selectedFilter) {
          selectedFilterValue = selectedFilter.values[0].toString()
        }

        return (
          <Select
            key={filter.id}
            onValueChange={(value) => handleFilterChange(filter.id, value, filter.name)}
            value={selectedFilterValue}
          >
            <SelectTrigger className="w-full xs:w-[171px]">
              <SelectValue placeholder={`Filter by ${filter.name.toLowerCase()}`} />
            </SelectTrigger>
            <SelectContent>
              <SelectGroup className="w-full">
                <SelectLabel>Filter by {filter.name.toLowerCase()}</SelectLabel>
                <SelectItem value={'reset'}>None</SelectItem>
                {filter.filters.map(({ id, value, matchedPublicationsCount }) => (
                  <SelectItem key={id} value={id.toString()}>
                    <div className="flex w-[150px] justify-between">
                      <span> {value} </span>
                      <span className="font-bold">{matchedPublicationsCount}</span>
                    </div>
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
