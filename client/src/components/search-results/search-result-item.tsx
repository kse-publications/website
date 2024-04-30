import type { PublicationSummary } from '@/types/publication-summary/publication-summary'
import { Card, CardDescription, CardTitle } from '../ui/card'
import { Badge } from '../ui/badge'
import { useEffect, useRef, useState } from 'react'
import { CSSTransition } from 'react-transition-group'

export const SearchResultItem = ({ publication }: { publication: PublicationSummary }) => {
  const [isVisible, setIsVisible] = useState(false)
  const ref = useRef(null)

  useEffect(() => {
    const observer = new IntersectionObserver(
      ([entry], observer) => {
        if (entry.isIntersecting) {
          setIsVisible(true)
          observer.disconnect()
        }
      },
      { threshold: 0.1 }
    )

    if (ref.current) {
      observer.observe(ref.current)
    }

    return () => {
      if (ref.current) {
        observer.unobserve(ref.current)
      }
    }
  }, [])

  return (
    <CSSTransition classNames="search-result-item" timeout={400}>
      <a
        ref={ref}
        className={`search-result-item ${isVisible ? 'in-view' : ''}`}
        aria-label={`Go to ${publication.title} publication`}
        href={`/publications/${publication.slug}`}
      >
        <Card className="mb-2 h-full p-4 shadow-none transition-all ease-linear hover:bg-accent hover:shadow-lg">
          <div className="mb-4 flex gap-2">
            <Badge className="text-left">{publication.year}</Badge>
            <Badge className="text-left">{publication.type}</Badge>
          </div>
          <CardTitle className="mb-4 text-left">{publication.title}</CardTitle>
          <CardDescription className="mb-1.5 text-left">
            Publisher: {publication.publisher}
          </CardDescription>
          <CardDescription className="text-left">
            Authors: {publication.authors.join(', ')}
          </CardDescription>
        </Card>
      </a>
    </CSSTransition>
  )
}
