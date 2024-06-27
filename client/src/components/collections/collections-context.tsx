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
import { DEFAULT_PAGE, DEFAULT_PAGE_SIZE } from '@/config/search-params'
import { getDetailedCollection } from '@/services/search/get-collections'

interface ICollectionsContext {
  relatedResults: PublicationSummary[]
  setRelatedResults: Dispatch<SetStateAction<PublicationSummary[]>>

  isLoading: boolean
  setIsLoading: Dispatch<SetStateAction<boolean>>

  error: string
  setError: Dispatch<SetStateAction<string>>

  loadMoreHandler: () => void
  totalResults: number
}

export const CollectionsContext = createContext<ICollectionsContext | null>(
  null
) as Context<ICollectionsContext>

interface CollectionsContextProviderProps {
  children: ReactNode
  initialResults: PublicationSummary[]
  initialTotalResults: number
  id: string
}

const CollectionsContextProvider = ({
  children,
  initialResults,
  initialTotalResults,
  id,
}: CollectionsContextProviderProps) => {
  const isLoadingMoreStarted = useRef(false)

  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [relatedResults, setRelatedResults] = useState<PublicationSummary[]>(initialResults || [])
  const [totalResults, setTotalResults] = useState(initialTotalResults || 0)

  useEffect(() => {
    setRelatedResults([])
    fetchCollections({ id })
  }, [])

  const currentPage = useMemo(
    () => Math.ceil(relatedResults.length / DEFAULT_PAGE_SIZE),
    [relatedResults]
  )

  const fetchCollections = useCallback(
    async ({ id = '', page = DEFAULT_PAGE }: { id: string; page?: number }) => {
      setIsLoading(true)
      setError('')

      try {
        const detailedCollection = await getDetailedCollection(id, page)

        setRelatedResults((prevResults) => [
          ...prevResults,
          ...detailedCollection.publications.items,
        ])
        setTotalResults(detailedCollection.publications.totalCount)
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

    fetchCollections({
      id,
      page: currentPage + 1,
    })
  }, [currentPage, fetchCollections, id, isLoading, relatedResults.length, totalResults])

  const value: ICollectionsContext = useMemo(
    () => ({
      loadMoreHandler,

      relatedResults,
      setRelatedResults,

      isLoading,
      setIsLoading,

      error,
      setError,

      totalResults,
    }),
    [loadMoreHandler, relatedResults, isLoading, error, totalResults]
  )

  return <CollectionsContext.Provider value={value}>{children}</CollectionsContext.Provider>
}

export const useCollectionsContext = () => useContext(CollectionsContext)

export default CollectionsContextProvider
