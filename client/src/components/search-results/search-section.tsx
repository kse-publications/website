import SearchContextProvider from '@/contexts/search-context'
import { SearchInput } from './search-input'
import { SearchFilters } from './search-filters'
import { SearchResults } from './search-resuts'
import type { PaginatedCollection } from '@/types/common/paginated-collection'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'

interface SearchSectionProps {
  initialPublications: PaginatedCollection<PublicationSummary>
}

export default function SearchSection({ initialPublications }: SearchSectionProps) {
  return (
    <SearchContextProvider
      initialSearchResults={initialPublications.items}
      initialTotalResults={initialPublications.count}
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
