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
            The KSE Publications website was launched in the Spring of 2024 to present a collection
            of the major academic and analytical publications produced by the KSE community. Its
            main objective is to disseminate original scholarly work relevant to KSE’s teaching and
            research focus.
          </p>
          <br />
          <p>
            The website collects publications authored by individuals affiliated with the KSE during
            their tenure at the institution, as well as works produced prior to or following their
            affiliation. Therefore, this collection does not constitute an official record of KSE’s
            institutional achievements and shall not be construed, interpreted, or represented as
            such.
          </p>
          <br />
          <p>
            This series also proudly publishes exceptional papers written by KSE students as part of
            their academic curriculum, promoting the culture of excellence at our university.
          </p>
          <br />
          <p>
            To further support this mission, in the Fall of 2024 KSE Publications launched a
            newsletter, "KSE Research Digest," to inform the students, faculty, and anyone
            interested in the academic life of the Kyiv School of Economics. With this newsletter,
            we hope to establish a strong research culture and stimulate networking within the KSE
            community and beyond.
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
