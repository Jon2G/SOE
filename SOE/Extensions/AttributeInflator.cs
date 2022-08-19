using System;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace FirestoreLINQ.Internals
{
    internal static class AttributeInflator
    {
        internal static FieldInfo GetField(TypeInfo t, string name)
        {
            FieldInfo? f = t.GetDeclaredField(name);
            if (f != null)
                return f;
            return GetField(t.BaseType.GetTypeInfo(), name);
        }
        internal static PropertyInfo GetProperty(TypeInfo t, string name)
        {
            PropertyInfo? f = t.GetDeclaredProperty(name);
            if (f != null)
                return f;
            return GetProperty(t.BaseType.GetTypeInfo(), name);
        }
        internal static object InflateAttribute(this CustomAttributeData x)
        {
            Type? atype = x.AttributeType;
            TypeInfo? typeInfo = atype.GetTypeInfo();
#if ENABLE_IL2CPP
			var r = Activator.CreateInstance (x.AttributeType);
#else
            object[]? args = x.ConstructorArguments.Select(a => a.Value).ToArray();
            object? r = Activator.CreateInstance(x.AttributeType, args);
            foreach (CustomAttributeNamedArgument arg in x.NamedArguments)
            {
                if (arg.IsField)
                {
                    GetField(typeInfo, arg.MemberName).SetValue(r, arg.TypedValue.Value);
                }
                else
                {
                    GetProperty(typeInfo, arg.MemberName).SetValue(r, arg.TypedValue.Value);
                }
            }
#endif
            return r;
        }
    }
}
