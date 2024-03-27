import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import { Card, CardDescription, CardTitle } from '../ui/card'
import { navigate } from 'astro:transitions/client'

export const SearchResultItem = ({ publication }: { publication: PublicationSummary }) => {
  const handleNavigate = (e: React.MouseEvent<HTMLButtonElement>) => {
    navigate(`/publications/${publication.id}`)
  }

  return (
    <button
      className="h-full w-full"
      onClick={handleNavigate}
      aria-label={`Go to ${publication.title} publication`}
    >
      <Card className="h-full p-4">
        <CardTitle className="mb-2 text-left">{publication.title}</CardTitle>
        <CardDescription className="text-left">{publication.keywords.join(', ')}</CardDescription>
      </Card>
    </button>
  )
}
