import { Button } from '@/components/ui/button'

import logoLight from '../../assets/images/logo-white.png'
import MobileMenuDrawer from './mobile-menu'

function Header({ light = false }) {
  return (
    <header className={`mb-6 py-7 lg:mb-10 ${light ? 'bg-transparent' : 'bg-[#31452B]'}`}>
      <div className="mx-auto flex w-full max-w-[1160px] flex-row flex-wrap items-center justify-between px-4">
        <a href="/">
          <img width={160} height={40} src={logoLight.src} alt="KSE logo" />
        </a>

        <div className="flex items-center justify-center">
          <div className="flex items-center justify-end sm:hidden">
            <MobileMenuDrawer />
          </div>

          <div
            className={`hidden space-x-5 rounded-full border border-white p-0.5 text-[17px] text-white sm:block`}
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
      </div>
    </header>
  )
}

export default Header
