import type { Publication } from '../publication/publication'

export type PublicationSummary = Pick<Publication, 'slug' | 'title' | 'type' | 'year'> & {
  authors: string[]
  publisher: string
}
