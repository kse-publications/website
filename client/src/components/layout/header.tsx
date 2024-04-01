import logo from '../../assets/images/logo.png'
import { Button } from '@/components/ui/button'

function Header() {
  return (
    <header className="mx-auto mb-10 flex max-w-[1160px] items-center justify-between px-4 py-9.5">
      <a href="/">
        <img width={160} height={40} src={logo.src} alt="KSE logo" />
      </a>

      <div className="mb- flex items-center justify-center rounded-full border border-gray-300 p-2 text-[17px]">
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