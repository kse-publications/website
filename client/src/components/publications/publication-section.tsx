export const prerender = true
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { buttonVariants } from '@/components/ui/button'
import type { Publication } from '@/types/publication/publication'
import { ChevronLeftIcon, ChevronRightIcon } from '@radix-ui/react-icons'

interface PublicationPageProps {
  data: Publication
}

function PublicationPage({ data }: PublicationPageProps) {
  const goBack = () => {
    window.history.back()
  }

  return (
    <>
      <div className="-ml-4.5 mb-4 flex items-center text-base">
        <Button variant="link" onClick={goBack}>
          <ChevronLeftIcon className="h-5 w-5" /> Go back
        </Button>
      </div>

      <div className="max-w-4xl mx-auto mb-10 overflow-auto rounded-lg border border-gray-300 bg-white p-6">
        <Badge className="mb-3" variant="secondary">
          {data.type}
        </Badge>
        <h1 className="mb-5 text-4xl font-bold text-gray-800">{data.title}</h1>

        <div className="metadata flex flex-row gap-10">
          <div className="year">
            <h4 className="text-l scroll-m-20 font-semibold tracking-tight">Year:</h4> {data.year}
          </div>
          <div className="published-at">
            <h4 className="text-l scroll-m-20 font-semibold tracking-tight">Published in:</h4>
            {data.publisher.name}
          </div>
          <div className="author mb-4 ">
            <h4 className="text-l scroll-m-20 font-semibold tracking-tight">Authors:</h4>{' '}
            {data.authors.map((author: any, index: number) => (
              <span key={author.id}>
                {author.name}
                {index < data.authors.length - 1 ? ', ' : ''}{' '}
              </span>
            ))}
          </div>
        </div>

        <div className="mt-2 flex flex-wrap gap-2">
          {data.keywords.map((keyword: string) => (
            <Badge key={keyword}>{keyword}</Badge>
          ))}
        </div>

        <p className="mt-4 leading-7">{data.abstract}</p>
        {data.link && (
          <div className="mt-6 flex items-center justify-center text-base">
            <a className={buttonVariants({ variant: 'default' })} href={data.link} target="_blank">
              <span className="flex w-40 items-center justify-center pb-1 align-middle leading-none">
                Go to Source
              </span>
              <ChevronRightIcon className="h-5 w-5" />
            </a>
          </div>
        )}
      </div>
    </>
  )
}

export default PublicationPage
