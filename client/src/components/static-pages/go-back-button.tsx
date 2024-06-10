import React, { useEffect, useState } from 'react'
import { Button } from '@/components/ui/button'
import { ChevronLeftIcon } from '@radix-ui/react-icons'

import MainPageButton from './to-main-button'

function GoBackButton() {
  const [buttonLabel, setButtonLabel] = useState('Go Back')

  useEffect(() => {
    if (window.history.length <= 1) {
      setButtonLabel('Go to Main Page')
    } else {
      setButtonLabel('Go Back')
    }
  }, [])

  const handleGoBack = () => {
    if (window.history.length > 1) {
      window.history.back()
    } else {
      window.location.href = '/'
    }
  }

  return (
    <div className="-ml-4.5 mb-4 flex items-center text-base">
      <Button variant="link" onClick={handleGoBack}>
        <ChevronLeftIcon className="h-5 w-5" />
        {buttonLabel}
      </Button>
    </div>
  )
}

export default GoBackButton
