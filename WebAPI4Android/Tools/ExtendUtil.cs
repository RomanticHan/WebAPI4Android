#region CopyRight

/********************************************************************************* 
 * File name   : StringExtension.cs
 * Author      : Hwj  
 * Create time : 2010-6-5
 * Description : 对System.String的扩展

 * 
 * Update time : xfl
 * Description : 2012-11-22 增加IniValue()方法
 * 
 * Update time : 
 * Description : 
 * **********************************************************************************/

#endregion

#region using region

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

#endregion

/// <summary>
///   字符串相关扩展方法
/// </summary>
public static class StringExtension
{

    private static byte[] iv = { 1, 2, 1, 6, 95, 23, 12, 92 };//定义偏移量  
    private static byte[] key = { 92, 12, 23, 95, 6, 1, 2, 1 };//定义密钥  

    /// <summary>
    /// MD5
    /// </summary>
    /// <param name="strSource"></param>
    /// <returns></returns>
    public static string Md5Encrypt(this string strSource)
    {
        //把字符串放到byte数组中  
        byte[] bytIn = Encoding.Default.GetBytes(strSource);

        //实例DES加密类  
        DESCryptoServiceProvider mobjCryptoService = new DESCryptoServiceProvider();
        mobjCryptoService.Key = key;
        mobjCryptoService.IV = iv;
        ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
        //实例MemoryStream流加密密文件  
        MemoryStream ms = new System.IO.MemoryStream();
        CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
        cs.Write(bytIn, 0, bytIn.Length);
        cs.FlushFinalBlock();

        string strOut = Convert.ToBase64String(ms.ToArray());
        return strOut;
    }


    /// <summary>  
    /// MD5解密  
    /// </summary>  
    /// <param name="Source">需要解密的字符串</param>  
    /// <returns>MD5解密后的字符串</returns>  
    public static string Md5Decrypt(this string Source)
    {
        //将解密字符串转换成字节数组  
        byte[] bytIn = Convert.FromBase64String(Source);
        //给出解密的密钥和偏移量，密钥和偏移量必须与加密时的密钥和偏移量相同  

        DESCryptoServiceProvider mobjCryptoService = new DESCryptoServiceProvider();
        mobjCryptoService.Key = key;
        mobjCryptoService.IV = iv;
        //实例流进行解密  
        MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
        ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
        CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
        StreamReader strd = new StreamReader(cs, Encoding.Default);
        return strd.ReadToEnd();
    }



}
