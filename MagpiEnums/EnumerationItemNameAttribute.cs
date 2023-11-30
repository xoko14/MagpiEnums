namespace MagpiEnums;

public class EnumerationItemNameAttribute: Attribute
{
    private string _name;

    public EnumerationItemNameAttribute(string name)
    {
        _name = name;
    }
}