import { Button } from '@/components/ui/button'
import { ChevronLeftIcon } from '@radix-ui/react-icons'

import MainPageButton from './to-main-button'

function GoBackButton() {
  const goBack = () => {
    window.history.back()
  }

  return (
    <>
      {window.history.length > 2 ? (
        <div className="-ml-4.5 mb-4 flex items-center text-base">
          <Button variant="link" onClick={goBack}>
            <ChevronLeftIcon className="h-5 w-5" />
            Go Back
          </Button>
        </div>
      ) : (
        <MainPageButton />
      )}
    </>
  )
}

export default GoBackButton
