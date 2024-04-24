import GoBackButton from './go-back-button'

function TeamPage() {
  return (
    <>
      <GoBackButton />
      <div className="max-w-4xl mx-auto mb-10 overflow-auto rounded-lg border border-gray-300 bg-white p-6">
        <h1 className="color-black pl-6 text-5xl">TEAM</h1>
        <div className="p-10 pl-20">
          <p>
            This website was collaboratively developed by students and academic faculty of the
            university as part of an initiative to promote our own publication repository and
            working paper series. All aspects of the website's creation were supervised and guided
            by Mark Motliuk.
          </p>
          <br />
          <p>The student team involved in the website's development included: [Names here].</p>
          <br />
          <p>The web design was crafted by Viktoriia Verych.</p>
          <br />
          <p>Currently, the role of content manager is held by [Some Name].</p>
        </div>
      </div>
    </>
  )
}

export default TeamPage
