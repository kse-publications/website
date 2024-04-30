import { useSearchContext } from '@/contexts/search-context'
import { Input } from '../ui/input'
import { useCallback } from 'react'

export const SearchInput = () => {
  const { searchText, setSearchText } = useSearchContext()

  const onChangeHandler = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchText(e.target.value)
  }, [])

  return (
    <Input
      className="mx-auto mb-6 w-full max-w-[562px]"
      aria-label="Search docs input"
      placeholder="Enter your search here"
      value={searchText}
      onChange={onChangeHandler}
    />
  )
}
