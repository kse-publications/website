import { useSearchContext } from '@/contexts/search-context'
import { SearchResultItem } from './search-result-item'
import { SearchSkeleton } from './search-skeleton'
import { LoadingTrigger } from './search-loading-trigger'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import { AnimatedHeadLine } from '../ui/animated-headline'

const getParsedResults = (searchResults: PublicationSummary[]) => {
  const leftColumn: PublicationSummary[] = []
  const rightColumn: PublicationSummary[] = []
  const idsSet = new Set<string>()

  searchResults.forEach((item, index) => {
    if (idsSet.has(item.slug)) return
    if (index % 2 === 0) {
      leftColumn.push(item)
    } else {
      rightColumn.push(item)
    }
    idsSet.add(item.slug)
  })

  return [...leftColumn, ...rightColumn]
}

export const SearchResults = () => {
  const { isRecent, error, isLoading, searchResults } = useSearchContext()

  return (
    <div className="w-full grow bg-[#f0f0f0] pb-4 pt-8">
      <div className="mx-auto max-w-[1160px] px-4">
        <AnimatedHeadLine>
          {isRecent ? 'Recent publications' : 'Found in publications'}
        </AnimatedHeadLine>
        {error ? (
          <div className="text-red-500">Error: {error}</div>
        ) : (
          <>
            {searchResults.length === 0 && !isLoading && <div>No publications found</div>}
            <div id="publications" className="summary-container">
              {getParsedResults(searchResults).map((publication) => (
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
