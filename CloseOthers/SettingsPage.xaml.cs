using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared.Helpers;
using System;
using System.Collections.Generic;
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
using System.IO;

namespace CloseOthers
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    [SettingsPageInfo("CloseOthers.SettingsPage", "关闭同类软件 (选项)")]
    public partial class SettingsPage : SettingsPageBase
    {
        public Settings Settings { get; set; } = new();

        public SettingsPage()
        {
            InitializeComponent();

            Datas datas = new();
            CW.IsChecked = Datas.Settings.CW;
            TimeNest.IsChecked = Datas.Settings.TimeNest;

            CW.Checked += (e, o) =>
            {
                Datas.Settings.CW = (bool)CW.IsChecked;
                ConfigureFileHelper.SaveConfig<Settings>(Path.Combine(datas.GetPluginConfigFolder(), "Settings.json"), Datas.Settings);
            };
            CW.Unchecked += (e, o) =>
            {
                Datas.Settings.CW = (bool)CW.IsChecked;
                ConfigureFileHelper.SaveConfig<Settings>(Path.Combine(datas.GetPluginConfigFolder(), "Settings.json"), Datas.Settings);
            };
            TimeNest.Checked += (e, o) =>
            {
                Datas.Settings.TimeNest = (bool)TimeNest.IsChecked;
                ConfigureFileHelper.SaveConfig<Settings>(Path.Combine(datas.GetPluginConfigFolder(), "Settings.json"), Datas.Settings);
            };
            TimeNest.Unchecked += (e, o) =>
            {
                Datas.Settings.TimeNest = (bool)TimeNest.IsChecked;
                ConfigureFileHelper.SaveConfig<Settings>(Path.Combine(datas.GetPluginConfigFolder(), "Settings.json"), Datas.Settings);
            };
            CheckAll.Click += (e, o) =>
            {
                CW.IsChecked = true;
                TimeNest.IsChecked = true;
            };
        }
    }
}
