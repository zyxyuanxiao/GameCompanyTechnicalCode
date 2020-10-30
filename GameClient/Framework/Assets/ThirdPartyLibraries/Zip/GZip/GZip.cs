﻿using UnityEngine;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

namespace TW
{
    /// <summary>
    /// 压缩文件类
    /// </summary>
    public class BestZip
    {
        /// <summary>
        /// Compress
        /// </summary>
        /// <param name="lpSourceFolder">The location of the files to include in the zip file, all files including files in subfolders will be included.</param>
        /// <param name="lpDestFolder">Folder to write the zip file into</param>
        /// <param name="zipFileName">Name of the zip file to write</param>
        public static GZipResult Compress(string lpSourceFolder, string lpDestFolder, string zipFileName)
        {
            return Compress(lpSourceFolder, "*.*", SearchOption.AllDirectories, lpDestFolder, zipFileName, true);
        }
        
        public static GZipResult Compress(string lpSourceFolder, string lpDestFolder, string searchPattern, string zipFileName)
        {
            return Compress(lpSourceFolder, searchPattern, SearchOption.AllDirectories, lpDestFolder, zipFileName, true);
        }

        /// <summary>
        /// Compress
        /// </summary>
        /// <param name="lpSourceFolder">The location of the files to include in the zip file</param>
        /// <param name="searchPattern">Search pattern (ie "*.*" or "*.txt" or "*.gif") to idendify what files in lpSourceFolder to include in the zip file</param>
        /// <param name="searchOption">Only files in lpSourceFolder or include files in subfolders also</param>
        /// <param name="lpDestFolder">Folder to write the zip file into</param>
        /// <param name="zipFileName">Name of the zip file to write</param>
        /// <param name="deleteTempFile">Boolean, true deleted the intermediate temp file, false leaves the temp file in lpDestFolder (for debugging)</param>
        public static GZipResult Compress(string lpSourceFolder, string searchPattern, SearchOption searchOption, string lpDestFolder, string zipFileName, bool deleteTempFile)
        {
            DirectoryInfo di = new DirectoryInfo(lpSourceFolder);
            FileInfo[] files = di.GetFiles(searchPattern, searchOption);
            return Compress(files, lpSourceFolder, lpDestFolder, zipFileName, deleteTempFile);
        }

        /// <summary>
        /// Compress
        /// </summary>
        /// <param name="files">Array of FileInfo objects to be included in the zip file</param>
        /// <param name="lpBaseFolder">Base folder to use when creating relative paths for the files 
        /// stored in the zip file. For example, if lpBaseFolder is 'C:\zipTest\Files\', and there is a file 
        /// 'C:\zipTest\Files\folder1\sample.txt' in the 'files' array, the relative path for sample.txt 
        /// will be 'folder1/sample.txt'</param>
        /// <param name="lpDestFolder">Folder to write the zip file into</param>
        /// <param name="zipFileName">Name of the zip file to write</param>
        public static GZipResult Compress(FileInfo[] files, string lpBaseFolder, string lpDestFolder, string zipFileName)
        {
            return Compress(files, lpBaseFolder, lpDestFolder, zipFileName, true);
        }

        /// <summary>
        /// Compress
        /// </summary>
        /// <param name="files">Array of FileInfo objects to be included in the zip file</param>
        /// <param name="lpBaseFolder">Base folder to use when creating relative paths for the files 
        /// stored in the zip file. For example, if lpBaseFolder is 'C:\zipTest\Files\', and there is a file 
        /// 'C:\zipTest\Files\folder1\sample.txt' in the 'files' array, the relative path for sample.txt 
        /// will be 'folder1/sample.txt'</param>
        /// <param name="lpDestFolder">Folder to write the zip file into</param>
        /// <param name="zipFileName">Name of the zip file to write</param>
        /// <param name="deleteTempFile">Boolean, true deleted the intermediate temp file, false leaves the temp file in lpDestFolder (for debugging)</param>
        public static GZipResult Compress(FileInfo[] files, string lpBaseFolder, string lpDestFolder, string zipFileName, bool deleteTempFile)
        {
            GZipResult result = new GZipResult();

            try
            {
                if (!lpDestFolder.EndsWith("/"))
                {
                    lpDestFolder += "/";
                }

                string lpTempFile = lpDestFolder + zipFileName + ".tmp";
                string lpZipFile = lpDestFolder + zipFileName;

                result.TempFile = lpTempFile;
                result.ZipFile = lpZipFile;

                if (files != null && files.Length > 0)
                {
                    CreateTempFile(files, lpBaseFolder, lpTempFile, result);

                    if (result.FileCount > 0)
                    {
                        CreateZipFile(lpTempFile, lpZipFile, result);
                    }

                    // delete the temp file
                    if (deleteTempFile)
                    {
                        File.Delete(lpTempFile);
                        result.TempFileDeleted = true;
                    }
                }
            }
            catch
            { //(Exception ex4)
                result.Errors = true;
            }
            return result;
        }
        
