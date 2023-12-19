using AutoBogus;
using AutoBogus.Conventions;
using Bogus;
using resume_builder.models;

namespace TestResumeBuilder.data;

internal class ProfileTestData: TestData
{
    static public Faker<Profile> BogusProfile = new AutoFaker<Profile>()
                                               .Configure(config => config.WithConventions(conv =>
                                                    conv.Email.Aliases("EmailAddress", "emailAddress")))
                                               .RuleFor(profile => profile.MiddleName,
                                                    Faker.Random.Word().OrNull(Faker))
                                                //.RuleFor(profile => profile.EmailAddress, (_, profile) => Faker.Internet.Email(profile.FirstName, profile.LastName))
                                               .RuleFor(profile => profile.Summary,
                                                    Faker.Lorem.Paragraph().OrNull(Faker));

    static public IEnumerable<Profile> InfiniteFakeProfiles => BogusProfile.GenerateForever();
    static public List<Profile> GetFakeProfiles(int count = TestRepetitions) => BogusProfile.Generate(count);
    static public Profile GetFakeProfile() => BogusProfile.Generate();
}