using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    ///     <MyNamespace:NumberInput/>
    ///
    /// </summary>
    [TemplatePart(Name = ValueTbName, Type =typeof(TextBox))]
    [TemplatePart(Name = PlusBtnName, Type =typeof(Button))]
    [TemplatePart(Name = MinusBtnName, Type =typeof(Button))]
    public class NumberInput : Control
    {
        public const string ValueTbName = "PART_ValueTb";
        public const string PlusBtnName = "PART_PlusBtn";
        public const string MinusBtnName = "PART_MinusBtn";

        static NumberInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberInput), new FrameworkPropertyMetadata(typeof(NumberInput)));
        }

        public long Max
        {
            get { return (long)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(long), typeof(NumberInput), new PropertyMetadata(0L));

        public long Min
        {
            get { return (long)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(long), typeof(NumberInput), new PropertyMetadata(0L));




        public uint Step
        {
            get { return (uint)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(uint), typeof(NumberInput), new PropertyMetadata((uint)1));



        public long Value
        {
            get { return (long)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(long), typeof(NumberInput), new PropertyMetadata(0L, OnValueChanged));

        public event RoutedPropertyChangedEventHandler<long>? ValueChanged;
        private TextBox? ValueTb;
        private Button? PlusBtn;
        private Button? MinusBtn;

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumberInput)?.UpdateValue(e.NewValue);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ValueTb = GetTemplateChild(ValueTbName) as TextBox;
            PlusBtn = GetTemplateChild(PlusBtnName) as Button;
            MinusBtn = GetTemplateChild(MinusBtnName) as Button;
            if (PlusBtn != null)
            {
                PlusBtn.Click += PlusBtn_Click;
            }
            if (MinusBtn != null)
            {
                MinusBtn.Click += MinusBtn_Click;
            }
            if (ValueTb != null)
            {
                ValueTb.TextChanged += ValueTb_TextChanged;
            }
            UpdateValue(Value);
        }

        private void UpdateValue(object val)
        {
            var v = val.ToString();
            if (v == null || ValueTb == null || ValueTb.Text.Trim() == v)
            {
                return;
            }
            ValueTb.Text = v;
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            var oldVal = Value;
            var val = Value - Step;
            if (val < Min)
            {
                val = Min;
            }
            Value = Convert.ToInt32(val);
            ValueTb!.Text = val.ToString();
            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<long>(oldVal, Value));
        }

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            var oldVal = Value;
            var val = Value + Step;
            if (Max > 0 && val > Max)
            {
                val = Max;
            }
            Value = Convert.ToInt32(val);
            ValueTb!.Text = val.ToString();
            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<long>(oldVal, Value));
        }

        private void ValueTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var oldVal = Value;
            var val = Convert.ToInt64((sender as TextBox)!.Text);
            if (val < Min)
            {
                val = Min;
            }
            else if (Max > 0 && val > Max)
            {
                val = Max;
            }
            SetCurrentValue(ValueProperty, val);
            if (!ValueTb!.IsFocused)
            {
                ValueTb!.Text = val.ToString();
            }
            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<long>(oldVal, Value));
        }

    }
}
