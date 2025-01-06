import SearchContextProvider from '@/contexts/search-context'
import { SearchResults } from './search-resuts'
import type { PaginatedCollection } from '@/types/common/paginated-collection'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import type { IFilter } from '@/types/common/fiters'
import { SearchBackground } from './search-background'
import ScrollToTop from '@/components/search-results/scroll-to-top-button.tsx'
import type { ICollection } from '@/types/common/collection'

interface SearchSectionProps {
  initialPublications: PaginatedCollection<PublicationSummary>
  isRecent?: boolean
  filters: IFilter[]
  collections: ICollection[]
}

export default function SearchSection({
  initialPublications,
  isRecent = false,
  filters,
  collections,
}: SearchSectionProps) {
  return (
    <SearchContextProvider
      initialSearchResults={initialPublications?.items || []}
      initialTotalResults={initialPublications?.totalCount || 0}
      initialFilters={filters}
      initialIsRecent={isRecent}
    >
      <section className="flex grow flex-col">
        <SearchBackground collections={collections} />
        <SearchResults />
        <ScrollToTop smooth />
      </section>
    </SearchContextProvider>
  )
}
