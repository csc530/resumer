using resume_builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestResumeBuilder
{
	internal class InitCommandTest : AppTest
	{
		[Test]
		public void Init_ShouldPass()
		{
			ExitCode exitcode = (ExitCode)cliapp.Run(args: new[] { "init" });
			Assert.That(exitcode, Is.EqualTo(ExitCode.Success));
		}
	}
}
