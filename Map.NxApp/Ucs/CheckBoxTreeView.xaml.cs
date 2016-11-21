using Map.NxApp.Common.Core;
using Map.NxApp.Common.Model;
using Map.NxApp.Common.VO;
using SuperMap.Mapping;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Map.NxApp.Ucs
{
    /// <summary>
    /// ZsmTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class CheckBoxTreeView : UserControl
    {
        #region 私有变量属性
        /// <summary>
        /// 控件数据
        /// </summary>
        private IList<TreeModel> _itemsSourceData;
        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public CheckBoxTreeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 控件数据
        /// </summary>
        public IList<TreeModel> ItemsSourceData
        {
            get { return _itemsSourceData; }
            set
            {
                _itemsSourceData = value;
                checkboxTreeId.ItemsSource = _itemsSourceData;
            }
        }

        /// <summary>
        /// 设置对应Id的项为选中状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetCheckedById(string id, IList<TreeModel> treeList)
        {
            foreach (var tree in treeList)
            {
                if (tree.Id.Equals(id))
                {
                    tree.IsChecked = true;
                    return 1;
                }
                if (SetCheckedById(id, tree.Children) == 1)
                {
                    return 1;
                }
            }

            return 0;
        }
        /// <summary>
        /// 设置对应Id的项为选中状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetCheckedById(string id)
        {
            foreach (var tree in ItemsSourceData)
            {
                if (tree.Id.Equals(id))
                {
                    tree.IsChecked = true;
                    return 1;
                }
                if (SetCheckedById(id, tree.Children) == 1)
                {
                    return 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// 获取选中项
        /// </summary>
        /// <returns></returns>
        public IList<TreeModel> CheckedItemsIgnoreRelation()
        {

            return GetCheckedItemsIgnoreRelation(_itemsSourceData);
        }

        /// <summary>
        /// 私有方法，忽略层次关系的情况下，获取选中项
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private IList<TreeModel> GetCheckedItemsIgnoreRelation(IList<TreeModel> list)
        {
            IList<TreeModel> treeList = new List<TreeModel>();
            foreach (var tree in list)
            {
                if (tree.IsChecked)
                {
                    treeList.Add(tree);
                }
                foreach (var child in GetCheckedItemsIgnoreRelation(tree.Children))
                {
                    treeList.Add(child);
                }
            }
            return treeList;
        }

        /// <summary>
        /// 选中所有子项菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSelectAllChild_Click(object sender, RoutedEventArgs e)
        {
            if (checkboxTreeId.SelectedItem != null)
            {
                TreeModel tree = (TreeModel)checkboxTreeId.SelectedItem;
                tree.IsChecked = true;
                tree.SetChildrenChecked(true);

                this.controlMapLayer(tree);
            }
        }

        /// <summary>
        /// 取消所有子项菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUnSelectAllChild_Click(object sender, RoutedEventArgs e)
        {
            if (checkboxTreeId.SelectedItem != null)
            {
                TreeModel tree = (TreeModel)checkboxTreeId.SelectedItem;
                tree.IsChecked = false;
                tree.SetChildrenChecked(false);

                this.controlMapLayer(tree);
            }
        }

        /// <summary>
        /// 定位图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void locationMenu_Click(object sender, RoutedEventArgs e)
        {
            if (checkboxTreeId.SelectedItem != null)
            {
                TreeModel tree = (TreeModel)checkboxTreeId.SelectedItem;
                if (tree.IsChecked && tree.Parent != null)
                {
                    this.locatorMapLayer(tree);
                }
            }
        }

        /// <summary>
        /// 全部展开菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuExpandAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeModel tree in checkboxTreeId.ItemsSource)
            {
                tree.IsExpanded = true;
                tree.SetChildrenExpanded(true);
            }
        }

        /// <summary>
        /// 全部折叠菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUnExpandAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeModel tree in checkboxTreeId.ItemsSource)
            {
                tree.IsExpanded = false;
                tree.SetChildrenExpanded(false);
            }
        }

        /// <summary>
        /// 全部选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeModel tree in checkboxTreeId.ItemsSource)
            {
                tree.IsChecked = true;
                tree.SetChildrenChecked(true);

                this.controlMapLayer(tree);
            }
        }

        /// <summary>
        /// 全部取消选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeModel tree in checkboxTreeId.ItemsSource)
            {
                tree.IsChecked = false;
                tree.SetChildrenChecked(false);

                this.controlMapLayer(tree);
            }
        }

        /// <summary>
        /// 鼠标右键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (item != null)
            {
                item.Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        /// 逐级向上查找父节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (item != null)
            {
                item.Focus();
                eachCheckedNode();
                e.Handled = true;
            }
        }

        private void eachCheckedNode()
        {
            if (checkboxTreeId.SelectedItem != null)
            {
                TreeModel tree = (TreeModel)checkboxTreeId.SelectedItem;
                tree.SetChildrenChecked(tree.IsChecked);
                tree.SetParentChecked(tree.IsChecked);

                this.controlMapLayer(tree);
            }
        }

        /// <summary>
        /// 系统初始化时选中状态发生变化时，自动选中其父子节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (item != null)
            {
                //当前节点
                TreeModel c = item.DataContext as TreeModel;
                //一级节点
                TreeModel p = c.Parent;

                //子节点
                if (p != null)
                {
                    //控制重复执行选中
                    if (p.IsChecked == false && c.IsChecked)
                    {
                        c.SetChildrenChecked(c.IsChecked);
                        c.SetParentChecked(c.IsChecked);
                    }
                }
                else //一级节点
                {
                    if (c.IsChecked)
                    {
                        c.SetChildrenChecked(c.IsChecked);
                    }
                }
                this.controlMapLayer(c);
            }
            e.Handled = true;
        }

        /// <summary>
        /// 定位到指定图层
        /// </summary>
        /// <param name="treeModel"></param>
        private void locatorMapLayer(TreeModel treeModel)
        {
            if (treeModel != null)
            {
                Layers ls = SmObjectLocator.getInstance().MapObject.Map.Layers;
                if (ls != null && ls.Count > 0)
                {
                    foreach (Layer layer in ls)
                    {
                        if (layer.Caption == treeModel.LayerVo.LayerCaption)
                        {
                            SmObjectLocator.getInstance().MapObject.Map.EnsureVisible(layer);
                            SmObjectLocator.getInstance().MapObject.Map.Refresh();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 图层控制器
        /// </summary>
        /// <param name="treeModel"></param>
        private void controlMapLayer(TreeModel treeModel)
        {
            ObservableCollection<LayerVO> ls = SysModelLocator.getInstance().LayerList;
            if (ls != null && ls.Count > 0)
            {
                //选中一级节点，控制子节点图层显隐
                if (treeModel.Parent == null)
                {
                    foreach (TreeModel child in treeModel.Children)
                    {
                        this.switchMapLayer(child, child.IsChecked);
                    }
                }
                else
                {
                    //子节点，控制其图层显隐
                    this.switchMapLayer(treeModel, treeModel.IsChecked);
                }
                SmObjectLocator.getInstance().MapObject.Map.Refresh();
            }
        }

        /// <summary>
        /// 图层显隐切换器
        /// </summary>
        /// <param name="treeModel"></param>
        /// <param name="isVisible"></param>
        private void switchMapLayer(TreeModel treeModel, bool isVisible)
        {
            Layers ls = SmObjectLocator.getInstance().MapObject.Map.Layers;
            if (ls != null && ls.Count > 0)
            {
                foreach (Layer layer in ls)
                {
                    if (layer.Caption == treeModel.LayerVo.LayerCaption)
                    {
                        layer.IsVisible = isVisible;
                        break;
                    }
                }
            }
        }
    }
}
