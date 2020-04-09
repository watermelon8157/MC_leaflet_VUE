using log4net;
using System;
using System.IO;


namespace Com.Mayaminer
{
    public class LogTool
    {
        /// <summary>
        /// 預設log檔案位置
        /// log路徑修改 AppDomain.CurrentDomain.BaseDirectory + "App_Data\\"
        /// http://blog.darkthread.net/post-2017-10-04-web-log-security-issue.aspx
        /// </summary>
        private static string default_log_save_path = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\";
        private static string other_save_path = "";

        private static string DefaultPath
        {
            get
            {
                return string.IsNullOrEmpty(other_save_path) ? default_log_save_path : other_save_path;
            }
        }
        /// <summary>
        /// 檢驗路徑的資料夾是否存在，若否則新增
        /// </summary>
        /// <param name="file_path"></param>
        /// <returns></returns>
        private static void CheckExistsAndCreateFolder(string file_path)
        {
            if (Directory.Exists(file_path) == false)
            {
                Directory.CreateDirectory(file_path);
            }
        }

        /// <summary>
        /// 記錄log
        /// </summary>
        /// <param name="message">文字訊息</param>
        /// <param name="log_type">記錄分類</param>
        /// <param name="save_path">記錄路徑</param>
        public static void SaveLogMessage(string message, string log_type, string save_path = "")
        {
            try
            {
                string current_log_path = LogTool.DefaultPath;
                // log file name
                string dic_path = current_log_path + "log\\" + (string.IsNullOrEmpty(save_path) ? "" : save_path + "\\");
                string file_name = log_type + "_" + DateTime.Now.ToString("yyyy_MM") + ".log";
                string log_file_name = dic_path + file_name;
                LogTool.CheckExistsAndCreateFolder(dic_path);

                string log_message = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + Environment.GetEnvironmentVariable("COMPUTERNAME") + "]" + message + "\n";
                using (StreamWriter sw = File.AppendText(log_file_name))
                {
                    sw.WriteLine(log_message);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 記錄log
        /// </summary>
        /// <param name="ex">例外物件</param>
        /// <param name="log_type">記錄分類</param>
        /// <param name="save_path">記錄路徑</param>
        public static void SaveLogMessage(Exception ex, string log_type, string save_path = "")
        {
            string save_message = ex.Message.ToString() + "\n" + ex.StackTrace.ToString();
            LogTool.SaveLogMessage(save_message, log_type, save_path);
        }
        /// <summary>
        /// 記錄log
        /// </summary>
        /// <param name="message">文字訊息</param>
        /// <param name="log_type">記錄分類</param>
        /// <param name="save_path">記錄路徑</param>
        public static void SaveLogMessage(string message, string log_type, GetLogToolCS log_path)
        {
            LogTool.SaveLogMessage(message, log_type, log_path.ToString());
        }

        /// <summary>
        /// 記錄log
        /// </summary>
        /// <param name="message">文字訊息</param>
        /// <param name="log_type">記錄分類</param>
        /// <param name="save_path">記錄路徑</param>
        public static void SaveLogMessage(Exception ex, string log_type, GetLogToolCS log_path)
        {
            string save_message = ex.Message.ToString() + "\n" + ex.StackTrace.ToString();
            LogTool.SaveLogMessage(save_message, log_type, log_path.ToString());
        }
    }
}