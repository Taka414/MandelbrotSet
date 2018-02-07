using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Taka.Sets.Mandelbort
{
    // 画面表示時の色を管理するクラス
    public class ColorPallet
    {
        /// <summary>
        /// 最大計算回数と計算回数を指定して Color オブジェクトを取得します。
        /// </summary>
        public static Color GetColor(int max, int n)
        {
            if (n > max)
            {
                return Color.FromRgb(0, 0, 0); // Black
            }

            // とりあえずグレースケール
            double cStep = byte.MaxValue / max;

            int baseColor = (int)(cStep * n);

            return Color.FromRgb((byte)baseColor, (byte)baseColor, (byte)baseColor);
        }
    }
}
