import { Card } from '../ui/card'
import { Skeleton } from '../ui/skeleton'
const SKELETON_CARDS_COUNT = 4

export const SearchSkeleton = () => {
  return (
    <div className="mb-4 grid grid-cols-2 gap-4">
      {Array.from({ length: SKELETON_CARDS_COUNT }).map((_, index) => (
        <Card key={index} className="p-4">
          <div className="mb-4 flex gap-2">
            <Skeleton className="h-6 w-14" />
            <Skeleton className="h-6 w-40" />
          </div>
          <Skeleton className="mb-2 h-14 w-full" />
          <Skeleton className="mb-1 h-4 w-1/2" />
          <Skeleton className="h-4 w-1/2" />
        </Card>
      ))}
    </div>
  )
}
