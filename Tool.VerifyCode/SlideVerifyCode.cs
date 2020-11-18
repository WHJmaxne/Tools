using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Tool.VerifyCode
{
    public class SlideVerifyCode
    {
        //裁剪位置X轴最小位置
        private int _MinRangeX;
        //裁剪位置X轴最大位置
        private int _MaxRangeX;
        //裁剪位置Y轴最小位置
        private int _MinRangeY;
        //裁剪位置Y轴最大位置
        private int _MaxRangeY;
        //裁剪X轴大小 裁剪成20张上10张下10张
        private int _CutX;
        //裁剪Y轴大小 裁剪成20张上10张下10张
        private int _CutY;
        //小图相对原图左上角的x坐标  x坐标保存到session 用于校验
        private int _PositionX;
        //小图相对原图左上角的y坐标  y坐标返回到前端
        private int _PositionY;
        private readonly VerifyCodeOptions _option;
        public SlideVerifyCode(IOptions<VerifyCodeOptions> option)
        {
            _option = option.Value;

            this._MinRangeX = _option.ImagesWidth / 3;
            this._MaxRangeX = (_option.ImagesWidth / 3) * 2;

            this._MinRangeY = _option.ImagesHeight / 3;
            this._MaxRangeY = (_option.ImagesHeight / 3) * 2;

            this._CutX = _option.ImagesWidth / 10;
            this._CutY = _option.ImagesHeight / 2;
        }

        /// <summary>
        /// 获取滑块验证码
        /// </summary>
        /// <returns></returns>
        public SlideVerifyCodeResult GetSlideVerifyCode()
        {
            SlideVerifyCodeResult result = new SlideVerifyCodeResult();
            result.Width = _option.ImagesWidth;
            result.Height = _option.ImagesHeight;
            result.ShearSize = _option.ShearSize;
            Random rd = new Random();
            _PositionX = rd.Next(_MinRangeX, _MaxRangeX);
            _PositionY = rd.Next(_MinRangeY, _MaxRangeY);
            using (Bitmap bmp = new Bitmap(Path.Combine(_option.ImagesPath, (new Random()).Next(1, GetImageCount()) + ".jpg")))
            {
                using Bitmap cut = CutImage(bmp, _option.ShearSize, _option.ShearSize, _PositionX, _PositionY);
                using Bitmap normal = GetNewBitMap(bmp, _PositionX, _PositionY);
                result.CutImage = "data:image/jpg;base64," + ImgToBase64String(cut);
                result.NormalImage = "data:image/jpg;base64," + ImgToBase64String(normal);
                result.PositionX = _PositionX;
                result.PositionY = _PositionY;
            }
            return result;
        }

        /// <summary>
        /// 获取裁剪的小图
        /// </summary>
        /// <param name="sFromBmp">原图</param>
        /// <param name="cutWidth">剪切宽度</param>
        /// <param name="cutHeight">剪切高度</param>
        /// <param name="x">X轴剪切位置</param>
        /// <param name="y">Y轴剪切位置</param>
        private Bitmap CutImage(Bitmap sFromBmp, int cutWidth, int cutHeight, int x, int y)
        {
            //载入底图
            Image fromImage = sFromBmp;
            //先初始化一个位图对象，来存储截取后的图像
            Bitmap bmpDest = new Bitmap(cutWidth, cutHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            using (Graphics g = Graphics.FromImage(bmpDest))
            {
                //这个矩形定义了，你将要在被截取的图像上要截取的图像区域的左顶点位置和截取的大小
                Rectangle rectSource = new Rectangle(x, y, cutWidth, cutHeight);

                //这个矩形定义了，你将要把 截取的图像区域 绘制到初始化的位图的位置和大小
                //我的定义，说明，我将把截取的区域，从位图左顶点开始绘制，绘制截取的区域原来大小
                Rectangle rectDest = new Rectangle(0, 0, cutWidth, cutHeight);

                //第一个参数就是加载你要截取的图像对象，第二个和第三个参数及如上所说定义截取和绘制图像过程中的相关属性，第四个属性定义了属性值所使用的度量单位

                g.DrawImage(fromImage, rectDest, rectSource, GraphicsUnit.Pixel);
                return bmpDest;
            }
        }


        /// <summary>
        /// 获取裁剪小图后的原图
        /// </summary>
        /// <param name="sFromBmp">原图</param>
        /// <param name="cutWidth">剪切宽度</param>
        /// <param name="cutHeight">剪切高度</param>
        /// <param name="spaceX">X轴剪切位置</param>
        /// <param name="spaceY">Y轴剪切位置</param>
        private Bitmap GetNewBitMap(Bitmap sFromBmp, int spaceX, int spaceY)
        {
            // 加载原图片 
            Bitmap oldBmp = sFromBmp;
            // 绑定画板 
            Graphics grap = Graphics.FromImage(oldBmp);
            // 加载水印图片 
            Bitmap bt = new Bitmap(_option.ShearSize, _option.ShearSize);
            Graphics g1 = Graphics.FromImage(bt);  //创建b1的Graphics
            g1.FillRectangle(Brushes.Black, new Rectangle(0, 0, _option.ShearSize, _option.ShearSize));   //把b1涂成红色
            bt = PTransparentAdjust(bt, 120);
            // 添加水印 
            grap.DrawImage(bt, spaceX, spaceY, _option.ShearSize, _option.ShearSize);
            grap.Dispose();
            g1.Dispose();
            return oldBmp;
        }

        /// <summary>
        /// 获取混淆拼接的图片
        /// </summary>
        /// <param name="a">无序数组</param>
        /// <param name="bmp">剪切小图后的原图</param>
        private Bitmap ConfusionImage(int[] a, Bitmap cutbmp)
        {
            Bitmap[] bmp = new Bitmap[20];
            for (int i = 0; i < 20; i++)
            {
                int x, y;
                x = a[i] > 9 ? (a[i] - 10) * _CutX : a[i] * _CutX;
                y = a[i] > 9 ? _CutY : 0;
                bmp[i] = CutImage(cutbmp, _CutX, _CutY, x, y);
            }
            Bitmap Img = new Bitmap(_option.ImagesWidth, _option.ImagesHeight);      //创建一张空白图片
            Graphics g = Graphics.FromImage(Img);   //从空白图片创建一个Graphics
            for (int i = 0; i < 20; i++)
            {
                //把图片指定坐标位置并画到空白图片上面
                g.DrawImage(bmp[i], new Point(i > 9 ? (i - 10) * _CutX : i * _CutX, i > 9 ? _CutY : 0));
                bmp[i].Dispose();
            }
            g.Dispose();
            return Img;
        }

        /// <summary>
        /// 获取半透明图像
        /// </summary>
        /// <param name="bmp">Bitmap对象</param>
        /// <param name="alpha">alpha分量。有效值为从 0 到 255。</param>
        private Bitmap PTransparentAdjust(Bitmap bmp, int alpha)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color bmpcolor = bmp.GetPixel(i, j);
                    //byte A = bmpcolor.A;
                    byte R = bmpcolor.R;
                    byte G = bmpcolor.G;
                    byte B = bmpcolor.B;
                    bmpcolor = Color.FromArgb(alpha, R, G, B);
                    bmp.SetPixel(i, j, bmpcolor);
                }
            }
            return bmp;
        }

        private int GetImageCount()
        {
            int count = Directory.GetFiles(this._option.ImagesPath).Length;
            return count;
        }


        //Bitmap转为base64编码的文本
        private string ImgToBase64String(Bitmap bmp)
        {
            try
            {
                using MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch
            {
                //ImgToBase64String 转换失败\nException:" + ex.Message);
                return null;
            }
        }
        //base64编码的文本转为Bitmap
        private Bitmap Base64StringToImage(string txtBase64)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(txtBase64);
                using MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                ms.Close();
                return bmp;
            }
            catch
            {
                //Base64StringToImage 转换失败\nException：" + ex.Message);
                return null;
            }
        }
    }
}
