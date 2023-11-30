# MagpiEnums

 Nesting enums (rust-like) (de)serialization made easy in C#.
 
```csharp
[Enumeration]
public abstract partial record Status{
    public record Loading(string message): Status;
    public record Loaded(Content content): Status;
    public record Error(Error error): Status;
}
```