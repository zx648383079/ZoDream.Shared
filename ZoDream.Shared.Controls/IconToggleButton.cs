using System;
using System.Collections.Generic;
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
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Shared.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Shared.Controls;assembly=ZoDream.Shared.Controls"
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
    ///     <MyNamespace:IconToggleButton/>
    ///
    /// </summary>
    [TemplatePart(Name = LabelTbName, Type = typeof(TextBlock))]
    public class IconToggleButton : ButtonBase
    {
        public const string LabelTbName = "PART_LabelTb";
        static IconToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconToggleButton), new FrameworkPropertyMetadata(typeof(IconToggleButton)));
        }

        private TextBlock? LabelTb;

        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(IconToggleButton), new PropertyMetadata(false, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as IconToggleButton)?.UpdateLabel();

        }

        public string OffLabel
        {
            get { return (string)GetValue(OffLabelProperty); }
            set { SetValue(OffLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffLabelProperty =
            DependencyProperty.Register("OffLabel", typeof(string), typeof(IconToggleButton),
                new PropertyMetadata(string.Empty, OnLabelChanged));

        public string OnLabel
        {
            get { return (string)GetValue(OnLabelProperty); }
            set { SetValue(OnLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnLabelProperty =
            DependencyProperty.Register("OnLabel", typeof(string), typeof(IconToggleButton), new PropertyMetadata(string.Empty, OnLabelChanged));

        public event RoutedPropertyChangedEventHandler<bool>? ValueChanged;

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as IconToggleButton)?.UpdateLabel();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            LabelTb = GetTemplateChild(LabelTbName) as TextBlock;
            UpdateLabel();
        }

        protected override void OnClick()
        {
            base.OnClick();
            var val = !Value;
            SetCurrentValue(ValueProperty, val);
            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<bool>(!val, val));
        }

        private void UpdateLabel()
        {
            if (LabelTb == null)
            {
                return;
            }
            LabelTb.Text = Value ? OnLabel : OffLabel;
        }
    }
}
