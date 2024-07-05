import { AnimatedHeadLine } from '../ui/animated-headline'
import MainPageButton from '../layout/to-main-button'
import AboutCard from './about-card'

interface AboutPageProps {
  totalPublication: number
  totalSearches: number
  publicationViews: number
}

function AboutPage({ totalPublication, totalSearches, publicationViews }: AboutPageProps) {
  return (
    <>
      <MainPageButton />
      <div className="mx-auto mb-10 w-full max-w-[1128px] overflow-auto rounded-lg border border-gray-300 bg-white p-6">
        <AnimatedHeadLine>ABOUT</AnimatedHeadLine>
        <div className="mb-5 pt-6">
          <p>
            The KSE Publications website presents a collection of the major academic and analytical
            publications produced by the KSE community. Its main objective is to disseminate KSE's
            original academic work that is relevant to the fields of political and social sciences
            in a broader context.
          </p>
          <br />
          <p>
            Specifically, the KSE Publications aim to actively promote the academic achievements of
            the faculty and researchers at Kyiv School of Economics. Additionally, this series
            proudly publishes exceptional papers written by KSE students as part of their academic
            curriculum, demonstrating and further enhancing the culture of excellence at our
            university.
          </p>
          <br />
          <p>
            The KSE Publications strives to advance the use of KSE research output and increase its
            impact globally and nationally. This goal is a central element of KSE's overarching
            strategy to establish itself as the forefront institution in spearheading and
            influencing the elevation of scientific and teaching standards throughout Ukraine.
          </p>
        </div>
        <div className="flex justify-around gap-6">
          <AboutCard lable="Publications in the repository" stat={totalPublication} />
          <AboutCard lable="Total search queries" stat={totalSearches} />
          <AboutCard lable="Total publication views" stat={publicationViews} />
        </div>
      </div>
    </>
  )
}

export default AboutPage
