// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using MagpiEnums;
using MagpiEnums.Example;

[Enumeration]
public abstract partial record TestEnumeration()
{
    
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