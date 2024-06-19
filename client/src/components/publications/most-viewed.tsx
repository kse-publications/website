import Masonry from 'react-masonry-css'
import { getRecentStatistics } from '@/services/search/get-stats'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'

import { SearchResultItem } from '../search-results/search-result-item'
import { AnimatedHeadLine } from '../ui/animated-headline'

const breakpointColumnsObj = {
  default: 2,
  1100: 2,
  600: 1,
}

const fetchRecentStats = async () => {
  let recent = await getRecentStatistics()
  return recent
}

let recentStats = await fetchRecentStats()

export const MostViewedPublications = () => {
  return (
    <div className="mx-auto mb-10 max-w-[1160px] px-4">
      <div className="flex flex-row justify-between gap-10">
        <AnimatedHeadLine>Most viewed publications for the past month</AnimatedHeadLine>
        Total views: {recentStats?.recentViewsCount}
      </div>
      <p>
        {recentStats?.topRecentlyViewedPublications.length > 0 && (
          <>
            <Masonry breakpointCols={breakpointColumnsObj} className="masonry-grid">
              {recentStats.topRecentlyViewedPublications.map((publication: PublicationSummary) => (
                <SearchResultItem key={publication.slug} publication={publication} />
              ))}
            </Masonry>
          </>
        )}
      </p>
    </div>
  )
}
