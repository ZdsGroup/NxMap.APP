using Map.NxApp.Common.VO;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Map.NxApp.Common.Model
{
    public class TreeModel : INotifyPropertyChanged
    {
        #region 私有变量
        /// <summary>
        /// Id值
        /// </summary>
        private string _id;
        /// <summary>
        /// 显示的名称
        /// </summary>
        private string _name;
        /// <summary>
        /// 图标路径
        /// </summary>
        private string _icon;
        /// <summary>
        /// 选中状态
        /// </summary>
        private bool _isChecked;
        /// <summary>
        /// 折叠状态
        /// </summary>
        private bool _isExpanded;
        /// <summary>
        /// 子项
        /// </summary>
        private IList<TreeModel> _children;
        /// <summary>
        /// 父项
        /// </summary>
        private TreeModel _parent;
        /// <summary>
        /// 关联项目，不在图层中显示
        /// </summary>
        private IList<TreeModel> _linkItems;
        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public TreeModel()
        {
            Children = new List<TreeModel>();
            LinkItems = new List<TreeModel>();
            _isChecked = false;
            IsExpanded = false;
            _icon = "/Map.NxApp;component/Images/tree/layergroup.png";
        }

        /// <summary>
        /// 键值
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// 显示的字符
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        /// <summary>
        /// 指针悬停时的显示说明
        /// </summary>
        public string ToolTip
        {
            get
            {
                return String.Format("{0}", Name);
            }
        }

        /// <summary>  
        /// 设置所属父节点的选中状态  
        /// </summary>  
        /// <param name="isChecked"></param>  
        public void SetParentChecked(bool isChecked)
        {
            if (Parent != null)
            {
                if (isChecked && !Parent.IsChecked)
                {
                    Parent.IsChecked = IsChecked;
                    Parent.SetParentChecked(isChecked);
                }
                else
                {
                    if (!isChecked && !Parent.getChildrenChecked())
                    {
                        Parent.IsChecked = IsChecked;
                        Parent.SetParentChecked(isChecked);
                    }
                }
            }
        }

        /// <summary>  
        /// 获取所有子项的选中状态  
        /// </summary>  
        /// <return value>true表示有选中的，false表示没有选中的</teturn>  
        public bool getChildrenChecked()
        {
            bool value = false;
            foreach (TreeModel child in Children)
            {
                if (child.IsChecked)
                {
                    value = true;
                    break;
                }
            }
            return value;
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    NotifyPropertyChanged("IsChecked");

                    //if (_isChecked)
                    //{
                    //    //如果选中则父项也应该选中
                    //    if (Parent != null)
                    //    {
                    //        Parent.IsChecked = true;
                    //    }
                    //}
                    //else
                    //{
                    //    //如果取消选中子项也应该取消选中
                    //    foreach (TreeModel child in Children)
                    //    {
                    //        child.IsChecked = false;
                    //    }
                    //}
                }
            }
        }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    //折叠状态改变
                    _isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        /// <summary>
        /// 父项
        /// </summary>
        public TreeModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>
        /// 子项
        /// </summary>
        public IList<TreeModel> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        /// <summary>
        /// 设置所有子项的选中状态
        /// </summary>
        /// <param name="isChecked"></param>
        public void SetChildrenChecked(bool isChecked)
        {
            foreach (TreeModel child in Children)
            {
                child.IsChecked = IsChecked;
                child.SetChildrenChecked(IsChecked);
            }
        }

        /// <summary>
        /// 设置所有子项展开状态
        /// </summary>
        /// <param name="isExpanded"></param>
        public void SetChildrenExpanded(bool isExpanded)
        {
            foreach (TreeModel child in Children)
            {
                child.IsExpanded = isExpanded;
                child.SetChildrenExpanded(isExpanded);
            }
        }

        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private LayerVO layerVo = null;
        /// <summary>
        /// 节点对应的图层信息
        /// </summary>
        public LayerVO LayerVo
        {
            get
            {
                return layerVo;
            }

            set
            {
                layerVo = value;
            }
        }

        /// <summary>
        /// 节点关联节点列表，与子节点无关
        /// </summary>
        public IList<TreeModel> LinkItems
        {
            get
            {
                return _linkItems;
            }

            set
            {
                _linkItems = value;
            }
        }
    }
}
