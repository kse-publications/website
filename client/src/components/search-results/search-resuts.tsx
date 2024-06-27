import Masonry from 'react-masonry-css'
import { useSearchContext } from '@/contexts/search-context'
import { SearchResultItem } from './search-result-item'
import { LoadingTrigger } from './search-loading-trigger'
import { AnimatedHeadLine } from '../ui/animated-headline'

import { ScrollIndicator } from '../layout/scroll-indicator'
import { MostViewedPublications } from '../publications/most-viewed'
import { SearchSkeleton } from './search-skeleton'

const breakpointColumnsObj = {
  default: 2,
  1100: 2,
  600: 1,
}

const SKELETON_CARDS_COUNT = 5

export const SearchResults = () => {
  const { isRecent, error, isLoading, searchResults, totalResults, loadMoreHandler } =
    useSearchContext()

  return (
    <div className="w-full grow bg-[#f0f0f0] pb-4 pt-8">
      <ScrollIndicator totalCards={totalResults} />
      {isRecent && <MostViewedPublications />}
      <div className="mx-auto max-w-[1160px] px-4">
        <div className="flex gap-5">
          <AnimatedHeadLine>
            {isRecent ? 'All publications' : `Found ${totalResults} publications`}
          </AnimatedHeadLine>
          {isRecent && <p className="p-1 opacity-70">{totalResults} total publications</p>}
        </div>
        {error ? (
          <div className="text-red-500">Error: {error}</div>
        ) : (
          <>
            {searchResults.length === 0 && !isLoading && <div>No publications found</div>}

            <Masonry breakpointCols={breakpointColumnsObj} className="masonry-grid">
              {searchResults.map((publication) => (
                <SearchResultItem key={publication.slug} publication={publication} />
              ))}
              {isLoading &&
                Array.from({ length: SKELETON_CARDS_COUNT }).map((_, index) => (
                  <SearchSkeleton key={index} />
                ))}
            </Masonry>

            <LoadingTrigger
              isLoading={isLoading}
              resultsLength={searchResults.length}
              totalResults={totalResults}
              loadMoreHandler={loadMoreHandler}
            />
          </>
        )}
      </div>
    </div>
  )
}
