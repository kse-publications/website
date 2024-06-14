import MainPageButton from '../layout/to-main-button'
import { buttonVariants } from '../ui/button'

function NotFoundPage() {
  return (
    <section className="mx-auto max-w-[600px] pt-6">
      <h2 className="mx-auto mb-8 w-fit text-6xl font-normal leading-none tracking-tight md:text-9xl">
        404 error
        <span className="line-grow -mt-3 block h-2 w-full bg-[#e4e541] md:-mt-5"></span>
      </h2>
      <p className="mb-6 text-center text-lg">
        Oops! The page you're looking for seems to have gone missing. But don't worry, you can
        always explore our vast collection of insightful academic and analytical publications
        produced by the KSE community.
      </p>
      <p className="text-center text-lg">
        Click the button below to return to the main page and continue your intellectual journey
        with us!
      </p>
      <div className="mt-6 flex items-center justify-center text-base">
        <a className={buttonVariants({ variant: 'default' })} href={'/'}>
          <span className="flex w-40 items-center justify-center pb-1 align-middle leading-none">
            Go to main page
          </span>
        </a>
      </div>
    </section>
  )
}

export default NotFoundPage
