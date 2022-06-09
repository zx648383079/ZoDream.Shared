using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZoDream.Shared.HexView
{
    public class HexLoadEventArgs: RoutedEventArgs
    {
        public HexLoadEventArgs(long position, int length)
        {
            Position = position;
            Length = length;
        }

        public long Position { get; private set; }

        public int Length { get; private set; }
    }
}
