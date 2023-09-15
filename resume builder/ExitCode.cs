namespace resume_builder;

public enum ExitCode
{
    Unknown = -1,
    Error = 1,
    Success = 0,
    Canceled = 2,
    InvalidArgument = 3,
    MissingArgument = 4,
    InvalidOption = 5,
    MissingOption = 6,
    TooManyArguments = 7,

}