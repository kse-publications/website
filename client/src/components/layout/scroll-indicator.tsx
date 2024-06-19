import { useState, useEffect } from 'react'
import { Progress } from '@/components/ui/progress'

interface ScrollIndicatorProps {
  totalCards: number
}

export const ScrollIndicator = ({ totalCards }: ScrollIndicatorProps) => {
  const [progressWidth, setProgressWidth] = useState(0)
  const avgCardHeight = 105

  useEffect(() => {
    const handleScroll = () => {
      const winScroll = document.body.scrollTop || document.documentElement.scrollTop
      const totalHeight = totalCards * avgCardHeight
      const height = totalHeight - document.documentElement.clientHeight
      const scrolled = (winScroll / height) * 100
      setProgressWidth(Math.min(scrolled, 100))
    }

    window.addEventListener('scroll', handleScroll)

    return () => {
      window.removeEventListener('scroll', handleScroll)
    }
  }, [totalCards, avgCardHeight])

  return <Progress value={progressWidth} className="fixed top-0 z-40 h-2 w-full rounded-none" />
}
