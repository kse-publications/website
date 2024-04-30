import { AnimatedHeadLine } from '../ui/animated-headline'
import MainPageButton from './go-back-button'

function SubmissionsPage() {
  return (
    <>
      <MainPageButton />
      <div className="max-w-4xl mx-auto mb-10 overflow-auto rounded-lg border border-gray-300 bg-white p-6">
        <AnimatedHeadLine>SUBMISSIONS</AnimatedHeadLine>
        <div className="pt-6">
          <p>
            We gather publications created by the KSE community, regardless of their type, from all
            sources relevant to academic or policy-making contexts. We welcome publications from
            various areas and disciplines, with a strong emphasis on exceptional quality from
            reputable publishers.
          </p>
          <br />
          <div>
            <span className="font-bold">Your submission should include the following details:</span>
            <ol className="list-decimal pl-12 pt-3">
              <li>
                Type of publication (e.g., journal article, analytical article, blog contribution)
              </li>
              <li>List of authors</li>
              <li>Title of the publication</li>
              <li>Source of the publication (journal, volume, issue, pages)</li>
              <li>Abstract</li>
              <li>4 to 5 keywords</li>
              <li>Link to the original publication</li>
              <li>Link to the PDF file, if available, for download</li>
            </ol>
          </div>
          <br />
          <div>
            <span>
              We also accept submissions of videos providing a cohesive overview of research papers,
              projects, or experiments.
            </span>
            <br />
            <span className="font-bold">
              To submit a video, ensure it meets the following requirements:
            </span>
            <br />
            <ol className="list-decimal pl-12 pt-3">
              <li>
                The video should be of an appropriate length, typically between 5 to 15 minutes.
              </li>
              <li>
                Make sure that the video is well-structured, presenting the information in a clear
                and coherent manner. Use appropriate transitions and visual aids to enhance
                understanding.
              </li>
              <li>
                The video should cover all essential aspects of the research paper or project,
                including the objective, methodology, findings, and conclusions.
              </li>
              <li>
                You are expected to provide sufficient details to give the audience a comprehensive
                understanding of the work. Utilize visual elements effectively to support your
                presentation. This may include charts, graphs, images, diagrams, or animations that
                help illustrate key concepts or data points.
              </li>
              <li>Maintain a professional demeanor throughout the video.</li>
              <li>
                Pay attention to the technical quality of the video. Ensure that the audio is clear,
                the visuals are of good quality, and the video resolution is suitable for viewing
              </li>
            </ol>
          </div>
          <br />
          <p>
            For any inquiries or further assistance, please contact{' '}
            <span className="font-bold">
              <a href="mailto:publications@kse.org.ua">publications@kse.org.ua</a>
            </span>
          </p>
        </div>
      </div>
    </>
  )
}

export default SubmissionsPage
