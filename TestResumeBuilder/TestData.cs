using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace TestResumeBuilder;
internal class TestData
{
    public static IEnumerable<string[]> RandomArgs() {
        yield return new Faker().Random.WordsArray(1, 20).ToArray();
        yield return new Faker().Random.WordsArray(1, 20).ToArray();
        yield return new Faker().Random.WordsArray(1, 20).ToArray();
}
    public static string r => "";
}