        public static byte[] getBytes(long s) 
        {
            byte[] buf = new byte[8];
            for (int i = buf.Length - 1; i >= 0; i--) 
            {
                buf[i] = (byte) (s & 0x00000000000000ff);
                s >>= 8;
            }
            return buf;
        }
                
        public static long getLong(byte[] buf) 
        {
            long r = 0;
            for (int i = 0; i < buf.Length; i++) 
            {
                r <<= 8;
                r |= (buf[i] & 0x00000000000000ff);
            }
            return r;
        }

        private static void CreateZipFile(string lpSourceFile, string lpZipFile, GZipResult result)
        {
            byte[] buffer;
            FileStream fsOut = null;
            FileStream fsIn = null;
            ICSharpCode.SharpZipLib.GZip.GZipOutputStream gzip = null;
            // compress the file into the zip file
            try
            {
                fsIn = new FileStream(lpSourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                buffer = new byte[fsIn.Length];
                fsIn.Read(buffer, 0, buffer.Length);
                fsIn.Close();
                fsIn = null;

                fsOut = new FileStream(lpZipFile, FileMode.Create, FileAccess.Write, FileShare.None);
                gzip = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(fsOut);
                // compress to the zip file
                long totalSize = result.TempFileSize;
                byte[] size = getBytes(totalSize);
                //Debug.Log("原始长度：" + totalSize);
                //写入原始长度
                gzip.Write(size, 0, size.Length);
                gzip.Write(buffer, 0, buffer.Length);

                result.ZipFileSize = fsOut.Length;
                result.CompressionPercent = GetCompressionPercent(result.TempFileSize, result.ZipFileSize);
                //Debug.Log("压缩比：" + result.CompressionPercent);
            }
            catch
            { //(Exception ex1)
                result.Errors = true;
            }
            finally
            {
                if (gzip != null)
                {
                    gzip.Close();
                    gzip = null;
                }
                if (fsOut != null)
                {
                    fsOut.Close();
                    fsOut = null;
                }
                if (fsIn != null)
                {
                    fsIn.Close();
                    fsIn = null;
                }
            }
        }

        private static void CreateTempFile(FileInfo[] files, string lpBaseFolder, string lpTempFile, GZipResult result)
        {
            byte[] buffer;
            byte[] header;
            string fileHeader = null;
            string fileModDate = null;
            int fileIndex = 0;
            string lpSourceFile = null;
            string vpSourceFile = null;
            GZipFileInfo gzf = null;
            FileStream fsOut = null;
            FileStream fsIn = null;
            string lpFolder = null;
            if (files != null && files.Length > 0)
            {
                try
                {
                    result.Files = new GZipFileInfo[files.Length];

                    // open the temp file for writing
                    long originLen = 0;
                    fsOut = new FileStream(lpTempFile, FileMode.Create, FileAccess.Write, FileShare.None);
                    
                    foreach (FileInfo fi in files)
                    {
                        try
                        {
                            gzf = new GZipFileInfo();
                            gzf.Index = fileIndex;

                            // read the source file, get its virtual path within the source folder
                            lpSourceFile = fi.FullName; 
                            gzf.LocalPath = lpSourceFile;
                            vpSourceFile = lpSourceFile.Replace('\\', '/');
                            lpBaseFolder = lpBaseFolder.Replace('\\', '/');
                            vpSourceFile = vpSourceFile.Replace(lpBaseFolder, string.Empty).Substring(1);
                            gzf.RelativePath = vpSourceFile;

                            fsIn = new FileStream(lpSourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                            buffer = new byte[fsIn.Length];
                            fsIn.Read(buffer, 0, buffer.Length);
                            fsIn.Close();
                            fsIn = null;

                            fileModDate = fi.LastWriteTimeUtc.ToString(CultureInfo.CurrentCulture);
                            gzf.Length = buffer.Length;

                            fileHeader = fileIndex.ToString() + "," + vpSourceFile + "," + fileModDate + "," + buffer.Length.ToString() + "\n";
                            header = Encoding.Default.GetBytes(fileHeader);

                            fsOut.Write(header, 0, header.Length);
                            fsOut.Write(buffer, 0, buffer.Length);
                            fsOut.WriteByte(10); // linefeed

                            gzf.AddedToTempFile = true;

                            // update the result object
                            result.Files[fileIndex] = gzf;

                            // increment the fileIndex
                            fileIndex++;
                        }
                        catch
                        { //(Exception ex1)
                            result.Errors = true;
                        }
                        finally
                        {
                            if (fsIn != null)
                            {
                                fsIn.Close();
                                fsIn = null;
                            }
                        }
                        if (fsOut != null)
                        {
                            result.TempFileSize = fsOut.Length;
                        }
                    }
                }
                catch
                { //(Exception ex2)
                    result.Errors = true;
                }
                finally
                {
                    if (fsOut != null)
                    {
                        fsOut.Close();
                        fsOut = null;
                    }
                }
            }

            result.FileCount = fileIndex;
        }

        public static GZipResult Decompress(string lpSourceFolder, string lpDestFolder, string zipFileName, ref GZipResult result)
        {
            return Decompress(lpSourceFolder, lpDestFolder, zipFileName, true, true, null, null, 4096, ref result);
        }

        public static GZipResult Decompress(string lpSourceFolder, string lpDestFolder, string zipFileName, bool writeFiles, string addExtension, ref GZipResult result)
        {
            return Decompress(lpSourceFolder, lpDestFolder, zipFileName, true, writeFiles, addExtension, null, 4096, ref result);
        }

        public static GZipResult Decompress(string lpSrcFolder, string lpDestFolder, string zipFileName,
                                             bool deleteTempFile, bool writeFiles, string addExtension, Hashtable htFiles, int bufferSize, ref GZipResult result)
        {
            if (!lpDestFolder.EndsWith("/"))
            {
                lpDestFolder += "/";
            }

            string lpTempFile = lpSrcFolder + zipFileName + ".tmp";
            string lpZipFile = lpSrcFolder + zipFileName;

            result.TempFile = lpTempFile;
            result.ZipFile = lpZipFile;

            string line = null;
            string lpFilePath = null;
            string lpFolder = null;
            GZipFileInfo gzf = null;
            FileStream fsTemp = null;
            ArrayList gzfs = new ArrayList();
            bool write = false;

            if (string.IsNullOrEmpty(addExtension))
            {
                addExtension = string.Empty;
            }
            else if (!addExtension.StartsWith("."))
            {
                addExtension = "." + addExtension;
            }

            // extract the files from the temp file
            try
            {
                fsTemp = UnzipToTempFile(lpZipFile, lpTempFile, ref result);
                if (fsTemp != null)
                {
                    while (fsTemp.Position != fsTemp.Length)
                    {
                        line = null;
                        while (string.IsNullOrEmpty(line) && fsTemp.Position != fsTemp.Length)
                        {
                            line = ReadLine(fsTemp);
                        }

                        if (!string.IsNullOrEmpty(line))
                        {
                            gzf = new GZipFileInfo();
                            if (gzf.ParseFileInfo(line) && gzf.Length > 0)
                            {
                                gzfs.Add(gzf);
                                lpFilePath = lpDestFolder + gzf.RelativePath;
                                lpFolder = GetFolder(lpFilePath);
                                gzf.LocalPath = lpFilePath;
                                write = false;
                                if (htFiles == null || htFiles.ContainsKey(gzf.RelativePath))
                                {
                                    gzf.RestoreRequested = true;
                                    write = writeFiles;
                                }

                                if (write)
                                {
                                    // make sure the folder exists
                                    if (!Directory.Exists(lpFolder))
                                    {
                                        Directory.CreateDirectory(lpFolder);
                                    }
                                    // read from fsTemp and write out the file
                                    gzf.Restored = WriteFile(fsTemp, gzf.Length, lpFilePath + addExtension, bufferSize);
                                }
                                else
                                {
                                    // need to advance fsTemp
                                    fsTemp.Position += gzf.Length;
                                }
                                    
                            }
                            result.UnZipPercent = 0.9f + ((float)fsTemp.Position / (float)fsTemp.Length) * 0.1f;  
                        }
                    }
                }
            }
            catch
            { //(Exception ex3)
                result.Errors = true;
            }
            finally
            {
                if (fsTemp != null)
                {
                    fsTemp.Close();
                    fsTemp = null;
                }
            }

            // delete the temp file
            try
            {
                if (deleteTempFile)
                {
                    File.Delete(lpTempFile);
                    result.TempFileDeleted = true;
                }
            }
            catch
            { //(Exception ex4)
                result.Errors = true;
            }

            result.FileCount = gzfs.Count;
            result.Files = new GZipFileInfo[gzfs.Count];
            gzfs.CopyTo(result.Files);
            result.UnZipPercent = 100;
            return result;
        }

        private static string ReadLine(FileStream fs)
        {
            string line = string.Empty;

            const int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            byte b = 0;
            byte lf = 10;
            int i = 0;

            while (b != lf)
            {
                b = (byte)fs.ReadByte();
                buffer[i] = b;
                i++;
            }

            line = System.Text.Encoding.Default.GetString(buffer, 0, i - 1);

            return line;
        }

        private static bool WriteFile(FileStream fs, int fileLength, string lpFile, int bufferSize)
        {
            bool success = false;
            FileStream fsFile = null;

            if (bufferSize == 0 || fileLength < bufferSize)
            {
                bufferSize = fileLength;
            }

            int count = 0;
            int remaining = fileLength;
            int readSize = 0;

            try
            {
                byte[] buffer = new byte[bufferSize];
                fsFile = new FileStream(lpFile, FileMode.Create, FileAccess.Write, FileShare.None);

                while (remaining > 0)
                {
                    if (remaining > bufferSize)
                    {
                        readSize = bufferSize;
                    }
                    else
                    {
                        readSize = remaining;
                    }

                    count = fs.Read(buffer, 0, readSize);
                    remaining -= count;

                    if (count == 0)
                    {
                        break;
                    }

                    fsFile.Write(buffer, 0, count);
                    fsFile.Flush();

                }
                fsFile.Flush();
                fsFile.Close();
                fsFile = null;

                success = true;
            }
            catch
            { //(Exception ex2)
                success = false;
            }
            finally
            {
                if (fsFile != null)
                {
                    fsFile.Flush();
                    fsFile.Close();
                    fsFile = null;
                }
            }
            return success;
        }

        private static string GetFolder(string lpFilePath)
        {
            return Path.GetDirectoryName(lpFilePath);
        }
        private static FileStream UnzipToTempFile(string lpZipFile, string lpTempFile, ref GZipResult result)
        {
            FileStream fsIn = null;
            ICSharpCode.SharpZipLib.GZip.GZipInputStream gzip = null;
            FileStream fsOut = null;
            FileStream fsTemp = null;

            const int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            int count = 0;

            try
            {
                fsIn = new FileStream(lpZipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                result.ZipFileSize = fsIn.Length;

                fsOut = new FileStream(lpTempFile, FileMode.Create, FileAccess.Write, FileShare.None);
                gzip = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(fsIn);
                
                byte[] buf = new byte[8];
                //读取原始长度
                gzip.Read(buf, 0, 8);
                long originSize = getLong(buf);
                //Debug.Log("获取原始长度：" + size);
                int pos = 0;
                while (true)
                {
                    count = gzip.Read(buffer, 0, bufferSize);
                    if (count != 0)
                    {
                        fsOut.Write(buffer, 0, count);
                    }
                    pos += count;
                    result.UnZipPercent = ((float)pos / originSize) * 0.9f;
                    if (count != bufferSize)
                    {
                        break;
                    }
                }
            }
            catch (System.Exception ex1)
            { 
                //(Exception ex1)
                //if(!Predefine.IS_RELEASE_VER) Debug.Log("UnzipToTempFile is Fail!!!   " + ex1.ToString());
                result.Errors = true;
            }
            finally
            {
                if (gzip != null)
                {
                    gzip.Close();
                    gzip = null;
                }
                if (fsOut != null)
                {
                    fsOut.Close();
                    fsOut = null;
                }
                if (fsIn != null)
                {
                    fsIn.Close();
                    fsIn = null;
                }
            }

            fsTemp = new FileStream(lpTempFile, FileMode.Open, FileAccess.Read, FileShare.None);
            if (fsTemp != null)
            {
                result.TempFileSize = fsTemp.Length;
            }
            return fsTemp;
        }

        private static int GetCompressionPercent(long tempLen, long zipLen)
        {
            double tmp = (double)tempLen;
            double zip = (double)zipLen;
            double hundred = 100;

            double ratio = (tmp - zip) / tmp;
            double pcnt = ratio * hundred;

            return (int)pcnt;
        }
        
        

        
        
    }
}