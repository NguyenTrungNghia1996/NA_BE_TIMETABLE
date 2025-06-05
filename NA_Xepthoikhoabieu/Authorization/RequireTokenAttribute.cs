using System;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class RequireTokenAttribute : Attribute
{
    /// <summary>
    /// Tên quyền cần kiểm tra (theo Enum), ví dụ "Reports", "Users", v.v...
    /// </summary>
    public string? Policy { get; set; }

    /// <summary>
    /// Constructor mặc định: chỉ yêu cầu token, không kiểm tra quyền.
    /// </summary>
    public RequireTokenAttribute() { }

    /// <summary>
    /// Constructor có tên quyền: yêu cầu token và kiểm tra quyền.
    /// </summary>
    /// <param name="policy">Tên quyền theo Enum MenuPermission</param>
    public RequireTokenAttribute(string policy)
    {
        Policy = policy;
    }
}
