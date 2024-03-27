import {
  createContext,
  type Context,
  type ReactNode,
  useContext,
  type Dispatch,
  type SetStateAction,
  useState,
  useCallback,
  useMemo,
  useEffect,
} from 'react'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import { searchPublications } from '@/services/search/get-publications'
import {
  DEFAULT_PAGE,
  DEFAULT_PAGE_SIZE,
  SEARCH_TEXT_DEBOUNCE_MS,
} from 'public/config/search-params'
import { useDebounce } from 'use-debounce'

interface ISearchContext {
  searchText: string
  setSearchText: Dispatch<SetStateAction<string>>

  filterType: string
  setFilterType: Dispatch<SetStateAction<string>>

  sortOrder: string
  setSortOrder: Dispatch<SetStateAction<string>>

  searchResults: PublicationSummary[]
  setSearchResults: Dispatch<SetStateAction<PublicationSummary[]>>

  totalResults: number
  setTotalResults: Dispatch<SetStateAction<number>>

  isLoading: boolean
  setIsLoading: Dispatch<SetStateAction<boolean>>

  error: string
  setError: Dispatch<SetStateAction<string>>

  loadMoreHandler: () => void
}

export const SearchContext = createContext<ISearchContext | null>(null) as Context<ISearchContext>

interface SearchContextProviderProps {
  children: ReactNode
  initialSearchResults: PublicationSummary[]
  initialTotalResults: number
}

const SearchContextProvider = ({
  children,
  initialSearchResults,
  initialTotalResults,
}: SearchContextProviderProps) => {
  const [searchText, setSearchText] = useState('')
  const [filterType, setFilterType] = useState('')
  const [sortOrder, setSortOrder] = useState('')

  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [totalResults, setTotalResults] = useState(initialTotalResults || 0)
  const [searchResults, setSearchResults] = useState<PublicationSummary[]>(
    initialSearchResults || []
  )

  const [debouncedSearchText] = useDebounce(searchText, SEARCH_TEXT_DEBOUNCE_MS)

  useEffect(() => {
    if (debouncedSearchText.trim()) {
      fetchPublications(debouncedSearchText)
    }
  }, [debouncedSearchText])

  const currentPage = useMemo(
    () => Math.ceil(searchResults.length / DEFAULT_PAGE_SIZE),
    [searchResults]
  )

  const fetchPublications = useCallback(async (searchText = '', page = DEFAULT_PAGE) => {
    setIsLoading(true)
    setError('')

    if (page === DEFAULT_PAGE) {
      setSearchResults([])
    }

    try {
      const paginatedResponse = await searchPublications(searchText, page)
      setSearchResults((prevResults) => [...prevResults, ...paginatedResponse.items])
      setTotalResults(paginatedResponse.count)
    } catch (error) {
      console.error(error)
      error instanceof Error && setError(error.message)
    } finally {
      setIsLoading(false)
    }
  }, [])

  const loadMoreHandler = useCallback(() => {
    fetchPublications(searchText, currentPage + 1)
  }, [searchText, currentPage])

  const value: ISearchContext = {
    loadMoreHandler,

    searchText,
    setSearchText,

    filterType,
    setFilterType,

    sortOrder,
    setSortOrder,

    searchResults,
    setSearchResults,

    totalResults,
    setTotalResults,

    isLoading,
    setIsLoading,

    error,
    setError,
  }

  return <SearchContext.Provider value={value}>{children}</SearchContext.Provider>
}

export const useSearchContext = () => useContext(SearchContext)

export default SearchContextProvider
