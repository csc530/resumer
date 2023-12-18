using Bogus;
using WaffleGenerator;

namespace TestResumeBuilder;

internal class TestData
{
    private protected const int TestRepetitions = 10;


    private static Random Random { get; set; } = new();
    protected static Faker Faker { get; set; } = new();

    static private protected int MaxRandomYearsAfterToday()
    {
        DateOnly.MaxValue.Deconstruct(out var maxYear, out _, out _);
        DateOnly.FromDateTime(DateTime.Today).Deconstruct(out var todayYear, out _, out _);
        return maxYear - todayYear;
    }

    static private protected int MaxRandomYearsBeforeToday()
    {
        DateOnly.MinValue.Deconstruct(out var minYear, out _, out _);
        DateOnly.FromDateTime(DateTime.Today).Deconstruct(out var todayYear, out _, out _);
        return todayYear - minYear;
    }

    public static TheoryData<object[]> RandomArgs() => [Faker.Random.WordsArray(2)];


    public static DateOnly RandomPastDate() => Faker.Date.PastDateOnly(MaxRandomYearsBeforeToday());
    public static DateOnly RandomFutureDate() => Faker.Date.FutureDateOnly(MaxRandomYearsAfterToday());
    public static DateOnly RandomDate() => Faker.Date.BetweenDateOnly(RandomPastDate(), RandomFutureDate());
    public static string? RandomTextOrNull() => Faker.Lorem.Paragraph().OrNull(Faker);

    public static string Waffle() => WaffleEngine.Text(Random.Next(TestRepetitions), Faker.Random.Bool());
    public static string? WaffleOrNull() => Waffle().OrNull(Faker);
}