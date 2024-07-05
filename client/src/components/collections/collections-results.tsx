import Masonry from 'react-masonry-css'
import { useCollectionsContext } from './collections-context'

import { SearchResultItem } from '../search-results/search-result-item'
import { SearchSkeleton } from '../search-results/search-skeleton'
import { LoadingTrigger } from '../search-results/search-loading-trigger'
import GoBackButton from '../layout/go-back-button'
import { AnimatedHeadLine } from '../ui/animated-headline'
import type { IDetailedCollection } from '@/types/common/collection'
import { useMemo } from 'react'

const breakpointColumnsObj = {
  default: 2,
  1100: 2,
  600: 1,
}

const SKELETON_CARDS_COUNT = 4

interface CollectionsResultsProps {
  collection: IDetailedCollection
}

export default function CollectionsResults({ collection }: CollectionsResultsProps) {
  const { error, isLoading, relatedResults, totalResults, loadMoreHandler } =
    useCollectionsContext()

  const totalCount = useMemo(
    () => collection.publications.totalCount,
    [collection.publications.totalCount]
  )

  return (
    <section>
      <h1 className="pt-6 text-center text-2xl text-black md:text-4xl xl:px-8">
        {`${collection.collection.icon} ${collection.collection.name} collection`}
      </h1>
      <p className="text-center text-lg text-black opacity-70">
        {totalCount} {totalCount > 1 ? 'publications' : 'publication'} in collection
      </p>

      <GoBackButton />

      {error ? (
        <div className="text-red-500">Error: {error}</div>
      ) : (
        <>
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
    </section>
  )
}
