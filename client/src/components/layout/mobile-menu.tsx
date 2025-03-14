import { Button } from '@/components/ui/button'
import { Sheet, SheetContent, SheetTrigger } from '@/components/ui/sheet'
import { Separator } from '@/components/ui/separator'
import { HamburgerMenuIcon } from '@radix-ui/react-icons'
import { captureEvent } from '@/services/posthog/posthog'
import { SyncStatus } from '../ui/sync-status'

interface IMenuItem {
  label: string
  href: string
}

interface MobileMenuDrawerProps {
  menuItems: IMenuItem[]
  isSync: boolean
}

export default function MobileMenuDrawer({ menuItems, isSync }: MobileMenuDrawerProps) {
  return (
    <Sheet key="top">
      <SheetTrigger asChild>
        <div className="relative">
          <HamburgerMenuIcon className="relative -top-1 h-7 w-7" color="white" />
          <SyncStatus isSync={isSync} />
        </div>
      </SheetTrigger>
      <SheetContent side="top" className="fixed inset-0 z-50 overflow-auto bg-white">
        <div className="mx-auto mt-10 w-full max-w-sm">
          {menuItems.map((item) => (
            <div key={item.label}>
              <a href={item.href} aria-label={`Go to ${item.label} page`}>
                <Button
                  onClick={() => captureEvent('menu_item_click', { item: item.label })}
                  variant="link"
                  className="mb-1 mt-1 pl-0 text-lg"
                >
                  {item.label}
                </Button>
              </a>
              <Separator />
            </div>
          ))}
        </div>
      </SheetContent>
    </Sheet>
  )
}
