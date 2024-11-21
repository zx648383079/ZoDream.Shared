using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public static class SkiaExtension
    {

        public static SKMatrix AsMatrix(this in Matrix3x2 transform)
        {
            return new SKMatrix
            {
                ScaleX = transform.M11,
                SkewX = transform.M21,
                TransX = transform.M31,
                SkewY = transform.M12,
                ScaleY = transform.M22,
                TransY = transform.M32,
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="deg">360</param>
        public static void Rotate(this SKCanvas canvas, float deg)
        {
            canvas.RotateDegrees(deg);
        }

        public static void Flip(this SKCanvas canvas, bool isHorizontal = true)
        {
            if (isHorizontal)
            {
                canvas.Scale(1, -1);
            }
            else
            {
                canvas.Scale(-1, 1);
            }
        }

        /// <summary>
        /// 计算旋转后的外边框高度
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static (int, int) ComputedRotate(int width, int height, float angle)
        {
            var radians = Math.PI * angle / 180;
            var sine = Math.Abs(Math.Sin(radians));
            var cosine = Math.Abs(Math.Cos(radians));
            var originalWidth = width;
            var originalHeight = height;
            var rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
            var rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);
            return (rotatedWidth, rotatedHeight);
        }

        public static SKBitmap Rotate(this SKBitmap bitmap, float angle)
        {
            var (rotatedWidth, rotatedHeight) = ComputedRotate(bitmap.Width, bitmap.Height, angle);
            return Mutate(rotatedWidth, rotatedHeight, surface => {
                surface.Translate(rotatedWidth / 2, rotatedHeight / 2);
                surface.RotateDegrees(angle);
                surface.Translate(-bitmap.Width / 2, -bitmap.Height / 2);
                surface.DrawBitmap(bitmap, new SKPoint());
            });
        }

        public static SKEncodedImageFormat ConvertFormat(string extension)
        {
            var i = extension.LastIndexOf('.');
            if (i >= 0)
            {
                extension = extension[(i + 1)..];
            }
            return extension.ToLower() switch
            {
                "jpg" or "jpeg" => SKEncodedImageFormat.Jpeg,
                "ico" => SKEncodedImageFormat.Ico,
                "bmp" => SKEncodedImageFormat.Bmp,
                "webp" => SKEncodedImageFormat.Webp,
                "avif" => SKEncodedImageFormat.Avif,
                "gif" => SKEncodedImageFormat.Gif,
                "ktx" => SKEncodedImageFormat.Ktx,
                _ => SKEncodedImageFormat.Png
            };
        }

        public static void SaveAs(this SKBitmap bitmap, string fileName)
        {
            using var fs = File.OpenWrite(fileName);
            bitmap.Encode(fs, ConvertFormat(fileName), 100);
        }

        public static void SaveAs(this SKImage image, string fileName)
        {
            using var fs = File.OpenWrite(fileName);
            using var pixMap = image.PeekPixels();
            pixMap?.Encode(fs, ConvertFormat(fileName), 100);
        }

        public static void Encode(this SKImage image, Stream dst, SKEncodedImageFormat format, int quality)
        {
            using var pixMap = image.PeekPixels();
            pixMap?.Encode(dst, format, quality);
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static SKBitmap CreateThumbnail(this SKBitmap source, int size)
        {
            return Mutate(size, size, canvas => {
                var scale = (float)size / Math.Max(source.Width, source.Height);
                var w = source.Width * scale;
                var h = source.Height * scale;
                canvas.DrawBitmap(source, SKRect.Create((size - w) / 2, (size - h) / 2, w, h));
            });
        }

        public static SKBitmap CreateThumbnail(this SKPicture source, int size)
        {
            return Mutate(size, size, canvas => {
                var scale = size / Math.Max(source.CullRect.Width, source.CullRect.Height);
                var w = source.CullRect.Width * scale;
                var h = source.CullRect.Height * scale;
                //canvas.DrawColor(SKColors.Transparent);
                canvas.Save();
                canvas.Scale(scale, scale);
                canvas.DrawPicture(source, (size - w) * scale / 2, (size - h) * scale / 2);
                canvas.Restore();
            });
        }

        public static SKBitmap? Clip(this SKBitmap source, SKRectI rect)
        {
            return Mutate(rect.Width, rect.Height, canvas => {
                // canvas.Clear(SKColors.Transparent);
                canvas.DrawBitmap(source, rect,
                    SKRect.Create(0, 0, rect.Width, rect.Height), new SKPaint()
                    {
                        FilterQuality = SKFilterQuality.High
                    });
            });
        }

        public static SKBitmap? Clip(this SKBitmap source, SKPath path)
        {
            var rect = path.Bounds;
            if (rect.IsEmpty || rect.Width < 1 || rect.Height < 1)
            {
                return null;
            }
            return Mutate((int)rect.Width, (int)rect.Height, canvas => {
                canvas.DrawBitmap(source, rect,
                   SKRect.Create(0, 0, rect.Width, rect.Height), new SKPaint()
                   {
                       FilterQuality = SKFilterQuality.High
                   });
                path.Offset(-rect.Left, -rect.Top);
                canvas.ClipPath(path, SKClipOperation.Difference);
                canvas.Clear();
            });
        }

        public static SKImage? Clip(this SKImage source, SKPath path)
        {
            var rect = path.Bounds;
            if (rect.IsEmpty || rect.Width < 1 || rect.Height < 1)
            {
                return null;
            }
            return MutateImage((int)rect.Width, (int)rect.Height, canvas => {
                canvas.DrawImage(source, rect,
                   SKRect.Create(0, 0, rect.Width, rect.Height), new SKPaint()
                   {
                       FilterQuality = SKFilterQuality.High
                   });
                path.Offset(-rect.Left, -rect.Top);
                canvas.ClipPath(path, SKClipOperation.Difference);
                canvas.Clear();
            });
        }

        public static SKImage? Clip(this SKImage source, SKRectI rect)
        {
            var target = SKImage.Create(new SKImageInfo(rect.Width, 
                rect.Height));
            using var targetPixMap = target.PeekPixels();
            using var imagePixMap = source.PeekPixels();
            if (imagePixMap.ExtractSubset(targetPixMap, 
                rect))
            {
                return target;
            }
            target.Dispose();
            return null;
        }

        public static SKBitmap Mutate(SKRect rect, Action<SKCanvas> action)
        {
            return Mutate((int)rect.Width, (int)rect.Height, action);
        }

        public static SKBitmap Mutate(int width, 
            int height, 
            Action<SKCanvas> action)
        {
            var bitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(bitmap);
            // canvas.Clear(SKColors.Transparent);
            action?.Invoke(canvas);
            return bitmap;
        }

        public static SKImage MutateImage(int width,
            int height,
            Action<SKCanvas> action)
        {
            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            // canvas.Clear(SKColors.Transparent);
            action?.Invoke(surface.Canvas);
            return surface.Snapshot();
        }
        public static SKImage Flip(this SKImage bitmap, bool isHorizontal = true)
        {
            return MutateImage(bitmap.Width, bitmap.Height, surface => {
                surface.Flip(isHorizontal);
                surface.DrawImage(bitmap, new SKPoint());
            });
        }
        public static SKImage Rotate(this SKImage bitmap, float angle)
        {
            var (rotatedWidth, rotatedHeight) = ComputedRotate(bitmap.Width, bitmap.Height, angle);
            return MutateImage(rotatedWidth, rotatedHeight, surface => {
                surface.Translate(rotatedWidth / 2, rotatedHeight / 2);
                surface.RotateDegrees(angle);
                surface.Translate(-bitmap.Width / 2, -bitmap.Height / 2);
                surface.DrawImage(bitmap, new SKPoint());
            });
        }

        public static SKImage? Resize(this SKImage image, SKImageInfo info, SKFilterQuality quality)
        {
            if (info.IsEmpty)
            {
                return null;
            }
            var target = SKImage.Create(info);
            using var targetPixMap = target.PeekPixels();
            using var imagePixMap = image.PeekPixels();
            if (imagePixMap.ScalePixels(targetPixMap, quality))
            {
                return target;
            }
            target.Dispose();
            return null;
        }
        /// <summary>
        /// 获取图片上所有物体轮廓
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static SKPath[] GetObject(this SKBitmap image)
        {
            using var imagePixMap = image.PeekPixels();
            return GetObject(imagePixMap);
        }
        /// <summary>
        /// 获取图片上所有物体轮廓
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Task<SKPath[]> GetObjectAsync(this SKBitmap image)
        {
            var imagePixMap = image.PeekPixels();
            return Task.Factory.StartNew(() => {
                try
                {
                    return GetObject(imagePixMap);
                } finally
                {
                    imagePixMap.Dispose();
                }
            });
        }
        /// <summary>
        /// 获取图片上所有物体轮廓
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static SKPath[] GetObject(this SKImage image)
        {
            using var imagePixMap = image.PeekPixels();
            return GetObject(imagePixMap);
        }

        /// <summary>
        /// 获取所有物体的区域
        /// </summary>
        /// <param name="pixMap"></param>
        /// <returns></returns>
        public static SKPath[] GetObject(SKPixmap pixMap)
        {
            var items = new List<SKPath>();
            for (var i = 0; i < pixMap.Height; i++)
            {
                for (var j = 0; j < pixMap.Width; j++)
                {
                    if (IsTransparent(pixMap, j, i) || Contains(items, j, i))
                    {
                        continue;
                    }
                    var path = TraceContour(pixMap, j, i);
                    if (path is null)
                    {
                        continue;
                    }
                    items.Add(path);
                }
            }
            return [.. items];
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

        /// <summary>
        /// 边界算法
        /// </summary>
        /// <param name="pixMap"></param>
        /// <param name="begin"></param>
        /// <returns></returns>
        private static SKPath? TraceContour(SKPixmap pixMap, int beginX, int beginY)
        {
            var path = new SKPath();
            path.MoveTo(beginX, beginY);
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
                while (i ++ <= directItems.Length)
                {
                    var x = curX + directItems[direct][0];
                    var y = curY + directItems[direct][1];
                    if (IsTransparent(pixMap, x, y))
                    {
                        direct = (direct + 1) % directItems.Length;
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
                    else
                    {
                        path.LineTo(curX, curY);
                    }
                    beginDirect = (direct + 5) % directItems.Length;
                    break;
                }
                if (!hasPoint)
                {
                    // 所有方向都没有结束
                    return null;
                }
            }
            return path;
        }

        private static bool LineIsIntersecting(SKPoint aBegin, SKPoint aEnd,
            SKPoint bBegin, SKPoint bEnd)
        {
            var aVector = aEnd - aBegin;
            var bVector = bEnd - bBegin;
            var cross = aVector.X * bVector.Y - aVector.Y * bVector.X;
            if (Math.Abs(cross) < 1e-8)
            {
                return false;
            }
            var diff = bBegin - aBegin;
            var t = (diff.X * bVector.Y - diff.Y * bVector.X) / cross;
            var u = (diff.X * aVector.Y - diff.Y * aVector.X) / cross;
            return 0 <= t && t <= 1 && 0 <= u && u <= 1;
        }
    }
}
