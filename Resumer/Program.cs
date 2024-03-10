using System.Data.Common;
using Resumer.cli.commands;
using Resumer.cli.commands.add;
using Resumer.cli.commands.config;
using Resumer.cli.commands.export;
using Resumer.cli.commands.get;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer;

public static class Program
{
    private static int Main(string[] args)
    {
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

        config.AddBranch("config", configurator => {
            configurator.SetDescription("configure resume builder settings");
            configurator.AddCommand<GetConfigCommand>("get")
                        .WithDescription("get configuration settings")
                        .WithAlias("g");
        });

        //todo: add option if adding an existing entry to edit it
        config.AddBranch<AddCommandSettings>("add", add => {
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
               .WithExample("add", "skill", "Teamwork", "soft")
               .WithExample("add", "skill", "'Psychoanalytic therapy'", "hard")
               .WithAlias("s")
               .WithAlias("skills");
            add.AddCommand<AddProjectCommand>("project")
               .WithDescription("add a new project")
               .WithAlias("p")
               .WithAlias("projects");
        });

        config.AddBranch<OutputCommandSettings>("get", get => {
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

        config.AddCommand<InitCommand>("init")
              .WithDescription("initializes resume database")
              .WithAlias("start");
    }
}