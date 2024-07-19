import { captureEvent } from '@/services/posthog/posthog'
import type { ICollection } from '@/types/common/collection'
import { ArrowRightIcon } from '@radix-ui/react-icons'
import { Button } from '../ui/button'

interface CollectionListProps {
  collections: ICollection[]
}

const DISABLE_COLLECTIONS = import.meta.env.PUBLIC_DISABLE_COLLECTIONS

export const CollectionList = ({ collections }: CollectionListProps) => {
  if (DISABLE_COLLECTIONS === 'true') {
    return null
  }

  return (
    <nav aria-label="collections" className="mx-auto mt-[77px]">
      <ul className="flex flex-wrap justify-center gap-4">
        {collections.map((collection) => (
          <li key={collection.id}>
            <a
              aria-label={`Go to ${collection.name} collection`}
              href={`/collections/${collection.slug}`}
              onClick={() => captureEvent('collection_click', { collection: collection.slug })}
            >
              <Button
                variant="ghost"
                className="flex items-center gap-3 rounded-full border border-white text-white hover:text-black"
              >
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
