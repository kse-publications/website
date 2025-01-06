import React, { useState, useEffect } from 'react'
import { ArrowUpIcon } from 'lucide-react'

type ScrollToTopProps = {
  top?: number
  smooth?: boolean
  className?: string
}

const ScrollToTop: React.FC<ScrollToTopProps> = ({ top = 20, smooth = false, className = '' }) => {
  const [visible, setVisible] = useState(false)

  useEffect(() => {
    const onScroll = () => {
      setVisible(window.scrollY >= top)
    }

    onScroll()
    window.addEventListener('scroll', onScroll)

    return () => window.removeEventListener('scroll', onScroll)
  }, [top])

  const scrollToTop = () => {
    if (smooth) {
      window.scrollTo({ top: 0, behavior: 'smooth' })
    } else {
      window.scrollTo({ top: 0 })
    }
  }

  return visible ? (
    <button
      onClick={scrollToTop}
      aria-label="Scroll to top"
      className={`fixed bottom-10 right-10 z-50 flex items-center justify-center rounded-full bg-white p-2 shadow-lg transition-transform active:scale-95 ${className}`}
    >
      <ArrowUpIcon className="h-6 w-6 !stroke-[1px] text-gray-700" />
    </button>
  ) : null
}

export default ScrollToTop
