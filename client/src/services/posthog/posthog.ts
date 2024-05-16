const ENVIRONMENT = import.meta.env.PUBLIC_ENVIRONMENT

const captureEvent = (event: string, properties?: Record<string, any>) => {
  if (ENVIRONMENT !== 'production') return

  // @ts-ignore
  window.posthog.capture(event, properties)
}

export { captureEvent }
