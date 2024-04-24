import { useState, useEffect } from 'react'
import { Button } from '@/components/ui/button'

import logo from '../../assets/images/logo.png'
import logoLight from '../../assets/images/logo-white.png'
import MobileMenuDrawer from './mobile-menu'

function Header({ light = false }) {
  const [isMobile, setIsMobile] = useState(window.innerWidth < 480)

  useEffect(() => {
    const handleResize = () => {
      setIsMobile(window.innerWidth < 480)
    }

    window.addEventListener('resize', handleResize)
    handleResize()

    return () => window.removeEventListener('resize', handleResize)
  }, [])

  return (
    <header className="w-100% mb-10 flex flex-wrap items-center justify-around gap-96 bg-[#E4E541] px-4 py-9.5">
      <a href="/">
        <img width={160} height={40} src={light ? logoLight.src : logo.src} alt="KSE logo" />
      </a>

      <div className="flex items-center justify-center" id="header-buttons">
        {isMobile ? (
          <div className="flex items-center justify-end">
            <MobileMenuDrawer />
          </div>
        ) : (
          <div className="rounded-full border border-black p-1 text-[17px]">
            <a href="/about" aria-label="Go to About page">
              <Button variant="ghost" className="rounded-full">
                About
              </Button>
            </a>
            <a href="/submissions" aria-label="Go to Submissions page">
              <Button variant="ghost" className="rounded-full">
                Submissions
              </Button>
            </a>
            <a href="/team" aria-label="Go to Team page">
              <Button variant="ghost" className="rounded-full">
                Team
              </Button>
            </a>
          </div>
        )}
      </div>
    </header>
  )
}

export default Header
