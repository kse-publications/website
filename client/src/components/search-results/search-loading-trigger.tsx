import { useSearchContext } from '@/contexts/search-context'
import { useEffect, useMemo, useRef } from 'react'

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
  }, [loadMoreHandler])

  const isTriggerHidden = useMemo(
    () => searchResults.length && searchResults.length >= totalResults && !isLoading,
    [searchResults, totalResults, isLoading]
  )

  if (isTriggerHidden) {
    return (
      <p className="pt-2 text-center text-sm text-muted-foreground">
        You have reached the end of the list
      </p>
    )
  }

  return <div ref={triggerRef} className="h-2" />
}
