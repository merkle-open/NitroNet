using System;
using System.Collections.Generic;
using System.Linq;

namespace NitroNet.ViewEngine.IO
{
    public class PathInfo : IEquatable<PathInfo>
    {
        public readonly static PathInfo Empty = new PathInfo(string.Empty);

        private IEnumerable<string> _enumerator;
        private string _unverifiedPath;
        private string _normalizedPath;
        private string[] _parts;

        private PathInfo(string unverifiedPath)
        {
            _unverifiedPath = unverifiedPath;
        }

        private PathInfo(string[] parts)
        {
            _parts = parts;
        }

        private PathInfo(IEnumerable<string> enumerator)
        {
            _enumerator = enumerator;
        }

        public PathInfo DirectoryName
        {
            get { return new PathInfo(GetEnumerateReverse().Skip(1)); }
        }

        public static PathInfo Create(string path)
        {
            return new PathInfo(path);
        }

        public override string ToString()
        {
            return VerifiedPath();
        }

        private string VerifiedPath()
        {
            if (_normalizedPath == null)
            {
                var parts = GetParts();
                _normalizedPath = string.Join("/", parts);
                _unverifiedPath = null;
            }

            return _normalizedPath;
        }

        private string[] GetParts()
        {
            if (_parts == null)
                _parts = VerifyRoot(Normalize(GetEnumerateReverse()).Reverse()).ToArray();
            
            return _parts;
        }

        private IEnumerable<string> VerifyRoot(IEnumerable<string> reverse)
        {
            bool isFirstRun = true;
            foreach (var val in reverse)
            {
                if (!isFirstRun && string.IsNullOrEmpty(val))
                    throw new ArgumentException("Can not combine root paths");

                isFirstRun = false;
                yield return val;
            }
        }

        private static IEnumerable<string> Normalize(IEnumerable<string> strings)
        {
            int skip = 0;
            foreach (var val in strings)
            {
                if ("..".Equals(val, StringComparison.Ordinal))
                    skip++;
                else if (".".Equals(val, StringComparison.Ordinal))
                    continue;
                else if (skip > 0)
                    skip--;
                else
                    yield return val;
            }

            for (int i = 0; i < skip; i++)
            {
                yield return "..";
            }
        }

        private IEnumerable<string> GetEnumerateReverse()
        {
            if (_enumerator == null)
                _enumerator = EnumerateReverse();

            if (_parts != null)
                return _parts.Reverse();

            return _enumerator;
        }

        private IEnumerable<string> EnumerateReverse()
        {
            var path = _normalizedPath ?? _unverifiedPath;

            return EnumerateReverse(path, 0);
        }

        private static IEnumerable<string> EnumerateReverse(string path, int skip)
        {
            if (string.IsNullOrEmpty(path))
                yield break;

            var strings = path.Split('\\', '/');
            for (int i = strings.Length - 1; i >= skip; i--)
            {
                var val = strings[i];
                if (string.IsNullOrEmpty(val))
                {
                    if (i == 0)
                    {
                        yield return string.Empty;
                        continue;
                    }

                    if (i == strings.Length - 1)
                        continue;

                    throw new ArgumentException("Duplicated path seperator");
                }

                yield return val;
            }
        }

        public PathInfo RemoveStartSlash()
        {
            var parts = GetParts();
            if (parts.Length > 0 && string.IsNullOrEmpty(parts[0]))
                return new PathInfo(parts.Skip(1).ToArray());

            return this;
        }

        public bool StartsWith(PathInfo directory)
        {
            return this.ToString().StartsWith(directory.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public bool HasExtension(string fileExtension)
        {
            return this.ToString().EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var p = obj as PathInfo;
            if (p != null)
                return this.Equals(p);

            return false;
        }

        public virtual bool Equals(PathInfo other)
        {
            return ToString().Equals(other.ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static PathInfo Combine(params PathInfo[] parts)
        {
            return new PathInfo(parts.Where(p => p != null).Reverse().SelectMany(p => p.GetEnumerateReverse()));
        }

        public static PathInfo GetSubPath(PathInfo basePath, string fullPath)
        {
            return new PathInfo(EnumerateReverse(fullPath, basePath.GetParts().Length));
        }
    }
}