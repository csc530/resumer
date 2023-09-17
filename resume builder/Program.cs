// See https://aka.ms/new-console-template for more information

// namespace resume_builder;
//
// internal static class Program
// {
//     public static int Main(string[] args) => new App().Run(args);
// }

using System.Reflection;
using resume_builder;
using resume_builder.cli;
using resume_builder.cli.commands;
using Spectre.Console;
using Spectre.Console.Cli;
using static resume_builder.cli.commands.add.App;

public class Program
{
	public static void appConfiguration(IConfigurator config)
	{
		if(config is null)
		{
			throw new ArgumentNullException(nameof(config));
		}
#if DEBUG
		config.PropagateExceptions();
		config.ValidateExamples();
#endif
		config.SetApplicationName("resume builder");
		config.SetApplicationVersion("1.0.0");
		config.CaseSensitivity(CaseSensitivity.None);

		config.AddBranch<AddSetting>("add", add =>
		{
			add.SetDescription("add new information to job database/bank");
			add.AddCommand<AddJobCommand>("job")
				.WithDescription("add a new job")
				.WithExample("add", "job", "-s", "2022-01-01", "-e", "2026-11-01", "-t", "foreman");
		});
		config.AddCommand<InitCommand>("init")
			.WithDescription("initializes resume database")
			.WithAlias("start");
	}

	private static void Main(string[] args)
	{
		var app = new CommandApp();
		app.Configure(appConfiguration);


		app.Run(args);

	}
	public static Action<IConfigurator> appconfig()
	{
		return config =>
		{
			if(config is null)
			{
				throw new ArgumentNullException(nameof(config));
			}
#if DEBUG
			config.PropagateExceptions();
			config.ValidateExamples();
#endif
			config.SetApplicationName("resume builder");
			config.SetApplicationVersion("1.0.0");
			config.CaseSensitivity(CaseSensitivity.None);

			config.AddBranch<AddSetting>("add", add =>
			{
				add.SetDescription("add new information to job database/bank");
				add.AddCommand<AddJobCommand>("job")
					.WithDescription("add a new job")
					.WithExample("add", "job", "-s", "2022-01-01", "-e", "2026-11-01", "-t", "foreman");
			});
			config.AddCommand<InitCommand>("init")
				.WithDescription("initializes resume database")
				.WithAlias("start");
		};
	}
}