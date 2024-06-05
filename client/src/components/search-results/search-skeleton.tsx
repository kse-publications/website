import { AnimatedCardWrapper } from '../ui/animated-card-wrapper'
import { Skeleton } from '../ui/skeleton'

export const SearchSkeleton = () => {
  return (
    <>
      <AnimatedCardWrapper>
        <div className="mb-4 flex h-4 gap-2">
          <Skeleton className="h-6 w-14" />
          <Skeleton className="h-6 w-40" />
        </div>
        <Skeleton className="mb-2 h-14 w-full" />
        <Skeleton className="mb-1 h-4 w-1/2" />
        <Skeleton className="h-4 w-1/2" />
      </AnimatedCardWrapper>
    </>
  )
}
