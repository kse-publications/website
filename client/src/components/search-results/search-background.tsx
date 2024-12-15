import { CSSTransition } from 'react-transition-group'
import bgImage from '../../assets/images/bg-image.jpg'
import bgImageWebp from '../../assets/images/bg-image.webp'
import bgImageAvif from '../../assets/images/bg-image.avif'
import { SearchInput } from './search-input'
import { SearchFilters } from './search-filters'
import { useSearchContext } from '@/contexts/search-context'
import type { ICollection } from '@/types/common/collection'
import { CollectionList } from '../collections/collections-list'
import { useRef } from 'react'

interface SearchBackgroundProps {
  collections: ICollection[]
}

export const SearchBackground = ({ collections }: SearchBackgroundProps) => {
  const { isRecent } = useSearchContext()
  const containerRef = useRef(null)
  const headingRef = useRef(null)

  return (
    <div>
      <picture>
        <source srcSet={bgImageWebp.src} type="image/webp" />
        <source srcSet={bgImageAvif.src} type="image/avif" />
        <img
          className="fixed left-0 top-0 -z-10 h-screen max-h-[1200px] w-full object-cover lg:max-h-[600px]"
          src={bgImage.src}
          alt="Background image"
        />
      </picture>
      <div className="fixed left-0 top-0 -z-[9] h-full h-screen max-h-[1200px] w-full bg-black object-cover opacity-40 lg:max-h-[600px]"></div>

      <CSSTransition
        in={!isRecent}
        timeout={400}
        classNames="change-padding"
        nodeRef={containerRef}
      >
        <div
          ref={containerRef}
          className={`relative mx-auto max-w-[1160px] px-4 pt-6 ${isRecent ? 'pb-10 lg:pb-14' : 'pb-6'}`}
        >
          <CSSTransition
            in={isRecent}
            timeout={400}
            classNames="slide"
            unmountOnExit
            nodeRef={headingRef}
          >
            <h1
              ref={headingRef}
              className={`text-center text-2xl text-white md:text-4xl xl:px-8 ${isRecent ? 'mb-[77px]' : ''}`}
            >
              KSE Publications is a collection of the major academic and analytical publications
              produced by the KSE community.
            </h1>
          </CSSTransition>
          <SearchInput />
          <SearchFilters />
          <CollectionList collections={collections} />
        </div>
      </CSSTransition>
    </div>
  )
}
