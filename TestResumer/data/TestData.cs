using Bogus;
using WaffleGenerator;

namespace TestResumer.data;

public class TestData
{
    private protected const int TestRepetition = 10;

    public const string space = " ";
    public const string tab = "\t";
    public const string newline = "\n";
    public const string zeroWidthSpace = "\u200b";
    public const string zeroWidthJoiner = "\u200d";
    public const string zeroWidthNonJoiner = "\u200c";
    public const string zeroWidthNoBreakSpace = "\u200b";
    public const string zeroWidthHairSpace = "\u200a";
    public const string sixPerEmSpace = "\u2006";
    public const string thinSpace = "\u2009";
    public const string punctuationSpace = "\u2008";
    public const string fourPerEmSpace = "\u2005";
    public const string threePerEmSpace = "\u2004";
    public const string figureSpace = "\u2007";
    public const string enSpace = "\u2002";
    public const string emSpace = "\u2003";

    public const string braillePatternBlank = "\u2800";


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

    public static DateOnly RandomPastDate() => Faker.Date.PastDateOnly(MaxRandomYearsBeforeToday());
    public static DateOnly RandomFutureDate() => Faker.Date.FutureDateOnly(MaxRandomYearsAfterToday());
    public static DateOnly RandomDate() => Faker.Date.BetweenDateOnly(RandomPastDate(), RandomFutureDate());
    public static string? RandomTextOrNull() => Faker.Lorem.Paragraph().OrNull(Faker);

    public static string Waffle() => WaffleEngine.Text(Random.Next(TestRepetition), Faker.Random.Bool());
    public static string? WaffleOrNull() => Waffle().OrNull(Faker);

    //#region white spaces
    public static IEnumerable<string> RandomWhiteSpaceString(int count = TestRepetition)
    {
        yield return Faker.Random.String2(Random.Next(count), space);
        yield return Faker.Random.String2(Random.Next(count), tab);
        yield return Faker.Random.String2(Random.Next(count), newline);
        yield return Faker.Random.String2(Random.Next(count), zeroWidthSpace);
        yield return Faker.Random.String2(Random.Next(count), zeroWidthJoiner);
        yield return Faker.Random.String2(Random.Next(count), zeroWidthNonJoiner);
        yield return Faker.Random.String2(Random.Next(count), zeroWidthNoBreakSpace);
        yield return Faker.Random.String2(Random.Next(count), zeroWidthHairSpace);
        yield return Faker.Random.String2(Random.Next(count), sixPerEmSpace);
        yield return Faker.Random.String2(Random.Next(count), thinSpace);
        yield return Faker.Random.String2(Random.Next(count), punctuationSpace);
        yield return Faker.Random.String2(Random.Next(count), fourPerEmSpace);
        yield return Faker.Random.String2(Random.Next(count), threePerEmSpace);
        yield return Faker.Random.String2(Random.Next(count), figureSpace);
        yield return Faker.Random.String2(Random.Next(count), enSpace);
        yield return Faker.Random.String2(Random.Next(count), emSpace);
        yield return Faker.Random.String2(Random.Next(count), braillePatternBlank);
    }
    //#endregion
}