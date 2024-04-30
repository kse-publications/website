import { Button } from '@/components/ui/button'
import { Sheet, SheetContent, SheetTrigger } from '@/components/ui/sheet'
import { Separator } from '@/components/ui/separator'
import { ListBulletIcon } from '@radix-ui/react-icons'

export default function MobileMenuDrawer() {
  return (
    <Sheet key="top">
      <SheetTrigger asChild>
        <ListBulletIcon className="h-9 w-9 rounded-full border border-white p-2" color="white" />
      </SheetTrigger>
      <SheetContent side="top" className="fixed inset-0 z-50 overflow-auto bg-white">
        <div className="max-w-sm mx-auto mt-10 w-full">
          <Separator />
          <a href="/about" aria-label="Go to About page">
            <Button variant="link" className="mb-1 mt-1 pl-0">
              About
            </Button>
          </a>
          <Separator />
          <a href="/about" aria-label="Go to About page">
            <Button variant="link" className="mb-1 mt-1 pl-0">
              Submissions
            </Button>
          </a>
          <Separator />
          <a href="/about" aria-label="Go to About page">
            <Button variant="link" className="mb-1 mt-1 pl-0">
              Team
            </Button>
          </a>
          <Separator />
        </div>
      </SheetContent>
    </Sheet>
  )
}
