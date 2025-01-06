import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import { CardDescription, CardTitle } from '../ui/card'
import { Badge } from '../ui/badge'
import { AnimatedCardWrapper } from '../ui/animated-card-wrapper'
import { captureEvent } from '@/services/posthog/posthog'

export const SearchResultItem = ({ publication }: { publication: PublicationSummary }) => {
  return (
    <a
      aria-label={`Go to ${publication.title} publication`}
      href={`/publications/${publication.slug}`}
      onClick={() => captureEvent('publication_click', { publication: publication.slug })}
    >
      <AnimatedCardWrapper>
        <div className="mb-4 flex gap-2">
          <Badge className="text-left">{publication.year}</Badge>
          <Badge className="text-left">{publication.type}</Badge>
        </div>
        <CardTitle className="mb-4 break-keep text-left">
          {publication.title.replace(/(\S+)-(\S+)/g, '$1‑$2')}
        </CardTitle>
        <CardDescription className="mb-1.5 text-left">
          Publisher: {publication.publisher}
        </CardDescription>
        <CardDescription className="text-left">
          Authors: {publication.authors.join(', ')}
        </CardDescription>
      </AnimatedCardWrapper>
    </a>
  )
}
