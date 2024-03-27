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
import { SortOrders } from '@/types/common/sort-orders'
import { FilterTypes } from '@/types/common/filter-types'

interface SelectItem {
  value: string
  label: string
}

const sortBySelectItems: SelectItem[] = [
  { value: FilterTypes.LAST_MODIFIED, label: 'Last modified' },
  { value: FilterTypes.YEAR, label: 'Year' },
  { value: FilterTypes.TITLE, label: 'Title' },
]

const orderBySelectItems: SelectItem[] = [
  { value: SortOrders.ASC, label: 'Ascending' },
  { value: SortOrders.DESC, label: 'Descending' },
]

export const SearchFilters = () => {
  const { sortOrder, setSortOrder, filterType, setFilterType } = useSearchContext()

  return (
    <div className="flex gap-4">
      <Select onValueChange={setFilterType as (value: string) => void} value={filterType || ''}>
        <SelectTrigger className="w-[180px]">
          <SelectValue placeholder="Select sort by" />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectLabel>Sort by</SelectLabel>
            {sortBySelectItems.map((item) => (
              <SelectItem key={item.value} value={item.value}>
                {item.label}
              </SelectItem>
            ))}
          </SelectGroup>
        </SelectContent>
      </Select>
      <Select onValueChange={setSortOrder as (value: string) => void} value={sortOrder || ''}>
        <SelectTrigger className="w-[180px]">
          <SelectValue placeholder="Select a order" />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectLabel>Order</SelectLabel>
            {orderBySelectItems.map((item) => (
              <SelectItem key={item.value} value={item.value}>
                {item.label}
              </SelectItem>
            ))}
          </SelectGroup>
        </SelectContent>
      </Select>
    </div>
  )
}
