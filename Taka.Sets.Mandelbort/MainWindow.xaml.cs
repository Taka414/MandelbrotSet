using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Taka.Sets.Mandelbort
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Rgb24ImageBuffer buffer;
        private MandelBrotSet mandel = new MandelBrotSet();

        public MainWindow()
        {
            this.InitializeComponent();
            this.mandel.SetDummyCondition();
        }

        // 初回表示
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.buffer = new Rgb24ImageBuffer((int)this.mandel.ScreenSizeX, (int)this.mandel.ScreenSizeY);
            this.updateScreen();
        }

        // 画面上をクリック : 中心点の移動
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this.canvas);
            Console.WriteLine("ww({0}, {1})", p.X, p.Y);

            // ここを中心に再設定して10倍に拡大する

            Console.WriteLine("wa({0}, {1})", this.canvas.ActualWidth, this.canvas.ActualHeight);

            // 実際のイメージの場所
            double x = p.X * this.mandel.ScreenSizeX / this.canvas.ActualWidth;
            double y = p.Y * this.mandel.ScreenSizeY / this.canvas.ActualHeight;

            Console.WriteLine("rp({0}, {1})", x, y);

            // 中心点を移動する

            // 実際の画像サイズで中心点からどれくらい距離があるのか計算する
            double nx = this.mandel.ScreenSizeX / 2.0 - x;
            double ny = this.mandel.ScreenSizeY / 2.0 - y;

            // 集合の描画領域を移動する
            nx = this.mandel.PitchX * nx;
            this.mandel.XS -= nx;
            this.mandel.XE -= nx;

            ny = this.mandel.PitchY * ny;
            this.mandel.YS -= ny;
            this.mandel.YE -= ny;

            this.mandel.ResetCondition();

            this.updateScreen();
        }

        // 現在のマンデル集合オブジェクトの状態で画面を再描画する
        private void updateScreen()
        {
            try
            {
                // 再描画
                //int[,] result = this.mandel.Calculate();
                int[,] result = this.mandel.CalculateParallel(); // 遅すぎるのでマルチスレッド化

                for (int j = 0; j < (int)this.mandel.ScreenSizeY; j++)
                {
                    for (int i = 0; i < (int)this.mandel.ScreenSizeX; i++)
                    {
                        this.buffer.SetPixel(i, j, ColorPallet.GetColor(mandel.MaxAmount, result[i, j]));

                        //this.buffer.SetPixel(i, j, Color.FromRgb(100, 0, 255));
                    }
                }

                var brush = new ImageBrush();
                brush.ImageSource =
                    BitmapSource.Create((int)this.mandel.ScreenSizeX, (int)this.mandel.ScreenSizeY, 96, 96,
                        PixelFormats.Rgb24, null, this.buffer.GetBuffer(), this.buffer.RawStride);

                Background = brush;
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show("これ以上拡大できません。\r\n" + ex.ToString(), "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // マウスホイール
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 拡大/縮小率の計算

            if (e.Delta > 0) // 拡大
            {
                //this.mandel.MaxAmount += 12;
                this.mandel.UpdateEnlargementFactor(80);
            }
            else // 縮小
            {
                this.mandel.UpdateEnlargementFactor(120);
            }
            this.updateScreen();
        }

        // 最大計算回数を5回増加させる
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.mandel.MaxAmount += 10;
            this.updateScreen();
        }
    }
}