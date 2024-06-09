#let dateFormat = "[month repr:short]. [year repr:full sign:automatic]"


= #fullName
== #phoneNumber
== #emailAddress

#location,

#website

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
]}

#if projects != none {
  [= Projects

    #for proj in projects {
      [=== #proj.title #if proj.type != none {[-- #proj.type]}]

      if proj.startDate != none { proj.startDate.display(dateFormat)
      " - "
        if proj.endDate != none {
          proj.endDate.display(dateFormat)
        } else { "Present" }
      }

      linebreak()

      proj.description

      for detail in proj.details {
        [- #detail]
      }
    }]
}

#if skills != none {
  [= Skills

  #for skill in skills {
    [- #skill.name]
  }]
}

#if education != none {
  [= Education

  #for edu in education {
    [=== #edu.school -- #edu.fieldOfStudy]

    if edu.startDate != none { edu.startDate.display(dateFormat)
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

#if interests != none {
  [= Interests

  #for interest in interests {
    [- #interest]
  }]
}
