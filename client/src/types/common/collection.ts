import type { PublicationSummary } from '../publication-summary/publication-summary'
import type { PaginatedCollection } from './paginated-collection'

export interface ICollection {
  icon: string
  name: string
  description: string
  publicationsCount: number
  id: string
  slug: string
}

export interface IDetailedCollection {
  collection: ICollection
  publications: PaginatedCollection<PublicationSummary>
}
