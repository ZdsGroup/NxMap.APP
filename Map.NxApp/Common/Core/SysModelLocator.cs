﻿using Map.NxApp.Common.Model;
using Map.NxApp.Common.VO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map.NxApp.Common.Core
{
    public class SysModelLocator
    {
        private static SysModelLocator sysModelLocator;
        /// <summary>
        /// 单例模式
        /// </summary>
        private SysModelLocator()
        {
            if (sysModelLocator != null)
            {
                throw new Exception("不能通过new关键字实例化对象，请调用静态函数getInstance()获取对象实例！");
            }
        }

        /// <summary>
        /// 单例模式获取单例实体
        /// </summary>
        /// <returns></returns>
        public static SysModelLocator getInstance()
        {
            if (sysModelLocator == null)
            {
                sysModelLocator = new SysModelLocator();
            }
            return sysModelLocator;
        }


        #region 系统全局模型实例
        /// <summary>
        /// 图层列表
        /// </summary>
        public ObservableCollection<LayerVO> LayerList = new ObservableCollection<LayerVO>();

        /// <summary>
        /// 图层分组字典表（图层一级节点）
        /// </summary>
        public Dictionary<string, TreeModel> LayerGroupDics = new Dictionary<string, TreeModel>();
        #endregion
    }
}