import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import { Card, CardDescription, CardTitle } from '../ui/card'

export const SearchResultItem = ({ publication }: { publication: PublicationSummary }) => {
  return (
    <a
      className="h-full w-full"
      rel="noopener noreferrer nofollow"
      target="_blank"
      aria-label={`Go to ${publication.title} publication`}
      href={`/publications/${publication.id}`}
    >
      <Card className="h-full p-4">
        <CardTitle className="mb-2">{publication.title}</CardTitle>
        <CardDescription>{publication.keywords.join(', ')}</CardDescription>
      </Card>
    </a>
  )
}
