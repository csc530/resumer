using Bogus;
using WaffleGenerator;

namespace TestResumer.data;

public class TestData
{
    private protected const int TestRepetition = 10;

    public const string Space = " ";
    public const string Tab = "\t";
    public const string Newline = "\n";
    public const string ZeroWidthSpace = "\u200b";
    public const string ZeroWidthJoiner = "\u200d";
    public const string ZeroWidthNonJoiner = "\u200c";
    public const string ZeroWidthNoBreakSpace = "\u200b";
    public const string ZeroWidthHairSpace = "\u200a";
    public const string SixPerEmSpace = "\u2006";
    public const string ThinSpace = "\u2009";
    public const string PunctuationSpace = "\u2008";
    public const string FourPerEmSpace = "\u2005";
    public const string ThreePerEmSpace = "\u2004";
    public const string FigureSpace = "\u2007";
    public const string EnSpace = "\u2002";
    public const string EmSpace = "\u2003";

    public const string BraillePatternBlank = "\u2800";


    private static Random Random { get; } = new();
    protected static Faker Faker { get; } = new();

    private protected static int MaxRandomYearsAfterToday()
    {
        DateOnly.MaxValue.Deconstruct(out var maxYear, out _, out _);
        DateOnly.FromDateTime(DateTime.Today).Deconstruct(out var todayYear, out _, out _);
        return maxYear - todayYear;
    }

    private protected static int MaxRandomYearsBeforeToday()
    {
        DateOnly.MinValue.Deconstruct(out var minYear, out _, out _);
        DateOnly.FromDateTime(DateTime.Today).Deconstruct(out var todayYear, out _, out _);
        return todayYear - minYear;
    }

    public static DateOnly RandomPastDate() => Faker.Date.PastDateOnly(MaxRandomYearsBeforeToday());
    public static DateOnly RandomFutureDate() => Faker.Date.FutureDateOnly(MaxRandomYearsAfterToday());
    public static DateOnly RandomDate() => Faker.Date.BetweenDateOnly(RandomPastDate(), RandomFutureDate());
    public static string? RandomTextOrNull() => Faker.Lorem.Paragraph().OrNull(Faker);

    public static List<string>? RandomListOfTextOrNull() => Enumerable.Range(1, 7)
                       .Select(_ => Faker.Random.Word())
                       .ToList().OrNull(Faker);

    public static string Waffle() => WaffleEngine.Text(Random.Next(TestRepetition), Faker.Random.Bool());
    public static string? WaffleOrNull() => Waffle().OrNull(Faker);

    //#region white spaces
    public static IEnumerable<string> RandomWhiteSpaceString(int count = TestRepetition)
    {
        yield return Faker.Random.String2(Random.Next(count), Space);
        yield return Faker.Random.String2(Random.Next(count), Tab);
        yield return Faker.Random.String2(Random.Next(count), Newline);
        yield return Faker.Random.String2(Random.Next(count), ZeroWidthSpace);
        yield return Faker.Random.String2(Random.Next(count), ZeroWidthJoiner);
        yield return Faker.Random.String2(Random.Next(count), ZeroWidthNonJoiner);
        yield return Faker.Random.String2(Random.Next(count), ZeroWidthNoBreakSpace);
        yield return Faker.Random.String2(Random.Next(count), ZeroWidthHairSpace);
        yield return Faker.Random.String2(Random.Next(count), SixPerEmSpace);
        yield return Faker.Random.String2(Random.Next(count), ThinSpace);
        yield return Faker.Random.String2(Random.Next(count), PunctuationSpace);
        yield return Faker.Random.String2(Random.Next(count), FourPerEmSpace);
        yield return Faker.Random.String2(Random.Next(count), ThreePerEmSpace);
        yield return Faker.Random.String2(Random.Next(count), FigureSpace);
        yield return Faker.Random.String2(Random.Next(count), EnSpace);
        yield return Faker.Random.String2(Random.Next(count), EmSpace);
        yield return Faker.Random.String2(Random.Next(count), BraillePatternBlank);
    }
    //#endregion
}