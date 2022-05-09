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
    ///     <MyNamespace:ContentDialog/>
    ///
    /// </summary>
    [TemplatePart(Name = MinBtnName, Type = typeof(IconButton))]
    [TemplatePart(Name = MaxBtnName, Type = typeof(IconButton))]
    [TemplatePart(Name = CloseBtnName, Type = typeof(IconButton))]
    public class ContentDialog : Control
    {
        public const string MinBtnName = "PART_MinBtn";
        public const string MaxBtnName = "PART_MaxBtn";
        public const string CloseBtnName = "PART_CloseBtn";
        static ContentDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentDialog), new FrameworkPropertyMetadata(typeof(ContentDialog)));
        }



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ContentDialog), new PropertyMetadata(string.Empty));



        public int ZIndex
        {
            get { return (int)GetValue(ZIndexProperty); }
            set { SetValue(ZIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZIndexProperty =
            DependencyProperty.Register("ZIndex", typeof(int), typeof(ContentDialog), new PropertyMetadata(99, (d, e) =>
            {
                Panel.SetZIndex(d as ContentDialog, (int)e.NewValue);
            }));



        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Left.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(ContentDialog), new PropertyMetadata(0, (d, e) =>
            {
                (d as ContentDialog)?.UpdateLeft();
            }));




        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Top.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(ContentDialog), new PropertyMetadata(0, (d, e) =>
            {
                (d as ContentDialog)?.UpdateTop();
            }));

        private bool IsMove = false;
        private Point LastPoint = new();
        private double[] OldSize = new double[4];
        private bool IsFull = false;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild(MinBtnName) is IconButton minBtn)
            {
                minBtn.Click += MinBtn_Click;
            }
            if (GetTemplateChild(MaxBtnName) is IconButton maxBtn)
            {
                maxBtn.Click += MaxBtn_Click;
            }
            if (GetTemplateChild(CloseBtnName) is IconButton closeBtn)
            {
                closeBtn.Click += CloseBtn_Click; ;
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaxBtn_Click(object sender, RoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this);
            if (parent != null && parent is FrameworkElement panel)
            {
                ToggleSize(panel);
                return;
            }
        }

        private void ToggleSize(FrameworkElement panel)
        {
            IsFull = !IsFull;
            if (IsFull)
            {
                OldSize[0] = Left;
                OldSize[1] = Top;
                OldSize[2] = Width;
                OldSize[3] = Height;
                Width = panel.ActualWidth;
                Height = panel.ActualHeight;
                Left = OldSize[0];
                Top = OldSize[1];

            } else
            {
                Width = OldSize[2];
                Height = OldSize[3];
                Left = OldSize[0];
                Top = OldSize[1];
            }
            if (GetTemplateChild(MaxBtnName) is IconButton maxBtn)
            {
                maxBtn.Icon = IsFull ? "\uE923" : "\uE922";
                maxBtn.Label = IsFull ? "还原" : "最大化";
            }
        }



        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        public void Close()
        {
            Hide();
            var parent = VisualTreeHelper.GetParent(this);
            if (parent != null && parent is Panel panel)
            {
                panel.Children.Remove(this);
                return;
            }
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                IsMove = true;
                LastPoint = e.GetPosition(this);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsMove)
            {
                var p = e.GetPosition(this);
                Left += p.X - LastPoint.X;
                Top += p.Y - LastPoint.Y;
                LastPoint = p;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            IsMove = false;
        }

        private void UpdateLeft()
        {
            var parent = VisualTreeHelper.GetParent(this);
            if (parent == null)
            {
                return;
            }
            if (parent is Canvas)
            {
                Canvas.SetLeft(this, Left);
                return;
            }
            UpdateMargin(parent as FrameworkElement);
        }

        private void UpdateTop()
        {
            var parent = VisualTreeHelper.GetParent(this);
            if (parent == null)
            {
                return;
            }
            if (parent is Canvas)
            {
                Canvas.SetTop(this, Top);
                return;
            }
            UpdateMargin(parent as FrameworkElement);
        }

        private void UpdateMargin(FrameworkElement? panel)
        {
            if (panel == null)
            {
                Margin = new Thickness(Left, Top, 0, 0);
                return;
            }
            Margin = new Thickness(Left, Top, panel.ActualWidth - Left - Width, 
                panel.ActualWidth - Top - Height);
        }
    }
}
