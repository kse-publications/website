import { useSearchContext } from '@/contexts/search-context'
import { Input } from '../ui/input'

export const SearchInput = () => {
  const { searchText, setSearchText } = useSearchContext()

  return (
    <Input
      className="my-4 w-full"
      aria-label="Search docs input"
      placeholder="I'm searching for..."
      value={searchText}
      onChange={(e) => setSearchText(e.target.value)}
    />
  )
}
