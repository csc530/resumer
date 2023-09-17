using Spectre.Console.Testing;
using System.ComponentModel;

namespace TestResumeBuilder
{
	public class AddJobTest : AppTest
	{
		[Test]
		public void WithNoArguments_ShouldFail()
		{
			Assert.Catch(() => cliapp.Run(new[] { "add", "job" }), "Expected to fail");
		}

		public static object[][] GetRequiredArgs()
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
			Assert.Catch(() => cliapp.Run(args), "Expected to fail");
		}
		private static DateOnly[] Dates => new[] { DateOnly.MinValue, DateOnly.MaxValue, DateOnly.FromDateTime(DateTime.Now) };
		[TestCaseSource(nameof(Dates))]
		public void WithoutJobTitle_ShouldFail(DateOnly startDate)
		{
			var args = new string[] { "add", "job", "--start", startDate.ToString() };
			Assert.Catch(() => cliapp.Run(args), "Expected to fail");
		}
		[TestCaseSource(nameof(GetRequiredArgs))]
		public void WithMinimumArgs_ShouldPass(string title, DateOnly startDate)
		{
			var args = new string[] { "add", "job", "--title", title, "--start", startDate.ToString() };
			int exitcode = cliapp.Run(args);
			Assume.That(title.Length < 100 && title.Length > 0);
			Assert.Multiple(() =>
			{
				Assert.That(exitcode, Is.EqualTo(0));
				Assert.That(console.Output, Contains.Value(title));
			});
		}
	}


}