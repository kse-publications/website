import { useState, useEffect } from 'react'
import { Button } from '@/components/ui/button'

import logo from '../../assets/images/logo.png'
import logoLight from '../../assets/images/logo-white.png'
import MobileMenuDrawer from './mobile-menu'

function Header({ light = false }) {
  return (
    <header
      className={`w-100% mb-10 flex flex-wrap items-center justify-around gap-96 px-4 py-9.5 ${light ? 'bg-transparent' : 'bg-[#E4E541]'}`}
    >
      <a href="/">
        <img width={160} height={40} src={light ? logoLight.src : logo.src} alt="KSE logo" />
      </a>

      <div className="flex items-center justify-center" id="header-buttons">
        <div className="flex items-center justify-end xs:hidden">
          <MobileMenuDrawer />
        </div>
        <div
          className={`hidden rounded-full border p-1  text-[17px] xs:block ${light ? 'border-white text-white' : 'border-black text-black'}`}
        >
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
      </div>
    </header>
  )
}

export default Header
