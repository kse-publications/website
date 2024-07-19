import { useEffect, useRef, useState } from 'react'
import { Card } from './card'

interface AnimatedCardWrapperProps {
  children: React.ReactNode
  className?: string
}

export const AnimatedCardWrapper = ({ children, className }: AnimatedCardWrapperProps) => {
  const ref = useRef<HTMLDivElement>(null)

  useEffect(() => {
    const observer = new IntersectionObserver(
      ([entry], observer) => {
        if (entry.isIntersecting && ref.current) {
          ref.current.classList.add('in-view')
          observer.disconnect()
        }
      },
      { threshold: 0.1 }
    )

    if (ref.current) {
      observer.observe(ref.current)
    }

    return () => {
      if (ref.current) {
        observer.unobserve(ref.current)
      }
    }
  }, [])

  return (
    <Card
      ref={ref}
      className={`search-result-item mb-2 h-full p-4 shadow-none transition-all ease-linear hover:bg-accent hover:shadow-lg`}
    >
      {children}
    </Card>
  )
}
