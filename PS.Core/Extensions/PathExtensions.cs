using System;
using System.IO;

namespace PS.Extensions
{
    public static class PathExtensions
    {
        #region Static members

        public static string NormalizePath(this Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (!uri.IsAbsoluteUri)
            {
                var currentDirectory = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
                uri = new Uri(new Uri(currentDirectory), uri);
            }
            //uri.IsFile
            var path = uri.LocalPath;
            return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        #endregion
    }
}