using System;
using System.Drawing;

namespace Tool.VerifyCode
{
    public interface IVerifyCodeService
    {
        #region 滑块验证码
        /// <summary>
        /// 滑块验证码
        /// </summary>
        /// <returns></returns>
        SlideVerifyCodeResult SlideVerifyCode();
        #endregion

        #region 图片验证码
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        string CreateVerifyCode();
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="type">验证码类型</param>
        /// <param name="length">验证码长度</param>
        /// <returns></returns>
        string CreateVerifyCode(VerifyCodeType type, int length);
        /// <summary>
        /// 图片验证码(Bitmap)
        /// </summary>
        /// <param name="verifyCode">验证码</param>
        /// <returns></returns>
        Bitmap BitmapVerifyCode(string verifyCode);

        /// <summary>
        /// 图片验证码(Bitmap)
        /// </summary>
        /// <param name="verifyCode">验证码</param>
        /// <param name="width">图片宽</param>
        /// <param name="height">图片高</param>
        /// <returns></returns>
        Bitmap BitmapVerifyCode(string verifyCode, int width, int height);

        /// <summary>
        /// 图片验证码(byte[])
        /// </summary>
        /// <param name="verifyCode">验证码</param>
        /// <returns></returns>
        byte[] BytesVerifyCode(string verifyCode);

        /// <summary>
        /// 图片验证码(byte[])
        /// </summary>
        /// <param name="verifyCode">验证码</param>
        /// <param name="width">图片宽</param>
        /// <param name="height">图片高</param>
        /// <returns>byte[]</returns>
        byte[] BytesVerifyCode(string verifyCode, int width, int height);

        #endregion
    }
}
