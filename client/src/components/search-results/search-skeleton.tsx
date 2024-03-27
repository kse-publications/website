import { Card } from '../ui/card'
import { Skeleton } from '../ui/skeleton'
const SKELETON_CARDS_COUNT = 4

export const SearchSkeleton = () => {
  return (
    <div className="mb-4 grid grid-cols-2 gap-4">
      {Array.from({ length: SKELETON_CARDS_COUNT }).map((_, index) => (
        <Card key={index} className="p-4">
          <Skeleton className="mb-2 h-[56px] w-full" />
          <Skeleton className="h-[16px] w-1/2" />
        </Card>
      ))}
    </div>
  )
}
