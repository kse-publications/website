import { useEffect, useRef, useCallback, useMemo } from 'react'
import { useRelatedAuthorsContext } from './authors-context'

export const AuthorsLoadingTrigger = () => {
  const { isLoading, relatedResults, totalResults, loadMoreHandler } = useRelatedAuthorsContext()
  const observer = useRef<IntersectionObserver | null>(null)
  const triggerRef = useRef<HTMLDivElement>(null)

  const handleIntersect = useCallback(
    (entries: IntersectionObserverEntry[]) => {
      const target = entries[0]
      if (target.isIntersecting && !isLoading && relatedResults.length < totalResults) {
        loadMoreHandler()
      }
    },
    [isLoading, relatedResults.length, totalResults, loadMoreHandler]
  )

  useEffect(() => {
    if (observer.current) observer.current.disconnect()

    observer.current = new IntersectionObserver(handleIntersect, {
      threshold: 0.5,
    })

    if (triggerRef.current) {
      observer.current.observe(triggerRef.current)
    }

    return () => {
      if (observer.current) {
        observer.current.disconnect()
      }
    }
  }, [handleIntersect])

  const isTriggerHidden = useMemo(
    () => relatedResults.length && relatedResults.length >= totalResults && !isLoading,
    [relatedResults.length, totalResults, isLoading]
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
