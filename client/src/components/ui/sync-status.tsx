import { useEffect, useState } from 'react'
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from './tooltip'
import { getSyncStatus } from '@/services/sync/get-sync-status'

interface SyncStatusProps {
  isSync: boolean
}

const SYNC_POLL_INTERVAL = 10000

export const SyncStatus = ({ isSync: isSyncProps }: SyncStatusProps) => {
  const [isSync, setIsSync] = useState(isSyncProps)

  useEffect(() => {
    if (!isSync) return

    const pollSyncStatus = async () => {
      try {
        const isSync = await getSyncStatus()
        setIsSync(isSync)
      } catch (error) {
        console.error('Error polling sync status:', error)
      }
    }

    const timeout = setTimeout(pollSyncStatus, SYNC_POLL_INTERVAL)

    return () => clearTimeout(timeout)
  }, [isSync])

  if (!isSync) return null

  return (
    <div className="absolute -left-9 top-1/2 flex -translate-y-1/2 items-center justify-center text-lg text-white">
      <TooltipProvider>
        <Tooltip>
          <TooltipTrigger>
            <div className="spinner"></div>
          </TooltipTrigger>
          <TooltipContent>
            <p>The website is being updated.</p>
          </TooltipContent>
        </Tooltip>
      </TooltipProvider>
    </div>
  )
}
