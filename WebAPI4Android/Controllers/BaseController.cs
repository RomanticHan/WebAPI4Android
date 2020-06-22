using Dapper;
using Ge;
using Ge.Public.WebChat;
using Newtonsoft.Json.Linq;
using SpApiV1;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI4Android.Models;
using WebAPI4Android.Tools;

namespace WebAPI4Android.Controllers
{
    /// <summary>
    /// Controller基类，封装一些基础工具
    /// </summary>
    public class BaseController : ApiController
    {
        

        public const int CheckToken_Success = 1;//token正常
        public const int CheckToken_NoToken = 2;//接口没有传token
        public const int CheckToken_NoLogin = 3;//数据库中没有token 说明没有登录
        public const int CheckToken_TimeOut = 4;//token时限已超出
        public const int CheckToken_TokenError = 5;//token校验错误/被修改   

        public const int CheckVCode_Success = 1;//验证码有效
        public const int CheckVCode_NoData = 2;//查询不到该用户的验证码数据
        public const int CheckVCode_VCodeError = 3;//验证码不匹配
        public const int CheckVCode_TimeOut = 4;//验证码已过时


        /// <summary>
        /// 创建并保存token到指定用户数据中
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>操作成功后返回token</returns>
        public static string CreatAndSaveToken(int UserID, bool isAndroid, bool isFormalDataBase)
        {
            //token规则 （UserID/时间戳/是否为安卓库/是否为正式库）
            string token = (UserID + "/" + GetTimeStampNow() + "/" + isAndroid + "/" + isFormalDataBase).Md5Encrypt();
            //UpDate结果为0则说明数据库中没有课UpDate的数据 则直接创建一条数据
            if (ToolDataBase.Exec("UPDATE dbo.t_emp_UserToken SET FToken = '" + token + "' WHERE FUserID = '" + UserID + "'", isAndroid, isFormalDataBase) == 0)
                ToolDataBase.Exec("INSERT INTO dbo.t_emp_UserToken VALUES ( '" + UserID + "','" + token + "')", isAndroid, isFormalDataBase);

            return token;
        }


        /// <summary>
        /// 检查并获取token的实体类
        /// </summary>
        /// <returns>检查通过后生成一个完整的token实体类，失败则生成一个只带有检查结果码的实体类</returns>
        public static TokenModel CheckAndGetToken()
        {
            //获取header里的token
            string ResultToken = HttpContext.Current.Request.Headers.Get("Token");

            //header的token为空则说明接口没有传输header
            if (ResultToken.IsEmpty()) return new TokenModel(CheckToken_NoToken);

            String[] data = null;
            try
            {
                data = ResultToken.Md5Decrypt().Split('/');
            }
            catch
            {
                return new TokenModel(CheckToken_TokenError);
            }
            //将header进行MD5解密并根据/拆分 得到token里的数据

            //如果拆分出来的数组不为4则说明token格式不正确
            if (data.Length != 4) return new TokenModel(CheckToken_TokenError);

            //根据接口传输回来的token中的（ID、时间戳、是否为安卓数据库、是否为正式库）4个参数去查询用户登录时生成的token
            string UserToken = ToolDataBase.QueryByID("SELECT TOP 1 FToken FROM dbo.t_emp_UserToken WHERE FUserID = '" + data[0] + "'", "FToken", bool.Parse(data[2]), bool.Parse(data[3]));

            //数据库中的token和接口收到的token进行对比  不一致则说明被篡改
            if (!ResultToken.Equals(UserToken)) return new TokenModel(CheckToken_TokenError);

            //如果查询结果为空则说明该用户没有登录过
            if (UserToken.IsEmpty()) return new TokenModel(CheckToken_NoLogin);

            //解码用户的token
            data = UserToken.Md5Decrypt().Split('/');

            //获取token中的时间戳(单位:秒)
            long date = long.Parse(data[1]);

            //检查时间戳与当前时间的差值，差值大于8小时则说明token时效超出
            if ((GetTimeStampNow() - date) > 8 * 60 * 60) return new TokenModel(CheckToken_TimeOut);

            //检查通过后生成Token实体类
            return new TokenModel(int.Parse(data[0]), long.Parse(data[1]), bool.Parse(data[2]), bool.Parse(data[3]));

        }

