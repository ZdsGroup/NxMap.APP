using SuperMap.Data;
using SuperMap.UI;
using System;

namespace Map.NxApp.Common.Core
{
    public class SmObjectLocator
    {
        private static SmObjectLocator smObjectLocator;
        private static Workspace wkspaceObject = null;
        private static MapControl mapObject = null;

        /// <summary>
        /// 单例模式添加单例实体
        /// </summary>
        /// <returns></returns>
        public static SmObjectLocator getInstance()
        {
            //初始化二维基本对象
            if (smObjectLocator == null)
            {
                smObjectLocator = new SmObjectLocator();
                wkspaceObject = new Workspace();
                mapObject = new MapControl();
                mapObject.Map.Workspace = wkspaceObject;
            }
            return smObjectLocator;
        }

        //单例模式
        private SmObjectLocator()
        {
            if (smObjectLocator != null)
            {
                throw new Exception("不能通过new关键字实例化对象，请调用静态函数getInstance()获取对象实例！");
            }
        }

        /// <summary>
        ///二维地图控件对象
        /// </summary>
        public MapControl MapObject
        {
            get  {  return mapObject; }
        }

        /// <summary>
        /// 工作空间对象
        /// </summary>
        public Workspace WkSpaceObject
        {
            get { return wkspaceObject; }
        }
    }
}
