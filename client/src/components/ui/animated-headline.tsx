import type { ReactNode } from 'react'

export const AnimatedHeadLine = ({ children }: { children: ReactNode }) => {
  return (
    <h2 className="mb-4 w-fit text-3xl font-semibold leading-none tracking-tight">
      {children}
      <span className="line-grow -mt-1 block h-1 w-full bg-[#e4e541]"></span>
    </h2>
  )
}
