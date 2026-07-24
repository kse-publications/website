import { Button } from '../ui/button'
import { AnimatedHeadLine } from '../ui/animated-headline'
import MainPageButton from '../layout/to-main-button'

function PersonLink(props: { name: string; link: string }) {
  return (
    <a href={props.link} className="underline" target="_blank">
      <Button variant="link" className="h-0 p-0 text-base">
        {props.name}
      </Button>
    </a>
  )
}

function TeamPage() {
  return (
    <>
      <MainPageButton />
      <div className="mx-auto mb-10 w-full max-w-[1128px] overflow-auto rounded-lg border border-gray-300 bg-white p-6">
        <AnimatedHeadLine>TEAM</AnimatedHeadLine>
        <div className="pt-6">
          <p>
            This website is collaboratively developed by students and academic faculty of the
            university as part of an initiative to promote KSE publications.
          </p>

          <br />

          <p>
            The project is currently managed by{' '}
            <PersonLink
              link="https://www.linkedin.com/in/vladyslava-homonenko/"
              name="Vladyslava Homonenko"
            />
            , who is also responsible for the project’s newsletter “KSE explores”, highlighting
            recent KSE publications, key research insights, and author interviews.
          </p>

          <br />

          <p>
            <PersonLink link="https://www.linkedin.com/in/annakorzhenko" name="Anna Korzhenko" /> is
            responsible for preparing a standalone educational module on academic and research
            writing.
          </p>

          <br />

          <p>
            <PersonLink
              link="https://www.linkedin.com/in/olha-holovan-41b514343/"
              name="Olha Holovan"
            />{' '}
            (KSE BA '28) serves as content manager, overseeing and curating publication content.
          </p>

          <br />

          <p>
            The website’s development is led and executed by{' '}
            <PersonLink link="https://www.linkedin.com/in/vladyslav-prudius/" name="Vlad Prudius" />{' '}
            (KSE BA '27), who also manages the technical team. The technical team includes{' '}
            <PersonLink
              link="https://www.linkedin.com/in/nazar-andriievych/"
              name="Nazar Andriievych"
            />
            , contributing to the project’s ongoing development.
          </p>

          <br />

          <p className="mt-1.5 leading-10">Acknowledgments.</p>
          <p>
            We would like to acknowledge and thank the following individuals for their valuable
            contributions to the project at earlier stages of its development:
          </p>
          <p>
            -{' '}
            <PersonLink
              link="https://www.linkedin.com/in/dr-larysa-tamilina-45a50431/"
              name="Dr. Larysa Tamilina"
            />{' '}
            – initiator and initial manager of the project
            <br />-{' '}
            <PersonLink link="https://www.linkedin.com/in/mark-motliuk/" name="Mark Motliuk" /> (KSE
            BA '25) – project co-manager
            <br />-{' '}
            <PersonLink
              link="https://www.linkedin.com/in/anastasia-shevelova-344a1b2a5/"
              name="Anastasia Shevelova"
            />{' '}
            (KSE BA '25) – project co-manager
            <br />-{' '}
            <PersonLink
              link="https://www.linkedin.com/in/christineborovkova/"
              name="Christine Borovkova"
            />{' '}
            (KSE SPS '26) – newsletter editing and author interviews
            <br />-{' '}
            <PersonLink
              link="https://www.linkedin.com/in/dmytro-kolisnyk-203a61235/"
              name="Dmytro Kolisnyk"
            />{' '}
            (KSE BA '27) – Front-End development lead
            <br />-{' '}
            <PersonLink
              link="https://www.linkedin.com/in/tetiana-stasiuk-16947b210/"
              name="Tetiana Statisuk"
            />{' '}
            (KSE MA '24) – Front-End development (Spring–Fall 2024)
            <br />-{' '}
            <PersonLink
              link="https://www.linkedin.com/in/viktoriia-verych-8842b725a/"
              name="Viktoriia Verych"
            />{' '}
            (KSE BA '26) – crafted the project’s design
            <br />-{' '}
            <PersonLink
              link="https://www.linkedin.com/in/anna-opanasenko-25a762264/"
              name="Anna Opanasenko"
            />{' '}
            (KSE BA '27) – Back-End development (Spring 2024)
          </p>
        </div>
      </div>
    </>
  )
}

export default TeamPage
