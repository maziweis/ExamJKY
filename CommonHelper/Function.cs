using ICSharpCode.SharpZipLib.Zip;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonHelper
{
    public class Function
    {
      public static string Zipkey =  System.Configuration.ConfigurationManager.AppSettings["zipKey"];
        /// <summary>
        /// 给一个字符串进行MD5加密
        /// </summary>
        /// <param name="strText">待加密字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strText));
            return System.Text.Encoding.Default.GetString(result);
        }

        #region 时间转换
        /// <summary>
        /// 格式化转换日期时间到字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConvertDateTime(DateTime? dt)
        {
            return dt == null ? "" : ((DateTime)dt).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 格式化转换日期到字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConvertDate(DateTime? dt)
        {
            return dt == null ? "" : ((DateTime)dt).ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Datetime转时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ConvertDateI(DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(dt - startTime).TotalMilliseconds; // 相差毫秒数
            return timeStamp;
        }

        /// <summary>
        /// Datetime转提交时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConvertDateII(DateTime dt)
        {
            string date = dt.ToString("yyyy-MM-dd") + " "; // 当地时区
            string week = dt.ToString("ddd", new System.Globalization.CultureInfo("zh-cn")) + " ";
            string timeStamp = dt.Hour + ":" + dt.Minute;
            return date + week + timeStamp;
        } 
        #endregion

        #region 将泛类型集合List类转换成DataTable
        /// <summary>
        /// 将泛类型集合List类转换成DataTable
        /// </summary>
        /// <param name="list">泛类型集合</param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> entitys)
        {
            //检查实体集合不能为空
            if (entitys == null || entitys.Count < 1)
            {
                return null;
            }
            //取出第一个实体的所有Propertie
            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            DataTable dt = new DataTable();
            for (int i = 0; i < entityProperties.Length; i++)
            {
                //dt.Columns.Add(entityProperties[i].Name, entityProperties[i].PropertyType);
                dt.Columns.Add(entityProperties[i].Name);
            }
            int j = 0;
            //将所有entity添加到DataTable中
            foreach (object entity in entitys)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }
                object[] entityValues = new object[entityProperties.Length];
                j++;
                for (int i = 0; i < entityProperties.Length; i++)
                {

                    entityValues[i] = i == 0 ? j : entityProperties[i].GetValue(entity, null);
                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }
        #endregion
        /// <summary>
        /// 验证身份证号，失败false
        /// </summary>
        /// <param name="idcard"></param>
        /// <returns></returns>
        public static bool MathIdCard(string idcard)
        {
            if (idcard.Length>13&&idcard.Length<18)
            {                
                return false;
            }
            return true;
        }
        /// <summary>
        /// 验证手机号，失败false
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool MathPhone(string num)
        {
            if ((!Regex.IsMatch(num, @"^1(3|4|5|6|7|8|9)\d{9}$", RegexOptions.IgnoreCase)))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="ZipFileName">压缩文件</param>
        /// <param name="TargetFolder">目标文件夹</param>
        public static void UnZipFile(string ZipFileName, string TargetFolder)
        {
            if (ZipFileName.ToLower().Contains("rar"))
            {
                if (!Directory.Exists(TargetFolder))
                {
                    Directory.CreateDirectory(TargetFolder);
                }
                using (Stream stream = File.OpenRead(ZipFileName))
                {
                    var reader = ReaderFactory.Open(stream);
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            reader.WriteEntryToDirectory(TargetFolder);
                        }
                    }
                }
            }
            else
            {
                if (!File.Exists(ZipFileName))
                    return;
                FileStream FileS = File.OpenRead(ZipFileName);
                ZipInputStream zipInStream = new ZipInputStream(FileS);
                zipInStream.Password = Zipkey;
                //zipInStream.Password = "123456";
                string FolderPath = Path.GetDirectoryName(TargetFolder);
                if (!Directory.Exists(TargetFolder))
                {
                    Directory.CreateDirectory(FolderPath);
                }
                ZipEntry tEntry;
                while ((tEntry = zipInStream.GetNextEntry()) != null)
                {
                    string fileName = Path.GetFileName(tEntry.Name);
                    string tempPath = TargetFolder + "\\" + Path.GetDirectoryName(tEntry.Name);
                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }
                    if (fileName != null && fileName.Length > 0)
                    {
                        FileStream streamWriter = File.Create(TargetFolder + "\\" + tEntry.Name);
                        byte[] data = new byte[2048];
                        System.Int32 size;
                        try
                        {
                            do
                            {
                                size = zipInStream.Read(data, 0, data.Length);
                                streamWriter.Write(data, 0, size);
                            } while (size > 0);
                        }
                        catch (System.Exception ex)
                        {
                            throw ex;
                        }
                        streamWriter.Close();
                    }
                }
                zipInStream.Close();
            }
        }


        /// <summary>
        /// 解压RAR和ZIP文件(需存在Winrar.exe(只要自己电脑上可以解压或压缩文件就存在Winrar.exe))
        /// </summary>
        /// <param name="UnPath">解压后文件保存目录</param>
        /// <param name="rarPathName">待解压文件存放绝对路径（包括文件名称）</param>
        /// <param name="IsCover">所解压的文件是否会覆盖已存在的文件(如果不覆盖,所解压出的文件和已存在的相同名称文件不会共同存在,只保留原已存在文件)</param>
        /// <param name="PassWord">解压密码(如果不需要密码则为空)</param>
        /// <returns>true(解压成功);false(解压失败)</returns>
        public static bool UnRarOrZip(string rarPathName , string UnPath, string PassWord, bool IsCover=true)
        {
            PassWord = Zipkey;
            if (!Directory.Exists(UnPath))
                Directory.CreateDirectory(UnPath);
            if (!File.Exists(rarPathName))
                return false;
            Process Process1 = new Process();
            Process1.StartInfo.FileName = "Winrar.exe";
            Process1.StartInfo.CreateNoWindow = true;
            string cmd = "";
            if (!string.IsNullOrEmpty(PassWord) && IsCover)
                //解压加密文件且覆盖已存在文件( -p密码 )
                cmd = string.Format(" x -p{0} -o+ {1} {2} -y", PassWord, rarPathName, UnPath);
            else if (!string.IsNullOrEmpty(PassWord) && !IsCover)
                //解压加密文件且不覆盖已存在文件( -p密码 )
                cmd = string.Format(" x -p{0} -o- {1} {2} -y", PassWord, rarPathName, UnPath);
            else if (IsCover)
                //覆盖命令( x -o+ 代表覆盖已存在的文件)
                cmd = string.Format(" x -o+ {0} {1} -y", rarPathName, UnPath);
            else
                //不覆盖命令( x -o- 代表不覆盖已存在的文件)
                cmd = string.Format(" x -o- {0} {1} -y", rarPathName, UnPath);
            //命令
            Process1.StartInfo.Arguments = cmd;
            Process1.Start();
            Process1.WaitForExit();//无限期等待进程 winrar.exe 退出
                                   //Process1.ExitCode==0指正常执行，Process1.ExitCode==1则指不正常执行
            if (Process1.ExitCode == 0)
            {
                Process1.Close();
                return true;
            }
            else
            {
                Process1.Close();
                return false;
            }

        }
    }
}
