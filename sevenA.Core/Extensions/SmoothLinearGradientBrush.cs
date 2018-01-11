namespace sevenA.Core.Extensions
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    [MarkupExtensionReturnType(typeof(Brush))]
    public class SmoothLinearGradientBrush : MarkupExtension
    {
        private static readonly PropertyInfo DpiX;
        private static readonly PropertyInfo DpiY;
        private static readonly byte[,] BayerMatrix =
        {
        { 1, 9, 3, 11 },
        { 13, 5, 15, 7 },
        { 1, 9, 3, 11 },
        { 16, 8, 14, 6 }
    };

        static SmoothLinearGradientBrush()
        {
            DpiX = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            DpiY = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
        }

        public Color From { get; set; }

        public Color To { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            int width = 20;
            int height = (int)SystemParameters.VirtualScreenHeight;
            int dpix = (int)DpiX.GetValue(null);
            int dpiy = (int)DpiY.GetValue(null);

            int stride = 4 * (((width * PixelFormats.Bgr24.BitsPerPixel) + 31) / 32);

            double bayerMatrixCoefficient = 1.0 / (BayerMatrix.Length + 1);
            int bayerMatrixSize = BayerMatrix.GetLength(0);

            byte[] buffer = new byte[height * stride];

            for (int line = 0; line < height; line++)
            {
                double scale = (double)line / height;

                for (int x = 0; x < width * 3; x += 3)
                {
                    double blue = (this.To.B * scale) + (this.From.B * (1.0 - scale));
                    double green = (this.To.G * scale) + (this.From.G * (1.0 - scale));
                    double red = (this.To.R * scale) + (this.From.R * (1.0 - scale));

                    buffer[x + (line * stride) + 1] = (byte)(green + (bayerMatrixCoefficient * BayerMatrix[x % bayerMatrixSize, line % bayerMatrixSize]));
                    buffer[x + (line * stride)] = (byte)(blue + (bayerMatrixCoefficient * BayerMatrix[x % bayerMatrixSize, line % bayerMatrixSize]));
                    buffer[x + (line * stride) + 2] = (byte)(red + (bayerMatrixCoefficient * BayerMatrix[x % bayerMatrixSize, line % bayerMatrixSize]));
                }
            }

            var image = BitmapSource.Create(width, height, dpix, dpiy, PixelFormats.Bgr24, null, buffer, stride);
            image.Freeze();
            var brush = new ImageBrush(image);
            brush.Freeze();
            return brush;
        }
    }
}