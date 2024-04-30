import { useEffect, useRef, useState } from 'react'
import { Card } from './card'

interface AnimatedCardWrapperProps {
  children: React.ReactNode
  className?: string
}

export const AnimatedCardWrapper = ({ children, className }: AnimatedCardWrapperProps) => {
  const [isVisible, setIsVisible] = useState(false)
  const ref = useRef(null)

  useEffect(() => {
    const observer = new IntersectionObserver(
      ([entry], observer) => {
        if (entry.isIntersecting) {
          setIsVisible(true)
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
      className={`${isVisible ? 'in-view' : ''} search-result-item mb-2 h-full p-4 shadow-none transition-all ease-linear hover:bg-accent hover:shadow-lg`}
    >
      {children}
    </Card>
  )
}
