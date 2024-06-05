import { useState, useEffect } from 'react'
import { useSearchContext } from '@/contexts/search-context'

import { SearchResultItem } from './search-result-item'
import { SearchSkeleton } from './search-skeleton'
import { LoadingTrigger } from './search-loading-trigger'

import { AnimatedHeadLine } from '../ui/animated-headline'
import Masonry from 'react-masonry-css'
import { AnimatedCardWrapper } from '../ui/animated-card-wrapper'
import { Skeleton } from '../ui/skeleton'

const breakpointColumnsObj = {
  default: 2,
  1100: 2,
  600: 1,
}

const SKELETON_CARDS_COUNT = 4

export const SearchResults = () => {
  const { isRecent, error, isLoading, searchResults, totalResults } = useSearchContext()

  return (
    <div className="w-full grow bg-[#f0f0f0] pb-4 pt-8">
      <div className="mx-auto max-w-[1160px] px-4">
        <AnimatedHeadLine>
          {isRecent ? 'Recent publications' : `Found ${totalResults} publications`}
        </AnimatedHeadLine>
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
                  <AnimatedCardWrapper key={index}>
                    <div className="mb-4 flex h-6 gap-2">
                      <Skeleton className="h-6 w-14" />
                      <Skeleton className="h-6 w-40" />
                    </div>
                    <Skeleton className="mb-2 h-14 w-full" />
                    <Skeleton className="mb-1 h-4 w-1/2" />
                    <Skeleton className="h-4 w-1/2" />
                  </AnimatedCardWrapper>
                ))}
            </Masonry>

            <LoadingTrigger />
          </>
        )}
      </div>
    </div>
  )
}
