using System.Configuration;

namespace Map.NxApp.Common.Utils
{
    /// <summary>
    /// 获取配置文件内容用帮助类
    /// </summary>
    public class AppConfigHelper
    {
        /// <summary>
        /// 从配置文件读取指定配置内容
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>键值</returns>
        public static string GetExternalValue(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }

        /// <summary>
        /// 根据序号从配置文件读取指定配置内容
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="index">序号</param>
        /// <returns>键值</returns>
        public static string GetExternalValue(string key, int index)
        {
            return ConfigurationManager.AppSettings.Get(string.Format(key, index));
        }

        /// <summary>
        /// 从配置文件读取int类型值
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>int键值</returns>
        public static int GetExternalValueByInt(string key)
        {
            string value = ConfigurationManager.AppSettings.Get(key);
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            else
            {
                return int.Parse(value);
            }
        }

        /// <summary>
        /// 从配置文件读取double类型值
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>double键值</returns>
        public static double GetExternalValueByDouble(string key)
        {
            string value = ConfigurationManager.AppSettings.Get(key);
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            else
            {
                return double.Parse(value);
            }
        }

        /// <summary>
        /// 从配置文件读取bool类型值
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>bool键值</returns>
        public static bool GetExternalValueByBool(string key)
        {
            string value = ConfigurationManager.AppSettings.Get(key);
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            else
            {
                return bool.Parse(value);
            }
        }
    }
}
