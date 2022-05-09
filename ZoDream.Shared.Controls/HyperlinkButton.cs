using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZoDream.Shared.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Studio.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Studio.Controls;assembly=ZoDream.Studio.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:HyperlinkButton/>
    ///
    /// </summary>
    public class HyperlinkButton : ButtonBase
    {
        static HyperlinkButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HyperlinkButton), new FrameworkPropertyMetadata(typeof(HyperlinkButton)));
        }

        public Uri NavigateUri
        {
            get { return (Uri)GetValue(NavigateUriProperty); }
            set { SetValue(NavigateUriProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigateUri.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigateUriProperty =
            DependencyProperty.Register("NavigateUri", typeof(Uri), typeof(HyperlinkButton), new PropertyMetadata(null));



        public string TargetName
        {
            get { return (string)GetValue(TargetNameProperty); }
            set { SetValue(TargetNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetNameProperty =
            DependencyProperty.Register("TargetName", typeof(string), typeof(HyperlinkButton), new PropertyMetadata(string.Empty));




        public bool UseSystemFocusVisuals
        {
            get { return (bool)GetValue(UseSystemFocusVisualsProperty); }
            set { SetValue(UseSystemFocusVisualsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseSystemFocusVisuals.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseSystemFocusVisualsProperty =
            DependencyProperty.Register("UseSystemFocusVisuals", typeof(bool), typeof(HyperlinkButton), new PropertyMetadata(true));



        public Thickness FocusVisualMargin
        {
            get { return (Thickness)GetValue(FocusVisualMarginProperty); }
            set { SetValue(FocusVisualMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FocusVisualMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FocusVisualMarginProperty =
            DependencyProperty.Register("FocusVisualMargin", typeof(Thickness), typeof(HyperlinkButton), new PropertyMetadata(new Thickness()));



        protected override void OnClick()
        {
            
            if (NavigateUri.IsAbsoluteUri && NavigateUri.Scheme.Contains("http", StringComparison.OrdinalIgnoreCase))
            {
                Process.Start(new ProcessStartInfo(NavigateUri.ToString())
                {
                    UseShellExecute = true
                });
            }
            base.OnClick();
        }

    }
}