        /// <summary>
        /// 从http Header上获取语言ID
        /// </summary>
        /// <returns></returns>
        public static string GetLanguage()
        {
            //获取语言ID
            string Language = HttpContext.Current.Request.Headers.Get("Language");
            if (Language.IsNullOrEmpty())
                return "1001";
            return Language;
        }


        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns>返回一个单位为秒的时间戳</returns>
        public static long GetTimeStampNow() => (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;


        /// <summary>
        /// 获取本地化的文本内容
        /// </summary>
        /// <param name="Language">语言ID</param>
        /// <param name="key">文本key值</param>
        /// <returns></returns>
        public static string GetLocalText(string Language, string key)
        {
            return ToolDataBase.GetLocalText(Language, key);
        }

        /// <summary>
        /// 根据检查token的返回码  读取相应的说明
        /// </summary>
        /// <param name="isChinese"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetTokenCodeText(string Language, int code)
        {
            switch (code)
            {
                //token正常
                case CheckToken_Success:
                    return GetLocalText(Language, "CheckToken_Success");

                //接口没有传token
                case CheckToken_NoToken:
                    return GetLocalText(Language, "CheckToken_NoToken");

                //数据库中没有token
                case CheckToken_NoLogin:
                    return GetLocalText(Language, "CheckToken_NoLogin");

                //token时限已超出                   
                case CheckToken_TimeOut:
                    return GetLocalText(Language, "CheckToken_TimeOut");

                //token校验错误/被修改
                case CheckToken_TokenError:
                    return GetLocalText(Language, "CheckToken_TokenError");

                //验证码不存在
                default:
                    return GetLocalText(Language, "CheckVCode_NotExist");
            }
        }


        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns></returns>
        public static RstArray SendMSG(string phone, string text)
        {
            //接口地址
            string methodUrl = "/cmc/sms/send";

            //初始化一信通SDK
            SpApi sdk = new SpApi();

            //企业编号
            sdk.SpCode = "280393";

            //用户名
            sdk.UserName = "jd_jtyzgs";

            //签名密钥
            sdk.Key = "3ddbe84d94509a2a30a91247d72539ae";

            //API地址
            sdk.ApiUrl = "https://api.ums86.com";

            //拼接参数
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("text", text);
            param.Add("sendObject", phone);


            //接收返回结果
            RstArray result_array = sdk.Api(methodUrl, param);
            return result_array;
            //-----------------结果示例-------------------
            //  result_array.Msg={"taskId":"20042716244413580995","returnCode":"200","returnMsg":"成功","productId":"1"}
            //  result_array.Ret=0
            //
            //  返回码列表 
            //
            //   200    成功
            //   499    服务错误
            //   1801   签名不合法
            //   1802   必填参数为空
            //   1803   接口服务未开通，账户无权限
            //   1804   频繁调用（同一接口同样的参数10分钟内连续调用超过5次会报此异常）
            //   1805   预约发送时间格式不正确，应为yyyyMMddHHmmss
            //   1806   含有无效的手机号码
            //   1807   Ip不合法
            //   1808   流水号格式不正确
            //   1809   流水号重复
            //   1810   余额不足
            //   1811   需要人工审核
            //   1812   发送内容与模板不符
            //   1813   发送内容含有禁止敏感词
            //   1814   同一号码相同内容发送次数太多
            //   1815   没有与充值流量相匹配的商品
            //   2801   系统错误
            //   2802   生成签名失败
            //   2803   账号或者密码错误
            //   3801   验密不通过
            //--------------------------------------------

            //结果转换成json并读取返回码 返回200则发送成功
            //   string returnCode = JObject.Parse(result_array.Msg)["returnCode"].ToString();
            //   return returnCode == "200" ? code : "";


        }


        /// <summary>
        /// 生成随机6位数
        /// </summary>
        /// <returns></returns>
        public static string CreatCode()
        {
            string code = String.Empty;
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                code += random.Next(10).ToString();
            }
            return code;
        }


