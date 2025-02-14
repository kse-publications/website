import { AnimatedHeadLine } from '@/components/ui/animated-headline.tsx'
import NewsletterCard from '@/components/newsletters/newsletter-card.tsx'
import GoBackButton from '@/components/layout/go-back-button.tsx'
import NewsletterSubscribe from '@/components/newsletters/newsletter-form.tsx'

const NewslettersPage = ({ newsletters }: { newsletters: any[] }) => {
  return (
    <section className="mx-auto w-full">
      <GoBackButton />

      <NewsletterSubscribe className="mb-10 md:hidden" />
      <div className="pb-10">
        <AnimatedHeadLine>Previous issues of The KSE Research Digest</AnimatedHeadLine>

        <div className="flex gap-8">
          <div className="flex flex-col gap-4">
            {newsletters.map((newsletter: any) => (
              <NewsletterCard newsletter={newsletter} />
            ))}
          </div>
          <NewsletterSubscribe className="hidden md:block" />
        </div>
      </div>
    </section>
  )
}

export default NewslettersPage
