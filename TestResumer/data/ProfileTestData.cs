using AutoBogus;
using AutoBogus.Conventions;
using Bogus;
using Resumer.models;

namespace TestResumer.data;

internal class ProfileTestData : TestData
{
    public static Faker<Profile> BogusProfile = new AutoFaker<Profile>()
                                               .Configure(config => config.WithConventions(conv => conv.Email.Aliases("EmailAddress", "emailAddress")))
                                               .RuleFor(profile => profile.MiddleName, Faker.Name.FirstName().OrNull(Faker)??Faker.Name.LastName().OrNull(Faker))
                                               //.RuleFor(profile => profile.EmailAddress, (_, profile) => Faker.Internet.Email(profile.FirstName, profile.LastName))
                                               .RuleFor(profile => profile.Objective, Faker.Lorem.Paragraph().OrNull(Faker));

    public static IEnumerable<Profile> InfiniteFakeProfiles => BogusProfile.GenerateForever();
    public static List<Profile> GetFakeProfiles(int count = TestRepetition) => BogusProfile.Generate(count);
    public static Profile GetFakeProfile() => BogusProfile.Generate();
}