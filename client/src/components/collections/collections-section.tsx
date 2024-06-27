export const prerender = true

import GoBackButton from '../layout/go-back-button'
import type { IDetailedCollection } from '@/types/common/collection'
import CollectionsContextProvider from './collections-context'
import CollectionsResults from './collections-results'

interface CollectionPageProps {
  collection: IDetailedCollection
}

function CollectionPage({ collection }: CollectionPageProps) {
  return (
    <CollectionsContextProvider
      initialResults={collection.publications.items || []}
      initialTotalResults={collection.publications.totalCount || 0}
      id={collection.collection.id}
    >
      <CollectionsResults collection={collection} />
    </CollectionsContextProvider>
  )
}

export default CollectionPage
