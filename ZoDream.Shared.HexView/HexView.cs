using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ZoDream.Shared.HexView
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Shared.HexView"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Shared.HexView;assembly=ZoDream.Shared.HexView"
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
    ///     <MyNamespace:HexView/>
    ///
    /// </summary>
    [TemplatePart(Name = TextHeaderTbName, Type = typeof(Label))]
    [TemplatePart(Name = ByteHeaderPanelName, Type = typeof(StackPanel))]
    [TemplatePart(Name = LinePanelName, Type = typeof(StackPanel))]
    [TemplatePart(Name = BytePanelName, Type = typeof(StackPanel))]
    [TemplatePart(Name = TextPanelName, Type = typeof(StackPanel))]
    [TemplatePart(Name = ByteScrollBarName, Type = typeof(ScrollBar))]
    public class HexView : Control
    {
        const string ByteHeaderPanelName = "PART_ByteHeaderPanel";
        const string TextHeaderTbName = "PART_TextHeaderTb";
        const string LinePanelName = "PART_LinePanel";
        const string BytePanelName = "PART_BytePanel";
        const string TextPanelName = "PART_TextPanel";
        const string ByteScrollBarName = "PART_ByteScrollBar";
        static HexView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HexView), new FrameworkPropertyMetadata(typeof(HexView)));
        }



        public int LineFromBase
        {
            get { return (int)GetValue(LineFromBaseProperty); }
            set { SetValue(LineFromBaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineFromBase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineFromBaseProperty =
            DependencyProperty.Register("LineFromBase", typeof(int), typeof(HexView), 
                new PropertyMetadata(16, (d, e)=>
                {
                    (d as HexView)?.UpdateLineMode();
                }));




        public int CountOfPerLine
        {
            get { return (int)GetValue(CountOfPerLineProperty); }
            set { SetValue(CountOfPerLineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CountOfPerLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountOfPerLineProperty =
            DependencyProperty.Register("CountOfPerLine", typeof(int), typeof(HexView), 
                new PropertyMetadata(16, (d, e) =>
                {
                    (d as HexView)?.UpdateByteLength();
                }));



        public int ByteFromBase
        {
            get { return (int)GetValue(ByteFromBaseProperty); }
            set { SetValue(ByteFromBaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ByteFromBase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ByteFromBaseProperty =
            DependencyProperty.Register("ByteFromBase", typeof(int), typeof(HexView), 
                new PropertyMetadata(16, (d, e) =>
                {
                    (d as HexView)?.UpdateByteMode();
                }));


        public Encoding TextEncoding
        {
            get { return (Encoding)GetValue(TextEncodingProperty); }
            set { SetValue(TextEncodingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextEncoding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextEncodingProperty =
            DependencyProperty.Register("TextEncoding", typeof(Encoding), typeof(HexView), new PropertyMetadata(Encoding.ASCII, (d, e) =>
            {
                (d as HexView)?.UpdateEncoding();
            }));

        public long Position
        {
            get { return (long)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(long), typeof(HexView), new PropertyMetadata(0L, (d, e) =>
            {
                (d as HexView)?.GotoPosition((long)e.NewValue);
            }));



        public long Length
        {
            get { return (long)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Length.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(long), typeof(HexView), new PropertyMetadata(0L, (d, e) =>
            {
                (d as HexView)?.UpdateSource();
            }));



        private Label? TextHeaderTb;
        private StackPanel? ByteHeaderPanel;
        private StackPanel? LinePanel;
        private StackPanel? BytePanel;
        private StackPanel? TextPanel;
        private ScrollBar? ByteScrollBar;
        private byte[]? OriginalBuffer;
        private Action<Point, bool>? OnMouseMoveEnd;
        public event RoutedPropertyChangedEventHandler<int>? SelectionChanged;
        public event RoutedPropertyChangedEventHandler<string>? DropChanged;
        public event HexLoadEventHandler? ByteLoad;
        private CancellationTokenSource TokenSource = new();

        public bool IsSelectionActive { get; private set; } = false;

        public Tuple<byte[], long> SelectionByte
        {
            get
            {
                var items = new List<byte>();
                var start = -1;
                ForEachByte((item, j) =>
                {
                    if (!item.IsActive)
                    {
                        return;
                    }
                    if (start < 0)
                    {
                        start = j;
                    }
                    items.Add(item.OriginalByte);
                });
                return Tuple.Create(items.ToArray(), start < 0 ? -1L : (start + Position));
            }
        }

        private double ByteWidth
        {
            get
            {
                return Math.Ceiling(Math.Log(256, ByteFromBase)) * FontSize * .7 + 10;
            }
        }

        private double ByteHeight
        {
            get
            {
                return FontSize + 20;
            }
        }

        private long LineCount
        {
            get
            {
                return (long)Math.Ceiling((double)Length / CountOfPerLine);
            }
        }

        private int PageLineCount
        {
            get
            {
                if (BytePanel is null)
                {
                    return 0;
                }
                return Math.Max((int)Math.Floor(BytePanel!.ActualHeight / ByteHeight), 1);
            }
        }

        private long PageCount
        {
            get
            {
                return (long)Math.Ceiling((double)LineCount / PageLineCount);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextHeaderTb = GetTemplateChild(TextHeaderTbName) as Label;
            ByteHeaderPanel = GetTemplateChild(ByteHeaderPanelName) as StackPanel;
            LinePanel = GetTemplateChild(LinePanelName) as StackPanel;
            BytePanel = GetTemplateChild(BytePanelName) as StackPanel;
            TextPanel = GetTemplateChild(TextPanelName) as StackPanel;
            ByteScrollBar = GetTemplateChild(ByteScrollBarName) as ScrollBar;

            if (ByteScrollBar != null)
            {
                ByteScrollBar.ValueChanged += ByteScrollBar_ValueChanged;
            }
            if (LinePanel != null)
            {
                LinePanel.MouseWheel += BytePanel_MouseWheel;
            }
            if (BytePanel != null)
            {
                BytePanel.MouseWheel += BytePanel_MouseWheel;
                BytePanel.MouseDown += BytePanel_MouseDown;
                BytePanel.MouseMove += BytePanel_MouseMove;
                BytePanel.MouseUp += BytePanel_MouseUp;
            }
            if (TextPanel != null)
            {
                TextPanel.MouseWheel += BytePanel_MouseWheel;
            }
            if (ContextMenu is not null)
            {
                ContextMenu.Visibility = Visibility.Collapsed;
            }
            UpdateByteHeader();
            UpdateTextHeader();
            if (Length > 0)
            {
                Refresh(true);
            }
        }

        public void Refresh(bool sizeChanged = false)
        {
            if (ByteScrollBar is null)
            {
                return;
            }
            if (sizeChanged)
            {
                var pageCount = PageCount;
                ByteScrollBar.Maximum = LineCount;
                ByteScrollBar.Value = 0;
                ByteScrollBar.Visibility = pageCount > 1 ? Visibility.Visible : Visibility.Collapsed;
            }
            GotoPosition(Position);
        }

        private void UpdateBytes(IList<byte> items, int lineCount)
        {
            ForEachChildren(BytePanel!, lineCount, (panel, i) =>
            {
                panel.Height = ByteHeight;
                var start = i * CountOfPerLine;
                ForEachChildren(panel, Math.Min(items.Count - start, CountOfPerLine), (tb, j) =>
                {
                    tb.Width = ByteWidth;
                    tb.FontSize = FontSize;
                    var itemIndex = start + j;
                    tb.OriginalByte = items[itemIndex];
                    tb.OriginalPosition = Position + itemIndex;
                    tb.Content = Convert.ToString(items[itemIndex], ByteFromBase);
                }, i =>
                {
                    return new HexLabel()
                    {
                        TextAlignment = HorizontalAlignment.Center,
                    };
                });
            }, i =>
            {
                return new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                };
            });
        }

        private void UpdateLine(long start, int lineCount)
        {
            ForEachChildren(LinePanel!, lineCount, (tb, i) =>
            {
                tb.Height = ByteHeight;
                tb.FontSize = FontSize;
                tb.Content = Convert.ToString(start + (i * CountOfPerLine), LineFromBase);
            }, i =>
            {
                return new HexLabel()
                {
                    TextAlignment = HorizontalAlignment.Right,
                    Padding = new Thickness(0, 0, 5, 0),
                };
            });
        }

        private void UpdateText(IList<byte> items, int lineCount)
        {
            ForEachChildren(TextPanel!, lineCount, (tb, i) =>
            {
                tb.Height = ByteHeight;
                tb.FontSize = FontSize;
                var start = i * CountOfPerLine;
                var bytes = new byte[Math.Min(items.Count - start, CountOfPerLine)];
                for (int j = 0; j < bytes.Length; j++)
                {
                    bytes[j] = items[start + j];
                }
                tb.Content = TextEncoding.GetString(bytes).Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
            }, i =>
            {
                return new HexLabel()
                {
                    TextAlignment = HorizontalAlignment.Left,
                };
            }, true);
        }

        private void UpdateTextHeader()
        {
            if (TextHeaderTb is null)
            {
                return;
            }
            TextHeaderTb!.Content = $"Text({TextEncoding.EncodingName})";
        }

        private void UpdateByteHeader()
        {
            var byteFormat = 16;
            ForEachChildren(ByteHeaderPanel!, CountOfPerLine, (tb, i) =>
            {
                tb.Width = ByteWidth;
                tb.FontSize = FontSize;
                tb.Content = Convert.ToString(i, byteFormat);
            }, i =>
            {
                return new HexLabel()
                {
                    TextAlignment = HorizontalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                };
            }, true);
        }

        private void ForEachChildren<T>(Panel panel, int count, Action<T, int> updateFn, Func<int, T> addFn, bool overRemove = false)
            where T : UIElement
        {
            if (panel is null)
            {
                return;
            }
            if (overRemove)
            {
                for (int j = panel.Children.Count - 1; j >= count; j--)
                {
                    panel.Children.RemoveAt(j);
                }
            }
            var i = -1;
            foreach (T item in panel.Children)
            {
                if (item == null)
                {
                    continue;
                }
                i++;
                if (i >= count)
                {
                    item.Visibility = Visibility.Collapsed;
                    continue;
                }
                item.Visibility = Visibility.Visible;
                updateFn.Invoke(item, i);
            }
            while (i < count - 1)
            {
                i++;
                var item = addFn.Invoke(i);
                panel.Children.Add(item);
                updateFn.Invoke(item, i);
            }
        }

        private void ForEachByte(Action<HexLabel, int> func)
        {
            var i = 0;
            foreach (StackPanel item in BytePanel!.Children)
            {
                foreach (HexLabel it in item.Children)
                {
                    if (it.Visibility == Visibility.Visible)
                    {
                        func.Invoke(it, i);
                        i++;
                    }
                }
            }
        }

        private void UpdateSource()
        {
            var pageCount = PageCount;
            if (ByteScrollBar is not null)
            {
                ByteScrollBar.Maximum = LineCount;
                ByteScrollBar.Value = 0;
                ByteScrollBar.Visibility = pageCount > 1 ? Visibility.Visible : Visibility.Collapsed;
            }
            SetCurrentValue(PositionProperty, 0L);
            if (Length <= 0)
            {
                Clear();
                return;
            }
            GotoPosition(0);
        }

        private void UpdateByteSource(byte[] buffer, long position = 0)
        {
            var lineCount = (int)Math.Ceiling((double)buffer.Length / CountOfPerLine);
            OriginalBuffer = buffer;
            Position = position;
            UpdateLine(position, lineCount);
            UpdateBytes(buffer, lineCount);
            UpdateText(buffer, lineCount);
        }


        private void UpdateByteMode()
        {
            UpdateByteHeader();
            if (OriginalBuffer == null)
            {
                return;
            }
            UpdateBytes(OriginalBuffer, (int)Math.Ceiling((double)OriginalBuffer.Length / CountOfPerLine));
        }

        private void UpdateLineMode()
        {
            if (OriginalBuffer == null)
            {
                return;
            }
            UpdateLine(Position, (int)Math.Ceiling((double)OriginalBuffer.Length / CountOfPerLine));
        }

        private void UpdateByteLength()
        {
            UpdateByteHeader();
            Refresh(true);
        }

        private void UpdateEncoding()
        {
            UpdateTextHeader();
            if (OriginalBuffer == null)
            {
                return;
            }
            UpdateText(OriginalBuffer, (int)Math.Ceiling((double)OriginalBuffer.Length / CountOfPerLine));
        }

        private void ToggleByteMode()
        {
            ByteFromBase = ByteFromBase switch
            {
                2 => 8,
                8 => 10,
                16 => 2,
                _ => 16,
            };
        }

        private void ToggleTextEncoding()
        {
            if (TextEncoding == Encoding.ASCII)
            {
                TextEncoding = Encoding.UTF8;
            }
            else if (TextEncoding == Encoding.UTF8)
            {
                TextEncoding = Encoding.Unicode;
            }
            else
            {
                TextEncoding = Encoding.ASCII;
            }
        }

        private void ToggleLineMode()
        {
            LineFromBase = LineFromBase == 10 ? 16 : 10;
        }

        private void ByteScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Length <= 0)
            {
                return;
            }
            TokenSource.Cancel();
            TokenSource = new CancellationTokenSource();
            var token = TokenSource.Token;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(200);
                if (token.IsCancellationRequested)
                {
                    return;
                }
                Dispatcher.Invoke(() =>
                {
                    GotoLine((long)(e.NewValue > e.OldValue ? Math.Ceiling(e.NewValue) : Math.Floor(e.NewValue)), token);
                });
            }, token);

        }

        public void GotoPage(double index, CancellationToken cancellationToken = default)
        {
            GotoLine((long)Math.Floor(index * PageLineCount), cancellationToken);
        }

        public void GotoLine(long index, CancellationToken cancellationToken = default)
        {
            if (index < 0)
            {
                index = 0;
            }
            GotoPosition(index * CountOfPerLine, cancellationToken);
        }

        public void GotoPosition(long index, CancellationToken cancellationToken = default)
        {
            if (Length <= 0)
            {
                return;
            }
            if (ByteScrollBar is not null)
            {
                ByteScrollBar.Value = Math.Floor((double)index / CountOfPerLine);
            }
            var length = PageLineCount * CountOfPerLine;
            if (length <= 0)
            {
                Clear();
                return;
            }
            if (BytePanel is null)
            {
                return;
            }
            ByteLoad?.Invoke(this, new HexLoadEventArgs(index, length));
            // var buffer = await Source.ReadAsync(, cancellationToken);
            //if (cancellationToken.IsCancellationRequested)
            //{
            //    return;
            //}
            //UpdateByteSource(buffer, index);
        }


        public void Select(long position, int count, bool IsFinish = true)
        {
            var i = position - Position;
            var end = i + count;
            var len = 0;
            ForEachByte((item, j) =>
            {
                var isActive = j >= i && j < end;
                item.IsActive = isActive;
                if (isActive)
                {
                    len++;
                }
            });
            IsSelectionActive = len > 0;
            if (IsFinish)
            {
                SelectionChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<int>(0, len));
                var isMenuVisible = Length <= 0 || len < 1;
                if (ContextMenu is not null)
                {
                    if (isMenuVisible)
                    {
                        ContextMenu.StaysOpen = false;
                    }
                    ContextMenu.Visibility = isMenuVisible ? Visibility.Collapsed : Visibility.Visible;
                }

            }
        }

        public void Select()
        {
            Select(0L, 0);
        }

        public void Select(Point begin, Point end, bool IsFinish = true)
        {
            var bY = Math.Floor(begin.Y / ByteHeight);
            var eY = Math.Floor(end.Y / ByteHeight);
            var bX = Math.Floor(begin.X / ByteWidth);
            var eX = Math.Floor(end.X / ByteWidth);
            if (bY < eY || (bY == eY && bX < eX))
            {
                var b = Position + bY * CountOfPerLine + bX;
                Select(Convert.ToInt64(b), Convert.ToInt32(Position + eY * CountOfPerLine + eX + 1 - b), IsFinish);
            }
            else
            {
                var e = Position + eY * CountOfPerLine + eX;
                Select(Convert.ToInt64(e), Convert.ToInt32(Position + bY * CountOfPerLine + bX + 1 - e), IsFinish);
            }
        }

        private void BytePanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ByteScrollBar_ValueChanged(ByteScrollBar!, new RoutedPropertyChangedEventArgs<double>(ByteScrollBar!.Value, ByteScrollBar.Value - (e.Delta / 120)));
        }

        private void ByteModeTb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ToggleByteMode();
        }

        private void TextHeaderTb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ToggleTextEncoding();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Refresh();
        }

        private void BytePanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            var start = e.GetPosition(BytePanel);
            OnMouseMoveEnd = (end, up) =>
            {
                Select(start, end, up);
            };
        }

        private void BytePanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            var end = e.GetPosition(BytePanel);
            OnMouseMoveEnd?.Invoke(end, true);
        }

        private void BytePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var end = e.GetPosition(BytePanel);
            OnMouseMoveEnd?.Invoke(end, false);
        }


        public void Clear()
        {
            if (ContextMenu is not null)
            {
                ContextMenu.Visibility = Visibility.Collapsed;
            }
            UpdateByteSource(Array.Empty<byte>());
        }

        public void Append(long begin, byte[] items)
        {
            UpdateByteSource(items, begin);
        }

        public void Append(HexLoadEventArgs e, byte[] items)
        {
            UpdateByteSource(items, e.Position);
        }
    }
}
