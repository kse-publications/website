import { useState, useEffect } from 'react'
import { Progress } from '@/components/ui/progress'

interface ScrollIndicatorProps {
  totalCards: number
}

const avgCardHeight = 105
const visibilityThreshold = 100

export const ScrollIndicator = ({ totalCards }: ScrollIndicatorProps) => {
  const [progressWidth, setProgressWidth] = useState(0)
  const [isVisible, setIsVisible] = useState(false)

  useEffect(() => {
    const handleScroll = () => {
      const windowWidth = window.innerWidth

      const winScroll = document.body.scrollTop || document.documentElement.scrollTop
      const totalHeight = totalCards * avgCardHeight
      const height = totalHeight - document.documentElement.clientHeight
      let scrolled = (winScroll / height) * 100

      if (windowWidth < 640) {
        scrolled /= 2.2
      }

      setProgressWidth(Math.min(scrolled, 100))
      setIsVisible(winScroll > visibilityThreshold)
    }

    window.addEventListener('scroll', handleScroll)

    return () => {
      window.removeEventListener('scroll', handleScroll)
    }
  }, [totalCards, avgCardHeight])

  return (
    <Progress
      value={progressWidth}
      className="fixed top-0 z-40 h-2 w-full rounded-none transition ease-linear"
      style={{ opacity: isVisible ? 1 : 0 }}
    />
  )
}
