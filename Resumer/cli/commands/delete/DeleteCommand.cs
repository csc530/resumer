using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.delete;

public abstract class
    DeleteCommand<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>: Command<DeleteCommandSettings>
    where T : class
{
    private string TypeName { get; } = typeof(T).Name.ToLower();
    protected ResumeContext Db { get; set; } = new();
    protected abstract DbSet<T> DbSet { get; }

    public sealed override int Execute(CommandContext context, DeleteCommandSettings settings)
    {
        if(!DbSet.Any())
            return CommandOutput.Error(ExitCode.Canceled, $"No {TypeName} found");

        var confirmDelete = !settings.NoConfirm;
        string prompt;
        List<T> selected;

        if(settings.DeleteAll)
        {
            prompt = $"Are you sure you want to delete all {DbSet.Count()} {TypeName}?";
            if(confirmDelete)
            {
                AnsiConsole.WriteLine(DbSet.Print());
                if(!AnsiConsole.Confirm(prompt, false))
                    return CommandOutput.Error(ExitCode.Canceled);
            }

            selected = DbSet.ToList();
        }
        else
        {
            selected = AnsiConsole.Prompt(
                new MultiSelectionPrompt<T>()
                    .Title($"Select {TypeName}s to delete")
                    .PageSize(10).AddChoices(DbSet)
            );
            prompt = selected.Count == 1
                ? $"Are you sure you want to delete this {TypeName} - {selected[0]}?"
                : $"Are you sure you want to delete these {selected.Count} {TypeName}s?";
            if(confirmDelete && !AnsiConsole.Confirm(prompt, false))
                return CommandOutput.Error(ExitCode.Canceled);
        }

        DbSet.RemoveRange(selected);
        var deleted = Db.SaveChanges();
        return CommandOutput.Success(deleted == 1 ? $"1 {TypeName} deleted." : $"{deleted} {TypeName} deleted.");
    }
}

public class DeleteCommandSettings: CommandSettings
{
    [CommandOption("-n|--no-confirm")]
    [Description("Do not ask for confirmation before deleting")]
    public bool NoConfirm { get; set; }

    [CommandOption("-a|--all")]
    [Description("Delete all entries")]
    public bool DeleteAll { get; set; }
}