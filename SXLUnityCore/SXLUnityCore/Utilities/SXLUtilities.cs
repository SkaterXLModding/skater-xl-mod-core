using System;


public static class SXLUtilities
{
    public static bool DoesAssemblyExist(string assemblyName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.FullName.StartsWith(assemblyName))
            {
                return true;
            }
        }
        return false;
    }

}
