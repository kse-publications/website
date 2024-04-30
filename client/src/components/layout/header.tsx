import { Button } from '@/components/ui/button'

import logoLight from '../../assets/images/logo-white.png'
import MobileMenuDrawer from './mobile-menu'

function Header({ light = false }) {
  return (
    <header
      className={`w-100% mb-10 flex flex-row flex-wrap items-center justify-around px-4 py-7 ${light ? 'bg-transparent' : 'bg-[#31452B]'}`}
    >
      <a href="/">
        <img width={160} height={40} src={logoLight.src} alt="KSE logo" />
      </a>

      <div className="flex items-center justify-center">
        <div className="flex items-center justify-end sm:hidden">
          <MobileMenuDrawer />
        </div>

        <div
          className={`hidden space-x-7 rounded-full border border-white text-[17px] text-white sm:block`}
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
