using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.VerifyCode
{
    public class SlideVerifyCodeResult
    {
        public string Key { get; set; } = Guid.NewGuid().ToString();
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string CutImage { get; set; }
        public string NormalImage { get; set; }
        public int[] ConfusionArray { get; set; }
        public string ConfusionImage { get; set; }
        public int ShearSize { get; set; }
    }
}
