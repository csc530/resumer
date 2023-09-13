CREATE TABLE companies
(
    id   INTEGER,
    name VARCHAR(255) NOT NULL,
    PRIMARY KEY (id AUTOINCREMENT)
);

CREATE TABLE jobs
(
    id                integer NOT NULL,
    company           integer,
    title             varchar(100),
    "start date"      date,
    "end date"        date,
    "job description" text,
    experience        text,
    CONSTRAINT id
        PRIMARY KEY (id AUTOINCREMENT),
    CONSTRAINT jobs_companies_id_fk
        FOREIGN KEY (company) REFERENCES companies
);

CREATE TABLE skills
(
    skill varchar(100) NOT NULL,
    id    integer      NOT NULL,
    CONSTRAINT skills_pk
        PRIMARY KEY (id AUTOINCREMENT),
    CONSTRAINT skills_pk2
        UNIQUE (skill)
);

CREATE TABLE job_skills
(
    jobID   integer NOT NULL,
    skillID integer NOT NULL,
    CONSTRAINT job_skills_pk
        PRIMARY KEY (jobID, skillID),
    CONSTRAINT job_skills_jobs_id_fk
        FOREIGN KEY (jobID) REFERENCES jobs,
    CONSTRAINT job_skills_skills_id_fk
        FOREIGN KEY (skillID) REFERENCES skills
)
    WITHOUT ROWID;

CREATE UNIQUE INDEX job_skills_jobID_uindex
    ON job_skills (jobID);