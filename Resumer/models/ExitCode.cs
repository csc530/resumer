namespace Resumer.models;

public enum ExitCode
{
    /// <summary>
    /// unknown error
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// Generic error
    /// </summary>
    Error = 1,

    /// <summary>
    /// all is dandy
    /// </summary>
    Success = 0,

    /// <summary>
    /// User canceled operation
    /// </summary>
    Canceled = 2,

    /// <summary>
    /// Invalid command argument
    /// </summary>
    InvalidArgument = 8,

    /// <summary>
    /// Missing command argument
    /// </summary>
    MissingArgument = 4,

    /// <summary>
    /// Invalid command option
    /// </summary>
    InvalidOption = 5,

    /// <summary>
    /// Missing command option
    /// </summary>
    MissingOption = 6,
    TooManyArguments = 7,
    Fail = 3,
    NotFound = 404,

    /// <summary>
    /// Database error; usually a SqliteException
    /// </summary>
    DbError,

    /// <summary>
    /// No or insufficient data found within the application
    /// </summary>
    NoData
}