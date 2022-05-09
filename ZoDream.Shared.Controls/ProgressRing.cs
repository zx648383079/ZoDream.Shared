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
    ///     <MyNamespace:ProgressRing/>
    ///
    /// </summary>
    public class ProgressRing : Control
    {
        static ProgressRing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressRing), new FrameworkPropertyMetadata(typeof(ProgressRing)));
        }

        public ProgressRing()
        {
            Loaded += ProgressRing_Loaded;
            Unloaded += ProgressRing_Unloaded;
        }

        private void ProgressRing_Unloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void ProgressRing_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(ProgressRing), 
                new PropertyMetadata(true, (d, s) =>
                {
                    (d as ProgressRing)?.InvalidateVisual();
                }));


        private int Counter = 0;
        private int MaxCounter = 720;


        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (!IsActive)
            {
                return;
            }
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (!IsActive)
            {
                return;
            }
            Counter++;
            if (Counter > MaxCounter)
            {
                Counter = 0;
            }
            var half = MaxCounter / 2;
            var centerX = (float)ActualWidth / 2;
            var centerY = (float)ActualHeight / 2;
            var radius = Math.Min(centerX, centerY);
            var start = Counter > 360 ? Counter - 360 : Counter;
            var end = (half - Math.Abs(Counter - half)) / 5 + 2 + start;
            RenderRing(drawingContext, centerX, centerY, radius, 10, start, end);
        }

        private void RenderRing(DrawingContext drawingContext, double cx, double cy,
            double radius, double thickness, double startAngle, double endAngle)
        {
            var innerRadius = radius - thickness;
            var borderRadius = thickness / 2;
            var s = RenderPoint(cx, cy, radius, startAngle);
            var e = RenderPoint(cx, cy, radius, endAngle);
            var innerS = RenderPoint(cx, cy, innerRadius, startAngle);
            var innerE = RenderPoint(cx, cy, innerRadius, endAngle);
            var figuare = new PathFigure(s, 
                new ArcSegment[]
                {
                    new ArcSegment(e,
                        new Size(radius, radius),
                        0d, false, SweepDirection.Clockwise, false),
                    new ArcSegment(innerE,
                            new Size(borderRadius, borderRadius),
                            0d, false, SweepDirection.Clockwise, false),
                    new ArcSegment(innerS,
                            new Size(innerRadius, innerRadius),
                            0d, false, SweepDirection.Counterclockwise, false),
                    new ArcSegment(s,
                            new Size(borderRadius, borderRadius),
                            0d, false, SweepDirection.Clockwise, false)
                }, true);
            var path = new PathGeometry(new PathFigure[]
            {
                figuare,
                // figuare2
            });
            drawingContext.DrawGeometry(Foreground,
                new Pen(BorderBrush, 0),
                path);
        }

        private Point RenderPoint(double cx, double cy, double radius, double angle)
        {
            return new Point(
                cx + radius * Math.Sin(ToDeg(angle)),
                cy - radius * Math.Cos(ToDeg(angle))
                );
        }

        private static double ToDeg(double a)
        {
            return a * Math.PI / 180;
        }
    }
}
