import { useSearchContext } from '@/contexts/search-context'
import { Input } from '../ui/input'
import { useCallback } from 'react'
import { MagnifyingGlassIcon } from '@radix-ui/react-icons'

export const SearchInput = () => {
  const { searchText, setSearchText } = useSearchContext()

  const onChangeHandler = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchText(e.target.value)
  }, [])

  return (
    <div className="relative mx-auto mb-6 w-full max-w-[700px]">
      <Input
        className="w-full p-4 py-6 pr-12 text-lg"
        aria-label="Search docs input"
        placeholder="Enter your search here"
        value={searchText}
        onChange={onChangeHandler}
      />
      <div className="absolute right-4 top-1/2 -translate-y-1/2">
        <MagnifyingGlassIcon className="h-6 w-6 text-muted-foreground" />
      </div>
    </div>
  )
}
