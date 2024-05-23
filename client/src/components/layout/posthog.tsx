import { captureEvent } from '@/services/posthog/posthog'
import posthog from 'posthog-js'
import { useEffect } from 'react'

interface PostHogProps {
  pageVisitEvent: string
}

const PostHog = ({ pageVisitEvent }: PostHogProps) => {
  useEffect(() => {
    posthog.init('phc_2fJTznUxuaQsrIG2RYq2Kak7qdLDGMeLK84imLOnaTM', {
      api_host: 'https://eu.i.posthog.com',
    })

    window.posthog = posthog

    captureEvent('page_visit', { page: pageVisitEvent })
  }, [])
  return <></>
}

export default PostHog
