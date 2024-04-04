import { useSearchContext } from '@/contexts/search-context'
import { SearchResultItem } from './search-result-item'
import { SearchSkeleton } from './search-skeleton'
import { useMemo } from 'react'
import { LoadingTrigger } from './search-loading-trigger'

export const SearchResults = () => {
  const { searchText, error, isLoading, searchResults } = useSearchContext()

  const isRecent: boolean = useMemo(
    () => !!(searchResults.length && searchText === ''),
    [searchResults, searchText]
  )

  return (
    <div>
      {isRecent && (
        <h2 className="mb-4 text-3xl font-semibold leading-none tracking-tight">Recent</h2>
      )}
      {error ? (
        <div className="text-red-500">Error: {error}</div>
      ) : (
        <>
          {searchResults.length === 0 && !isLoading && <div>No publications found</div>}
          <div className="mb-4 grid gap-4 md:grid-cols-2">
            {searchResults.map((publication) => (
              <SearchResultItem key={publication.slug} publication={publication} />
            ))}
          </div>
          {isLoading && <SearchSkeleton />}
          <LoadingTrigger />
        </>
      )}
    </div>
  )
}
