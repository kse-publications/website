import logo from '../../assets/images/logo.png'
import { Button } from '@/components/ui/button'

function Header() {
  return (
    <header className="mx-auto mb-10 flex max-w-[1160px] flex-wrap items-center justify-between gap-4 px-4 py-9.5">
      <a href="/">
        <img width={160} height={40} src={logo.src} alt="KSE logo" />
      </a>

      <div className="flex items-center justify-center rounded-full border border-gray-300 p-1 text-[17px]">
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
    </header>
  )
}

export default Header
