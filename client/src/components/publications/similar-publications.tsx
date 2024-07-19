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
  hideHeadline?: boolean
}

export default function SimilarPublicationsResults({
  similarResults,
  hideHeadline,
}: SimilarPublicationsProps) {
  return (
    <div className="more-publications mb-10">
      {!hideHeadline && (
        <div className="mb-5 flex content-center gap-5">
          <h3 className="w-fit text-center text-2xl font-bold">Recommended</h3>
          <p className="mt-0.5 text-black opacity-70">{similarResults.length} publications found</p>
        </div>
      )}
      {
        <Masonry breakpointCols={breakpointColumnsObj} className="masonry-grid">
          {similarResults.map((publication) => (
            <SearchResultItem key={publication.slug} publication={publication} />
          ))}
        </Masonry>
      }
      <p className="pt-2 text-center text-sm text-muted-foreground">
        You have reached the end of the list
      </p>
    </div>
  )
}
