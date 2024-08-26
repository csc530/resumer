// A colorful resume template for Typst by: elegaanz (@ana.gelez.xyz)

#import "@preview/vercanard:1.0.0": *
#let dateFormat = "[month repr:short]. [year repr:full sign:automatic]"

#let duration(start, end) = [
  #if end == none {
    start.display(dateFormat) + " - Present"
  } else {
    start.display(dateFormat) + " - " + end.display(dateFormat)
  }
]


#show: resume.with(
  // The title of your resume, generally your name
  name: wholeName,
  // The subtitle, which is the position you are looking for most of the time
  title: [#link("mailto:" + emailAddress) #if website != none [\ #underline(link(website))] \ #link("tel:" + phoneNumber)],
  // The accent color to use (here a catppucin sky)
  accent-color: color.rgb("#04a5e5"),
  // the margins (only used for top and left page margins actually,
  // but the other ones are proportional)
  margin: 2.6cm,
  // The content to put in the right aside block
  aside: [
    #set list(tight: false, body-indent: 0cm)

    #if (skills != none and skills.len() > 0) [
      = skills

      #for skill in skills {
        [- #skill.name]
      }
    ]


    #if (languages != none and languages.len() > 0) [
      = Languages
      #for language in languages {
        [- #language]
      }
    ]
  ],
)

// And finally the main body of your resume can come here

#if (jobs != none and jobs.len() > 0) [
  = Exprience

  #for job in jobs {
    entry(job.title, job.company, duration(job.startDate, job.endDate))
  }
]

#if (projects != none and projects.len() > 0) [
  = Projects

  #for project in projects {
    entry(
      project.title,
      duration(project.startDate, project.endDate),
      project.type,
    )
  }
]

#if (education != none and education.len() > 0) [
  = Education

  #for edu in education {
    entry(edu.school)[#edu.degree -- #edu.fieldOfStudy][#duration(edu.startDate, edu.endDate)]
  }
]