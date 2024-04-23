import SearchContextProvider from '@/contexts/search-context'
import { SearchInput } from './search-input'
import { SearchFilters } from './search-filters'
import { SearchResults } from './search-resuts'
import type { PaginatedCollection } from '@/types/common/paginated-collection'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import type { IFilter } from '@/types/common/fiters'

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
      <section className="pb-4">
        <div className="mb-8">
          <SearchInput />
          <SearchFilters />
        </div>
        <SearchResults />
      </section>
    </SearchContextProvider>
  )
}
