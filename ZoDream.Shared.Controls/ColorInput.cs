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
    ///     <MyNamespace:ColorInput/>
    ///
    /// </summary>
    [TemplatePart(Name = ValueTbName, Type = typeof(Label))]
    [TemplatePart(Name = PopupName, Type = typeof(Popup))]
    [TemplatePart(Name = PickerName, Type = typeof(ColorPicker))]
    public class ColorInput : Control
    {
        public const string ValueTbName = "PART_ValueTb";
        public const string PopupName = "PART_Popup";
        public const string PickerName = "PART_Picker";
        static ColorInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorInput), new FrameworkPropertyMetadata(typeof(ColorInput)));
        }


        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(ColorInput), 
                new PropertyMetadata("#fff", (d, e) =>
                {
                    (d as ColorInput)?.UpdateValue();
                }));


        public event RoutedPropertyChangedEventHandler<string>? ValueChanged;


        private Label? ValueTb;
        private Popup? PopupPanel;
        private ColorPicker? Picker;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ValueTb = GetTemplateChild(ValueTbName) as Label;
            PopupPanel = GetTemplateChild(PopupName) as Popup;
            Picker = GetTemplateChild(PickerName) as ColorPicker;

            if (ValueTb != null)
            {
                ValueTb.MouseLeftButtonDown += ValueTb_GotFocus;
                ValueTb.MouseLeave += ValueTb_LostFocus;
                // ValueTb.LostFocus += ValueTb_LostFocus;
            }
            if (Picker != null)
            {
                Picker.ValueChanged += Picker_ValueChanged;
            }
            if (PopupPanel != null)
            {
                PopupPanel.MouseLeave += PopupPanel_MouseLeave;
            }
            UpdateValue();
        }

        private void ValueTb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PopupPanel == null)
            {
                return;
            }
            PopupPanel.StaysOpen = false;
        }

        private void PopupPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            PopupPanel!.StaysOpen = false;
        }

        private void Picker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            var oldVal = Value;
            var val = ColorHelper.To(e.NewValue);
            SetCurrentValue(ValueProperty, val);
            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<string>(oldVal, val));
            UpdateValue(e.NewValue);
        }

        private void ValueTb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PopupPanel == null)
            {
                return;
            }
            PopupPanel.StaysOpen = true;
            PopupPanel.IsOpen = true;
            if (Picker == null)
            {
                return;
            }
            Picker.Value = ColorHelper.From(Value);
        }

        private void UpdateValue()
        {
            UpdateValue(Value);
        }

        private void UpdateValue(string val)
        {
            UpdateValue(ColorHelper.From(val));
        }

        private void UpdateValue(Color color)
        {
            if (ValueTb != null)
            {
                ValueTb.Content = ColorHelper.To(color);
                ValueTb.Background = new SolidColorBrush(color);
            }
        }
    }
}
