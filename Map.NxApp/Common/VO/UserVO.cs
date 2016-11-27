using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map.NxApp.Common.VO
{
    public class UserVO
    {
        #region 用户信息
        private string userName;
        private string userId;
        private string passWord;
        private string userRole;
        private string loginDate;
        private string otherInfo;

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                userName = value;
            }
        }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string PassWord
        {
            get
            {
                return passWord;
            }

            set
            {
                passWord = value;
            }
        }

        /// <summary>
        /// 用户角色
        /// </summary>
        public string UserRole
        {
            get
            {
                return userRole;
            }

            set
            {
                userRole = value;
            }
        }

        /// <summary>
        /// 登录时间
        /// </summary>
        public string LoginDate
        {
            get
            {
                return loginDate;
            }

            set
            {
                loginDate = value;
            }
        }

        /// <summary>
        /// 补充信息
        /// </summary>
        public string OtherInfo
        {
            get
            {
                return otherInfo;
            }

            set
            {
                otherInfo = value;
            }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId
        {
            get
            {
                return userId;
            }

            set
            {
                userId = value;
            }
        }
        #endregion
    }
}
