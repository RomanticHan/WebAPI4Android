using System;
using System.Collections.Generic;
using Dapper;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WebAPI4Android.Tools
{
    public class ToolDataBase
    {
        /// <summary>
        /// 查询数据库中指定Key的Value
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="FieldName">Key</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>查询成功返回string 失败返回""</returns>
        public static string QueryByID(string sql, string FieldName, bool isAndroid, bool isFormalDataBase)
        {
            using (IDbConnection connection = new SqlConnection(GetServer(isAndroid, isFormalDataBase)))
            {
                DataTable table = new DataTable();
                var reader = connection.ExecuteReader(sql);
                table.Load(reader);
                reader.Close();
                return (table != null && table.Rows.Count > 0) ? table.Rows[0][FieldName].ToString() : "";
            }
        }


        /// <summary>
        /// 查询数据库并返回一个实体类
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>查询成功返回对应的泛型，失败则返回null</returns>
        public static T Query<T>(string sql, bool isAndroid, bool isFormalDataBase)
        {
            using (IDbConnection connection = new SqlConnection(GetServer(isAndroid, isFormalDataBase)))
            {
                return connection.Query<T>(sql).FirstOrDefault();
            }
        }



        /// <summary>
        /// 查询数据库并返回一个List<T>
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>查询成功返回对应的List<泛型>，失败则返回null</returns>
        public static List<T> QueryList<T>(string sql, bool isAndroid, bool isFormalDataBase)
        {
            using (IDbConnection connection = new SqlConnection(GetServer(isAndroid, isFormalDataBase)))
            {
                return connection.Query<T>(sql).ToList();
            }
        }

        /// <summary>
        /// 查询数据库并返回一个实体类(防SQL注入)
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="Parameter">参数</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">查询成功返回对应的泛型，失败则返回null</param>
        /// <returns></returns>
        public static T Query<T>(string sql, Dictionary<string, object> Parameter, bool isAndroid, bool isFormalDataBase)
        {
            using (IDbConnection connection = new SqlConnection(GetServer(isAndroid, isFormalDataBase)))
            {
                //声明动态参数
                DynamicParameters Parameters = new DynamicParameters();
                try
                {
                    T result;
                    if (Parameter == null || Parameter.Count <= 0)
                        result = Query<T>(sql, isAndroid, isFormalDataBase);
                    foreach (var key in Parameter.Keys)
                        Parameters.Add(key, Parameter[key]);
                    result = connection.Query<T>(sql, Parameters).FirstOrDefault();
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// 查询数据库并返回一个List<T>(防SQL注入)
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="Parameter">参数</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>查询成功返回对应的List<泛型>，失败则返回null</returns>
        public static List<T> QueryList<T>(string sql, Dictionary<string, object> Parameter, bool isAndroid, bool isFormalDataBase)
        {
            using (IDbConnection connection = new SqlConnection(GetServer(isAndroid, isFormalDataBase)))
            {
                //声明动态参数
                DynamicParameters Parameters = new DynamicParameters();
                try
                {
                    if (Parameter == null || Parameter.Count <= 0)
                        return QueryList<T>(sql, isAndroid, isFormalDataBase);
                    foreach (var key in Parameter.Keys)
                        Parameters.Add(key, Parameter[key]);
                    return connection.Query<T>(sql, Parameters).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }



        /// <summary>
        /// 获取数据库地址
        /// </summary>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>返回一个服务器地址string</returns>
        public static string GetServer(bool isAndroid, bool isFormalDataBase) => isAndroid ?
                (
                    //安卓库
                    isFormalDataBase ?
                        //正式库
                        ConfigurationManager.AppSettings["AndroidFormalDataBase"].ToString().Decrypt() :
                        //测试库
                        ConfigurationManager.AppSettings["AndroidTestDataBase"].ToString().Decrypt()
                 ) : (
                    //微信库
                    isFormalDataBase ?
                        //正式库
                        ConfigurationManager.AppSettings["WeChatFormalDataBase"].ToString().Decrypt() :
                        //测试库
                        ConfigurationManager.AppSettings["WeChatTestDataBase"].ToString().Decrypt()
                );



        /// <summary>
        /// 写入数据库
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="isAndroid">是否为安卓库</param>
        /// <param name="isFormalDataBase">是否为正式库</param>
        /// <returns>写入成功返回写入了多少行数据，失败则返回0</returns>
        public static int Exec(string sql, bool isAndroid, bool isFormalDataBase)
        {
            using (IDbConnection connection = new SqlConnection(GetServer(isAndroid, isFormalDataBase)))
            {
                return connection.Execute(sql);
            }
        }

        /// <summary>
        /// 获取本地化的文本内容
        /// </summary>
        /// <param name="Language">语言ID</param>
        /// <param name="key">文本key值</param>
        /// <returns></returns>
        public static string GetLocalText(string Language, string key)
        {
            string sql = string.Format("SELECT  FValue FROM dbo.Sys_LanguagePack where FLanguageID='{0}' and FKey='{1}'", Language, key);
            using (IDbConnection connection = new SqlConnection(ConfigurationManager.AppSettings["LocaDataBase"].ToString().Decrypt()))
            {
                DataTable table = new DataTable();
                var reader = connection.ExecuteReader(sql);
                table.Load(reader);
                reader.Close();
                return (table != null && table.Rows.Count > 0) ? table.Rows[0]["FValue"].ToMString() : "";
            }
        }
    }
}