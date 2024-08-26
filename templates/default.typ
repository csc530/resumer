// resumer's default template

#let dateFormat = "[month repr:short]. [year repr:full sign:automatic]"

= #fullName
== #phoneNumber
== #emailAddress

#location,

#if website != none {
  underline(link(website))
}
#line(length: 100%)

#objective

#if jobs != none {
  [== Work Experience

    #for job in jobs {
      [=== #job.title -- #job.company]

      job.startDate.display(dateFormat)
      " - "
      if job.endDate != none {
        job.endDate.display(dateFormat)
      } else { "Present" }

      linebreak()

      for desc in job.description {
        [- #desc]
      }
    }
  ]
}
\
#if projects != none {
  [= Projects

    #for proj in projects {
      link(
        proj.link.absoluteUri,
      )[=== #proj.title #if proj.type != none { [-- #proj.type] }]
      if proj.link.absoluteUri != none {
        underline(link(proj.link.absoluteUri))
        linebreak()
      }

      if proj.startDate != none {
        proj.startDate.display(dateFormat)
        " - "
        if proj.endDate != none {
          proj.endDate.display(dateFormat)
        } else { "Present" }
        linebreak()
      }

      proj.description

      for detail in proj.details {
        [- #detail]
      }
    }]
}

#if skills != none {
  [= Skills

    #box(height: 15%, width: 100%, outset: 25%)[
      #columns(calc.ceil(skills.len() / 8))[
        #for skill in skills {
          [- #skill.name]
        }
      ]
    ]
  ]
}

#if education != none {
  [= Education

    #for edu in education {
      [=== #edu.school -- #edu.fieldOfStudy]

      edu.degree
      linebreak()

      if edu.startDate != none {
        edu.startDate.display(dateFormat)
        " - "
        if edu.endDate != none {
          edu.endDate.display(dateFormat)
        } else { "Present" }
      }

      linebreak()
      edu.location
      linebreak()

      edu.additionalInformation
    }]
}

#if languages != none {
  [= Languages

    #for lang in languages {
      [- #lang]
    }]
}