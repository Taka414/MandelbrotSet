using System;
using System.Threading;

namespace Taka.Sets.Mandelbort
{
    // マンデル・ブロー集合クラス
    public class MandelBrotSet
    {
        // 実際の描画領域の大きさ
        public double ScreenSizeX { get; set; }
        public double ScreenSizeY { get; set; }

        // 横方向の計算開始 - 終了の値
        public double XS { get; set; }
        public double XE { get; set; }

        // 縦方向の計算開始 - 終了の値
        public double YS { get; set; }
        public double YE { get; set; }

        //最大計算回数
        public int MaxAmount { get; set; }

        // 横方向と縦方向の増加量
        public double PitchX { get; set; }
        public double PitchY { get; set; }

        // ダミー条件の設定
        public void SetDummyCondition()
        {
            //this.ScreenSizeX = 1920d; // 描画領域の横幅
            //this.ScreenSizeY = 1080d; // 描画領域の縦幅

            this.ScreenSizeX = 750d; // 描画領域の横幅
            this.ScreenSizeY = 580d; // 描画領域の縦幅

            this.XS = -2.3;
            this.XE = 0.9;

            this.YS = -1.2;
            this.YE = 1.2;

            this.MaxAmount = 15;

            this.ResetCondition();
        }

        public void ResetCondition()
        {
            this.PitchX = Math.Abs((this.XS - this.XE) / this.ScreenSizeX);
            this.PitchY = Math.Abs((this.YS - this.YE) / this.ScreenSizeY);
        }

        // オブジェクトの条件に従って計算を実行
        public int[,] Calculate()
        {
            int[,] result = new int[(int)this.ScreenSizeX + 2, (int)this.ScreenSizeY + 2];
            int i = 0;
            int j = 0;

            for (double ix = XS; ix < XE; ix += PitchX, i++)
            {
                //for (double iy = y; iy < yn; iy += yPitch)
                for (double iy = YS; iy < YE; iy += PitchY, j++)
                {
                    result[i, j] = MandelBrotSet.caclPoint(ix, iy, this.MaxAmount);
                }
                j = 0;
            }
            return result;
        }

        public int[,] CalculateParallel()
        {
            // スレッド開始条件の初期化
            this.Result = new int[(int)this.ScreenSizeX + 2, (int)this.ScreenSizeY + 2];
            this.tHandle.Reset();
            this.threadCnt = 0;
            
            int i = 0;
            
            for (double ix = XS; ix < XE; ix += PitchX, i++)
            {
                ThreadPool.QueueUserWorkItem(calcRow_for_thread, new object[] { i, ix });
            }

            this.maxCnt = i;

            this.tHandle.WaitOne();

            return this.Result;
        }


        // ----->> CalculateParallel用の処理類

        // マルチスレッド用の変数
        private int[,] Result;
        private AutoResetEvent tHandle = new AutoResetEvent(false);
        private int threadCnt = 0;
        private int maxCnt;

        // スレッド終了時に呼び出すメソッド
        private void threadComplete()
        {
            lock (this)
            {
                if (++threadCnt >= maxCnt)
                {
                    this.tHandle.Set();
                }
            }
        }

        // スレッドプールに登録するためのメソッド
        private void calcRow_for_thread(object o)
        {
            object[] oarray = (object[])o;
            this.calcRow((int)oarray[0], (double)oarray[1]);
        }

        // マルチスレッドの実際の計算処理メソッド
        private void calcRow(int i, double ix)
        {
            int j = 0;
            for (double iy = YS; iy < YE; iy += PitchY, j++)
            {
                this.Result[i, j] = MandelBrotSet.caclPoint(ix, iy, this.MaxAmount);
            }

            this.threadComplete();
        }

        // <<----- CalculateParallel用の処理類


        // ある1点の計算を行う
        public static int caclPoint(double a, double b, int max)
        {
            int i = 0; // 現在の繰り返し回数

            double x;
            double xn = 0.0;
            double y;
            double yn = 0.0;
            double s = 0.0; // x^2 + y^2の計算結果
            double so = 0.0;

            do
            {
                x = Math.Pow(xn, 2) - Math.Pow(yn, 2) + a;
                y = 2 * xn * yn + b;

                s = Math.Pow(x, 2) + Math.Pow(y, 2);

                xn = x;
                yn = y;

                i++;

                if (s == so)
                {
                    i = max;
                }
                so = s;
                //Console.WriteLine("{0}回目, x = {1}, y = {2}, S = {3}", i, x, y, s);

            } while (s < 4.0 && i < max);

            return i;
        }

        // 拡大/縮小率の指定。パーセントを整数で指定する(rate = 20 : 20%)
        public void UpdateEnlargementFactor(int rate)
        {
            double _rate = rate / 100.0;

            double _xs = this.XS * _rate;
            double _xe = this.XE * _rate;
            double _xpitch = Math.Abs((_xs - _xe) / this.ScreenSizeX);

            double _ys = this.YS * _rate;
            double _ye = this.YE * _rate;
            double _ypitch = Math.Abs((_ys - _ye) / this.ScreenSizeY);

            double _new_x = this.ScreenSizeX / 2 * _xpitch;
            double _new_y = this.ScreenSizeY / 2 * _ypitch;

            double _x_center = (this.XS + this.XE) / 2.0;
            double _y_center = (this.YS + this.YE) / 2.0;

            this.XS = _x_center - _new_x;
            this.XE = _x_center + _new_x;
            this.YS = _y_center - _new_y;
            this.YE = _y_center + _new_y;

            //this.XS *= _rate;
            //this.XE *= _rate;
            //this.YS *= _rate;
            //this.YE *= _rate;
            this.ResetCondition();
        }
    }
}
