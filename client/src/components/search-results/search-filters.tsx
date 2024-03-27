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

export const SearchFilters = () => {
  const { sortOrder, setSortOrder, filterType, setFilterType } = useSearchContext()

  return (
    <div className="flex gap-4">
      <Select>
        <SelectTrigger className="w-[180px]">
          <SelectValue placeholder="Select sort by" />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectLabel>Sort by</SelectLabel>
            <SelectItem value="lastModified">Last modified</SelectItem>
            <SelectItem value="year">Year</SelectItem>
            <SelectItem value="title">Title</SelectItem>
          </SelectGroup>
        </SelectContent>
      </Select>
      <Select>
        <SelectTrigger className="w-[180px]">
          <SelectValue placeholder="Select a order" />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectLabel>Order</SelectLabel>
            <SelectItem value="Ascending">Ascending</SelectItem>
            <SelectItem value="Descending">Descending</SelectItem>
          </SelectGroup>
        </SelectContent>
      </Select>
    </div>
  )
}
