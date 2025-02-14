import { Button } from '@/components/ui/button'
import { GitHubLogoIcon } from '@radix-ui/react-icons'

import logoLight from '../../assets/images/logo-white.png'
import MobileMenuDrawer from './mobile-menu'
import { captureEvent } from '@/services/posthog/posthog'
import { SyncStatus } from '../ui/sync-status'

const menuItems = [
  { label: 'Newsletter', href: '/newsletters' },
  { label: 'Submissions', href: '/submissions' },
  { label: 'About', href: '/about' },
  { label: 'Team', href: '/team' },
]

interface HeaderProps {
  light?: boolean
  isSync: boolean
}

function Header({ light = false, isSync }: HeaderProps) {
  return (
    <header className={`mb-6 py-7 lg:mb-10 ${light ? 'bg-transparent' : 'bg-[#31452B]'}`}>
      <div className="mx-auto flex w-full max-w-[1160px] flex-row flex-wrap items-center justify-between px-4">
        <a href="/">
          <img width={160} height={40} src={logoLight.src} alt="KSE logo" />
        </a>

        <div className="flex items-center justify-center">
          <div className="flex items-center justify-end sm:hidden">
            <MobileMenuDrawer isSync={isSync} menuItems={menuItems} />
          </div>

          <nav role="navigation" className="relative hidden sm:block">
            <SyncStatus isSync={isSync} />
            <div
              className={`space-x-5 rounded-full border border-white p-0.5 text-[17px] text-white`}
            >
              {menuItems.map((item) => (
                <a
                  onClick={() => captureEvent('menu_item_click', { item: item.label })}
                  key={item.label}
                  href={item.href}
                  aria-label={`Go to ${item.label} page`}
                >
                  <Button variant="ghost" className="rounded-full align-middle">
                    {item.label}
                  </Button>
                </a>
              ))}
              <a href="https://github.com/kse-publications/website" target="_blank">
                <Button
                  variant="ghost"
                  className="h-9 rounded-full px-2 align-middle opacity-70 hover:opacity-100"
                >
                  <GitHubLogoIcon className="h-5 w-5" />
                </Button>
              </a>
            </div>
          </nav>
        </div>
      </div>
    </header>
  )
}

export default Header
