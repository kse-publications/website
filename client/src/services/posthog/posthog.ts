const ENVIRONMENT = import.meta.env.PUBLIC_ENVIRONMENT

const captureEvent = (event: string, properties?: Record<string, any>) => {
  if (ENVIRONMENT !== 'production') return
  console.log(event, properties)
  window.posthog.capture(event, properties)
}

export { captureEvent }
