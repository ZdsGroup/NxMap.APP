using Map.NxApp.Common.Core;
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

			//Mask窗口Loaded事件
			this.Loaded += MaskShell_Loaded;
		}

		//Mask窗口的Loaded事件
		private void MaskShell_Loaded(object sender, RoutedEventArgs e)
		{
            //系统名称
            this.SystemTitleId.Text = ConfigurationManager.AppSettings.Get(systemTitle);

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
        private void switchLoginPanel(bool isLogined=false)
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
    }
}
