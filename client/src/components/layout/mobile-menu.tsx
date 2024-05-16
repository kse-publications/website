import { Button } from '@/components/ui/button'
import { Sheet, SheetContent, SheetTrigger } from '@/components/ui/sheet'
import { Separator } from '@/components/ui/separator'
import { ListBulletIcon } from '@radix-ui/react-icons'
import { captureEvent } from '@/services/posthog/posthog'

interface IMenuItem {
  label: string
  href: string
}

interface MobileMenuDrawerProps {
  menuItems: IMenuItem[]
}

export default function MobileMenuDrawer({ menuItems }: MobileMenuDrawerProps) {
  return (
    <Sheet key="top">
      <SheetTrigger asChild>
        <ListBulletIcon className="h-9 w-9 rounded-full border border-white p-2" color="white" />
      </SheetTrigger>
      <SheetContent side="top" className="fixed inset-0 z-50 overflow-auto bg-white">
        <div className="max-w-sm mx-auto mt-10 w-full">
          {menuItems.map((item) => (
            <div key={item.label}>
              <a href={item.href} aria-label={`Go to ${item.label} page`}>
                <Button
                  onClick={() => captureEvent('menu_item_click', { item: item.label })}
                  variant="link"
                  className="mb-1 mt-1 pl-0"
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
