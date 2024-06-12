import { captureEvent } from '@/services/posthog/posthog'
import type { ICollection } from '@/types/common/collection'
import { ArrowRightIcon } from '@radix-ui/react-icons'
import { Button } from '../ui/button'

interface CollectionListProps {
  collections: ICollection[]
}

export const CollectionList = ({ collections }: CollectionListProps) => {
  console.log(collections)

  return (
    <nav aria-label="collections" className="mx-auto mt-6">
      <ul className="flex flex-wrap justify-center gap-4">
        {collections.map((collection) => (
          <li key={collection.id}>
            <a
              aria-label={`Go to ${collection.name} collection`}
              href={`/collections/${collection.slug}`}
              onClick={() => captureEvent('collection_click', { collection: collection.slug })}
            >
              <Button className="button flex items-center gap-3 rounded-lg bg-yellow px-4 py-2 text-black transition-colors hover:text-white">
                <span>{collection.icon}</span>
                <h3>{collection.name}</h3>
                <ArrowRightIcon className="h-5 w-5" />
              </Button>
            </a>
          </li>
        ))}
      </ul>
    </nav>
  )
}
