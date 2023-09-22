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
		//[Test]
		//todo:  find way to pass text to test command app for prompts
		public void Init_ShouldPass()
		{
			Spectre.Console.Testing.CommandAppResult commandAppResult = TestApp.Run("init");
			Assert.That(commandAppResult.ExitCode, Is.EqualTo(ExitCode.Success.ToInt()));
		}
	}
}