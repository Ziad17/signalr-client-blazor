using System.Reflection;
using System;
using System.Linq;

namespace SignalRClient.Extensions
{
    public static class TypeExtensions
    {
        public static MethodInfo GetMethodsBySignature(this Type type, Type returnType, params Type[] parameterTypes)
        {
            return type.GetMethods().FirstOrDefault((m) =>
            {
                if (m.ReturnType != returnType) return false;
                var parameters = m.GetParameters();
                if ((parameterTypes == null || parameterTypes.Length == 0))
                    return parameters.Length == 0;
                if (parameters.Length != parameterTypes.Length)
                    return false;
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    if (parameters[i].ParameterType != parameterTypes[i])
                        return false;
                }
                return true;
            });
        }
    }
}
