import { captureEvent } from '@/services/posthog/posthog.ts'
import { Badge } from '@/components/ui/badge.tsx'
import { Card, CardDescription, CardTitle } from '@/components/ui/card.tsx'

const NewsletterCard = ({ newsletter }: { newsletter: any }) => {
  return (
    <a
      aria-label={`Go to ${newsletter.title} newsletter`}
      href={newsletter.link}
      target="_blank"
      onClick={() => captureEvent('newsletter_click', { newsletter: newsletter.date })}
      className="max-w-[48rem]"
    >
      <Card
        className={`h-full p-6 shadow-none transition-all ease-linear hover:bg-accent hover:shadow-lg`}
      >
        <div className="mb-4 flex gap-2">
          <Badge className="text-left">{newsletter.date}</Badge>
          {newsletter.badges.map((badge: string) => (
            <Badge
              className={`text-left ${badge === 'Interview' && 'border-yellow shadow-sm shadow-yellow'}`}
            >
              {badge}
            </Badge>
          ))}
        </div>
        <CardTitle className="mb-6 break-keep text-left">
          {newsletter.title.replace(/(\S+)-(\S+)/g, '$1‑$2')}
        </CardTitle>
        <CardDescription className="text-left">
          Researchers: {newsletter.people.join(', ')}
        </CardDescription>
      </Card>
    </a>
  )
}

export default NewsletterCard
