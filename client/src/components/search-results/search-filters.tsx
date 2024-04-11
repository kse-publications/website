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

interface SelectItem {
  value: string
  label: string
}

const sortBySelectItems: SelectItem[] = Object.values(FilterTypes).map((value) => ({
  value,
  label: value,
}))

export const SearchFilters = () => {
  const { filterType, setFilterType } = useSearchContext()

  return (
    <Select onValueChange={setFilterType as (value: string) => void} value={filterType || ''}>
      <SelectTrigger className="w-[180px]">
        <SelectValue placeholder="Filter by type" />
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          <SelectLabel>Filter by</SelectLabel>
          {sortBySelectItems.map((item) => (
            <SelectItem key={item.value} value={item.value}>
              {item.label}
            </SelectItem>
          ))}
        </SelectGroup>
      </SelectContent>
    </Select>
  )
}
