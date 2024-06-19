import SearchContextProvider from '@/contexts/search-context'
import { SearchResults } from './search-resuts'
import type { PaginatedCollection } from '@/types/common/paginated-collection'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import type { IFilter } from '@/types/common/fiters'
import { SearchBackground } from './search-background'
import ScrollToTop from 'react-scroll-to-top'
import { ScrollIndicator } from '../layout/scroll-indicator'

interface SearchSectionProps {
  initialPublications: PaginatedCollection<PublicationSummary>
  isRecent?: boolean
  filters: IFilter[]
}

export default function SearchSection({
  initialPublications,
  isRecent = false,
  filters,
}: SearchSectionProps) {
  return (
    <SearchContextProvider
      initialSearchResults={initialPublications?.items || []}
      initialTotalResults={initialPublications?.totalCount || 0}
      initialFilters={filters}
      initialIsRecent={isRecent}
    >
      <section className="flex grow flex-col">
        <SearchBackground />
        <ScrollIndicator totalCards={initialPublications?.totalCount || 0} />
        <SearchResults />
        <ScrollToTop
          smooth
          height="15px"
          className="flex items-center justify-center"
          style={{ borderRadius: '50%' }}
        />
      </section>
    </SearchContextProvider>
  )
}
