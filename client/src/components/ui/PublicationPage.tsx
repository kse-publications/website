import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'

function PublicationPage({ data }: { data: any }) {
  return (
    <>
      <div className="mx-auto mt-10 max-w-4xl rounded-lg bg-white p-6 shadow-lg">
        <a href="/">
          <Button variant="link">Go back</Button>
        </a>
        <h1 className="mb-5 text-4xl font-bold text-gray-800">{data.title}</h1>

        <div className="metadata mb-4 flex flex-row justify-between">
          <div className="author">
            <h4 className="scroll-m-20 text-xl font-semibold tracking-tight">Authors:</h4>{' '}
            {data.authors.map((author: any) => (
              <span key={author.id}>{author.name}, </span>
            ))}
          </div>
          <div className="year">
            <h4 className="scroll-m-20 text-xl font-semibold tracking-tight">Year:</h4> {data.year}
          </div>
          <div className="published-at">
            <h4 className="scroll-m-20 text-xl font-semibold tracking-tight">Published at:</h4>
            {data.publisher.name},
          </div>
        </div>

        <div className="mb-6 flex flex-wrap gap-2">
          {data.keywords.map((keyword: string) => (
            <Badge>{keyword}</Badge>
          ))}
        </div>

        <p className="leading-7 [&:not(:first-child)]:mt-6">{data.abstract}</p>
      </div>
    </>
  )
}

export default PublicationPage
