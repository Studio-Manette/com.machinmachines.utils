using System;
using UnityEngine;

namespace StudioManette
{
    namespace Utils
    {
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
        public class IsFilepathAttribute : PropertyAttribute
        {
            public enum FileType
            {
                File,
                Directory
            }

            public readonly bool IsReadOnly;
            public readonly string ExtensionMatch;
            public readonly FileType Filetype;

            public IsFilepathAttribute(bool isReadOnly, string extensionMatch = "", FileType filetype = FileType.File)
            {
                IsReadOnly = isReadOnly;
                ExtensionMatch = extensionMatch;
                Filetype = filetype;
            }
        }
    }
}
