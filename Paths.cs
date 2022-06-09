using System;
using System.IO;

namespace StudioManette
{
    namespace Utils
    {
        public class Path
        {
            public static string SanitiseFilename(string input)
            {
                return String.Join("_", input.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
            }

            public static string GetRelativePath(string fromPath, string toPath)
            {
                // Yay, yet another shoutout to https://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
                if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
                if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

                Uri fromUri = new Uri(Path.GetFullPath(fromPath));
                Uri toUri = new Uri(Path.GetFullPath(toPath));

                if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

                Uri relativeUri = fromUri.MakeRelativeUri(toUri);
                string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

                if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
                {
                    relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                }

                return relativePath;
            }
        }
    }
}