        /// <summary>
        /// 保存验证码
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="code">验证码</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        public static void SaveVerificationCode(string phone, string code, bool isAndroid, bool isFormalDataBase)
        {

            string UserID = ToolDataBase.QueryByID("SELECT FUserID FROM s_user t INNER JOIN t_Emp t0 ON t.FEmpID = t0.FitemID WHERE t0.FPhone = '" + phone + "'", "FUserID", isAndroid, isFormalDataBase);
            long date = GetTimeStampNow();
            //UpDate结果为0则说明数据库中没有课UpDate的数据 则直接创建一条数据
            if (ToolDataBase.Exec("UPDATE dbo.t_emp_UserVCode SET FVerificationCode = '" + code + "' ,FDate='" + date + "' WHERE FUserID = '" + UserID + "'", isAndroid, isFormalDataBase) == 0)
                ToolDataBase.Exec("INSERT INTO dbo.t_emp_UserVCode VALUES ( '" + UserID + "','" + code + "','" + date + "')", isAndroid, isFormalDataBase);

        }

        /// <summary>
        /// 检查验证码
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="VCode">验证码</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>1、验证码有效。2、查询不到该用户的验证码数据。3、验证码为空。4、验证码不匹配。5、验证码已过时</returns>
        public static int CheckVerificationCode(int UserID, string VCode,bool isAndroid, bool isFormalDataBase)
        {
            VCodeModel VCM = ToolDataBase.Query<VCodeModel>("SELECT *  FROM dbo.t_emp_UserVCode WHERE FUserID = '" + UserID + "'", isAndroid, isFormalDataBase);
           
            //没有该用户的验证码信息
            if (VCM == null) return CheckVCode_Success;

            //验证码不匹配
            if(!VCM.FVerificationCode.Equals(VCode)) return 3;

            //验证码已过时
            if ((GetTimeStampNow() - VCM.FDate) > 10 * 60) return 4 ;

            //验证码有效
            return 0;
        }

        public static string GetVCodeText(string Language, int code)
        {
            switch (code)
            {
                //查询不到该用户的验证码数据
                case CheckVCode_NoData:
                    return GetLocalText(Language, "CheckVCode_NoData");

                //验证码不匹配                   
                case CheckVCode_VCodeError:
                    return GetLocalText(Language, "CheckVCode_VCodeError");

                //验证码已过时                   
                case CheckVCode_TimeOut:
                    return GetLocalText(Language, "CheckVCode_TimeOut");

                //验证码不存在
                default:
                    return GetLocalText(Language, "CheckVCode_NotExist");
            }
        }

