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
  useRef,
} from 'react'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import { getInitialPublications, searchPublications } from '@/services/search/get-publications'
import {
  DEFAULT_FILTER_TYPE,
  DEFAULT_PAGE,
  DEFAULT_PAGE_SIZE,
  DEFAULT_SORT_ORDER,
  SEARCH_TEXT_DEBOUNCE_MS,
} from '@/config/search-params'
import { useDebounce } from 'use-debounce'
import { SortOrders } from '@/types/common/sort-orders'
import type { FilterTypes } from '@/types/common/filter-types'
import type { PaginatedCollection } from '@/types/common/paginated-collection'

interface ISearchContext {
  searchText: string
  debouncedSearchText: string
  setSearchText: Dispatch<SetStateAction<string>>

  filterType: FilterTypes | null
  setFilterType: Dispatch<SetStateAction<FilterTypes | null>>

  sortOrder: SortOrders | null
  setSortOrder: Dispatch<SetStateAction<SortOrders | null>>

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
  const isInitialPublications = useRef(true)

  const [searchText, setSearchText] = useState('')
  const [filterType, setFilterType] = useState<FilterTypes | null>(null)
  const [sortOrder, setSortOrder] = useState<SortOrders | null>(null)

  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [totalResults, setTotalResults] = useState(initialTotalResults || 0)
  const [searchResults, setSearchResults] = useState<PublicationSummary[]>(
    initialSearchResults || []
  )

  const [debouncedSearchText] = useDebounce(searchText, SEARCH_TEXT_DEBOUNCE_MS)

  useEffect(() => {
    if (isInitialPublications.current) {
      isInitialPublications.current = false
      return
    }
    fetchPublications({ searchText: debouncedSearchText })
  }, [debouncedSearchText])

  useEffect(() => {
    if (!filterType || !sortOrder) return
    fetchPublications({ filterType, sortOrder, searchText })
  }, [sortOrder, filterType])

  const currentPage = useMemo(
    () => Math.ceil(searchResults.length / DEFAULT_PAGE_SIZE),
    [searchResults]
  )

  const fetchPublications = useCallback(
    async ({
      searchText = '',
      page = DEFAULT_PAGE,
      filterType = DEFAULT_FILTER_TYPE,
      sortOrder = DEFAULT_SORT_ORDER,
    }) => {
      setIsLoading(true)
      setError('')

      if (page === DEFAULT_PAGE) {
        setSearchResults([])
      }

      try {
        let paginatedResponse: PaginatedCollection<PublicationSummary>

        if (searchText === '') {
          paginatedResponse = await getInitialPublications(currentPage + 1)
        } else {
          paginatedResponse = await searchPublications({
            searchText,
            page,
            filterType,
            sortOrder,
          })
        }

        setSearchResults((prevResults) => [...prevResults, ...paginatedResponse.items])
        setTotalResults(paginatedResponse.totalCount)
      } catch (error) {
        console.error(error)
        error instanceof Error && setError(error.message)
      } finally {
        setIsLoading(false)
      }
    },
    [currentPage]
  )

  const loadMoreHandler = useCallback(() => {
    fetchPublications({
      searchText,
      page: currentPage + 1,
      filterType: filterType ?? undefined,
      sortOrder: sortOrder ?? undefined,
    })
  }, [searchText, currentPage, filterType, sortOrder])

  const value: ISearchContext = {
    loadMoreHandler,

    searchText,
    debouncedSearchText,
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
