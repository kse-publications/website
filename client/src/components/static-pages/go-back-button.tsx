import { Button } from '@/components/ui/button'
import { ChevronLeftIcon } from '@radix-ui/react-icons'

function MainPageButton() {
  return (
    <div className="-ml-4.5 mb-4 flex items-center text-base">
      <a href="/">
        <Button variant="link">
          <ChevronLeftIcon className="h-5 w-5" />
          To the main page
        </Button>
      </a>
    </div>
  )
}

export default MainPageButton
