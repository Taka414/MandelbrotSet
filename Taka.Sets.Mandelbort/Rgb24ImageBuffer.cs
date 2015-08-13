using System.Windows.Media;

namespace Taka.Sets.Mandelbort
{
    // RGB24形式の画像バッファーを表します。
    public class Rgb24ImageBuffer
    {
        private byte[] buffer;
        private int x;
        private int y;
        private int rawStride;

        public int RawStride { get { return this.rawStride; } }

        /// <summary>
        /// 画像バッファーのサイズを指定してオブジェクトを初期化します。
        /// </summary>
        public Rgb24ImageBuffer(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.rawStride = this.calculateRawStride();
            this.buffer = new byte[rawStride * y];
        }

        /// <summary>
        /// 指定した座標の色を更新します。
        /// </summary>
        public void SetPixel(int x, int y, Color c)
        {
            int xIndex = x * 3;
            int yIndex = y * this.rawStride;
            this.buffer[xIndex + yIndex] = c.R;
            this.buffer[xIndex + yIndex + 1] = c.G;
            this.buffer[xIndex + yIndex + 2] = c.B;
        }

        /// <summary>
        /// 現在のバッファーを取得します。
        /// </summary>
        public byte[] GetBuffer()
        {
            byte[] _tempBuffer = new byte[this.buffer.Length];
            for (int i = 0; i < this.buffer.Length; i++)
            {
                _tempBuffer[i] = this.buffer[i];
            }
            return _tempBuffer;
        }

        // RGB24のRawStrideを計算します。
        private int calculateRawStride()
        {
            return (x * PixelFormats.Rgb24.BitsPerPixel + 7) / 8;
        }
    }
}
