using NUnit.Framework.Internal;
using resume_builder;
using resume_builder.models;

namespace TestResumeBuilder;

[TestFixture]
[SingleThreaded]
public class DatabaseTest
{
    protected const string BackupResumeSqliteFileName = "backup_resume.sqlite";
    internal const string ResumeSqliteFileName = "resume.sqlite";
	protected internal Database? Database { get; set; }



    
}