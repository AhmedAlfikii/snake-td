using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.FileManagement
{
    public static class FileManagement
    {
        /// <summary>
        /// Returns the count of all the folders in the given path.
        /// </summary>
        /// <param name="pathIsRelative">Is the path is relative to the project's "Assets" folder?</param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetFoldersCountInDirectory(string path, bool pathIsRelative = true)
        {
            DirectoryInfo dir = new DirectoryInfo((pathIsRelative ? (Application.dataPath + "/") : "") + path);

            FileSystemInfo[] infos = dir.GetFileSystemInfos();

            int count = 0;

            foreach (var i in infos)
            {
                if (i is DirectoryInfo)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Returns the count of all files in the given path except meta files (files that end with ".meta").
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathIsRelative">Is the path is relative to the project's "Assets" folder?</param>
        /// <param name="includeMetaFiles"></param>
        /// <returns></returns>
        public static int GetFilesCountInDirectory(string path, bool pathIsRelative = true, bool includeMetaFiles = false)
        {
            DirectoryInfo dir = new DirectoryInfo((pathIsRelative? (Application.dataPath + "/") : "") + path);

            FileSystemInfo[] infos = dir.GetFileSystemInfos();

            int count = 0;

            foreach (var i in infos)
            {
                if (i is FileInfo)
                {
                    if (includeMetaFiles || !i.Name.EndsWith(".meta"))
                        count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Returns a list containing the names of all the folders in the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathIsRelative">Is the path is relative to the project's "Assets" folder?</param>
        /// <returns></returns>
        public static List<string> GetFolderNamesInDirectory(string path, bool pathIsRelative = true)
        {
            DirectoryInfo dir = new DirectoryInfo((pathIsRelative ? (Application.dataPath + "/") : "") + path);

            FileSystemInfo[] infos = dir.GetFileSystemInfos();

            List<string> names = new List<string>();

            foreach (var i in infos)
            {
                if (i is DirectoryInfo)
                {
                    names.Add(i.Name);
                }
            }

            return names;
        }

        /// <summary>
        /// Returns a list of all the file names in the given path (including their extension).
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathIsRelative">Is the path is relative to the project's "Assets" folder?</param>
        /// <param name="includeMetaFiles"></param>
        /// <returns></returns>
        public static List<string> GetFileNamesInDirectory(string path, bool pathIsRelative = true, bool includeMetaFiles = false)
        {
            DirectoryInfo dir = new DirectoryInfo((pathIsRelative ? (Application.dataPath + "/") : "") + path);

            FileSystemInfo[] infos = dir.GetFileSystemInfos();

            List<string> names = new List<string>();

            foreach (var i in infos)
            {
                if (i is FileInfo)
                {
                    if (includeMetaFiles || !i.Name.EndsWith(".meta"))
                    {
                        names.Add(i.Name);
                    }
                }
            }

            return names;
        }

    }
}