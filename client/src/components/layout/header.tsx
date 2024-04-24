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
    <header className="mx-auto mb-10 flex max-w-[1160px] flex-wrap items-center justify-between gap-4 px-4 py-9.5">
      <a href="/">
        <img width={160} height={40} src={light ? logoLight.src : logo.src} alt="KSE logo" />
      </a>

      <div className="flex items-center justify-center" id="header-buttons">
        {isMobile ? (
          <div className="flex items-center justify-end">
            <MobileMenuDrawer />
          </div>
        ) : (
          <div
            className={`rounded-full border border-gray-300 p-1 text-[17px] ${light ? 'text-white' : 'text-black'}`}
          >
            <Button variant="ghost" className="rounded-full">
              About
            </Button>
            <Button variant="ghost" className="rounded-full">
              Submissions
            </Button>
            <Button variant="ghost" className="rounded-full">
              Team
            </Button>
          </div>
        )}
      </div>
    </header>
  )
}

export default Header
