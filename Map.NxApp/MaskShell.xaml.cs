using Map.NxApp.Common.Core;
using Map.NxApp.Common.Model;
using Map.NxApp.Common.VO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;

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


        public MaskShell()
        {
            InitializeComponent();

            this.LayerTreeViewId.MaxHeight = SystemParameters.WorkArea.Height - 150;
            this.FeatureQueryPanelId.MaxHeight = SystemParameters.WorkArea.Height - 150;

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
        }

        /// <summary>
        /// 初始化地图图层数据
        /// </summary>
        private void bindMapLayerData()
        {
            ObservableCollection<LayerVO> ls = SysModelLocator.getInstance().LayerList;
            if (ls != null && ls.Count > 0)
            {
                //设置要素查询类型
                this.featureTypeListId.ItemsSource = ls;
                this.QueryResultListBoxId.ItemsSource = ls;
                //if (this.featureTypeListId.Items.Count > 0)
                //{
                //    this.featureTypeListId.SelectedIndex = 0;
                //}

                //创建图层树
                ObservableCollection<TreeModel> treeModelCol = new ObservableCollection<TreeModel>();
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
                this.LayerTreeViewId.ItemsSourceData = treeModelCol;
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
        /// 显示系统管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemCenterID_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

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
    }
}
