using Microsoft.EntityFrameworkCore;
using Resumer.cli.commands.add;
using Resumer.cli.commands.config;
using Resumer.cli.commands.delete;
using Resumer.cli.commands.edit;
using Resumer.cli.commands.export;
using Resumer.cli.commands.get;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console.Cli;

namespace Resumer;

public static class Program
{
    private static int Main(string[] args)
    {
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
    #endif

        config.SetApplicationName("resume builder");
        config.SetApplicationVersion("1.0.0");
        config.CaseSensitivity(CaseSensitivity.None);

        config.AddCommand<ExportCommand>("export")
            .WithDescription("export resume to various formats")
            .WithAlias("e");


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
        });

        config.AddBranch("edit", edit =>
        {
            edit.SetDescription("edit job information in database");
            edit.AddCommand<EditJobCommand>("job")
                .WithAlias("j")
                .WithAlias("jobs");
            edit.AddCommand<EditProfileCommand>("profile")
                .WithAlias("user")
                .WithAlias("u")
                .WithAlias("users")
                .WithAlias("p")
                .WithAlias("profiles");
            edit.AddCommand<EditProjectCommand>("project")
                .WithDescription("edit a project")
                .WithAlias("projects");
        });

        config.AddBranch("delete", delete =>
            {
                delete.SetDescription("delete job information from database");
                delete.AddCommand<DeleteJobCommand>("job")
                    .WithAlias("j")
                    .WithAlias("jobs");
                delete.AddCommand<DeleteProfileCommand>("profile")
                    .WithAlias("user")
                    .WithAlias("u")
                    .WithAlias("users")
                    .WithAlias("p")
                    .WithAlias("profiles");
                delete.AddCommand<DeleteSkillCommand>("skill")
                    .WithAlias("s")
                    .WithAlias("skills");
                delete.AddCommand<DeleteProjectCommand>("project")
                    .WithDescription("delete a project")
                    .WithAlias("projects");
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
                .WithAlias("settings");
            get.AddCommand<GetSkillCommand>("skill")
                .WithAlias("s")
                .WithAlias("skills");
            get.AddCommand<GetCompanyCommand>("company")
                .WithAlias("c")
                .WithAlias("companies")
                .WithDescription("list companies you've worked for");
            get.AddCommand<GetJobCommand>("job")
                .WithAlias("jobs")
                .WithAlias("j");
            get.AddCommand<GetProfileCommand>("profile")
                .WithAlias("user")
                .WithAlias("u")
                .WithAlias("users")
                .WithAlias("profiles");
            get.AddCommand<GetProjectCommand>("project")
                .WithDescription("list projects")
                .WithAlias("p")
                .WithAlias("projects");
        });
    }
}