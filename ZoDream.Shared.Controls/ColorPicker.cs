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
    ///     <MyNamespace:ColorPicker/>
    ///
    /// </summary>
    [TemplatePart(Name = PickerPanelName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PickerBtnName, Type = typeof(Border))]
    [TemplatePart(Name = SliderTbName, Type = typeof(ProgressBar))]
    [TemplatePart(Name = RTextTbName, Type = typeof(NumberInput))]
    [TemplatePart(Name = GTextTbName, Type = typeof(NumberInput))]
    [TemplatePart(Name = BTextTbName, Type = typeof(NumberInput))]
    [TemplatePart(Name = ATextTbName, Type = typeof(NumberInput))]
    [TemplatePart(Name = ValueTbName, Type = typeof(TextBox))]
    public class ColorPicker : Control
    {
        public const string PickerPanelName = "PART_PickerPanel";
        public const string PickerBtnName = "PART_PickerBtn";
        public const string SliderTbName = "PART_SliderTb";
        public const string RTextTbName = "PART_RTb";
        public const string GTextTbName = "PART_GTb";
        public const string BTextTbName = "PART_BTb";
        public const string ATextTbName = "PART_ATb";
        public const string ValueTbName = "PART_ValueTb";

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }



        public SolidColorBrush BaseColor
        {
            get { return (SolidColorBrush)GetValue(BaseColorProperty); }
            set { SetValue(BaseColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BaseColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BaseColorProperty =
            DependencyProperty.Register("BaseColor", typeof(SolidColorBrush), typeof(ColorPicker), new PropertyMetadata(Brushes.Red));



        public Color Value
        {
            get { return (Color)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.White));

        public event RoutedPropertyChangedEventHandler<Color>? ValueChanged;

        private FrameworkElement? PickerPanel;
        private Border? PickerBtn;
        private ProgressBar? SliderTb;
        private NumberInput? RTb;
        private NumberInput? GTb;
        private NumberInput? BTb;
        private NumberInput? ATb;
        private TextBox? ValueTb;
        /// <summary>
        ///     颜色分隔集合
        /// </summary>
        private readonly List<Color> ColorSeparateItems = new()
        {
            Color.FromRgb(255, 0, 0),
            Color.FromRgb(255, 0, 255),
            Color.FromRgb(0, 0, 255),
            Color.FromRgb(0, 255, 255),
            Color.FromRgb(0, 255, 0),
            Color.FromRgb(255, 255, 0)
        };

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PickerPanel = GetTemplateChild(PickerPanelName) as FrameworkElement;
            PickerBtn = GetTemplateChild(PickerBtnName) as Border;
            SliderTb = GetTemplateChild(SliderTbName) as ProgressBar;
            RTb = GetTemplateChild(RTextTbName) as NumberInput;
            GTb = GetTemplateChild(GTextTbName) as NumberInput;
            BTb = GetTemplateChild(BTextTbName) as NumberInput;
            ATb = GetTemplateChild(ATextTbName) as NumberInput;
            ValueTb = GetTemplateChild(ValueTbName) as TextBox;

            if (PickerPanel != null)
            {
                PickerPanel.MouseLeftButtonDown += PickerPanel_MouseLeftButtonDown;
            }

            if (SliderTb != null)
            {
                SliderTb.ValueChanged += SliderTb_ValueChanged;
            }
            if (ValueTb != null)
            {
                ValueTb.TextChanged += ValueTb_TextChanged;
            }
            if (RTb != null)
            {
                RTb.ValueChanged += RTb_ValueChanged;
            }
            if (GTb != null)
            {
                GTb.ValueChanged += RTb_ValueChanged;
            }
            if (BTb != null)
            {
                BTb.ValueChanged += RTb_ValueChanged;
            }
            if (ATb != null)
            {
                ATb.ValueChanged += RTb_ValueChanged;
            }
            UpdateColor(Value);
            UpdatePointByColor(Value);
        }

        private void RTb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<long> e)
        {
            var color = ColorHelper.FromRGBA(
                Convert.ToByte(RTb!.Value),
                Convert.ToByte(GTb!.Value),
                Convert.ToByte(RTb!.Value),
                Convert.ToByte(ATb!.Value));
            if (ValueTb != null)
            {
                ValueTb.Text = ColorHelper.To(color);
            }
            UpdatePointByColor(color);
            TriggerColor(color);
        }

        private void ValueTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var color = ColorHelper.From(ValueTb!.Text);
            UpdateRGBColor(color);
            UpdatePointByColor(color);
            TriggerColor(color);
        }

        private void PickerPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(sender as FrameworkElement);
            SetPickerPoint(p.X, p.Y);
            UpdateByPoint(p);
        }

        private void SliderTb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var index = Math.Min(Math.Floor(e.NewValue), 5);
            var sub = e.NewValue - index;
            var val = 255 * sub;
            BaseColor = new SolidColorBrush(index switch
            {
                1 => ColorHelper.FromRGB(Convert.ToByte(255 - val), 0, 255),
                2 => ColorHelper.FromRGB(0, Convert.ToByte(val), 255),
                3 => ColorHelper.FromRGB(0, 255, Convert.ToByte(255 - val)),
                4 => ColorHelper.FromRGB(Convert.ToByte(val), 255, 0),
                5 => ColorHelper.FromRGB(255, Convert.ToByte(255 - val), 0),
                _ => ColorHelper.FromRGB(255, 0, Convert.ToByte(val)),
            });
            UpdateByPoint(GetPickerPoint());
        }


        private void SetPickerPoint(double x, double y)
        {
            if (PickerBtn == null)
            {
                return;
            }
            Canvas.SetTop(PickerBtn, y - 6);
            Canvas.SetLeft(PickerBtn, x - 6);
        }

        private Point GetPickerPoint()
        {
            if (PickerBtn == null)
            {
                return new Point(0, 0);
            }
            return new Point(Canvas.GetLeft(PickerBtn) + 6, Canvas.GetTop(PickerBtn) + 6);
        }

        private void UpdateByPoint(Point p)
        {
            if (PickerPanel == null)
            {
                return;
            }
            
            var scaleX = p.X / PickerPanel.ActualWidth;
            var scaleY = 1 - p.Y / PickerPanel.ActualHeight;

            var colorYLeft = Color.FromRgb((byte)(255 * scaleY), (byte)(255 * scaleY), (byte)(255 * scaleY));
            var colorYRight = Color.FromRgb((byte)(BaseColor.Color.R * scaleY), (byte)(BaseColor.Color.G * scaleY), (byte)(BaseColor.Color.B * scaleY));

            var subR = colorYLeft.R - colorYRight.R;
            var subG = colorYLeft.G - colorYRight.G;
            var subB = colorYLeft.B - colorYRight.B;

            var color = ColorHelper.FromRGB((byte)(colorYLeft.R - subR * scaleX),
                (byte)(colorYLeft.G - subG * scaleX), (byte)(colorYLeft.B - subB * scaleX));
            TriggerColor(color);
            UpdateColor(color);
        }

        private void TriggerColor(Color color)
        {
            var oldVal = Value;
            SetCurrentValue(ValueProperty, color);
            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<Color>(oldVal, color));
        }

        private void UpdateColor(Color color)
        {
            if (ValueTb != null)
            {
                ValueTb.Text = ColorHelper.To(color);
            }
            UpdateRGBColor(color);
        }

        private void UpdateRGBColor(Color color)
        {
            if (RTb != null)
            {
                RTb.Value = color.R;
            }
            if (GTb != null)
            {
                GTb.Value = color.G;
            }
            if (BTb != null)
            {
                BTb.Value = color.B;
            }
            if (ATb != null)
            {
                ATb.Value = color.A;
            }
        }

        private void UpdatePointByColor(Color color)
        {
            var r = color.R;
            var g = color.G;
            var b = color.B;
            var list = ColorToList(color);

            var max = list.Max();
            var min = list.Min();

            if (min == max)
            {
                if (!(r == g && b == g))
                {
                    BaseColor = Brushes.Red;
                    if (SliderTb != null)
                    {
                        SliderTb.Value = 0;
                    }
                }
            }
            else
            {
                var maxIndex = list.IndexOf(max);
                var minIndex = list.IndexOf(min);
                var commonIndex = 3 - maxIndex - minIndex;
                if (commonIndex == 3)
                {
                    BaseColor = Brushes.Red;
                    if (SliderTb != null)
                    {
                        SliderTb.Value = 0;
                    }
                }
                else
                {
                    var common = list[commonIndex];
                    list[maxIndex] = 255;
                    list[minIndex] = 0;
                    common = (byte)(255 * (min - common) / (double)(min - max));
                    list[commonIndex] = common;
                    BaseColor = new SolidColorBrush(Color.FromRgb(list[0], list[1], list[2]));

                    list[commonIndex] = 0;
                    var cIndex = ColorSeparateItems.IndexOf(Color.FromRgb(list[0], list[1], list[2]));
                    int sub;
                    var direc = 0;
                    if (cIndex is < 5 and > 0)
                    {
                        var nextColorList = ColorToList(ColorSeparateItems[cIndex + 1]);
                        var prevColorList = ColorToList(ColorSeparateItems[cIndex - 1]);
                        if (nextColorList[minIndex] > 0)
                        {
                            var target = prevColorList[commonIndex];
                            direc = 1;
                            sub = target - common;
                        }
                        else
                        {
                            sub = common;
                        }
                    }
                    else if (cIndex == 0)
                    {
                        sub = common;
                        if (minIndex == 2)
                        {
                            sub = 255 - common;
                            direc = -5;
                        }
                    }
                    else
                    {
                        sub = 255 - common;
                    }
                    var scale = sub / 255.0;
                    var scaleTotal = cIndex - direc + scale;
                    if (SliderTb != null)
                    {
                        SliderTb.Value = scaleTotal;
                    }
                }
            }
            if (PickerPanel == null)
            {
                return;
            }
            var x = max == 0 ? 0 : (1 - min / (double)max) * PickerPanel.ActualWidth;
            var y = (1 - max / 255.0) * PickerPanel.ActualHeight;
            SetPickerPoint(x, y);
        }

        private List<byte> ColorToList(Color color)
        {
            return new List<byte>()
            {
                color.R,
                color.G,
                color.B
            };
        }
    }
}
