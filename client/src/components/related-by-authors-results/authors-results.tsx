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

export default function RelatedAuthorsResults() {
  const { error, isLoading, relatedResults, totalResults, loadMoreHandler } =
    useRelatedAuthorsContext()

  return (
    <div className="more-publications mb-10">
      <div className="flex gap-5">
        <h3 className="mb-5 text-2xl font-bold">Other publications by authors</h3>
        <p className="p-1 opacity-70">{totalResults} publications found</p>
      </div>
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
          </Masonry>

          {isLoading && <SearchSkeleton />}
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
