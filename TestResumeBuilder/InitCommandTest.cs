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
		//todo: currently hangs becuase of the ansi prompt
		//need to find way to pass text to test command app
		public void Init_ShouldPass()
		{
			
			Spectre.Console.Testing.CommandAppResult commandAppResult = TestApp.Run("init");
			Assert.That(commandAppResult.ExitCode, Is.EqualTo(ExitCode.Success.ToInt()));
		}
	}
}