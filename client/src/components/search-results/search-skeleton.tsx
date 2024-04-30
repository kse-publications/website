import { AnimatedCardWrapper } from '../ui/animated-card-wrapper'
import { Skeleton } from '../ui/skeleton'
const SKELETON_CARDS_COUNT = 4

export const SearchSkeleton = () => {
  return (
    <div className="summary-container">
      {Array.from({ length: SKELETON_CARDS_COUNT }).map((_, index) => (
        <AnimatedCardWrapper key={index}>
          <div className="mb-4 flex gap-2">
            <Skeleton className="h-6 w-14" />
            <Skeleton className="h-6 w-40" />
          </div>
          <Skeleton className="mb-2 h-14 w-full" />
          <Skeleton className="mb-1 h-4 w-1/2" />
          <Skeleton className="h-4 w-1/2" />
        </AnimatedCardWrapper>
      ))}
    </div>
  )
}
