# using [fakedata](https://github.com/lucapette/fakedata)

def "random job" [] {
    let start = fakedata date --limit 1
    {
        Id: (fakedata uuidv7 --limit 1 | into string | str upcase)
        Company: ($'(if (random bool) {fakedata adjectives --limit (random int 1..3) | lines | str join " " })  (fakedata industry --limit 1)' | str title-case)
        Description: (if (random bool) { (fakedata sentence --limit (random int 1..5) | lines) } else { [] })
        StartDate: $start
        EndDate: (if (random bool) and ($start|is-not-empty) { (($start | into datetime) + (random int 1..5 | into int | into duration --unit (fakedata enum:ns,us,ms,sec,min,hr,day,wk -l 1))) | format date "%Y-%m-%d" })
        Title: (fakedata occupation --limit 1)
    }
}

def "random profile" [] {
    {
        Id: (fakedata uuidv7 --limit 1 | into string | str upcase)
        FirstName: (fakedata name.first --limit 1)
        LastName: (fakedata name.last --limit 1)
        MiddleName: (if (random bool) {if (random bool) { (fakedata name.first --limit 1) } else { fakedata name.last --limit 1 } })
        EmailAddress:  (fakedata email --limit 1)
        PhoneNumber: (if (random bool) { (fakedata phone --limit 1) } else { fakedata phone:(random int 8..12) --limit 1 })
        Interests: (if (random bool) { (fakedata noun --limit (random int 1..5) | lines) } else { [] })
        Website: (if (random bool) { (fakedata username domain.tld --limit 1 --separator .) })
        Objective: (if (random bool) { fakedata sentence --limit (random int 1..5) | lines | str join })
        Location: (if (random bool) {fakedata city --limit 1})
        Languages: (if (random bool) { (fakedata noun --limit (random int 1..5) | lines) } else { [] })
    }
}

def "random skill" [] {
    {
        Name: (fakedata noun --limit 1)
        Type: (fakedata enum:0,1 --limit 1)
    }
}

def "random project" [] {
    let date = fakedata date --limit 1
    {
        Id: (fakedata uuidv7 --limit 1 | into string | str upcase)
        Title: (fakedata noun --limit 1)
        Type: (if (random bool) { (fakedata noun --limit 1) })
        Description: (if (random bool) { (fakedata sentence --limit 1) })
        StartDate: (if (random bool) { $date })
        EndDate: (if (random bool) { (fakedata date:($date) --limit 1) })
        Link: (if (random bool) { ('https://{{ Domain }}' | fakedata --limit 1) })
        Details: (if (random bool) { (fakedata sentence --limit (random int 1..5) | lines) })
    }
}

def "populate db" [num?:int = 10] {
    generate {|_| {out: (random job), next: true}} null | first $num | upsert Description {|e| '[' + ($e.Description? | par-each {|i| if ($i | is-not-empty) {$'"($i)"'}} | str join ,) + ']'} | into sqlite resume.db --table-name Jobs
    generate {|_| {out: (random profile), next: true}} null | first $num | upsert Languages {|e| '[' + ($e.Languages | par-each {|i| if ($i | is-not-empty) {$'"($i)"'}} | str join ,) + ']'} | update Interests {|e| '[' + ($e.Interests | par-each {|i| if ($i | is-not-empty) {$'"($i)"'}} | str join ,) + ']'} | into sqlite resume.db --table-name Profiles
    generate {|_| {out: (random skill), next: true}} null | first $num | into sqlite resume.db --table-name Skills
    generate {|_| {out: (random project), next: true}} null | first $num | upsert Details {|e| '[' + ($e.Details? | par-each {|i| if ($i | is-not-empty) {$'"($i)"'}} | str join ,) + ']'} |  into sqlite resume.db --table-name Projects
}