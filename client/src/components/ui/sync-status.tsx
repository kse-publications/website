import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from './tooltip'

interface SyncStatusProps {
  isSync: boolean
}

export const SyncStatus = ({ isSync }: SyncStatusProps) => {
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
