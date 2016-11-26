using SuperMap.Data;

namespace Map.NxApp.Common.VO
{
    public class LayerVO
    {
		private string layerOrigin;
		/// <summary>
		/// 图层原始名称，用显示隐藏定位图层等
		/// </summary>
		public string LayerOrigin
		{
			get { return layerOrigin; }
			set { layerOrigin = value; }
		}

        private string layerType;

        /// <summary>
        /// 图层类型
        /// </summary>
        public string LayerType
        {
            get { return layerType; }
            set { layerType = value; }
        }

        private string layerName;

        /// <summary>
        /// 图层名称
        /// </summary>
        public string LayerName
        {
            get { return layerName; }
            set { layerName = value; }
        }

        private string layerId;

        /// <summary>
        /// 图层ID
        /// </summary>
        public string LayerId
        {
            get { return layerId; }
            set { layerId = value; }
        }

        private bool layerVisible = false;
        /// <summary>
        /// 图层是否可见
        /// </summary>
        public bool LayerVisible
        {
            get { return layerVisible; }
            set { layerVisible = value; }
        }

        private Rectangle2D layerBounds;
        /// <summary>
        /// 图层边界
        /// </summary>
        public Rectangle2D LayerBounds
        {
            get { return layerBounds; }
            set { layerBounds = value; }
        }

        private Point2D layerCenter;
        /// <summary>
        /// 图层中心点
        /// </summary>
        public Point2D LayerCenter
        {
            get { return layerCenter; }
            set { layerCenter = value; }
        }

        private bool isQueryLayer=false;
        /// <summary>
        /// 是否是查询图层
        /// </summary>
        public bool IsQueryLayer
        {
            get { return isQueryLayer; }
            set { isQueryLayer = value; }
        }

		private string layerCaption;
		/// <summary>
		/// 图层标题
		/// </summary>
		public string LayerCaption
		{
			get { return layerCaption; }
			set { layerCaption = value; }
		}
        private string parentGroupName;

        /// <summary>
        /// 图层父分组名称
        /// </summary>
        public string ParentGroupName
        {
            get
            {
                return parentGroupName;
            }

            set
            {
                parentGroupName = value;
            }
        }

        
              
    }
}
