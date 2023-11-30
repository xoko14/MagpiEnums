namespace MagpiEnums;

public class EnumerationAttribute: Attribute
{
    private string _type;

    public EnumerationAttribute(string type = "type")
    {
        _type = type;
    }
}