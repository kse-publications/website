export const prerender = true
import { ChevronRightIcon } from '@radix-ui/react-icons'

import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { buttonVariants } from '@/components/ui/button'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'

import type { Publication } from '@/types/publication/publication'
import type { PaginatedCollection } from '@/types/common/paginated-collection'
import type { PublicationSummary } from '@/types/publication-summary/publication-summary'

import GoBackButton from '../layout/go-back-button'
import RelatedAuthorsContextProvider from '../related-by-authors-results/authors-context'
import RelatedAuthorsResults from '../related-by-authors-results/authors-results'
import SimilarPublicationsResults from './similar-publications'
import { AnimatedHeadLine } from '../ui/animated-headline'

interface PublicationPageProps {
  data: Publication
  relatedPublications: PaginatedCollection<PublicationSummary>
  similarPublications: PublicationSummary[]
}

function PublicationPage({ data, relatedPublications, similarPublications }: PublicationPageProps) {
  return (
    <>
      <RelatedAuthorsContextProvider
        initialResults={relatedPublications?.items || []}
        initialTotalResults={relatedPublications?.totalCount || 0}
        id={data.id + ''}
        authors={data.authors.map((author: any) => author.id).join('-')}
      >
        <GoBackButton />
        <div className="mx-auto mb-10 w-full max-w-[1128px] overflow-auto rounded-lg border border-gray-300 bg-white p-6">
          <Badge className="mb-3" variant="secondary">
            {data.type}
          </Badge>
          <h1 className="mb-5 text-4xl font-bold text-gray-800">{data.title}</h1>

          <div className="flex flex-col gap-5 sm:flex-row sm:gap-10">
            <div className="year">
              <h4 className="text-l scroll-m-20 font-semibold tracking-tight">Year:</h4> {data.year}
            </div>
            <div className="published-at">
              <h4 className="text-l scroll-m-20 font-semibold tracking-tight">Published in:</h4>
              {data.publisher.name}
            </div>
            <div className="author mb-4">
              <h4 className="text-l scroll-m-20 font-semibold tracking-tight">Authors:</h4>{' '}
              {data.authors.map((author: any, index: number) => (
                <span key={author.slug}>
                  <a href={`/?searchText=${author.name.toLowerCase().replace(/\s/g, '+')}`}>
                    <Button variant="link" className="h-0 p-0 text-base font-normal">
                      {author.name}
                    </Button>
                  </a>
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

          <p className="mt-4 leading-7">{data.abstracttext}</p>
          {data.link && (
            <div className="mt-6 flex items-center justify-center text-base">
              <a
                className={buttonVariants({ variant: 'default' })}
                href={data.link}
                target="_blank"
              >
                <span className="flex w-40 items-center justify-center pb-1 align-middle leading-none">
                  Go to Source
                </span>
                <ChevronRightIcon className="h-5 w-5" />
              </a>
            </div>
          )}
        </div>
        {similarPublications?.length > 0 && relatedPublications?.totalCount > 0 && (
          <>
            <Tabs defaultValue="author">
              <div className="flex items-start gap-5">
                <AnimatedHeadLine>Other publications by</AnimatedHeadLine>
                <p className="text-black opacity-70">
                  <TabsContent value="topic">
                    {similarPublications.length} publications found
                  </TabsContent>
                  <TabsContent value="author">
                    {relatedPublications.totalCount} publications found
                  </TabsContent>
                </p>
              </div>
              <TabsList className="my-2">
                <TabsTrigger className="!py-2 px-4 text-xl" value="author">
                  Author
                </TabsTrigger>
                <TabsTrigger className="!py-2 px-4 text-xl" value="topic">
                  Topic
                </TabsTrigger>
              </TabsList>
              <TabsContent value="author">
                <RelatedAuthorsResults hideHeadline />
              </TabsContent>
              <TabsContent value="topic">
                <SimilarPublicationsResults hideHeadline similarResults={similarPublications} />
              </TabsContent>
            </Tabs>
          </>
        )}
        {similarPublications?.length == 0 && relatedPublications?.totalCount > 0 && (
          <RelatedAuthorsResults />
        )}
      </RelatedAuthorsContextProvider>
    </>
  )
}

export default PublicationPage
