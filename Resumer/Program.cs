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

        config.AddBranch("config", configurator =>
        {
            configurator.SetDescription("configure resume builder settings");
            configurator.AddCommand<GetConfigCommand>("get")
                .WithDescription("get configuration settings")
                .WithAlias("g");
        });

        config.AddBranch<AddCommandSettings>("add", add =>
        {
            add.SetDescription("add new information to job database/bank");
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
        });

        config.AddBranch("edit", edit =>
        {
            edit.SetDescription("edit job information in database/bank");
            edit.AddCommand<EditJobCommand>("job")
                .WithAlias("j")
                .WithAlias("jobs");
            edit.AddCommand<EditProfileCommand>("profile")
                .WithAlias("user")
                .WithAlias("u")
                .WithAlias("users")
                .WithAlias("p")
                .WithAlias("profiles");
        });

        config.AddBranch("delete", delete =>
            {
                delete.SetDescription("delete job information from database/bank");
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
            })
            .WithAlias("d")
            .WithAlias("del")
            .WithAlias("remove")
            .WithAlias("rm");


        config.AddBranch<OutputCommandSettings>("get", get =>
        {
            get.SetDescription("get job information from database/bank");
            get.AddCommand<GetSkillCommand>("skill")
                .WithAlias("s")
                .WithAlias("skills");
            get.AddCommand<GetCompanyCommand>("company")
                .WithAlias("c")
                .WithAlias("companies")
                .WithDescription("get company information you've worked for");
            get.AddCommand<GetJobCommand>("job")
                .WithAlias("jobs")
                .WithAlias("j");
            get.AddCommand<GetProfileCommand>("profile")
                .WithAlias("user")
                .WithAlias("u")
                .WithAlias("users")
                .WithAlias("profiles");
        });
    }
}