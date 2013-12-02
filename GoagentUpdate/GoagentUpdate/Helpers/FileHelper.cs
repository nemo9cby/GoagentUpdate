using System;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Text;

namespace sherlock99.Toolkit
{
    public class FileHelper
    {
        #region 读写文件
        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Read(string path)
        {
            return Read(path, Encoding.Default);
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Read(string path, Encoding encoding)
        {
            if (!File.Exists(path))
            {
                return "";
            }

            string retVal = "";
            using (StreamReader sr = new StreamReader(path, encoding))
            {
                retVal = sr.ReadToEnd();
            }

            return retVal;
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="content"></param>
        /// <param name="path"></param>
        public static bool Write(string content, string path)
        {
            return Write(content, path, Encoding.Default);
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="content"></param>
        /// <param name="path"></param>
        public static bool Write(string content, string path, Encoding encoding)
        {
            if (File.Exists(path))
            {
                return false;
            }

            using (StreamWriter sw = new StreamWriter(path, false, encoding))
            {
                sw.Write(content);
            }

            return true;
        }

        #endregion

        /// <summary>
        /// 文件夹copy函数
        /// 源路径以文件夹结尾
        /// </summary>
        /// <param name="source_url">源路径</param>
        /// <param name="target_url">目标路径</param>
        /// <param name="overwrite">是否覆盖同名文件</param>
        /// <returns>1成功    0源目录不存在    -2其他错误</returns>
        public static int Copy(string source_url, string target_url, bool overwrite = true)
        {
            int flag = 0;

            //判断传入路径是否合法
            if (source_url.Length == 0 || target_url.Length == 0)
                return 0;

            //拷贝文件
            flag = RecursiveCopy(source_url, target_url, overwrite);

            return flag;
        }

        /// <summary>
        /// 文件夹delete函数
        /// </summary>
        /// <param name="source_url">目标路径</param>
        /// <returns></returns>
        public static int Delete(string source_url)
        {
            int flag = 0;

            //判断传入路径是否合法
            if (source_url.Length == 0)
                return 0;

            //删除文件
            flag = RecursiveDelete(source_url);

            return flag;
        }

        #region 辅助函数

        /// <summary>
        /// 递归调用的拷贝文件函数 自动生成不存在的文件夹  overwrite指示是否自动覆盖同名文件
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="aimPath"></param>
        /// <param name="overwrite"></param>
        /// <returns>1正常  0源目录不存在  -2其他错误</returns>
        private static int RecursiveCopy(string srcPath, string aimPath, bool overwrite = true)
        {
            try
            {
                if (!Directory.Exists(srcPath))
                    return 0;

                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;

                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);

                string[] fileList = Directory.GetFileSystemEntries(srcPath);

                foreach (string file in fileList)
                {
                    // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    if (Directory.Exists(file))
                        RecursiveCopy(file, aimPath + Path.GetFileName(file), overwrite);
                    // 否则直接Copy文件  覆盖同名文件
                    else
                    {
                        System.IO.File.Copy(file, aimPath + Path.GetFileName(file), overwrite);
                    }
                }
            }
            catch
            {
                return -2;
            }
            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcPath"></param>
        /// <returns></returns>
        private static int RecursiveDelete(string srcPath)
        {
            try
            {
                if (!Directory.Exists(srcPath))
                    return 0;

                string[] fileList = Directory.GetFileSystemEntries(srcPath);

                foreach (string file in fileList)
                {
                    // 先当作目录处理如果存在这个目录就递归删除该目录下面的文件
                    if (Directory.Exists(file))
                        RecursiveDelete(file);
                    // 否则直接删除文件
                    else
                    {
                        System.IO.File.Delete(file);
                    }
                }
                Directory.Delete(srcPath);
            }
            catch
            {
                return -2;
            }
            return 1;
        }

        #endregion
    }
}
