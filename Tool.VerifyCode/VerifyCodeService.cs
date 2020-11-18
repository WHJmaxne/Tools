using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tool.VerifyCode
{
    public class VerifyCodeService : IVerifyCodeService
    {
        private readonly VerifyCode _verifyCode;
        private readonly SlideVerifyCode _slideVerifyCode;
        private readonly VerifyCodeOptions _options;
        public VerifyCodeService(VerifyCode verifyCode, SlideVerifyCode slideVerifyCode, IOptions<VerifyCodeOptions> option)
        {
            this._verifyCode = verifyCode;
            this._slideVerifyCode = slideVerifyCode;
            this._options = option.Value;
        }

        #region 滑块验证码
        /// <summary>
        /// 滑块验证码
        /// </summary>
        /// <returns></returns>
        public SlideVerifyCodeResult SlideVerifyCode()
        {
            return this._slideVerifyCode.GetSlideVerifyCode();
        }
        #endregion

        #region 图片验证码
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public string CreateVerifyCode()
        {
            return this.CreateVerifyCode(_options.VerifyCodeType, _options.Length);
        }
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="type">验证码类型</param>
        /// <param name="length">验证码长度</param>
        /// <returns></returns>
        public string CreateVerifyCode(VerifyCodeType type, int length)
        {
            return this._verifyCode.CreateVerifyCode(type, length);
        }

        /// <summary>
        /// 图片验证码(Bitmap)
        /// </summary>
        /// <param name="verifyCode">验证码</param>
        /// <returns></returns>
        public Bitmap BitmapVerifyCode(string verifyCode)
        {
            var bitmap = this.BitmapVerifyCode(verifyCode, _options.Width, _options.Height);
            return bitmap;
        }

        /// <summary>
        /// 图片验证码(Bitmap)
        /// </summary>
        /// <param name="verifyCode">验证码</param>
        /// <param name="width">图片宽</param>
        /// <param name="height">图片高</param>
        /// <returns></returns>
        public Bitmap BitmapVerifyCode(string verifyCode, int width, int height)
        {
            var bitmap = this._verifyCode.CreateBitmapByImgVerifyCode(verifyCode, width, height);
            return bitmap;
        }

        /// <summary>
        /// 图片验证码(byte[])
        /// </summary>
        /// <param name="verifyCode">验证码</param>
        /// <returns></returns>
        public byte[] BytesVerifyCode(string verifyCode)
        {
            return this.BytesVerifyCode(verifyCode, _options.Width, _options.Height);
        }

        /// <summary>
        /// 图片验证码(byte[])
        /// </summary>
        /// <param name="verifyCode">验证码</param>
        /// <param name="width">图片宽</param>
        /// <param name="height">图片高</param>
        /// <returns>byte[]</returns>
        public byte[] BytesVerifyCode(string verifyCode, int width, int height)
        {
            return this._verifyCode.CreateByteByImgVerifyCode(verifyCode, width, height);
        }

        #endregion

    }
}
