using Map.NxApp.Common.Core;
using Map.NxApp.Common.Model;
using Map.NxApp.Common.VO;
using SuperMap.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace Map.NxApp
{
    /// <summary>
    /// MaskWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MaskShell : Window
    {
        /// 系统标题Key
        /// </summary>
        private string systemTitle = "SystemTitle";
        /// <summary>
        /// 查询字段名称Key
        /// </summary>
        private string queryNameField = "QueryNameFieldKey";
        /// <summary>
        /// 查询字段编码Key
        /// </summary>
        private string queryCodeField = "QueryCodeFieldKey";
        /// <summary>
		/// 追踪图层中属性或空间查询图标 tag 后缀
		/// </summary>
		private const string QUERY = "Query";
        private string[] orderByArray = {"行政区空间数据集",
                                        "交通路线空间数据集",
                                        "土地资源空间数据集",
                                        "功能区划空间数据集",
                                        "地形空间数据集",
                                        "基础设施空间数据集",
                                        "野生植物空间数据集",
                                        "野生动物空间数据集"};


        #region 图标高亮相关变量
        private GeoStyle normalStyle = null;
        private GeoStyle highLightStyle = null;
        private GeoPoint lastGPot = null;
        private string lastTag = "";
        private int lastIndex = -1;
        #endregion

        /// <summary>
		/// 初始化要素图标样式
		/// </summary>
		private void InitGeoPointParams(bool isReset = false)
        {
            if (isReset)
            {
                this.lastGPot = null;
                this.lastTag = "";
                this.lastIndex = -1;
            }
            else
            {
                GeoStyle sty2D = new GeoStyle();
                sty2D.MarkerSize = new Size2D(6, 6);
                sty2D.MarkerSymbolID = 1;
                normalStyle = sty2D;

                GeoStyle sty2DHigh = new GeoStyle();
                sty2DHigh.MarkerSize = new Size2D(6, 6);
                sty2DHigh.MarkerSymbolID = 2;
                highLightStyle = sty2DHigh;
            }
        }

        public MaskShell()
        {
            InitializeComponent();

            this.LayerTreeViewId.MaxHeight = SystemParameters.WorkArea.Height - 150;
            this.FeatureQueryPanelId.MaxHeight = SystemParameters.WorkArea.Height - 150;
            this.featureDetailId.MaxHeight = (SystemParameters.WorkArea.Height - 150 - 150) / 2;

            //Mask窗口Loaded事件
            this.Loaded += MaskShell_Loaded;
        }

        //Mask窗口的Loaded事件
        private void MaskShell_Loaded(object sender, RoutedEventArgs e)
        {
            //系统名称
            this.SystemTitleId.Text = ConfigurationManager.AppSettings.Get(systemTitle);

            this.LayerManagerPanelID.Height = this.Height - 100;

            //绑定地图图层数据
            this.bindMapLayerData();

            //初始化查询要素列表
            this.InitQueryLayerList();

            this.InitGeoPointParams();

            SmObjectLocator.getInstance().MapObject.MouseDown += MapObject_MouseDown;
            SmObjectLocator.getInstance().MapObject.MouseMove += MapObject_MouseMove;
        }

        /// <summary>
        /// 鼠标移动显示要素详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapObject_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.None)
                {
                    Point2D point2D = SmObjectLocator.getInstance().MapObject.Map.PixelToMap(e.Location);
                    int fid = SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.HitTest(point2D, 0);
                    if (fid > -1 && fid < SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.Count)
                    {
                        // 查询到对象，更换鼠标状态
                        SmObjectLocator.getInstance().MapObject.IsCursorCustomized = true;
                        SmObjectLocator.getInstance().MapObject.Cursor = System.Windows.Forms.Cursors.Arrow;
                    }
                    else
                    {
                        // 未查询到对象，更换鼠标状态
                        SmObjectLocator.getInstance().MapObject.IsCursorCustomized = true;
                        SmObjectLocator.getInstance().MapObject.Cursor = System.Windows.Forms.Cursors.Hand;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);
            }
        }

        /// <summary>
        /// 点击要素图标显示要素详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapObject_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    Point2D point2D = SmObjectLocator.getInstance().MapObject.Map.PixelToMap(e.Location);
                    int fid = SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.HitTest(point2D, 0);
                    if (fid > -1 && fid < SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.Count)
                    {
                        GeoPoint p = SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.Get(fid) as GeoPoint;

                        this.HighLightFeature(p, fid);

                        //反向定位要素項目
                        string tag = SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.GetTag(fid);
                        if (SysModelLocator.getInstance().recordList.Count > 0)
                        {
                            foreach (QueryRecordVO qrvo in SysModelLocator.getInstance().recordList)
                            {
                                string tempTag = string.Format("{0}#{1}#{2}", qrvo.RecordName, qrvo.RecordIndex, QUERY);
                                if (tempTag == tag)
                                {
                                    this.QueryResultListBoxId.SelectedItem = qrvo;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 初始化查询图层的列表
        /// </summary>
        private void InitQueryLayerList()
        {
            this.featureTypeListId.ItemsSource = SysModelLocator.getInstance().LayerList.Where(p => p.IsQueryLayer == true && p.LayerCaption.IndexOf("#") == -1);
            if (this.featureTypeListId.Items.Count > 0)
            {
                this.featureTypeListId.SelectedIndex = 0;
            }

        }

        /// <summary>
        /// 初始化地图图层数据
        /// </summary>
        private void bindMapLayerData()
        {
            ObservableCollection<LayerVO> ls = SysModelLocator.getInstance().LayerList;
            if (ls != null && ls.Count > 0)
            {
                //创建图层树
                ObservableCollection<TreeModel> treeModelCol = new ObservableCollection<TreeModel>();
                ObservableCollection<TreeModel> sortedTreeModelCol = new ObservableCollection<TreeModel>();
                for (int i = 0; i < ls.Count; i++)  
                {
                    LayerVO lvo = ls[i];
                    string tempParentName = lvo.ParentGroupName;
                    TreeModel parentNode = null;
                    TreeModel childNode = null;

                    //如果包含#则为待关联图层，不在图层树中显示,此处排除关联节点
                    if (lvo.LayerCaption.IndexOf("#") == -1)
                    {
                        if (!SysModelLocator.getInstance().LayerGroupDics.ContainsKey(tempParentName))
                        {
                            //创建一级节点
                            parentNode = new TreeModel();
                            parentNode.Name = lvo.ParentGroupName;
                            parentNode.IsExpanded = true;
                            //parentNode.IsChecked = true;
                            SysModelLocator.getInstance().LayerGroupDics.Add(lvo.ParentGroupName, parentNode);

                            //添加到图层模型对象
                            treeModelCol.Add(parentNode);
                        }
                        else
                        {
                            //查找一级节点
                            parentNode = SysModelLocator.getInstance().LayerGroupDics[tempParentName];
                        }

                        //创建二级节点
                        childNode = new TreeModel();
                        childNode.LayerVo = lvo;
                        childNode.Name = lvo.LayerName;
                        childNode.Id = lvo.LayerId;
                        childNode.IsChecked = lvo.LayerVisible;
                        childNode.Icon = "/Map.NxApp;component/Images/tree/layer.png";
                        childNode.Parent = parentNode;
                        parentNode.Children.Add(childNode);
                    }
                }

                //创建关联节点
                for (int j = 0; j < ls.Count; j++)
                {
                    LayerVO lvo = ls[j];
                    TreeModel linkParent = null;
                    TreeModel linkChild = null;
                    //如果包含#则为待关联图层，不在图层树中显示
                    if (lvo.LayerCaption.IndexOf("#") > -1)
                    {
                        //创建关联节点
                        linkChild = new TreeModel();
                        linkChild.LayerVo = lvo;
                        //作为关联节点，此处不能用处理过的LayerName，必须用完整的图层名。
                        linkChild.Name = lvo.LayerOrigin;
                        linkChild.Id = lvo.LayerId;
                    }

                    //查找二级节点作为关联节点的父节点
                    foreach (string key in SysModelLocator.getInstance().LayerGroupDics.Keys)
                    {
                        TreeModel firstNode = SysModelLocator.getInstance().LayerGroupDics[key];
                        IList<TreeModel> secondNodeList = firstNode.Children;
                        if (secondNodeList != null && secondNodeList.Count > 0)
                        {
                            foreach (TreeModel item in secondNodeList)
                            {
                                if (lvo.LayerCaption.IndexOf(item.LayerVo.LayerCaption) > -1 && item.LayerVo.LayerCaption.IndexOf("#") == -1)
                                {
                                    linkParent = item;
                                    break;
                                }
                            }
                            if (linkParent != null)
                                break;
                        }
                    }

                    //排除空节点
                    if (linkParent != null && linkChild != null)
                    {
                        //追加到二级节点的关联节点列表中
                        linkParent.LinkItems.Add(linkChild);
                    }
                }

                //先根据人为规则排序
                for (int i = 0; i < this.orderByArray.Length; i++)
                {
                    foreach (TreeModel item in treeModelCol)
                    {
                        if (item.Name==orderByArray[i].ToString())
                        {
                            sortedTreeModelCol.Add(item);
                            break;//跳出内嵌循环，不影响外围循环。
                        }
                    }
                }
                //绑定排序之后的数组
                this.LayerTreeViewId.ItemsSourceData = sortedTreeModelCol;
            }
        }

        private void CloseSysBtn_Click(object sender, RoutedEventArgs e)
        {
            //SmObjectLocator.getInstance().MapObject.Map.Close();
            SmObjectLocator.getInstance().MapObject.Dispose();
            //SmObjectLocator.getInstance().WkSpaceObject.Close();
            SmObjectLocator.getInstance().WkSpaceObject.Dispose();
            Application.Current.Shutdown(-1);
        }

        /// <summary>
        /// 切换登录面板，默认未登录-false，已登录-true
        /// </summary>
        /// <param name="isLogined"></param>
        private void switchLoginPanel(bool isLogined = false)
        {
            if (isLogined)
            {
                //已登录
                this.LogInedPanelFactor.Height = new GridLength(1, GridUnitType.Star);
                this.LogInPanelFactor.Height = new GridLength(0);
                this.LogInedPanelID.Visibility = Visibility.Visible;
                this.LogInPanelID.Visibility = Visibility.Collapsed;
            }
            else
            {
                //待登录
                this.LogInedPanelFactor.Height = new GridLength(0);
                this.LogInPanelFactor.Height = new GridLength(1, GridUnitType.Star);
                this.LogInedPanelID.Visibility = Visibility.Collapsed;
                this.LogInPanelID.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 点击登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogInBtn_Click(object sender, RoutedEventArgs e)
        {
            this.switchLoginPanel(true);
        }

        /// <summary>
        /// 点击注销
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            this.switchLoginPanel(false);
        }

        /// <summary>
        /// 显示用户中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserCenterID_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            this.UeserCenterPanelID.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 关闭用户中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseUserCenterPanelID_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.UeserCenterPanelID.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 显示图层中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayerManagerCenterID_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.LayerManagerPanelID.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 关闭图层中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseLayerManagerPanelID_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.LayerManagerPanelID.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 关闭要素查询中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseFeatureQueryPanelID_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.QueryFeaturePanelID.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 显示要素查询中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queryFeatureCenterID_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.QueryFeaturePanelID.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 执行要素关键字查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void executeQueryBtnId_Click(object sender, RoutedEventArgs e)
        {
            this.resetResultPanel();

            this.InitGeoPointParams(true);

            this.executeQuery();
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        private void executeQuery()
        {
            SysModelLocator.getInstance().recordList.Clear();

            //数据源名称
            string dSourceName = "";
            //数据集名称
            string dSetName = "";
            if (this.featureTypeListId.Items.Count > 0)
            {
                dSourceName = this.featureTypeListId.SelectedValue.ToString();
                dSetName = this.featureTypeListId.Text.ToString();
            }
            //获取数据源
            Datasource dSource = SmObjectLocator.getInstance().WkSpaceObject.Datasources[dSourceName];
            if (dSource != null)
            {
                DatasetVector dSetV = (DatasetVector)dSource.Datasets[dSetName];
                if (dSetV != null)
                {
                    //查询字段
                    string queryFieldName = ConfigurationManager.AppSettings.Get(queryNameField);
                    string QueryFieldCode = ConfigurationManager.AppSettings.Get(queryCodeField);
                    Recordset recordset = null;
                    //关键字
                    string keyword = this.keywordId.Text.Trim();
                    QueryParameter queryParameter = new QueryParameter();
                    queryParameter.CursorType = SuperMap.Data.CursorType.Static;
                    queryParameter.HasGeometry = true;
                    if (keyword != "")
                    {
                        queryParameter.AttributeFilter = queryFieldName + " like '%" + keyword + "%'";
                    }

                    recordset = dSetV.Query(queryParameter);

                    if (recordset != null && recordset.RecordCount > 0)
                    {
                        ObservableCollection<QueryRecordVO> recordList = SysModelLocator.getInstance().recordList;
                        recordList.Clear();
                        bool isExist = false;
                        FieldInfos fis = recordset.GetFieldInfos();
                        for (int j = 0; j < fis.Count; j++)
                        {
                            FieldInfo fi = fis[j];
                            if (fi != null)
                            {
                                if (fi.Name.ToString().ToUpper() == queryFieldName)
                                {
                                    isExist = true;
                                    break;
                                }
                                continue;
                            }
                        }

                        if (isExist)
                        {
                            for (recordset.MoveFirst(); recordset.IsEOF == false; recordset.MoveNext())
                            {
                                QueryRecordVO qVO = new QueryRecordVO();
                                qVO.RecordLayerId = dSetName;
                                if (recordset.GetFieldValue(queryFieldName) != null)
                                {
                                    qVO.RecordName = recordset.GetFieldValue(queryFieldName).ToString();
                                }
                                else
                                {
                                    qVO.RecordName = "";
                                }

                                qVO.RecordIndex = recordset.GetFieldValue(QueryFieldCode).ToString();
                                qVO.RecordCenterX = recordset.GetGeometry().InnerPoint.X.ToString();
                                qVO.RecordCenterY = recordset.GetGeometry().InnerPoint.Y.ToString();
                                recordList.Add(qVO);
                            }
                            this.QueryResultListBoxId.ItemsSource = recordList;
                            this.queryInfoId.Text = "结果合计：" + recordList.Count + "条";

                            this.InitQueryListOnMap(recordset);
                        }
                        else
                        {
                            this.QueryResultListBoxId.ItemsSource = null;
                            this.queryInfoId.Text = "默认查询选定类型的全部要素";
                        }
                    }
                    else
                    {
                        SysModelLocator.getInstance().recordList.Clear();
                        this.QueryResultListBoxId.ItemsSource = null;
                        this.queryInfoId.Text = "查询结果合计：0条";
                    }
                }
            }
        }

        /// <summary>
        /// 根据查询结果调整界面布局
        /// </summary>
        /// <param name="isShowDetail"></param>
        private void resetResultPanel(bool isShowDetail = false)
        {
            this.featureListFactor.Height = new GridLength(1, GridUnitType.Star);
            if (isShowDetail)
            {
                this.featureDetailFactor.Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                this.featureDetailFactor.Height = new GridLength(0);
            }
        }

        /// <summary>
        /// 在地图上初始化显示查询结果列表
        /// </summary>
        private void InitQueryListOnMap(Recordset rs)
        {
            //清空追踪图层数据
            SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.Clear();

            ObservableCollection<QueryRecordVO> recordList = SysModelLocator.getInstance().recordList;
            if (recordList.Count > 0)
            {
                double centerX = 0.0;
                double centerY = 0.0;
                for (int i = 0; i < recordList.Count; i++)
                {
                    QueryRecordVO vo = vo = recordList[i];
                    if (Double.TryParse(vo.RecordCenterX, out centerX) == true && Double.TryParse(vo.RecordCenterY, out centerY) == true)
                    {
                        GeoPoint gpt = new GeoPoint(centerX, centerY);
                        gpt.Style = normalStyle;
                        SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.Add(gpt, string.Format("{0}#{1}#{2}", vo.RecordName, vo.RecordIndex, QUERY));
                    }
                }
                if (rs != null)
                {
                    SmObjectLocator.getInstance().MapObject.Map.EnsureVisible(rs);
                }
                SmObjectLocator.getInstance().MapObject.Map.Refresh();
            }
        }


        /// <summary>
        /// 高亮显示要素并显示要素详情
        /// </summary>
        /// <param name="p"></param>
        /// <param name="fid"></param>
        private void HighLightFeature(GeoPoint p, int fid)
        {
            if (p != null)
            {
                string tag = SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.GetTag(fid);
                if (this.lastGPot == null)
                {
                    this.lastGPot = p;
                    this.lastTag = tag;
                    this.lastIndex = fid;
                }
                else if (p != this.lastGPot)
                {
                    this.lastGPot.Style = normalStyle;
                    SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.Set(this.lastIndex, this.lastGPot);
                    SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.SetTag(this.lastIndex, this.lastTag);
                }

                p.Style = highLightStyle;
                SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.Set(fid, p);
                SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.SetTag(fid, tag);

                this.lastGPot = p;
                this.lastTag = tag;
                this.lastIndex = fid;

                //显示要素详情
                this.ShowMarkDetailInfo(tag);

                SmObjectLocator.getInstance().MapObject.Map.Refresh();
            }
        }

        /// <summary>
        /// 显示mark的详细信息属性窗
        /// </summary>
        private void ShowMarkDetailInfo(string tag)
        {
            if (this.QueryResultListBoxId.Items.Count > 0)
            {
                //数据源名称
                string dSourceName = "";
                //数据集名称
                string dSetName = "";
                if (this.featureTypeListId.Items.Count > 0)
                {
                    dSourceName = this.featureTypeListId.SelectedValue.ToString();
                    dSetName = this.featureTypeListId.Text.ToString();
                }

                //获取数据源
                Datasource dSource = SmObjectLocator.getInstance().WkSpaceObject.Datasources[dSourceName];
                if (dSource != null)
                {
                    DatasetVector dSetV = (DatasetVector)dSource.Datasets[dSetName];

                    if (dSetV != null)
                    {
                        string fieldName = ConfigurationManager.AppSettings.Get(queryNameField);
                        string fieldCode = ConfigurationManager.AppSettings.Get(queryCodeField);
                        QueryParameter queryParameter = new QueryParameter();
                        queryParameter.CursorType = SuperMap.Data.CursorType.Static;
                        queryParameter.HasGeometry = true;
                        string[] tempArr = tag.Split('#');
                        queryParameter.AttributeFilter = fieldName + " = '" + tempArr[0] + "' and " + fieldCode + " = '" + tempArr[1] + "'";

                        Recordset recordset = dSetV.Query(queryParameter);
                        if (recordset != null && recordset.RecordCount > 0)
                        {
                            bool isExist = false;
                            FieldInfos fis = recordset.GetFieldInfos();
                            for (int i = 0; i < fis.Count; i++)
                            {
                                FieldInfo fi = fis[i];
                                if (fi != null)
                                {
                                    if (fi.Name.ToString().ToUpper() == fieldName)
                                    {
                                        isExist = true;
                                        break;
                                    }
                                    continue;
                                }
                            }

                            if (isExist)
                            {
                                ObservableCollection<DetailVO> detailList = new ObservableCollection<DetailVO>();
                                for (recordset.MoveFirst(); recordset.IsEOF == false; recordset.MoveNext())
                                {
                                    FieldInfos fs = recordset.GetFieldInfos();
                                    for (int j = 0; j < fs.Count; j++)
                                    {
                                        FieldInfo fi = fs[j];
                                        if (fi != null && fi.Name.ToLower().IndexOf("sm") == -1)
                                        {
                                            DetailVO dv = new DetailVO();
                                            if (fi.Name != null)
                                            {
                                                dv.FeatureField = fi.Name;
                                                if (recordset.GetFieldValue(fi.Name) != null)
                                                {
                                                    dv.FeatureValue = recordset.GetFieldValue(fi.Name).ToString();
                                                    if (fi.Name.ToString().ToUpper() == fieldName)
                                                    {
                                                        this.featureTitle.Text = recordset.GetFieldValue(fieldName).ToString();
                                                    }
                                                }
                                                detailList.Add(dv);
                                            }
                                        }
                                    }
                                }
                                this.featureDetailId.ItemsSource = detailList;
                                this.resetResultPanel(true);
                            }

                        }
                    }
                }

            }
        }

        /// <summary>
        /// 清除查询结果
        /// </summary>
        private void ClearQueryResult()
        {
            this.InitGeoPointParams(true);

            //清空结果数据
            SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.Clear();
            SmObjectLocator.getInstance().MapObject.Map.Refresh();

            this.QueryResultListBoxId.ItemsSource = null;
            this.queryInfoId.Text = "默认查询选定类型的全部要素";
            this.keywordId.Text = "";
            this.featureTitle.Text = "";
            this.featureDetailId.ItemsSource = null;

            this.resetResultPanel();
        }

        /// <summary>
        /// 选定指定的要素，高亮并显示要素详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultListBoxId_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (sender is RadListBox)
            {
                if (this.QueryResultListBoxId.SelectedItem != null)
                {
                    try
                    {
                        QueryRecordVO qVO = this.QueryResultListBoxId.SelectedItem as QueryRecordVO;
                        double lat = Convert.ToDouble(qVO.RecordCenterX.Trim());
                        double lon = Convert.ToDouble(qVO.RecordCenterY.Trim());
                        if (!Double.IsNaN(lat) && !Double.IsNaN(lon))
                        {
                            GeoPoint gp = new GeoPoint(new Point2D(lat, lon));
                            SmObjectLocator.getInstance().MapObject.Map.EnsureVisible(gp);
                        }

                        //高亮要素，显示详情
                        int fid = SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.IndexOf(string.Format("{0}#{1}#{2}", qVO.RecordName, qVO.RecordIndex, QUERY));
                        if (fid >= 0)
                        {
                            GeoPoint p = SmObjectLocator.getInstance().MapObject.Map.TrackingLayer.Get(fid) as GeoPoint;
                            this.HighLightFeature(p, fid);
                        }

                        SmObjectLocator.getInstance().MapObject.Map.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "要素坐标错误！");
                    }
                }
            }
        }

        /// <summary>
        /// 切换要素类型，重置条件及结果面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void featureTypeListId_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.ClearQueryResult();
        }

        /// <summary>
        /// 重置地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearSystemID_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.ClearQueryResult();
            SmObjectLocator.getInstance().MapObject.Map.ViewEntire();
        }
    }
}
