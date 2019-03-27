using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Deployer.Tests.Real.Tasks
{
    public static class FileAssertions
    {
        public static void AssertEqual(string sourceDir, string destDir)
        {
            var sources = Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories).Select(s =>
                new FileComparison
                {
                    File = s.Substring(sourceDir.Length),
                    FullName = s
                });
            var destFiles = Directory.EnumerateFiles(destDir, "*", SearchOption.AllDirectories).Select(s =>
                new FileComparison
                {
                    File = s.Substring(destDir.Length),
                    FullName = s
                });

            var unmatched = sources.Where(file => !IsMatch(file, destFiles)).ToList();
            if (unmatched.Any())
            {
                throw new ApplicationException();
            }
        }

        private static bool IsMatch(FileComparison a, IEnumerable<FileComparison> b)
        {
            var match = b.FirstOrDefault(x => x.File == a.File);

            if (match == null)
            {
                return false;
            }

            return AreEqual(a.FullName, match.FullName);
        }


        private static bool AreEqual(string a, string b)
        {
            return ByteArrayCompare(GetMd5(a), GetMd5(b));
        }

        private static byte[] GetMd5(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }

        private static bool ByteArrayCompare(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }

        private class FileComparison
        {
            public string File { get; set; }
            public string FullName { get; set; }
        }
    }
}