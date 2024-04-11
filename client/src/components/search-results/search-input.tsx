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
      className="my-4 w-full"
      aria-label="Search docs input"
      placeholder="Enter your search here"
      value={searchText}
      onChange={onChangeHandler}
    />
  )
}
