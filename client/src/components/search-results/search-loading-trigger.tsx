import { useEffect, useMemo, useRef } from 'react'

interface LoadingTriggerProps {
  isLoading: boolean
  resultsLength: number
  totalResults: number
  loadMoreHandler: () => void
}

export const LoadingTrigger = ({
  isLoading,
  resultsLength,
  totalResults,
  loadMoreHandler,
}: LoadingTriggerProps) => {
  const observer = useRef<IntersectionObserver | null>(null)
  const triggerRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    observer.current = new IntersectionObserver(
      (entries) => {
        const target = entries[0]
        if (target.isIntersecting && resultsLength < totalResults && !isLoading) {
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
  }, [loadMoreHandler, isLoading, resultsLength, totalResults])

  const isTriggerHidden = useMemo(
    () => resultsLength && resultsLength >= totalResults && !isLoading,
    [resultsLength, totalResults, isLoading]
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
