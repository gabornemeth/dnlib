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
        private static Dictionary<string, string> typeMap = new Dictionary<string, string>();
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
            if (typeMap.Count == 0)
            {
                // Windows Runtime types that map to .NET Framework types with a different name and/or namespace.
                typeMap.Add("Windows.Foundation.Metadata.AttributeUsageAttribute", "System.AttributeUsageAttribute");
                typeMap.Add("Windows.Foundation.Metadata.AttributeTargets", "System.AttributeTargets");
                typeMap.Add("Windows.Foundation.DateTime", "System.DateTimeOffset");
                typeMap.Add("Windows.Foundation.EventHandler", "System.EventHandler");
                typeMap.Add("Windows.Foundation.EventRegistrationToken", "System.Runtime.InteropServices.WindowsRuntime.EventRegistrationToken");
                typeMap.Add("Windows.Foundation.HResult", "System.Exception");
                typeMap.Add("Windows.Foundation.IReference", "System.Nullable");
                typeMap.Add("Windows.Foundation.TimeSpan", "System.TimeSpan");
                typeMap.Add("Windows.Foundation.Uri", "System.Uri");
                typeMap.Add("Windows.Foundation.IClosable", "System.IDisposable");
                typeMap.Add("Windows.Foundation.Collections.IIterable", "System.Collections.Generic.IEnumerable");
                typeMap.Add("Windows.Foundation.Collections.IVector", "System.Collections.Generic.IList");
                typeMap.Add("Windows.Foundation.Collections.IVectorView", "System.Collections.Generic.IReadOnlyList");
                typeMap.Add("Windows.Foundation.Collections.IMap", "System.Collections.Generic.IDictionary");
                typeMap.Add("Windows.Foundation.Collections.IMapView", "System.Collections.Generic.IReadOnlyDictionary");
                typeMap.Add("Windows.Foundation.Collections.IKeyValuePair", "System.Collections.Generic.KeyValuePair");
                typeMap.Add("Windows.UI.Xaml.Interop.IBindableIterable", "System.Collections.IEnumerable");
                typeMap.Add("Windows.UI.Xaml.Interop.IBindableVector", "System.Collections.IList");
                typeMap.Add("Windows.UI.Xaml.Interop.INotifyCollectionChanged", "System.Collections.Specialized.INotifyCollectionChanged");
                typeMap.Add("Windows.UI.Xaml.Interop.NotifyCollectionChangedEventHandler", "System.Collections.Specialized.NotifyCollectionChangedEventHandler");
                typeMap.Add("Windows.UI.Xaml.Interop.NotifyCollectionChangedEventArgs", "System.Collections.Specialized.NotifyCollectionChangedEventArgs");
                typeMap.Add("Windows.UI.Xaml.Interop.NotifyCollectionChangedAction", "System.Collections.Specialized.NotifyCollectionChangedAction");
                typeMap.Add("Windows.UI.Xaml.Data.INotifyPropertyChanged", "System.ComponentModel.INotifyPropertyChanged");
                typeMap.Add("Windows.UI.Xaml.Data.PropertyChangedEventHandler", "System.ComponentModel.PropertyChangedEventHandler");
                typeMap.Add("Windows.UI.Xaml.Data.PropertyChangedEventArgs", "System.ComponentModel.PropertyChangedEventArgs");
                typeMap.Add("Windows.UI.Xaml.Interop.TypeName", "System.Type");

                // Windows Runtime types that map to .NET Framework types with the same name and namespace. (.NET Framework uses reference assembly)
                typeMap.Add("Windows.UI.Color", "Windows.UI.Color");
                typeMap.Add("Windows.Foundation.Point", "Windows.Foundation.Point");
                typeMap.Add("Windows.Foundation.Rect", "Windows.Foundation.Rect");
                typeMap.Add("Windows.Foundation.Size", "Windows.Foundation.Size");
                typeMap.Add("Windows.UI.Xaml.Input.ICommand", "Windows.UI.Xaml.Input.ICommand");
                typeMap.Add("Windows.UI.Xaml.CornerRadius", "Windows.UI.Xaml.CornerRadius");
                typeMap.Add("Windows.UI.Xaml.Duration", "Windows.UI.Xaml.Duration");
                typeMap.Add("Windows.UI.Xaml.DurationType", "Windows.UI.Xaml.DurationType");
                typeMap.Add("Windows.UI.Xaml.GridLength", "Windows.UI.Xaml.GridLength");
                typeMap.Add("Windows.UI.Xaml.GridUnitType", "Windows.UI.Xaml.GridUnitType");
                typeMap.Add("Windows.UI.Xaml.Thickness", "Windows.UI.Xaml.Thickness");
                typeMap.Add("Windows.UI.Xaml.Controls.Primitives.GeneratorPosition", "Windows.UI.Xaml.Controls.Primitives.GeneratorPosition");
                typeMap.Add("Windows.UI.Xaml.Media.Matrix", "Windows.UI.Xaml.Media.Matrix");
                typeMap.Add("Windows.UI.Xaml.Media.Animation.KeyTime", "Windows.UI.Xaml.Media.Animation.KeyTime");
                typeMap.Add("Windows.UI.Xaml.Media.Animation.RepeatBehavior", "Windows.UI.Xaml.Media.Animation.RepeatBehavior");
                typeMap.Add("Windows.UI.Xaml.Media.Animation.RepeatBehaviorType", "Windows.UI.Xaml.Media.Animation.RepeatBehaviorType");
                typeMap.Add("Windows.UI.Xaml.Media.Media3D.Matrix3D", "Windows.UI.Xaml.Media.Media3D.Matrix3D");
            }

            var aName = GetWinRTTypeName(a.ReflectionFullName);
            var bName = GetWinRTTypeName(b.ReflectionFullName);

            if (typeMap.ContainsKey(aName)) // a is WinRT type
                return bName.Equals(typeMap[aName]);
            else if (typeMap.ContainsKey(bName)) // b is WinRT type
                return aName.Equals(typeMap[bName]);

            return false; // could not map between WinRT and .NET
        }

    }
}
