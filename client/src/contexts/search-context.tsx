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
import { DEFAULT_PAGE, DEFAULT_PAGE_SIZE, SEARCH_TEXT_DEBOUNCE_MS } from '@/config/search-params'
import type { PaginatedCollection } from '@/types/common/paginated-collection'
import type { SearchPublicationsQueryParams } from '@/types/common/query-params'
import type { IFilter } from '@/types/common/fiters'
import { useDebounce } from 'use-debounce'
import { captureEvent } from '@/services/posthog/posthog'
import type { SelectedFilters } from '@/types/common/selected-filters'
import { getFiltersFromString, getFiltersString } from '@/utils/parse-filters'
import { getFilters } from '@/services/filters/filters'

interface ISearchContext {
  debouncedSearchText: string
  searchText: string
  setSearchText: Dispatch<SetStateAction<string>>

  selectedFilters: SelectedFilters
  setSelectedFilters: Dispatch<SetStateAction<SelectedFilters>>

  filters: IFilter[]

  searchResults: PublicationSummary[]
  setSearchResults: Dispatch<SetStateAction<PublicationSummary[]>>

  totalResults: number
  setTotalResults: Dispatch<SetStateAction<number>>

  isLoading: boolean
  setIsLoading: Dispatch<SetStateAction<boolean>>

  error: string
  setError: Dispatch<SetStateAction<string>>

  loadMoreHandler: () => void

  isRecent?: boolean
}

export const SearchContext = createContext<ISearchContext | null>(null) as Context<ISearchContext>

interface SearchContextProviderProps {
  children: ReactNode
  initialSearchResults: PublicationSummary[]
  initialTotalResults: number
  initialIsRecent: boolean
  initialFilters: IFilter[]
}

const SearchContextProvider = ({
  children,
  initialSearchResults,
  initialTotalResults,
  initialIsRecent,
  initialFilters,
}: SearchContextProviderProps) => {
  const isInitialPublications = useRef(true)
  const isLoadingMoreStarted = useRef(false)

  const [isRecent, setIsRecent] = useState<boolean>(initialIsRecent)

  const [searchText, setSearchText] = useState('')
  const [selectedFilters, setSelectedFilters] = useState<SelectedFilters>([])
  const [filters, setFilters] = useState<IFilter[]>(initialFilters)

  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [totalResults, setTotalResults] = useState(initialTotalResults || 0)
  const [searchResults, setSearchResults] = useState<PublicationSummary[]>(
    initialSearchResults || []
  )

  const [debouncedSearchText] = useDebounce(searchText, SEARCH_TEXT_DEBOUNCE_MS)

  useEffect(() => {
    const searchParams = new URLSearchParams(window.location.search)
    setSearchText(searchParams.get('searchText') || '')
    setSelectedFilters(getFiltersFromString(searchParams.get('filters')))

    const rehydrateTimeout = setTimeout(() => {
      isInitialPublications.current = false
    }, 500)

    return () => clearTimeout(rehydrateTimeout)
  }, [])

  useEffect(() => {
    if (isInitialPublications.current) return

    setSearchResults([])
    fetchPublications({ searchText, filters: selectedFilters })
    fetchFilters({
      filters: selectedFilters,
      searchText,
    })

    updateQuery()

    const isRecent = !searchText && !selectedFilters.length
    setIsRecent(isRecent)
    !isRecent && captureEvent('search', { searchText, filters: selectedFilters })
  }, [debouncedSearchText, selectedFilters])

  const updateQuery = useCallback(() => {
    const urlSearchParams = new URLSearchParams()

    if (searchText) urlSearchParams.append('searchText', searchText)
    else urlSearchParams.delete('searchText')

    if (selectedFilters.length) urlSearchParams.append('filters', getFiltersString(selectedFilters))
    else urlSearchParams.delete('filters')

    if (!searchText && !selectedFilters.length) {
      window.history.replaceState({}, '', window.location.origin)
      return
    }

    const queryString = urlSearchParams.toString()
    const newUrl = window.location.pathname + '?' + queryString

    window.history.replaceState({}, '', newUrl)
  }, [searchText, selectedFilters])

  const currentPage = useMemo(
    () => Math.ceil(searchResults.length / DEFAULT_PAGE_SIZE),
    [searchResults]
  )

  const fetchFilters = useCallback(
    async ({ filters, searchText }: { filters: SelectedFilters; searchText: string }) => {
      try {
        const newFilters = await getFilters({ filters, searchText })
        setFilters(newFilters)
      } catch (error) {
        console.error(error)
        error instanceof Error && setError(error.message)
      }
    },
    []
  )

  const fetchPublications = useCallback(
    async ({
      searchText = '',
      page = DEFAULT_PAGE,
      filters = [],
    }: SearchPublicationsQueryParams) => {
      setIsLoading(true)
      setError('')

      if (page === DEFAULT_PAGE) {
        setSearchResults([])
      }

      try {
        let paginatedResponse: PaginatedCollection<PublicationSummary>

        if (searchText === '') {
          paginatedResponse = await getInitialPublications({ page, filters })
        } else {
          paginatedResponse = await searchPublications({
            searchText,
            page,
            filters,
          })
        }

        setSearchResults((prevResults) => [...prevResults, ...paginatedResponse.items])
        setTotalResults(paginatedResponse.totalCount)
      } catch (error) {
        console.error(error)
        error instanceof Error && setError(error.message)
      } finally {
        setIsLoading(false)
        isLoadingMoreStarted.current = false
      }
    },
    []
  )

  const loadMoreHandler = useCallback(() => {
    if (isLoadingMoreStarted.current) return

    isLoadingMoreStarted.current = true

    fetchPublications({
      searchText,
      page: currentPage + 1,
      filters: selectedFilters,
    })
  }, [searchText, currentPage, selectedFilters])

  const value: ISearchContext = {
    loadMoreHandler,

    searchText,
    setSearchText,
    debouncedSearchText,

    filters,

    selectedFilters,
    setSelectedFilters,

    searchResults,
    setSearchResults,

    totalResults,
    setTotalResults,

    isLoading,
    setIsLoading,

    error,
    setError,

    isRecent,
  }

  return <SearchContext.Provider value={value}>{children}</SearchContext.Provider>
}

export const useSearchContext = () => useContext(SearchContext)

export default SearchContextProvider
