using Microsoft.EntityFrameworkCore;
using Resumer.cli.commands;
using Resumer.cli.commands.add;
using Resumer.cli.commands.config;
using Resumer.cli.commands.delete;
using Resumer.cli.commands.edit;
using Resumer.cli.commands.get;
using Resumer.models;
using Spectre.Console.Cli;

namespace Resumer;

public static class Program
{
    private static int Main(string[] args)
    {
        Directory.CreateDirectory(TempPath);
        using(var ctx = new ResumeContext())
        {
            var database = ctx.Database;
            if(database.GetPendingMigrations().Any())
                database.Migrate();
        }

        var app = new CommandApp();
        app.Configure(AppConfiguration);
        return app.Run(args);
    }

    //todo: don't like that parent options and arguments are positional; spectre problem
    public static void AppConfiguration(IConfigurator config)
    {
    #if DEBUG
        config.PropagateExceptions();
        config.ValidateExamples();
        config.UseStrictParsing();
    #endif

        config.CaseSensitivity(CaseSensitivity.None);

        config.AddCommand<ExportCommand>("export")
            .WithDescription("export resume to various formats")
            .WithAlias("e");

        config.AddCommand<GenerateExampleTypstTemplate>("generate")
            .WithDescription("generate example typst resume template")
            .WithAlias("gen");

        config.AddBranch("add", add =>
        {
            add.SetDescription("add new information to database");
            add.AddCommand<AddJobCommand>("job")
                .WithDescription("add a new job")
                .WithAlias("j")
                .WithAlias("jobs");
            add.AddCommand<AddProfileCommand>("profile")
                .WithDescription("add a new profile")
                .WithAlias("user")
                .WithAlias("u")
                .WithAlias("users")
                .WithAlias("profiles");
            add.AddCommand<AddSkillCommand>("skill")
                .WithDescription("add a new skill")
                .WithAlias("s")
                .WithAlias("skills");
            add.AddCommand<AddProjectCommand>("project")
                .WithDescription("add a new project")
                .WithAlias("p")
                .WithAlias("projects");
            add.AddCommand<AddPdfTemplateCommand>("template")
                .WithAlias("t")
                .WithDescription("add a typst file template to export resume in pdf format");
            add.AddCommand<AddEducationCommand>("education")
                .WithAlias("e")
                .WithAlias("edu")
                .WithAlias("educations")
                .WithAlias("schools")
                .WithAlias("school")
                .WithAlias("degree")
                .WithDescription("add a new education");
        });

        config.AddBranch("edit", edit =>
        {
            edit.SetDescription("edit job information in database");
            edit.AddCommand<EditJobCommand>("job")
                .WithAlias("j")
                .WithAlias("jobs")
                .WithDescription("edit a job's details");
            edit.AddCommand<EditProfileCommand>("profile")
                .WithAlias("user")
                .WithAlias("u")
                .WithAlias("users")
                .WithAlias("profiles")
                .WithAlias("pro")
                .WithDescription("change a profile's information");
            edit.AddCommand<EditProjectCommand>("project")
                .WithDescription("edit a project's details")
                .WithAlias("projects")
                .WithAlias("proj");
            edit.AddCommand<EditEducationCommand>("education")
                .WithAlias("edu")
                .WithAlias("educations")
                .WithAlias("schools")
                .WithAlias("school")
                .WithAlias("degree")
                .WithDescription("edit an education's details");
        });

        config.AddBranch<DeleteCommandSettings>("delete", delete =>
            {
                delete.SetDescription("remove data from resumer database");
                delete.AddCommand<DeleteJobCommand>("job")
                    .WithDescription("delete a job")
                    .WithAlias("j")
                    .WithAlias("jobs");
                delete.AddCommand<DeleteProfileCommand>("profile")
                    .WithAlias("user")
                    .WithAlias("u")
                    .WithAlias("users")
                    .WithDescription("delete a profile")
                    .WithAlias("p")
                    .WithAlias("profiles");
                delete.AddCommand<DeleteSkillCommand>("skill")
                    .WithAlias("s")
                    .WithDescription("delete a skill")
                    .WithAlias("skills");
                delete.AddCommand<DeleteProjectCommand>("project")
                    .WithDescription("delete a project")
                    .WithAlias("projects");
                delete.AddCommand<DeleteTypstTemplateCommand>("template")
                    .WithDescription("delete a typst file template")
                    .WithAlias("t")
                    .WithAlias("templates");
                delete.AddCommand<DeleteEducationCommand>("education")
                    .WithAlias("edu")
                    .WithAlias("educations")
                    .WithAlias("schools")
                    .WithAlias("school")
                    .WithAlias("degree")
                    .WithDescription("delete an education");
            })
            .WithAlias("d")
            .WithAlias("del")
            .WithAlias("remove")
            .WithAlias("rm");


        config.AddBranch<OutputCommandSettings>("get", get =>
        {
            get.SetDescription("get job information from database");
            get.AddCommand<GetConfigCommand>("config")
                .WithAlias("configuration")
                .WithAlias("conf")
                .WithAlias("setting")
                .WithDescription("get app's current configuration settings")
                .WithAlias("settings");
            get.AddCommand<GetSkillCommand>("skill")
                .WithAlias("s")
                .WithDescription("list skills")
                .WithAlias("skills");
            get.AddCommand<GetCompanyCommand>("company")
                .WithAlias("c")
                .WithDescription("list companies")
                .WithAlias("companies")
                .WithDescription("list companies you've worked for");
            get.AddCommand<GetJobCommand>("job")
                .WithAlias("jobs")
                .WithDescription("list jobs")
                .WithAlias("j");
            get.AddCommand<GetProfileCommand>("profile")
                .WithAlias("user")
                .WithAlias("u")
                .WithAlias("users")
                .WithDescription("list user profiles")
                .WithAlias("profiles");
            get.AddCommand<GetProjectCommand>("project")
                .WithDescription("list projects")
                .WithAlias("p")
                .WithAlias("projects");
            get.AddCommand<GetTypstTemplateCommand>("template")
                .WithAlias("t")
                .WithAlias("templates")
                .WithDescription("list typst templates");
            get.AddCommand<GetEducationCommand>("education")
                .WithAlias("edu")
                .WithAlias("educations")
                .WithAlias("schools")
                .WithAlias("school")
                .WithAlias("degree")
                .WithDescription("list education");
        })
        .WithAlias("g")
        .WithAlias("list")
        .WithAlias("ls");
    }

    public static string TempPath { get; } = Path.GetTempPath() + "resumer" + Path.DirectorySeparatorChar;
}