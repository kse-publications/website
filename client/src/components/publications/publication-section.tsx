export const prerender = true
import { navigate } from 'astro:transitions/client'
import { Badge } from '@/components/ui/badge'
import { buttonVariants } from '@/components/ui/button'
import { ChevronLeftIcon } from '@radix-ui/react-icons'

function PublicationPage({ data }: { data: any /* FIXME: Replace 'any' with smth useful */ }) {
  return (
    <>
      <div className="max-w-4xl mx-auto mb-10 overflow-auto rounded-lg border border-gray-300 bg-white p-6">
        <div className="-ml-4.5 flex items-center text-base">
          <a className={buttonVariants({ variant: 'link' })} href="/">
            <ChevronLeftIcon className="h-5 w-5" /> Go back
          </a>
        </div>

        <h1 className="mb-5 text-4xl font-bold text-gray-800">{data.title}</h1>

        <div className="metadata mb-4 flex flex-row gap-10">
          <div className="year">
            <h4 className="scroll-m-20 text-xl font-semibold tracking-tight">Year:</h4> {data.year}
          </div>
          <div className="published-at">
            <h4 className="scroll-m-20 text-xl font-semibold tracking-tight">Published at:</h4>
            {data.publisher.name}
          </div>
        </div>

        <div className="author mb-4 ">
          <h4 className="scroll-m-20 text-xl font-semibold tracking-tight">Authors:</h4>{' '}
          {data.authors.map((author: any, index: number) => (
            <span key={author.id}>
              {author.name}
              {index < data.authors.length - 1 ? ', ' : ''}{' '}
            </span>
          ))}
        </div>
        <div className="link mb-4 break-words">
          <h4 className="scroll-m-20 text-xl font-semibold tracking-tight">Link:</h4>
          <a href={data.link}>{data.link}</a>
        </div>

        <div className="mb-6 flex flex-wrap gap-2">
          {data.keywords.map((keyword: string) => (
            <Badge key={keyword}>{keyword}</Badge>
          ))}
        </div>

        <p className="leading-7 [&:not(:first-child)]:mt-6">{data.abstract}</p>
      </div>
    </>
  )
}

export default PublicationPage
