import bgImage from '../../assets/images/bg-image.jpg'
import bgImageWebp from '../../assets/images/bg-image.webp'
import bgImageAvif from '../../assets/images/bg-image.avif'
import { SearchInput } from './search-input'
import { SearchFilters } from './search-filters'

export const SearchBackground = () => {
  return (
    <div className="overflow-hidden">
      <picture>
        <source srcSet={bgImageWebp.src} type="image/webp" />
        <source srcSet={bgImageAvif.src} type="image/avif" />
        <img
          className="fixed left-0 top-0 -z-10 h-screen max-h-[1200px] w-full object-cover lg:max-h-[600px]"
          src={bgImage.src}
          alt="Background image"
        />
      </picture>
      <div className="fixed left-0 top-0 -z-[9] h-full w-full bg-black opacity-40"></div>

      <div className="mx-auto max-w-[1160px]  px-4 pb-32">
        <p className="mb-[77px] px-8 text-center text-2xl text-white md:text-4xl">
          KSE Publications is a collection of the major academic and analytical publications
          produced by the KSE community.
        </p>
        <SearchInput />
        <SearchFilters />
      </div>
    </div>
  )
}
