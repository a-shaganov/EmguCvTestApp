using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace EmguCvTestApp.Converters
{
    /// <summary>
    /// Конвертирование рисунка в BitmapSource
    /// </summary>
    public static class BitmapSourceConvert
    {
        /// <summary>
        /// Удаляем GDI объект
        /// </summary>
        /// <param name="o">Указатель на удаляемый объект GDI</param>
        /// <returns>Результат</returns>
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Преобразование изображения в WPF BitmapSource
        /// </summary>
        /// <param name="bitmap">Неотконвертированное изображение</param>
        /// <returns>WPF BitmapSource</returns>
        public static BitmapSource ToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            using (System.Drawing.Bitmap source = bitmap)
            {
                IntPtr ptr = source.GetHbitmap();
                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(ptr);
                return bs;
            }
        }
    }
}
