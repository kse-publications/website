import { useSearchContext } from '@/contexts/search-context'
import { captureEvent } from '@/services/posthog/posthog'

import { ChevronDownIcon } from '@radix-ui/react-icons'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'

export const SearchFilters = () => {
  const { filters, selectedFilters, setSelectedFilters } = useSearchContext()

  const handleFilterChange = (id: number, inputValue: string, name: string) => {
    captureEvent('filter_change', { filter: name, value: inputValue })

    setSelectedFilters((prev) => {
      const value = +inputValue

      if (inputValue === 'reset') {
        return prev.filter((filter) => filter.id !== id)
      }

      const index = prev.findIndex((filter) => filter.id === id)

      if (index === -1) {
        return [...prev, { id, values: [value] }]
      } else {
        const filterGroup = prev[index]
        const valueIndex = filterGroup.values.indexOf(value)

        if (valueIndex === -1) {
          filterGroup.values.push(value)
        } else {
          filterGroup.values.splice(valueIndex, 1)
        }

        if (filterGroup.values.length === 0) {
          prev.splice(index, 1)
        }

        return [...prev]
      }
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
          <DropdownMenu key={filter.id}>
            <DropdownMenuTrigger asChild>
              <Button variant="outline" className="flex justify-between font-normal xs:w-[171px]">
                Filter by {filter.name.toLowerCase()}
                <ChevronDownIcon color="grey" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent>
              <DropdownMenuLabel className="pl-8">
                Filter by {filter.name.toLowerCase()}
              </DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuCheckboxItem
                checked={selectedFilterValue.length === 0}
                onCheckedChange={() => handleFilterChange(filter.id, 'reset', filter.name)}
              >
                None
              </DropdownMenuCheckboxItem>
              {filter.filters.map(({ id, value, matchedPublicationsCount }) => (
                <DropdownMenuCheckboxItem
                  key={id}
                  checked={selectedFilters.some((f) => f.id === filter.id && f.values.includes(id))}
                  onCheckedChange={() => handleFilterChange(filter.id, id.toString(), filter.name)}
                >
                  <div className="flex w-[150px] justify-between">
                    <span> {value} </span>
                    <span className="text-gray-500">{matchedPublicationsCount}</span>
                  </div>
                </DropdownMenuCheckboxItem>
              ))}
            </DropdownMenuContent>
          </DropdownMenu>
        )
      })}
    </div>
  )
}
