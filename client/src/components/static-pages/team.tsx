import { AnimatedHeadLine } from '../ui/animated-headline'
import MainPageButton from './to-main-button'

function TeamPage() {
  return (
    <>
      <MainPageButton />
      <div className="max-w-4xl mx-auto mb-10 overflow-auto rounded-lg border border-gray-300 bg-white p-6">
        <AnimatedHeadLine>TEAM</AnimatedHeadLine>
        <div className="pt-6">
          <p>
            This website is collaboratively developed by students and academic faculty of the
            university as part of an initiative to promote KSE publications. The project is
            proposed, organized, and executed by{' '}
            <a
              href="https://www.linkedin.com/in/dr-larysa-tamilina-45a50431/"
              className="underline"
              target="_blank"
            >
              Dr. Larysa Tamilina
            </a>{' '}
            (Associate Professor at KSE). All aspects of the website's creation are supervised and
            guided by{' '}
            <a
              href="https://www.linkedin.com/in/mark-motliuk/"
              className="underline"
              target="_blank"
            >
              Mark Motliuk
            </a>{' '}
            (KSE BA '25).
          </p>
          <br />
          <p>The technical team comprises four students:</p>
          <p>
            - Back-End development is led by{' '}
            <a
              href="https://www.linkedin.com/in/vladyslav-prudius/"
              className="underline"
              target="_blank"
            >
              Vlad Prudius
            </a>{' '}
            (KSE BA '27) and assisted by{' '}
            <a
              href="https://www.linkedin.com/in/anna-opanasenko-25a762264/"
              className="underline"
              target="_blank"
            >
              Anna Opanasenko
            </a>{' '}
            (KSE BA '27);
          </p>
          <p>
            - Front-End development is led by{' '}
            <a
              href="https://www.linkedin.com/in/dmytro-kolisnyk-203a61235/"
              className="underline"
              target="_blank"
            >
              Dmytro Kolisnyk
            </a>{' '}
            (KSE BA '27) and assisted by{' '}
            <a
              href="https://www.linkedin.com/in/tetiana-stasiuk-16947b210/"
              className="underline"
              target="_blank"
            >
              Tetiana Stasiuk
            </a>{' '}
            (KSE MA '24).
          </p>
          <br />
          <p>
            The web design is crafted by{' '}
            <a
              href="https://www.linkedin.com/in/viktoriia-verych-8842b725a/"
              className="underline"
              target="_blank"
            >
              Viktoriia Verych
            </a>{' '}
            (KSE BA '26).
          </p>
        </div>
      </div>
    </>
  )
}

export default TeamPage
