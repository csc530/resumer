using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.delete;

public class DeleteTypstTemplateCommand: Command<DeleteCommandSettings>
{
    public override int Execute(CommandContext context, DeleteCommandSettings settings)
    {
        var db = new ResumeContext();
        var typstTemplates = db.Templates;
        if(typstTemplates.Count() <= 1)
            return CommandOutput.Success("No templates found");

        var selectedTypstTemplates = AnsiConsole.Prompt(new MultiSelectionPrompt<TypstTemplate>()
            .Title("Select templates to delete")
            .PageSize(10)
            .AddChoices(typstTemplates.OrderBy(template=>template.Name)));

        var prompt = selectedTypstTemplates.Count == 1
            ? $"Are you sure you want to delete this typst template - {selectedTypstTemplates[0].Name}?"
            : $"Are you sure you want to delete these {selectedTypstTemplates.Count} templates?";
        if(!settings.NoConfirm && !AnsiConsole.Confirm(prompt, false))
            return CommandOutput.Error(ExitCode.Canceled);
        typstTemplates.RemoveRange(selectedTypstTemplates);
        var deleted = db.SaveChanges();
        return CommandOutput.Success(deleted == 1 ? "template deleted" : $"{deleted} typst templates deleted");
    }
}