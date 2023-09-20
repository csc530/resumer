CREATE TABLE companies
(
    name varchar(100) NOT NULL
        CONSTRAINT companies_pk
            PRIMARY KEY
);

CREATE TABLE jobs
(
    id                integer NOT NULL
        CONSTRAINT id
            PRIMARY KEY AUTOINCREMENT,
    company           varchar(100)
        CONSTRAINT jobs_companies_name_fk
            REFERENCES companies,
    title             varchar(100),
    "start date"      date,
    "end date"        date,
    "job description" text,
    experience        text
);

CREATE TABLE skills
(
    skill varchar(100) NOT NULL
        CONSTRAINT skills_pk
            PRIMARY KEY
);

CREATE TABLE job_skills
(
    jobID   integer NOT NULL
        CONSTRAINT job_skills_jobs_id_fk
            REFERENCES jobs
            ON DELETE CASCADE,
    skillID integer NOT NULL
        CONSTRAINT job_skills_skills_id_fk
            REFERENCES skills
            ON DELETE CASCADE,
    CONSTRAINT job_skills_pk
        PRIMARY KEY (jobID, skillID)
)
    WITHOUT ROWID;

CREATE UNIQUE INDEX job_skills_jobID_uindex
    ON job_skills (jobID);

CREATE TABLE user
(
    id             integer      NOT NULL
        PRIMARY KEY AUTOINCREMENT,
    "first name"   varchar(100) NOT NULL,
    "middle name"  varchar(100),
    "last name"    integer TEXT,
    "phone number" integer,
    email          varchar(100),
    website        varchar(500),
    summary        text
);


