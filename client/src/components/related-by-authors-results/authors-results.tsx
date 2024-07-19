import Masonry from 'react-masonry-css'
import { useRelatedAuthorsContext } from './authors-context'

import { SearchResultItem } from '../search-results/search-result-item'
import { SearchSkeleton } from '../search-results/search-skeleton'
import { LoadingTrigger } from '../search-results/search-loading-trigger'

const breakpointColumnsObj = {
  default: 2,
  1100: 2,
  600: 1,
}

interface RelatedAuthorsResultsProps {
  hideHeadline?: boolean
}

const SKELETON_CARDS_COUNT = 5

export default function RelatedAuthorsResults({ hideHeadline }: RelatedAuthorsResultsProps) {
  const { error, isLoading, relatedResults, totalResults, loadMoreHandler } =
    useRelatedAuthorsContext()

  return (
    <div className="more-publications mb-10">
      {!hideHeadline && (
        <div className="mb-5 flex items-center gap-5">
          <h3 className="w-fit text-center text-2xl font-bold">Related by autor</h3>
          <p className="mt-0.5 text-black opacity-70">{totalResults} publications found</p>
        </div>
      )}
      {error ? (
        <div className="text-red-500">Error: {error}</div>
      ) : (
        <>
          {relatedResults.length === 0 && !isLoading && (
            <div>No additional publications found by these authors.</div>
          )}
          <Masonry breakpointCols={breakpointColumnsObj} className="masonry-grid">
            {relatedResults.map((publication) => (
              <SearchResultItem key={publication.slug} publication={publication} />
            ))}
            {isLoading &&
              Array.from({ length: SKELETON_CARDS_COUNT }).map((_, index) => (
                <SearchSkeleton key={index} />
              ))}
          </Masonry>

          <LoadingTrigger
            isLoading={isLoading}
            resultsLength={relatedResults.length}
            totalResults={totalResults}
            loadMoreHandler={loadMoreHandler}
          />
        </>
      )}
    </div>
  )
}
