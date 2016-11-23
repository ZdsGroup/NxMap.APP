using Map.NxApp.Common.Core;
using Map.NxApp.Common.Helpers.Themes;
using Map.NxApp.Common.Helpers.Windows;
using Map.NxApp.Common.VO;
using SuperMap.Data;
using SuperMap.Mapping;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Map.NxApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Mask窗口
        /// </summary>
        private MaskShell maskShell = null;

        //工作空间路径
        private string wksPathKey = "wksPath";

        //地图名称
        private string mapNameKey = "mapName";

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;

            this.Deactivated += MainWindow_Deactivated;
            this.Activated += MainWindow_Activated;
            this.StateChanged += MainWindow_StateChanged;
        }

        #region 事件逻辑
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //默认加载的系统主题
            ApplicationThemeManager.GetInstance().EnsureResourcesForTheme(ApplicationThemeManager.DefaultThemeName);

            //初始化地图窗口
            this.initMapObject();

            //加载三维场景
            string wksPath = ConfigurationManager.AppSettings.GetValues(wksPathKey)[0];
            string mapName = ConfigurationManager.AppSettings.GetValues(mapNameKey)[0];
            this.openMap(wksPath, mapName);
            System.Windows.Forms.Application.DoEvents();

            this.InitMaskShell();

            WindowsHelper winHelper = new WindowsHelper();
            winHelper.adjustWindow(this, this.maskShell, false);
        }


        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //SmObjectLocator.getInstance().MapObject.Map.Close();
            SmObjectLocator.getInstance().MapObject.Dispose();
            //SmObjectLocator.getInstance().WkSpaceObject.Close();
            SmObjectLocator.getInstance().WkSpaceObject.Dispose();
            Application.Current.Shutdown(-1);
        }

        /// <summary>
        /// 实现mask面板状态与主面板状态同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.maskShell != null)
            {
                if (this.WindowState == System.Windows.WindowState.Maximized ||
                    this.WindowState == System.Windows.WindowState.Normal)
                {
                    this.maskShell.Topmost = true;
                    this.maskShell.WindowState = System.Windows.WindowState.Maximized;
                }
                else if (this.WindowState == System.Windows.WindowState.Minimized)
                {
                    this.maskShell.Topmost = false;
                    this.maskShell.WindowState = System.Windows.WindowState.Minimized;
                }
            }
        }

        void MainWindow_Activated(object sender, EventArgs e)
        {
            if (this.maskShell != null)
            {
                this.maskShell.Topmost = true;
            }
        }

        void MainWindow_Deactivated(object sender, EventArgs e)
        {
            if (this.maskShell != null)
            {
                this.maskShell.Topmost = false;
            }
        }
        #endregion

        #region 界面逻辑
        private void initMapObject()
        {
            SmObjectLocator.getInstance().MapObject.Action = SuperMap.UI.Action.Pan;
            this.hostMapControl.Child = SmObjectLocator.getInstance().MapObject;

            SmObjectLocator.getInstance().MapObject.MouseClick += MapObject_MouseClick;
        }

        /// <summary>
        /// 禁用鼠标右键选择功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapObject_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                SmObjectLocator.getInstance().MapObject.Action = SuperMap.UI.Action.Pan;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                SmObjectLocator.getInstance().MapObject.Action = SuperMap.UI.Action.Pan;
            }
        }

        /// <summary>
        /// 打开工作空间中的地图
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mapName"></param>
        private void openMap(string path, string mapName)
        {
            WorkspaceConnectionInfo conInfo = new WorkspaceConnectionInfo(path);
            conInfo.Type = WorkspaceType.SMWU;
            bool isOpened = SmObjectLocator.getInstance().WkSpaceObject.Open(conInfo);
            if (isOpened)
            {
                SmObjectLocator.getInstance().MapObject.Map.Open(mapName);
                Layers ls = SmObjectLocator.getInstance().MapObject.Map.Layers;
                if (ls != null && ls.Count > 0)
                {
                    for (int i = 0; i < ls.Count; i++)
                    {
                        Layer layer = ls[i];
                        layer.IsSelectable = false;
                        layer.IsEditable = false;
                        layer.IsSnapable = false;
                        LayerVO layerVo = new LayerVO();
                        layerVo.LayerBounds = layer.Bounds;
                        layerVo.LayerCenter = layer.Bounds.Center;
                        layerVo.LayerName = layer.Caption.Substring(0, layer.Caption.IndexOf("@"));
                        string tempParentName = layer.Caption.Substring(layer.Caption.IndexOf("@") + 1);
                        if (tempParentName.IndexOf("#")>-1)
                        {
                            layerVo.ParentGroupName = tempParentName.Substring(0, tempParentName.IndexOf("#")) ;
                        }
                        else
                        {
                            layerVo.ParentGroupName = tempParentName;
                        }
                        layerVo.LayerId = i.ToString();
                        layerVo.LayerVisible = layer.IsVisible;
                        layerVo.IsQueryLayer = true;
                        layerVo.LayerCaption = layer.Caption;
                        layerVo.LayerOrigin = layer.Name;
                        SysModelLocator.getInstance().LayerList.Add(layerVo);
                    }
                }
            }
        }

        /// <summary>
        /// 初始化遮罩
        /// </summary>
        private void InitMaskShell()
        {
            if (maskShell == null)
            {
                maskShell = new MaskShell();
                maskShell.Owner = this;
                maskShell.WindowState = WindowState.Maximized;
            }

            maskShell.Show();
        }
        #endregion
    }
}
