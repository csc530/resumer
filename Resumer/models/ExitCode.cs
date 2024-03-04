namespace Resumer.models;

public enum ExitCode
{
	Unknown = -1,
	Error = 1,
	Success = 0,
	Canceled = 2,
	InvalidArgument = 8,
	MissingArgument = 4,
	InvalidOption = 5,
	MissingOption = 6,
	TooManyArguments = 7,
	Fail = 3,
	NotFound = 404,
	DbError
}