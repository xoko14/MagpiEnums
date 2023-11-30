// See https://aka.ms/new-console-template for more information
namespace MagpiEnums.Example;
using System.Text.Json;
using MagpiEnums;

[Enumeration]
public abstract partial record LoadStatus
{
    public record Loading(string message) : LoadStatus;
    public record Loaded(Content Content) : LoadStatus;
    public record Error(string message) : LoadStatus;
}

public class Content
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public static partial class Program
{
    public static void Main()
    {
        LoadStatus state = new LoadStatus.Loading("Loading...");
        Console.WriteLine(JsonSerializer.Serialize(state));

        state = new LoadStatus.Loaded(new()
        {
            Title = "Content",
            Description = "This is some loaded content."
        });
        Console.WriteLine(JsonSerializer.Serialize(state));
    }
}