import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import { Card, CardDescription, CardTitle } from '../ui/card'

export const SearchResultItem = ({ publication }: { publication: PublicationSummary }) => {
  return (
    <a
      aria-label={`Go to ${publication.title} publication`}
      href={`/publications/${publication.slug}`}
    >
      <Card className="h-full p-4 transition-all ease-linear hover:bg-accent hover:shadow-lg">
        <CardTitle className="mb-2 text-left">{publication.title}</CardTitle>
        <CardDescription className="text-left">{publication.keywords.join(', ')}</CardDescription>
      </Card>
    </a>
  )
}
