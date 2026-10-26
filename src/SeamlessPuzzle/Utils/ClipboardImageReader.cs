using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SeamlessPuzzle.Utils
{
    public static class ClipboardImageReader
    {
        public static BitmapSource? Read()
        {
            // Explicit PNG carries meaningful transparency; a Windows DIB may have
            // unused alpha bytes set to zero even when it is an opaque screenshot.
            foreach (string name in new[] { "PNG", "image/png" })
            {
                if (!Clipboard.ContainsData(name)) continue;
                object value = Clipboard.GetData(name);
                byte[]? bytes = value as byte[];
                if (value is MemoryStream memory) bytes = memory.ToArray();
                if (bytes == null) continue;
                using var stream = new MemoryStream(bytes);
                var frame = BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                return Snapshot(frame, false);
            }
            if (!Clipboard.ContainsImage()) return null;
            var bitmap = Clipboard.GetImage();
            return bitmap == null ? null : NormalizeDib(bitmap);
        }

        public static BitmapSource NormalizeDib(BitmapSource bitmap)
            => Snapshot(bitmap, true);

        private static BitmapSource Snapshot(BitmapSource bitmap, bool repairDibAlpha)
        {
            var converted = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
            int stride = checked(converted.PixelWidth * 4);
            byte[] bytes = new byte[checked(stride * converted.PixelHeight)];
            converted.CopyPixels(bytes, stride, 0);
            bool allTransparent = true;
            for (int i = 3; i < bytes.Length; i += 4)
                if (bytes[i] != 0) { allTransparent = false; break; }
            if (repairDibAlpha && allTransparent)
                for (int i = 3; i < bytes.Length; i += 4) bytes[i] = 255;
            BitmapSource result = BitmapSource.Create(converted.PixelWidth, converted.PixelHeight, 96, 96, PixelFormats.Bgra32, null, bytes, stride);
            result.Freeze();
            return result;
        }
    }
}
