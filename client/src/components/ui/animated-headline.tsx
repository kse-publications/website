import type { ReactNode } from 'react'

export const AnimatedHeadLine = ({ children }: { children: ReactNode }) => {
  return (
    <h2 className="mb-4 w-fit text-3xl font-semibold leading-none tracking-tight">
      {children}
      <span className="line-grow -mt-1 hidden h-1 w-full bg-yellow md:block"></span>
    </h2>
  )
}
