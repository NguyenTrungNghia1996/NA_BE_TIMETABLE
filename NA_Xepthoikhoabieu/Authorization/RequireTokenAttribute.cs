using System;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class RequireTokenAttribute : Attribute
{
    // Có thể thêm các thông số nếu cần
}