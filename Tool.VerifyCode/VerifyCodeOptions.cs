using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.VerifyCode
{
    public class VerifyCodeOptions
    {
        /// <summary>
        /// 图片集路径 => 滑块验证码
        /// </summary>
        public string ImagesPath { get; set; }
        /// <summary>
        /// 图片集宽度 => 滑块验证码
        /// </summary>
        public int ImagesWidth { get; set; }
        /// <summary>
        /// 图片集高度 => 滑块验证码
        /// </summary>
        public int ImagesHeight { get; set; }
        /// <summary>
        /// 裁剪块大小 => 滑块验证码
        /// </summary>
        public int ShearSize { get; set; }

        /// <summary>
        /// 验证码类型 => 图片验证码
        /// </summary>
        public VerifyCodeType VerifyCodeType { get; set; } = VerifyCodeType.NumberVerifyCode;

        /// <summary>
        /// 验证码长度 => 图片验证码
        /// </summary>
        public int Length { get; set; } = 4;

        /// <summary>
        /// 验证码图片宽度 => 图片验证码
        /// </summary>
        public int Width { get; set; } = 100;

        /// <summary>
        /// 验证码图片高度 => 图片验证码
        /// </summary>
        public int Height { get; set; } = 40;
    }
}
