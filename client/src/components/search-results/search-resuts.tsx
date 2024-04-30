import { useSearchContext } from '@/contexts/search-context'
import { SearchResultItem } from './search-result-item'
import { SearchSkeleton } from './search-skeleton'
import { LoadingTrigger } from './search-loading-trigger'

export const SearchResults = () => {
  const { isRecent, error, isLoading, searchResults } = useSearchContext()

  return (
    <div className="w-full bg-white pb-4 pt-8">
      <div className="mx-auto max-w-[1160px] px-4">
        <h2 className="mb-4 w-fit text-3xl font-semibold leading-none tracking-tight">
          {isRecent ? 'Recent publications' : 'Found in publications'}
          <span className="-mt-1 block h-1 w-full bg-[#e4e541]"></span>
        </h2>
        {error ? (
          <div className="text-red-500">Error: {error}</div>
        ) : (
          <>
            {searchResults.length === 0 && !isLoading && <div>No publications found</div>}
            <div id="publications" className="pb-2">
              {searchResults.map((publication) => (
                <SearchResultItem key={publication.slug} publication={publication} />
              ))}
            </div>
            {isLoading && <SearchSkeleton />}
            <LoadingTrigger />
          </>
        )}
      </div>
    </div>
  )
}
