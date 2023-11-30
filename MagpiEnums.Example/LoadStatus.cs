namespace MagpiEnums.Example
{

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
}
