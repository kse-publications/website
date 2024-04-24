import { Button } from '@/components/ui/button'
import { ChevronLeftIcon } from '@radix-ui/react-icons'

function GoBackButton() {
  const goBack = () => {
    window.history.back()
  }

  return (
    <div className="-ml-4.5 mb-4 flex items-center text-base">
      <Button variant="link" onClick={goBack}>
        <ChevronLeftIcon className="h-5 w-5" /> Go back
      </Button>
    </div>
  )
}

export default GoBackButton
