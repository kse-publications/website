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
import { getRelatedByAuthors } from '@/services/search/get-publications'
import { DEFAULT_PAGE, DEFAULT_PAGE_SIZE, SEARCH_TEXT_DEBOUNCE_MS } from '@/config/search-params'
import type { PaginatedCollection } from '@/types/common/paginated-collection'
import type { RelatedPublicationsQueryParams } from '@/types/common/query-params'
import { useDebounce } from 'use-debounce'

interface IRelatedAuthorsContext {
  debouncedAuthors: string

  relatedResults: PublicationSummary[]
  setRelatedResults: Dispatch<SetStateAction<PublicationSummary[]>>

  isLoading: boolean
  setIsLoading: Dispatch<SetStateAction<boolean>>

  error: string
  setError: Dispatch<SetStateAction<string>>

  loadMoreHandler: () => void
  totalResults: number
}

export const RelatedAuthorsContext = createContext<IRelatedAuthorsContext | null>(
  null
) as Context<IRelatedAuthorsContext>

interface RelatedAuthorsContextProviderProps {
  children: ReactNode
  initialResults: PublicationSummary[]
  initialTotalResults: number
  id: string
  authors: string
}

const RelatedAuthorsContextProvider = ({
  children,
  initialResults,
  initialTotalResults,
  id,
  authors,
}: RelatedAuthorsContextProviderProps) => {
  const isInitialPublications = useRef(true)
  const isLoadingMoreStarted = useRef(false)

  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [relatedResults, setRelatedResults] = useState<PublicationSummary[]>(initialResults || [])
  const [totalResults, setTotalResults] = useState(initialTotalResults || 0)

  const [debouncedAuthors] = useDebounce(authors, SEARCH_TEXT_DEBOUNCE_MS)

  useEffect(() => {
    if (isInitialPublications.current) return

    setRelatedResults([])
    fetchPublications({ id, authors: debouncedAuthors })
  }, [debouncedAuthors])

  const currentPage = useMemo(
    () => Math.ceil(relatedResults.length / DEFAULT_PAGE_SIZE),
    [relatedResults]
  )

  const fetchPublications = useCallback(
    async ({ id = '', authors = '', page = DEFAULT_PAGE }: RelatedPublicationsQueryParams) => {
      setIsLoading(true)
      setError('')

      try {
        let paginatedResponse: PaginatedCollection<PublicationSummary> = {
          items: [],
          totalCount: 0,
          resultCount: 0,
        }

        if (authors) {
          paginatedResponse = await getRelatedByAuthors(id, page, authors)
        }

        setRelatedResults((prevResults) => [...prevResults, ...paginatedResponse.items])
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
    if (isLoading || isLoadingMoreStarted.current || relatedResults.length >= totalResults) return

    isLoadingMoreStarted.current = true

    fetchPublications({
      id,
      authors,
      page: currentPage + 1,
    })
  }, [authors, currentPage, fetchPublications, id, isLoading, relatedResults.length, totalResults])

  const value: IRelatedAuthorsContext = useMemo(
    () => ({
      loadMoreHandler,
      debouncedAuthors,

      relatedResults,
      setRelatedResults,

      isLoading,
      setIsLoading,

      error,
      setError,

      totalResults,
    }),
    [loadMoreHandler, debouncedAuthors, relatedResults, isLoading, error, totalResults]
  )

  return <RelatedAuthorsContext.Provider value={value}>{children}</RelatedAuthorsContext.Provider>
}

export const useRelatedAuthorsContext = () => useContext(RelatedAuthorsContext)

export default RelatedAuthorsContextProvider
