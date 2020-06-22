using Ge.Public.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI4Android.Tools;

namespace WebAPI4Android.Controllers
{
    /// <summary>
    /// 继承自BaseController 只提供Android客户端使用，所有判断Android的地方均为true
    /// </summary>

    [ApiPermissionFilter]
    public class BaseAndroidController : BaseController
    {
        protected string Language = "1001";
        List<string> NoTokenUrl = new List<string>() { "/api/user/login", "/api/user/getdatabaselist", "/api/user/sendverificationcode", "/api/user/checktoken", "/api/user/updatauserpicture" };
        protected Models.TokenModel tokenModel = null;
        public BaseAndroidController() : base()
        {
            //获取访问语言
            Language = GetLanguage();
            HttpRequest httpRequest = HttpContext.Current.Request;
            string path = httpRequest.Path;
            string RawUrl = httpRequest.RawUrl;
            TraceEx.Write(SysMessageLevel.SysReceiveInfo, string.Format("访问路径：{0}", RawUrl));
            if (!NoTokenUrl.Contains(path.ToLower()))
            {
                tokenModel = CheckAndGetToken();
                HttpContext.Current.Request.Headers.Add("TokenStatus", tokenModel.CheckResult.ToString());
            }
            else
                HttpContext.Current.Request.Headers.Add("TokenStatus", CheckToken_Success.ToString());
        }

        public string GetVCodeText(int code) => GetVCodeText(Language, code);

        public string GetLocalText(string key) => GetLocalText(Language, key);

        public string GetTokenCodeText(int code) => GetTokenCodeText(Language, code);

        /// <summary>
        /// 重载查询数据库中指定Key的Value
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="FieldName">Key</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>查询成功返回string 失败返回""</returns>
        public string QueryByID(string sql, string FieldName, bool isFormalDataBase) => ToolDataBase.QueryByID(sql, FieldName, true, isFormalDataBase);




        /// <summary>
        /// 重载查询数据库并返回一个实体类
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>查询成功返回对应的泛型，失败则返回null</returns>
        public T Query<T>(string sql, bool isFormalDataBase) => ToolDataBase.Query<T>(sql, true, isFormalDataBase);



        /// <summary>
        /// 重载查询数据库并返回一个实体类(防sql注入)
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="Parameter">参数</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>查询成功返回对应的泛型，失败则返回null</returns>
        public T Query<T>(string sql, Dictionary<string, object> Parameter, bool isFormalDataBase) => ToolDataBase.Query<T>(sql, Parameter, true, isFormalDataBase);




        /// <summary>
        /// 重载查询数据库并返回一个List<T>
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>查询成功返回对应的List<泛型>，失败则返回null</returns>
        public List<T> QueryList<T>(string sql, bool isFormalDataBase) => ToolDataBase.QueryList<T>(sql, true, isFormalDataBase);



        /// <summary>
        /// 重载查询数据库并返回一个List<T>(防sql注入)
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="Parameter">参数</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>>查询成功返回对应的List<泛型>，失败则返回null</returns>
        public List<T> QueryList<T>(string sql, Dictionary<string, object> Parameter, bool isFormalDataBase) => ToolDataBase.QueryList<T>(sql, Parameter, true, isFormalDataBase);



        /// <summary>
        /// 重载获取数据库地址
        /// </summary>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>返回一个服务器地址string</returns>
        public string GetServer(bool isFormalDataBase) => ToolDataBase.GetServer(true, isFormalDataBase);




        /// <summary>
        /// 写入数据库
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>写入成功返回写入了多少行数据，失败则返回0</returns>
        public int Exec(string sql, bool isFormalDataBase) => ToolDataBase.Exec(sql, true, isFormalDataBase);

        /// <summary>
        /// 重载查询数据库中指定Key的Value
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="FieldName">Key</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>查询成功返回string 失败返回""</returns>
        public string QueryByID(string sql, string FieldName) => ToolDataBase.QueryByID(sql, FieldName, true, tokenModel.isFormalDataBase);




        /// <summary>
        /// 重载查询数据库并返回一个实体类
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>查询成功返回对应的泛型，失败则返回null</returns>
        public T Query<T>(string sql) => ToolDataBase.Query<T>(sql, true, tokenModel.isFormalDataBase);



        /// <summary>
        /// 重载查询数据库并返回一个实体类(防sql注入)
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="Parameter">参数</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>查询成功返回对应的泛型，失败则返回null</returns>
        public T Query<T>(string sql, Dictionary<string, object> Parameter) => ToolDataBase.Query<T>(sql, Parameter, true, tokenModel.isFormalDataBase);




        /// <summary>
        /// 重载查询数据库并返回一个List<T>
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>查询成功返回对应的List<泛型>，失败则返回null</returns>
        public List<T> QueryList<T>(string sql) => ToolDataBase.QueryList<T>(sql, true, tokenModel.isFormalDataBase);



        /// <summary>
        /// 重载查询数据库并返回一个List<T>(防sql注入)
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="Parameter">参数</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>>查询成功返回对应的List<泛型>，失败则返回null</returns>
        public List<T> QueryList<T>(string sql, Dictionary<string, object> Parameter) => ToolDataBase.QueryList<T>(sql, Parameter, true, tokenModel.isFormalDataBase);



        /// <summary>
        /// 重载获取数据库地址
        /// </summary>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>返回一个服务器地址string</returns>
        public string GetServer() => ToolDataBase.GetServer(true, tokenModel.isFormalDataBase);




        /// <summary>
        /// 写入数据库
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>写入成功返回写入了多少行数据，失败则返回0</returns>
        public int Exec(string sql) => ToolDataBase.Exec(sql, true, tokenModel.isFormalDataBase);



        /// <summary>
        /// 创建并保存token到指定用户数据中
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>操作成功后返回token</returns>
        public string CreatAndSaveToken(int UserID, bool isFormalDataBase) => CreatAndSaveToken(UserID,true, isFormalDataBase);

        /// <summary>
        /// 保存验证码
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="code">验证码</param>
        public void SaveVerificationCode(string phone,string code, bool isFormalDataBase) => SaveVerificationCode(phone, code,true, isFormalDataBase);


        /// <summary>
        /// 检查验证码
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="VCode"></param>
        /// <param name="isFormalDataBase"></param>
        /// <returns>0、验证码有效。1、查询不到该用户的验证码数据。2、验证码为空。3、验证码不匹配。4、验证码已过时</returns>
        public int CheckVerificationCode(int UserID, string VCode, bool isFormalDataBase) => CheckVerificationCode( UserID,  VCode,  true,  isFormalDataBase);
    }

    /// <summary>
    /// 拦截器
    /// </summary>
    public class ApiPermissionFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            try
            {
                int TokenStatus = HttpContext.Current.Request.Headers.Get("TokenStatus").ToInt32();
                if (TokenStatus != BaseController.CheckToken_Success) 
                {
                    string result = JsonConvert.SerializeObject(new Ge.DynamicPublic.ReturnMessage(Ge.DynamicPublic.ReturnMessType.Error, BaseController.GetTokenCodeText(BaseController.GetLanguage(), TokenStatus)));
                    HttpContext.Current.Response.Write(result);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                }
            }
            catch (Exception ex)
            {
                string result = JsonConvert.SerializeObject(new Ge.DynamicPublic.ReturnMessage(Ge.DynamicPublic.ReturnMessType.Error, ex.Message));
                HttpContext.Current.Response.Write(result);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            }
            finally
            {
                //SmartCardCommon.NhibernaterSessionHelper.CloseSession();
            }
        }
    }
}
