// See https://aka.ms/new-console-template for more information

using resume_builder.cli.commands;
using resume_builder.cli.commands.add;
using resume_builder.cli.commands.get;
using resume_builder.cli.commands.get.job;
using resume_builder.cli.commands.search;
using resume_builder.cli.commands.Search;
using resume_builder.cli.commands.search.job;
using resume_builder.cli.commands.Search.job;
using resume_builder.models;
using Spectre.Console.Cli;

namespace resume_builder;

public static class Program
{
	//todo: don't like that parent options and arguments are positional; spectre problem
	public static void AppConfiguration(IConfigurator config)
	{
		if(config is null)
			throw new ArgumentNullException(nameof(config));

	#if DEBUG
		config.PropagateExceptions();
		config.ValidateExamples();
	#endif

		config.SetApplicationName("resume builder");
		config.SetApplicationVersion("1.0.0");
		config.CaseSensitivity(CaseSensitivity.None);

		//todo: add option if adding an existing entry to edit it
		config.AddBranch<AddCommandSettings>("add", add =>
		{
			add.SetDescription("add new information to job database/bank");
			add.AddCommand<AddJobCommand>("job")
			   .WithDescription("add a new job")
			   .WithExample("add", "job", "-s", "2022-01-01", "-e", "2026-11-01", "-t", "foreman");
			add.AddCommand<AddProfileCommand>("profile")
			   .WithDescription("add a new profile")
			   .WithAlias("user");
			add.AddCommand<AddSkillCommand>("skill")
			   .WithDescription("add a new skill")
			   .WithExample("add", "skill", "Teamwork", "--type", "soft")
			   .WithExample("add", "skill", "'Psychoanalytic therapy'", "--type", "hard");
		});
		config.AddBranch<GetCommandSettings>("get", get =>
		{
			get.SetDescription("get information from job database/bank");

			get.AddCommand<GetJobCommand>("job")
			   .WithAlias("jobs");
		});
		config.AddBranch("search", search =>
		{
			search.AddBranch("job", SearchJobBranchConfig);
			search.AddBranch("jobs", SearchJobBranchConfig);
			return;

			void SearchJobBranchConfig(IConfigurator<SearchCommandSettings> searchJobConfig)
			{
				searchJobConfig.SetDefaultCommand<SearchJobCommand>();
				searchJobConfig.AddCommand<SearchJobIdCommand>("id");
				searchJobConfig.AddCommand<SearchJobDescriptionCommand>("description")
				               .WithAlias("desc")
				               .WithAlias("details")
				               .WithAlias("d");
				searchJobConfig.AddCommand<SearchJobExperienceCommand>("experience")
				               .WithAlias("exp")
				               .WithAlias("ex");
				// SearchJob.AddCommand<SearchJobSkillsCommand>("skills");
				searchJobConfig.AddCommand<SearchJobStartDateCommand>("start")
				               .WithAlias("s")
				               .WithAlias("start date");
				searchJobConfig.AddCommand<SearchJobEndDateCommand>("end");
				searchJobConfig.AddCommand<SearchJobTitleCommand>("title")
				               .WithAlias("t");

				searchJobConfig.AddCommand<SearchJobCommand>("")
				               .WithAlias("jobs")
				               .WithDescription("retrieve job(s) from database");
			}
		});

		config.AddCommand<InitCommand>("init")
		      .WithDescription("initializes resume database")
		      .WithAlias("start");
	}

	private static void Main(string[] args)
	{
		var app = new CommandApp();
		app.Configure(AppConfiguration);
		app.Run(args);
	}
}