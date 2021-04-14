
namespace SwiftCode.Core.Utilities
{
    using System;

    public static class TypeValidator
    {
        public static bool IsTypeOf(Type propType, string namepace)
        {
            // ? Using reflection to identify, the type is a ""user-defined"
            // ! However, there is no a "safe" way to check for user-defined types and
            // ! used a project namespace to identify my types
            // ? More info -> https://social.msdn.microsoft.com/Forums/en-US/0adf2482-3e99-4927-9fb1-8a8c8990467f/using-reflection-how-to-identify-the-type-whether-it-is-user-defiend-type-or-not?forum=csharplanguage
            return (!propType.IsValueType) && !(propType.IsPrimitive || propType.IsEnum) && !(string.IsNullOrWhiteSpace(propType.Namespace)) && (propType.Namespace.StartsWith(namepace));
        }
    }
}
