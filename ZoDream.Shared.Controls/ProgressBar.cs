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
    ///     <MyNamespace:ProgressBar/>
    ///
    /// </summary>
    public class ProgressBar : Control
    {
        static ProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressBar), new FrameworkPropertyMetadata(typeof(ProgressBar)));
        }



        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ProgressBar), 
                new PropertyMetadata(.0, (d, e) =>
                {
                    (d as ProgressBar)?.InvalidateVisual();
                }));



        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(ProgressBar), 
                new PropertyMetadata(100.0, (d, e) =>
                {
                    (d as ProgressBar)?.InvalidateVisual();
                }));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), 
                typeof(ProgressBar), new PropertyMetadata(Orientation.Horizontal, (d, e) =>
                {
                    (d as ProgressBar)?.InvalidateVisual();
                }));



        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Thickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(ProgressBar), 
                new PropertyMetadata(6.0, (d, e) =>
                {
                    (d as ProgressBar)?.InvalidateVisual();
                }));


        public event RoutedPropertyChangedEventHandler<double>? ValueChanged;

        private bool? IsMove;
        private Point CenterPoint = new();
        private readonly double MaxRadius = 1.7;
        private readonly double HoverRadius = 1.3;
        private readonly double MinRadius = 1.0;

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var width = ActualWidth;
            var height = ActualHeight;
            var pen = new Pen(BorderBrush, 1);
            double maxRadius;
            if (Orientation == Orientation.Horizontal)
            {
                var centerY = height / 2;
                var y = centerY - Thickness / 2;
                var w = Math.Max(Math.Min(Value * width / Max, width), 0);
                drawingContext.DrawRectangle(Background, pen, new Rect(0, y, width, Thickness));
                if (Value > 0)
                {
                    drawingContext.DrawRectangle(Foreground, pen, new Rect(0, y, w, Thickness));
                }
                maxRadius = height / 2;
                CenterPoint = new Point(w, centerY);
                
            } else
            {
                var centerX = width / 2;
                var x = centerX - Thickness / 2;
                var h = Math.Max(Math.Min(Value * height / Max, height), 0);
                drawingContext.DrawRectangle(Background, pen, new Rect(x, 0, Thickness, height));
                if (Value > 0)
                {
                    drawingContext.DrawRectangle(Foreground, pen, new Rect(x, 0, Thickness, h));
                }
                maxRadius = width / 2;
                CenterPoint = new Point(centerX, h);
            }
            var radius = Math.Min(Thickness * MaxRadius, maxRadius);
            drawingContext.DrawEllipse(Background, pen, CenterPoint, radius, radius);
            radius = Math.Min(Thickness * (IsMouseOver ? HoverRadius : MinRadius), maxRadius);
            drawingContext.DrawEllipse(Foreground, pen, CenterPoint, radius, radius);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            var p = e.GetPosition(this);
            var diff = p - CenterPoint;
            IsMove = diff.Length < Thickness * MaxRadius;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed || IsMove is null)
            {
                return;
            }
            IsMove = true;
            Move(e.GetPosition(this));
        }

        private void Move(Point point)
        {
            var offset = Orientation == Orientation.Horizontal ? point.X : point.Y;
            var width = Orientation == Orientation.Horizontal ? ActualWidth : ActualHeight;
            var val = Math.Min(Max, Math.Max(offset * Max / width, 0));
            var old = Value;
            if (old == val)
            {
                return;
            }
            SetCurrentValue(ValueProperty, val);
            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(old, val));
            InvalidateVisual();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (IsMove == false)
            {
                Move(e.GetPosition(this));
            }
            IsMove = null;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            IsMove = null;
            InvalidateVisual();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            InvalidateVisual();
        }
    }
}
