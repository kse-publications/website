import { useSearchContext } from '@/contexts/search-context'
import { useEffect, useRef } from 'react'

export const LoadingTrigger = () => {
  const { isLoading, searchResults, totalResults, loadMoreHandler } = useSearchContext()
  const observer = useRef<IntersectionObserver | null>(null)
  const triggerRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    observer.current = new IntersectionObserver(
      (entries) => {
        const target = entries[0]
        if (target.isIntersecting && searchResults.length < totalResults && !isLoading) {
          loadMoreHandler()
        }
      },
      {
        threshold: 0.5,
      }
    )

    if (triggerRef.current) {
      observer.current.observe(triggerRef.current)
    }

    return () => {
      if (observer.current && triggerRef.current) {
        observer.current.unobserve(triggerRef.current)
      }
    }
  }, [isLoading, loadMoreHandler, searchResults.length, totalResults])

  return <div ref={triggerRef} style={{ height: '10px' }} />
}
