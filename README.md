# resumer

save your job details and easily build a résumé from them

## why?

well, you know, formatting in Word is just eeghh😵‍💫😵‍💫🥴, ya
and I hate the copy-paste from my *plain* docx resume to online formatters, just to get paywalled😑,
so I made this and hopefully i don't quit, and it turns into something nice :)

## installation

*requires existing dotnet runtime on system*

### from source

```bash
git clone https://github.com/csc530/resumer.git
cd resumer
dotnet publish -c Release -o ./path/to/publish
```

## todo ✅

- [ ] add corruption handler and detector for db
    - [ ] if it exists but isn't populated
    - [ ] missing or mal formatted tables
- [ ] managed personal job details
    - [ ] projects
    - [ ] jobs
    - [ ] volunteering
    - [ ] skills
    - [ ] references
- [ ] curate profile per resume
    - select skills, jobs, summary