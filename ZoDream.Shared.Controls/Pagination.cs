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
    ///     <MyNamespace:Pagination/>
    ///
    /// </summary>
    [TemplatePart(Name = PanelName, Type = typeof(Panel))]
    public class Pagination : Control
    {
        public const string PanelName = "PART_Panel";
        static Pagination()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pagination), new FrameworkPropertyMetadata(typeof(Pagination)));
        }




        public int PerPage
        {
            get { return (int)GetValue(PerPageProperty); }
            set { SetValue(PerPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PerPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerPageProperty =
            DependencyProperty.Register("PerPage", typeof(int), typeof(Pagination), 
                new PropertyMetadata(20, (d, e) =>
                {
                    (d as Pagination)?.InitPage();
                }));




        public long Page
        {
            get { return (long)GetValue(PageProperty); }
            set { SetValue(PageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Page.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageProperty =
            DependencyProperty.Register("Page", typeof(long), typeof(Pagination), 
                new PropertyMetadata(1L, (d, e) =>
                {
                    (d as Pagination)?.InitPage();
                }));



        public bool DirectionLinks
        {
            get { return (bool)GetValue(DirectionLinksProperty); }
            set { SetValue(DirectionLinksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DirectionLinks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DirectionLinksProperty =
            DependencyProperty.Register("DirectionLinks", typeof(bool), typeof(Pagination), 
                new PropertyMetadata(false, (d, e) =>
                {
                    (d as Pagination)?.InitPage();
                }));



        public long Total
        {
            get { return (long)GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Total.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register("Total", typeof(long), typeof(Pagination), 
                new PropertyMetadata(0L, (d, e) =>
                {
                    (d as Pagination)?.InitPage();
                }));



        public int ButtonCount
        {
            get { return (int)GetValue(ButtonCountProperty); }
            set { SetValue(ButtonCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonCountProperty =
            DependencyProperty.Register("ButtonCount", typeof(int), typeof(Pagination), new PropertyMetadata(0, (d, e) =>
            {
                (d as Pagination)?.InitPage();
            }));



        public Style DisabledStyle
        {
            get { return (Style)GetValue(DisabledStyleProperty); }
            set { SetValue(DisabledStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisabledStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisabledStyleProperty =
            DependencyProperty.Register("DisabledStyle", typeof(Style), typeof(Pagination), new PropertyMetadata(null));



        public Style ActiveStyle
        {
            get { return (Style)GetValue(ActiveStyleProperty); }
            set { SetValue(ActiveStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveStyleProperty =
            DependencyProperty.Register("ActiveStyle", typeof(Style), typeof(Pagination), new PropertyMetadata(null));




        public Style NormalStyle
        {
            get { return (Style)GetValue(NormalStyleProperty); }
            set { SetValue(NormalStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NormalStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NormalStyleProperty =
            DependencyProperty.Register("NormalStyle", typeof(Style), typeof(Pagination), new PropertyMetadata(null));


        public event RoutedPropertyChangedEventHandler<long>? PageChanged;
        private Panel? Panel;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Panel = GetTemplateChild(PanelName) as Panel;
            InitPage();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InitPage();
        }

        private void InitPage()
        {
            if (Panel is null || ActualWidth <= 0)
            {
                return;
            }
            Panel.Children.Clear();
            var pageCount = Math.Ceiling((double)Total / PerPage);
            if (pageCount <= 0)
            {
                return;
            }
            var count = ButtonCount > 0 ? ButtonCount : (ActualWidth > 600 ? 7:3);
            var page = Page < 1 ? 1 : Page;
            var canPrevious = page > 1;
            var canNext = page < pageCount;
            Button btn;
            if (DirectionLinks)
            {
                btn = AddButton("«", canPrevious);
                btn.Click += (d, e) =>
                {
                    if (canPrevious)
                    {
                        Navigate(page - 1);
                    }
                };
            }
            AddPageButton(1, page == 1);
            var lastList = Math.Floor((double)count / 2);
            var i = page - lastList;
            var length = page + lastList;
            if (i < 2)
            {
                i = 2;
                length = i + count;
            }
            if (length > pageCount - 1)
            {
                length = pageCount - 1;
                i = Math.Max(2, length - count);
            }
            if (i > 2)
            {
                AddPageButton(0, false);
            }
            for (; i <= length; i++)
            {
                AddPageButton((long)i, i == page);
            }
            if (length < pageCount - 1)
            {
                AddPageButton(0, false);
            }
            AddPageButton((long)pageCount, pageCount == page);
            if (DirectionLinks)
            {
                btn = AddButton("»", canPrevious);
                btn.Click += (d, e) =>
                {
                    if (canNext)
                    {
                        Navigate(page + 1);
                    }
                };
            }
        }

        private Button AddButton(string val, bool disabled)
        {
            var button = new Button()
            {
                Content = val,
                Style = disabled ? DisabledStyle : NormalStyle,
                IsEnabled = !disabled,
            };
            Panel!.Children.Add(button);
            return button;
        }

        private void AddPageButton(long page, bool active)
        {
            if (page < 1)
            {
                Panel!.Children.Add(new Label()
                {
                    Content = "...",
                    Style = DisabledStyle,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                });
                return;
            }
            var button = new Button()
            {
                Content = page.ToString(),
                Style = active ? ActiveStyle : NormalStyle,
            };
            Panel!.Children.Add(button);
            button.Click += (s, e) => {
                if (!active && page > 0)
                {
                    Navigate(page);
                }
            };
        }

        public void Navigate(long page)
        {
            if (page < 1)
            {
                return;
            }
            var pageCount = Math.Ceiling((double)Total / PerPage);
            if (page > pageCount)
            {
                return;
            }
            var args = new RoutedPropertyChangedEventArgs<long>(Page, page);
            PageChanged?.Invoke(this, args);
            if (args.Handled)
            {
                return;
            }
            SetCurrentValue(PageProperty, page);
        }
    }
}
