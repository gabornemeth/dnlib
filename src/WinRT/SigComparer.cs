using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace dnlib.WinRT
{
    /// <summary>
    /// Compares WinRT signatures with .NET signatures.
    /// </summary>
    internal struct SigComparer
    {
        private static Dictionary<string, string> _typeMap = new Dictionary<string, string>();
        private const string TypeNamePrefix = "<CLR>";

        private string GetWinRTTypeName(string name)
        {
            int index = name.IndexOf('`'); // remove generic parameter (dangerous, <T> can be different)
            var justName = index > 0 ? name.Substring(0, index) : name;
            return justName;
        }

        public bool Equals_TypeNames(UTF8String a, UTF8String b)
        {
            if (a.StartsWith(TypeNamePrefix))
                return a.Substring(TypeNamePrefix.Length).Equals(b);
            else if (b.StartsWith(TypeNamePrefix))
                return b.Substring(TypeNamePrefix.Length).Equals(a);

            return false;
        }

        public bool Equals(TypeSig a, TypeSig b)
        {
            // TODO: add more robust generic support!
            if (_typeMap.Count == 0)
            {
                _typeMap.Add("Windows.Foundation.Uri", "System.Uri");
                _typeMap.Add("Windows.Foundation.HResult", "System.Exception");
                _typeMap.Add("Windows.UI.Xaml.Interop.TypeName", "System.Type");
                _typeMap.Add("Windows.Foundation.Collections.IIterable", "System.Collections.Generic.IEnumerable");
            }

            var aName = GetWinRTTypeName(a.ReflectionFullName);
            var bName = GetWinRTTypeName(b.ReflectionFullName);
            if (_typeMap.ContainsKey(aName)) // a is WinRT type
                return bName.Equals(_typeMap[aName]);
            else if (_typeMap.ContainsKey(bName)) // b is WinRT type
                return aName.Equals(_typeMap[bName]);

            return false; // could not map between WinRT and .NET
        }

    }
}