        public static string SetPicture(string EmpID, string ImageBase64, string UserID)
        {
        
            try
            {

                byte[] ByteImage = Convert.FromBase64String(ImageBase64);
                //1.连接图片服务器，配置服务器信息及调用FTPHelper
                string FServerURL = "ftp://192.168.99.79:1688/";
                string FServerUserName = "xingti";
                string FServerUserPassWord = "erp77966";
                FTPHelper myFtpHelper = null;
                List<Ge.Public.FTP.AttachFileInfo> FileInfos = new List<Ge.Public.FTP.AttachFileInfo>();

                Uri url = new Uri(FServerURL);
                myFtpHelper = new FTPHelper(url, FServerUserName, FServerUserPassWord);
                var sqlstr = string.Format(@" 
                    SELECT t3.Fname + '/员工/' + CAST(year(t1.FBeginHireDate) AS varchar(4)) + '/' + CAST(month(t1.FBeginHireDate) AS varchar(4)) + '/' + t1.Fname + '/' + t1.Fnumber + '.jpg' AS FFullName
	                    , Fpicture
                        , T1.FitemID
                        , t3.Fname + '/员工/' + CAST(year(t1.FBeginHireDate) AS varchar(4)) + '/' + CAST(month(t1.FBeginHireDate) AS varchar(4)) + '/' + t1.Fname AS FPreDir
                    FROM t_emp t1
	                    INNER JOIN dbo.t_Emp_picture t2 ON t1.FitemID = t2.FitemID
	                    INNER JOIN dbo.s_GR_FrameWork t3 ON t1.FOrganizeID = t3.FitemID
                    WHERE t1.FleaveDate IS NULL And Fpicture is not null AND t1.FitemID='{0}'", EmpID);//(FIsUploadFile = 0 OR FIsUploadFile IS NULL) AND 

                var dt = ToolDataBase.Query<string>(sqlstr,true,true);

                if (dt != null)
                {
                  
                    /*    //byte[] datas = row["Fpicture"] as byte[];
                        string FFullName = row["FFullName"].ToMString();
                        string FitemID = row["FitemID"].ToMString();
                        string FPreDir = row["FPreDir"].ToMString();
                        //3.1服务器创建文件
                        if (!myFtpHelper.DirectoryExist(FPreDir))
                        {
                            myFtpHelper.MakeDirectory(FPreDir);
                        }
                        //3.2 上传文件
                        myFtpHelper.UploadFileAsync(ByteImage, FFullName, true);
                        //3.3 插入hq_From_AttachFiles 先删除再添加
                        string insertSql = string.Format(@"
                        delete from hq_From_AttachFiles where FInterID={0};
                        INSERT INTO hq_From_AttachFiles
                        (FEntryID, FOrganizeID, FYear, FPeriod, FClassID
                        , FClassName, FInterID, FFullName, FFullPath, FFileName
                        , FFileLength, FFileID, FFieldName, FType, FUpLoaderID
                        , FUpLoadDate, FFileNote, FClassificationID)
                        SELECT 1 AS FEntryID, t1.FOrganizeID, year(t1.FBeginHireDate) AS FYear
                            , month(t1.FBeginHireDate) AS FPeriod, 130 AS FClassID, '员工' AS FClassName
                            , t1.FitemID AS FInterID
	                        , t3.Fname + '/员工/' + CAST(year(t1.FBeginHireDate) AS varchar(4)) + '/' + CAST(month(t1.FBeginHireDate) AS varchar(4)) + '/' + t1.Fname + '/' + t1.Fnumber + '.jpg' AS FFullName
                            , t3.Fname + '/员工/' + CAST(year(t1.FBeginHireDate) AS varchar(4)) + '/' + CAST(month(t1.FBeginHireDate) AS varchar(4)) + '/' + t1.Fname AS FFullPath
                            , t1.Fnumber + '.jpg' AS FFileName
                            , 15524 AS FFileLength, 0 AS FFileID, '' AS FFieldName, 0 AS FType, {1} AS FUpLoaderID
                            , GETDATE() AS FUpLoadDate, '' AS FFileNote, '' AS FClassificationID
                        FROM t_emp t1
                        INNER JOIN dbo.t_Emp_picture t2 ON t1.FitemID = t2.FitemID
                        INNER JOIN dbo.s_GR_FrameWork t3 ON t1.FOrganizeID = t3.FitemID
                        WHERE  t1.FleaveDate IS NULL And Fpicture is not null AND t2.FitemID={0} ", FitemID, UserID);//(FIsUploadFile = 0 OR FIsUploadFile IS NULL) AND
                        Db.Main.Exec(insertSql);
                        //3.4 修改t_Emp_picture表
                        string updateSql = string.Format(@"update t_Emp_picture set FisUploadFIle=1 where FitemID={0}", FitemID);
                        Db.Main.Exec(updateSql);*/
                    
                    return "上传成功!";
                }
                else
                {
                    return "上传失败!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
          
        }

    }

}
