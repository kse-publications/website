import { Card, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'

interface AboutStatsProps {
  lable: string
  stat: number
}

function AboutCard({ lable, stat }: AboutStatsProps) {
  return (
    <Card className="w-full">
      <CardHeader className="p-4">
        <CardTitle className="flex justify-center text-4xl font-bold">{stat}</CardTitle>
        <CardDescription className="text-center">{lable}</CardDescription>
      </CardHeader>
    </Card>
  )
}

export default AboutCard
