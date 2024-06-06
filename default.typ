#let firstName="Keream";
#let middleName="Dimdle";
#let lastName="Doe";
#let fullName="Keream Doe";
#let wholeName="Keream Dimdle Doe";
#let emailAddress="bFJpL@example.com";
#let phoneNumber="123-456-7890";
#let location="Vancouver, Canada";
#let website="https://keream_dimdle.ex";
#let objective="objective";
#let education=((id: "8dc60523-0be3-458b-8eda-02137ca6046f",school: "school",degree: "degree",fieldOfStudy: "field",startDate: datetime(year: 1, month: 1, day: 1),endDate: datetime(year: 9999, month: 12, day: 31),gradePointAverage: 4,location: "location",additionalInformation: "additional info"),);
#let languages=("en","fr");
#let interests=("learning","creating","reading");
#let jobs=((id: "e755a22a-19bb-41f3-9c1c-7d94a2f360b9",title: "job title",company: "company",startDate: datetime(year: 2024, month: 6, day: 5),endDate: none,duration: "Jun. 2024 - present",description: (),experience: ()),);
#let skills=((name: "hard",type: "Hard"),(name: "hard",type: "Soft"));
#let projects=((id: "a998cf1f-f6f5-45c3-8a76-86bbd6f373ed",title: "project title",type: "Independent",description: "project description",details: ("project's","details"),link: (absolutePath: "/",absoluteUri: "https://myproject.ca/",localPath: "/",authority: "myproject.ca",hostNameType: "Dns",isDefaultPort: true,isFile: false,isLoopback: false,pathAndQuery: "/",segments: ("/",),isUnc: false,host: "myproject.ca",port: 443,query: "",fragment: "",scheme: "https",originalString: "https://myproject.ca",dnsSafeHost: "myproject.ca",idnHost: "myproject.ca",isAbsoluteUri: true,userEscaped: false,userInfo: ""),startDate: datetime(year: 1, month: 1, day: 1),endDate: datetime(year: 9999, month: 12, day: 31)),);


= #fullName
== #phoneNumber
== #emailAddress
#location,

#website

#objective

#if jobs != none {
  [== Work Experience

  #for job in jobs {
    [=== #job.title, #job.company]

    job.duration


    for desc in job.description {
      - desc
    }
  }
]}

#if projects != none {
  [= Projects

    #for proj in projects {
      [=== #proj.title #if proj.type != none {[-- #proj.type]}]

      proj.description

      for detail in proj.details {
        [- #detail]
      }
    }]
}

#if skills != none {
  [= Skills

  #for skill in skills {
    [- skill.name]
  }]
}

#if education != none {
  [= Education

  #for edu in education {
    [=== #edu.school -- #edu.fieldOfStudy]
    [#edu.startDate.display() - #edu.endDate.display()]

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