using resume_builder;
using Spectre.Console.Testing;
using System.ComponentModel;
using resume_builder.cli.commands.add;

namespace TestResumeBuilder
{
	public class AddJobTest : AppTest
	{
		[Test]
		public void WithNoArguments_ShouldFail()
		{
			var res = TestApp.Run(Array.Empty<string>());
			Assert.That(res.ExitCode, Is.Not.EqualTo(ExitCode.Success.ToInt()));
		}

		private static object[][] GetRequiredArgs()
		{
			var arr = Array.Empty<object[]>();
			for(int i = 0; i < JobTitles.Length; i++)
				arr = arr.Append(new object[] { JobTitles[i], Dates[i % Dates.Length] }).ToArray();
			return arr;
		}

		private static string[] JobTitles { get; } = { "developer", "student", "lead executive office manager", "clothing cashier" };
		[Test]
		[TestCaseSource(nameof(JobTitles))]
		public void WithoutStartDate_ShouldFail(string title)
		{
			var args = new string[] { "add", "job", "--title", title };
			Assert.Catch(() => TestApp.Run(args));
		}
		private static DateOnly[] Dates => new[] { DateOnly.MinValue, DateOnly.MaxValue, DateOnly.FromDateTime(DateTime.Now) };
		[TestCaseSource(nameof(Dates))]
		public void WithoutJobTitle_ShouldFail(DateOnly startDate)
		{
			var args = new string[] { "add", "job", "--start", startDate.ToString() }; Assert.Catch(() => TestApp.Run(args));
		}
		[TestCaseSource(nameof(GetRequiredArgs))]
		public void WithMinimumArgs_ShouldPass(string title, DateOnly startDate)
		{
			var args = new string[] { "add", "job", "--title", title, "--start", startDate.ToString() };
			Assume.That(title.Length < 100 && title.Length > 0);
			var result = TestApp.Run(args);
			Assert.Multiple(() =>
			{
				Assert.That(result.ExitCode, Is.EqualTo(ExitCode.Success.ToInt()));
				Assert.That(result.Settings, Is.InstanceOf<AddJobSettings>());
			});
		}
	}


}