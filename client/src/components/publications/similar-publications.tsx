import Masonry from 'react-masonry-css'
import { SearchResultItem } from '../search-results/search-result-item'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'

const breakpointColumnsObj = {
  default: 2,
  1100: 2,
  600: 1,
}

interface SimilarPublicationsProps {
  similarResults: PublicationSummary[]
}

export default function SimilarPublicationsResults({ similarResults }: SimilarPublicationsProps) {
  return (
    <div className="more-publications mb-10">
      <div className="mb-5 flex content-center gap-5">
        <h3 className="w-fit text-center text-2xl font-bold">Similar publications</h3>
        <p className="flex items-center pl-1 pt-1 opacity-70">
          {similarResults.length} publications found
        </p>
      </div>
      {
        <Masonry breakpointCols={breakpointColumnsObj} className="masonry-grid">
          {similarResults.map((publication) => (
            <SearchResultItem key={publication.slug} publication={publication} />
          ))}
        </Masonry>
      }
    </div>
  )
}
