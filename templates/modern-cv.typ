// A modern resume template written in Typst, inspired by Awesome-CV from @DeveloperPaul123 on GitHub
#import "@preview/modern-cv:0.3.1": *

#show: resume.with(
  author: (
    firstname: firstName,
    lastname: lastName,
    email: emailAddress,
    phone: phoneNumber,
    github: none,
    linkedin:  none,
    address: location,
    positions: (),
  ),
  date: datetime.today().display(),
  language: "en",
  colored-headers: true,
)

#let dateRange = (start, end) => start.display() + " - " + if end != none {end.display()} else { "Present" }

#if jobs != none and jobs.len() > 0 {
  [= Experience

  #for job in jobs {
    resume-entry(
      title: job.title,
      // location: exp.location,
      date: dateRange(job.startDate, job.endDate),
      description: job.company,
    )

    resume-item[
      #for description in job.description {
        [- #description]
      }
    ]
  }]
}

#if projects != none and projects.len() > 0 {
  [= Projects

    #for proj in projects {
      resume-entry(
        title: proj.title,
        location: proj.link.originalString, //github-link("DeveloperPaul123/thread-pool"),
        date: dateRange(proj.startDate, proj.endDate),
        description: proj.description,
      )

      resume-item[
        #for detail in proj.details {
          [- #detail]
        }
      ]
    }]
}

#if skills != none and skills.len() > 0 {
  [= Skills

  #let hardSkills = skills.filter(s => s.type == "Hard").map(s => s.name)
  #let softSkills = skills.filter(s => s.type == "Soft").map(s => s.name)

    #resume-skill-item(
      "Hard Skills",
      hardSkills
    )

    #resume-skill-item(
      "Soft Skills",
      softSkills,
    )
  ]
}

#if education != none and education.len() > 0 {
  [= Education

  #for edu in education {
    resume-entry(
      title: edu.school,
      location: edu.location,
      date: dateRange(edu.startDate, edu.endDate),
      description: edu.degree + " in " + edu.fieldOfStudy,
    )

    resume-item[#edu.additionalInformation]
  }]
}