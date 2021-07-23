using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Assets.CommonLibs.Utility
{
    public static class MD5FilesGenerator
    {
        //public const string HashListFileName = "files.txt";
        //public const string VersionFileName = "version.txt";
        private static MD5 __md5;

        private static MD5 GetMd5()
        {
            if (null == __md5)
            {
                __md5 = MD5.Create();
            }
            return __md5;
        }

        private static void DisposeMD5()
        {
            if (null != __md5)
            {
                __md5.Clear();
            }
        }

        //public static string OptimizeRun(string path, Dictionary<string, string> lastFiles, Dictionary<string, string> newFiles, List<string> rmFiles)
        //{
        //    var version = "";
        //    var fullListPath = Path.Combine(path, HashListFileName);
        //    var fullVersionPath = Path.Combine(path, VersionFileName);
        //    var sb = new StringBuilder();
        //    foreach (var file in rmFiles)
        //    {
        //        lastFiles.Remove(file);
        //    }
        //    foreach (var filePair in newFiles)
        //    {
        //        lastFiles[filePair.Key] = filePair.Value;
        //    }
        //    foreach (var filePair in lastFiles)
        //    {
        //        sb.AppendLine(string.Format("{0}:{1}", filePair.Key, filePair.Value));
        //    }
        //    var result = sb.ToString();
        //    File.WriteAllText(fullListPath, result);
        //    var md5hash = GetMd5();
        //    version = GetMd5Hash(md5hash, result);
        //    File.WriteAllText(fullVersionPath, version);
        //    DisposeMD5();
        //    return version;
        //}

        //public static string Run(string path, Func<string, bool> fileFilter)
        //{
        //    var version = "";
        //    var fullListPath = Path.Combine(path, HashListFileName);
        //    var fullVersionPath = Path.Combine(path, VersionFileName);
        //    if (File.Exists(fullListPath))
        //    {
        //        File.Delete(fullListPath);
        //    }
        //    var list = new List<string>();
        //    CommonUtility.LoopAllFiles(path, list);
        //    var sb = new StringBuilder();
        //    MD5 md5hash = GetMd5();
        //    foreach (var file in list)
        //    {
        //        if (!fileFilter(file))
        //        {
        //            continue;
        //        }
        //        var hash = GetMd5Hash(md5hash, File.ReadAllBytes(file));
        //        sb.AppendLine(string.Format("{0}:{1}", file.Replace(path, "").Replace("\\", "/"), hash));
        //    }
        //    var result = sb.ToString();
        //    File.WriteAllText(fullListPath, result);
        //    version = GetMd5Hash(md5hash, result);
        //    File.WriteAllText(fullVersionPath, version);
        //    DisposeMD5();
        //    return version;
        //}

        public static string GetMd5Hash(string text)
        {
            return GetMd5Hash(GetMd5(), text);
        }

        public static string GetMd5Hash(Stream stream)
        {
            var md5 = GetMd5();
            return BytesToString(md5.ComputeHash(stream));
        }

        public static string GetMd5Hash(byte[] buffer, int offset, int count)
        {
            var md5 = GetMd5();
            return BytesToString(md5.ComputeHash(buffer, offset, count));
        }

        private static StringBuilder m_sBuilder = new StringBuilder();
        private static string BytesToString(byte[] bytes)
        {
            m_sBuilder.Length = 0;
            // Create a new Stringbuilder to collect the bytes
            // and create a string.

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < bytes.Length; i++)
            {
                m_sBuilder.Append(bytes[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return m_sBuilder.ToString();
        }

        public static string GetMd5Hash(byte[] bytes)
        {
            return GetMd5Hash(GetMd5(), bytes);
        }

        static string GetMd5Hash(MD5 md5Hash, string text)
        {
            return GetMd5Hash(md5Hash, Encoding.UTF8.GetBytes(text));
        }

        static string GetMd5Hash(MD5 md5Hash, byte[] input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(input);
            return BytesToString(data);
        }
    }
}
