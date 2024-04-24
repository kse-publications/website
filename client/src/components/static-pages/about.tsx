import GoBackButton from './go-back-button'

function AboutPage() {
  return (
    <>
      <GoBackButton />
      <div className="max-w-4xl mx-auto mb-10 overflow-auto rounded-lg border border-gray-300 bg-white p-6">
        <div className="ml-5 w-fit">
          <h1 className="color-black text-5xl font-semibold leading-none tracking-tight">ABOUT</h1>
          <span className="-mt-1 block h-1 w-full bg-[#e4e541]"></span>
        </div>
        <div className="p-10 pl-20">
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
      </div>
    </>
  )
}

export default AboutPage
