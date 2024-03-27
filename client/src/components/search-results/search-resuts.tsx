import { useSearchContext } from '@/contexts/search-context'
import { SearchResultItem } from './search-result-item'
import { Button } from '../ui/button'
import { SearchSkeleton } from './search-skeleton'
import { useMemo } from 'react'

export const SearchResults = () => {
  const { error, isLoading, searchResults, totalResults, loadMoreHandler } = useSearchContext()

  const isLoadMoreShowed: boolean = useMemo(
    () => !!(searchResults.length && totalResults > searchResults.length && !isLoading),
    [searchResults, totalResults, isLoading]
  )

  return (
    <div>
      {error ? (
        <div className="text-red-500">Error: {error}</div>
      ) : (
        <>
          {searchResults.length === 0 && !isLoading && <div>No publications found</div>}
          <div className="mb-4 grid grid-cols-2 gap-4">
            {searchResults.map((publication) => (
              <SearchResultItem key={publication.id} publication={publication} />
            ))}
          </div>
          {isLoading && <SearchSkeleton />}
          {isLoadMoreShowed && (
            <Button aria-label="Load more" onClick={loadMoreHandler}>
              Load more
            </Button>
          )}
        </>
      )}
    </div>
  )
}
