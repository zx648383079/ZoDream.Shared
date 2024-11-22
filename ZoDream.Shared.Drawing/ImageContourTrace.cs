using SkiaSharp;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    /// <summary>
    /// 物体轮廓获取
    /// </summary>
    public class ImageContourTrace
    {
        public ImageContourTrace()
        {
            
        }

        public ImageContourTrace(bool isOutline)
        {
            IsOutline = isOutline;
        }
        /// <summary>
        /// 外边框，即靠近物体的透明区域
        /// </summary>
        public bool IsOutline { get; set; }
        /// <summary>
        /// 是否需要获取一个点
        /// </summary>
        public bool IsAllowDot { get; set; }

        /// <summary>
        /// 获取图片上所有物体轮廓
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<SKPath[]> GetContourAsync(SKBitmap image, CancellationToken token = default)
        {
            using var imagePixMap = image.PeekPixels();
            return await GetContourAsync(imagePixMap, token);
        }

        /// <summary>
        /// 获取图片上所有物体轮廓
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<SKPath[]> GetContourAsync(SKImage image, CancellationToken token = default)
        {
            using var imagePixMap = image.PeekPixels();
            return await GetContourAsync(imagePixMap, token);
        }

        /// <summary>
        /// 获取图片上所有物体轮廓
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public Task<SKPath[]> GetContourAsync(SKPixmap pixMap, CancellationToken token = default)
        {
            return Task.Factory.StartNew(() => {
                return GetContour(pixMap, token);
            }, token);
        }
        /// <summary>
        /// 获取所有物体的轮廓
        /// </summary>
        /// <param name="pixMap"></param>
        /// <returns></returns>
        public SKPath[] GetContour(SKPixmap pixMap, CancellationToken token = default)
        {
            var items = new List<SKPath>();
            for (var i = 0; i < pixMap.Height; i++)
            {
                for (var j = 0; j < pixMap.Width; j++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return [..items];
                    }
                    if (IsTransparent(pixMap, j, i) || Contains(items, j, i))
                    {
                        continue;
                    }
                    var path = GetContour(pixMap, j, i);
                    if (path is null)
                    {
                        continue;
                    }
                    items.Add(path);
                }
            }
            return [.. items];
        }

        /// <summary>
        /// 根据坐标获取轮廓边界算法
        /// </summary>
        /// <param name="pixMap"></param>
        /// <param name="beginX"></param>
        /// <param name="beginY"></param>
        /// <returns></returns>
        public SKPath? GetContour(SKPixmap pixMap, int beginX, int beginY)
        {
            var path = new SKPath();
            path.MoveTo(beginX, beginY - (IsOutline ? 1 : 0));
            var directItems = new int[][] {
                [0, -1], [1, -1],
                         [1, 0],
                [1, 1], [0, 1], [-1, 1],
                [-1, 0], [-1, -1]
            };
            var beginDirect = 0;
            var isBegin = false;
            var curX = beginX;
            var curY = beginY;
            while (!isBegin)
            {
                var i = 0;
                var direct = beginDirect;
                var hasPoint = false;
                while (i++ <= directItems.Length)
                {
                    var x = curX + directItems[direct][0];
                    var y = curY + directItems[direct][1];
                    if (IsTransparent(pixMap, x, y))
                    {
                        direct = (direct + 1) % directItems.Length;
                        if (IsOutline)
                        {
                            path.LineTo(x, y);
                        }
                        continue;
                    }
                    hasPoint = true;
                    curX = x;
                    curY = y;
                    if (curX == beginX && curY == beginY)
                    {
                        isBegin = true;
                        path.Close();
                    }
                    else if (!IsOutline)
                    {
                        path.LineTo(curX, curY);
                    }
                    beginDirect = (direct + 6) % directItems.Length;
                    break;
                }
                if (!hasPoint)
                {
                    if (IsOutline)
                    {
                        path.Close();
                        return path;
                    }
                    // 所有方向都没有结束，就是一个点
                    return IsAllowDot ? path : null;
                }
            }
            return path;
        }

        private static bool Contains(IEnumerable<SKPath> items, int x, int y)
        {
            foreach (var item in items)
            {
                if (item.Contains(x, y))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsTransparent(SKPixmap pixMap, int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return true;
            }
            return pixMap.GetPixelColor(x, y).Alpha == 0;
        }
    }
}
