import { Button } from '@/components/ui/button'
import { Sheet, SheetContent, SheetTrigger } from '@/components/ui/sheet'
import { Separator } from '@/components/ui/separator'
import { ListBulletIcon } from '@radix-ui/react-icons'

export default function MobileMenuDrawer() {
  return (
    <Sheet key="top">
      <SheetTrigger asChild>
        <ListBulletIcon className="h-9 w-9 rounded-full border border-black p-2" />
      </SheetTrigger>
      <SheetContent side="top" className="fixed inset-0 z-50 overflow-auto bg-white">
        <div className="max-w-sm mx-auto mt-10 w-full">
          <Separator />
          <Button variant="link" className="mb-1 mt-1 pl-0">
            About
          </Button>
          <Separator />
          <Button variant="link" className="mb-1 mt-1 pl-0">
            Submissions
          </Button>
          <Separator />
          <Button variant="link" className="mb-1 mt-1 pl-0">
            Team
          </Button>
          <Separator />
        </div>
      </SheetContent>
    </Sheet>
  )
}
