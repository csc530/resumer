using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace TestResumeBuilder;
internal class TestData
{
    static private Faker Faker { get; set; } = new();
    public static TheoryData<string[]> RandomArgs() => [Faker.Random.WordsArray(2)];
}
