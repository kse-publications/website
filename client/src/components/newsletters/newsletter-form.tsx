import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input.tsx'
import { MailCheckIcon } from 'lucide-react'
import { cn } from '@/lib/utils.ts'

const getBaseUrl = () => {
  const isServer = typeof window === 'undefined'
  const SSR_URL = import.meta.env.PUBLIC_SSR_API_URL

  if (isServer && SSR_URL) {
    return SSR_URL
  }

  return import.meta.env.PUBLIC_API_URL
}

async function submitEmail(email: string) {
  const BASE_URL = getBaseUrl()
  const res = await fetch(`${BASE_URL}/subscriptions`, {
    method: 'POST',
    body: JSON.stringify({ email }),
    headers: {
      'Content-Type': 'application/json',
    },
  })

  if (res.status === 400) {
    return false
  }

  return true
}

const NewsletterSubscribe = ({ className }: { className?: string }) => {
  const [isSubscribed, setIsSubscribed] = useState(false)
  const [email, setEmail] = useState('')
  const [error, setError] = useState('')

  const validateEmail = (email: string) => {
    return email
      .toLowerCase()
      .match(
        /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
      )
  }

  const onSubscribe = () => {
    const isValidEmail = validateEmail(email)
    if (!isValidEmail) {
      setError('Please enter a valid email address.')
    }

    if (isValidEmail) {
      submitEmail(email)
        .then((success) => {
          setError('')
          if (success) {
            setIsSubscribed(true)
          }
        })
        .catch((e) => {
          console.error(e)
          setError('An error occurred while subscribing. Please try again later.')
        })
    }
  }

  const onEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setEmail(e.target.value)
    setError('')
  }

  return (
    <div className={cn('h-fit rounded-lg border border-gray-300 bg-white px-4 py-6', className)}>
      <div className="flex flex-col items-start justify-center gap-10 sm:flex-row sm:justify-between sm:gap-4 md:flex-col md:justify-center md:gap-10">
        <div>
          <h3 className="mb-4 w-fit text-2xl font-semibold leading-none tracking-tight">
            Subscribe to the Newsletter
          </h3>
          <p className="max-w-[18rem]">
            Join our mailing list to stay informed on the academic life of the Kyiv School of
            Economics.
          </p>
        </div>
        {isSubscribed ? (
          <div className="flex h-10 w-full min-w-[19rem] items-center justify-center rounded-md bg-[#c6d364] bg-opacity-90">
            <p className="text-lg font-semibold text-[#31452b]">Thank you for subscribing!</p>
          </div>
        ) : (
          <div className="flex min-w-[19rem] max-w-[19rem] flex-col items-start justify-center gap-1.5">
            {error && <p className="max-w-full text-sm text-red-500">{error}</p>}
            <Input
              className="h-12 w-full text-base"
              name="email"
              placeholder="Email Address"
              value={email}
              onChange={onEmailChange}
            />
            <Button onClick={onSubscribe} className="w-full px-12">
              Subscribe
              <MailCheckIcon className="ml-4 size-5" />
            </Button>
          </div>
        )}
      </div>
    </div>
  )
}

export default NewsletterSubscribe
