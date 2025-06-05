using System;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class RequireTokenAttribute : Attribute
{
    public string? Policy { get; set; }
    public RequireTokenAttribute() { }
    public RequireTokenAttribute(string policy)
    {
        Policy = policy;
    }
}
