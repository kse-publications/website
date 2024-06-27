/// <reference path="../.astro/types.d.ts" />
/// <reference types="astro/client" />

interface PostHog {
  capture: (eventName: string, properties?: Record<string, any>) => void
}

interface Window {
  posthog: PostHog
}
