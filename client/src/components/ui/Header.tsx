import logo from '../../assets/images/logo.png'

function Header() {
  return (
    <header className="mx-auto mb-22.5 flex max-w-[1160px] items-center justify-between px-4 py-9.5">
      <a href="/">
        <img width={160} height={40} src={logo.src} alt="KSE logo" />
      </a>

      <div className="mb- flex items-center justify-center gap-3 rounded-full border border-gray-500 px-5 py-2.5 text-[17px]">
        <a href="/about" className="hover:text-gray-400">
          About
        </a>
        <a href="/submissions" className="hover:text-gray-400">
          Submissions
        </a>
        <a href="/team" className="hover:text-gray-400">
          Team
        </a>
      </div>
    </header>
  )
}

export default Header
