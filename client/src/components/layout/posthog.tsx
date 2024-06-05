import { captureEvent } from '@/services/posthog/posthog'
import posthog from 'posthog-js'
import { useEffect } from 'react'

interface PostHogProps {
  pageVisitEvent: string
}

const POSTHOG_API_KEY = import.meta.env.PUBLIC_POSTHOG_API_KEY
const POSTHOG_HOST = import.meta.env.PUBLIC_POSTHOG_HOST

const PostHog = ({ pageVisitEvent }: PostHogProps) => {
  useEffect(() => {
    posthog.init(POSTHOG_API_KEY, {
      api_host: POSTHOG_HOST,
      ui_host: 'https://eu.posthog.com',
      
      autocapture: false,
      disable_surveys: true,
  
      capture_pageview: false,
      capture_pageleave: true,

      enable_recording_console_log: false,
      enable_heatmaps: true
    })

    window.posthog = posthog

    captureEvent('page_visit', { page: pageVisitEvent })
  }, [])
  return <></>
}

export default PostHog
