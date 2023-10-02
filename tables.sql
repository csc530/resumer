CREATE TABLE company
(
    name varchar(100) NOT NULL
        CONSTRAINT companies_pk
            PRIMARY KEY
);

CREATE TABLE job
(
    id           integer NOT NULL
        CONSTRAINT id
            PRIMARY KEY AUTOINCREMENT,
    company      varchar(100)
        CONSTRAINT jobs_companies_name_fk
            REFERENCES company,
    title        varchar(100),
    startDate date,
    endDate   date,
    description  text,
    experience   text
);

CREATE TABLE skill
(
    name varchar(100) NOT NULL
        CONSTRAINT skills_pk
            PRIMARY KEY,
    type  varchar(100)
);
-- todo: figure if i want to relate skills to jobs
-- CREATE TABLE job_skills
-- (
--     jobID   integer NOT NULL
--         CONSTRAINT job_skills_jobs_id_fk
--             REFERENCES job,
--     skillID varchar(100) NOT NULL
--         CONSTRAINT job_skills_skills_id_fk
--             REFERENCES skill,
--     CONSTRAINT job_skills_pk
--         PRIMARY KEY (jobID, skillID)
-- )
--     WITHOUT ROWID;
--
-- CREATE UNIQUE INDEX job_skills_jobID_uindex
--     ON job_skills (jobID);

CREATE TABLE profile
(
    id             integer      NOT NULL
        PRIMARY KEY AUTOINCREMENT,
    firstName   varchar(100) NOT NULL,
    middleName  varchar(100),
    lastName    integer TEXT,
    phoneNumber integer,
    email          varchar(100),
    website        varchar(500),
    summary        text
);


