using Ge.DynamicPublic;
using Newtonsoft.Json.Linq;
using SpApiV1;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebAPI4Android.Models;

namespace WebAPI4Android.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : BaseAndroidController
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="Phone">手机号码</param>
        /// <param name="Password">密码</param>
        /// <param name="isFormalDataBase">是否进正式库</param>
        /// <returns>登录成功返回用户实体类，登录失败则返回失败说明</returns>
        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login(string Phone, string Password, string VCode, bool isFormalDataBase)
        {
            string user = @"SELECT 
                        t0.FDeptmentID AS DepartmentID,--部门ID
                        t0.FDutyID AS DutyID,--职务ID
                        t0.FitemID AS EmpID,--员工ID
                        t0.FNumber AS Number,--员工工号
                        t0.FOrganizeID AS OrganizeID,--组织ID
                        t0.Fname AS Name,--姓名
                        CASE WHEN t0.FsexID = 42 THEN '男' ELSE '女'END AS Sex,--性别
                        CASE WHEN t.FUserID IS NULL THEN 0 ELSE t.FUserID END  AS UserID ,--用户ID
                        t2.FName AS DepartmentName,--部门名称
                        t3.FName AS Position,--职称
                        t1.FFactoryArea AS Factory,--厂区
                        CASE WHEN t.FDefault_StockID IS NULL THEN 0 ELSE t.FDefault_StockID END  AS DefaultStockID,--默认仓库ID
                        t.FDefault_DiningRoomID as DiningRoomID,--默认就餐食堂ID
                        'http://220.189.245.171:8083/'+t4.FFullName AS PicUrl,--照片路径
						t.FPassWord AS PassWord,--密码
						t.FPasswordPolicyID AS QuickLoginType,--快速登录类型
                        CASE WHEN t.FIsAppAutoLock IS NULL THEN 0 ELSE t.FIsAppAutoLock END AS IsAppAutoLock, --APP自动上锁

                        CASE WHEN t5.FSAPRoleNo IS NULL THEN '' ELSE  t5.FSAPRoleNo END AS SAPRole, --SAP角色
						t6.FSAPNumber AS SAPLineNumber,--SAP线别编号
						tt.FSAPNumber AS SAPFactory --SAP厂区

                        FROM dbo.t_emp t0 
                        INNER JOIN dbo.s_GR_FrameWork t1 ON t1.FitemID= t0.FOrganizeID 
                        LEFT JOIN s_GR_FrameWork_SAPOrg2MES tt ON t1.FItemID=tt.FItemID
                        LEFT JOIN s_user t ON t.FEmpID = t0.FitemID AND t0.FleaveDate IS NULL
                        LEFT JOIN t_item  t2 ON t0.FDeptmentID = t2.FItemID AND t2.FItemClassID = 2
                        LEFT JOIN t_EmpSubMessage t3 ON t0.FDutyID = t3.FItemID
                        LEFT JOIN dbo.hq_From_AttachFiles t4 ON t4.FInterID= t0.FItemID AND t4.FClassID= 130 AND t4.FType=0
                        LEFT JOIN t_EmpSubMessage_SAP2MES t5 ON t0.FDutyID=t5.FMESRoleID
                        LEFT JOIN t_Department t6 ON t0.FDeptmentID=t6.FItemID
                        WHERE t0.FPhone = '" + Phone + "' AND t0.FleaveDate IS NULL ";



            //用户名为空
            if (Phone == null || Phone.Length == 0) return Ok(new ReturnMessage(ReturnMessType.Error, GetLocalText("Login_UserName_Empty")));

            //密码为空
            if (Password == null || Password.Length == 0) return Ok(new ReturnMessage(ReturnMessType.Error, GetLocalText("Login_Password_Empty")));

            if (VCode == null || VCode.Length == 0) return Ok(new ReturnMessage(ReturnMessType.Error, GetLocalText("CheckVCode_Empty")));

            //获取用户数据实体类
            UserModel um = Query<UserModel>(user, isFormalDataBase);

            //用户不存在
            if (um == null) return Ok(new ReturnMessage(ReturnMessType.Error, GetLocalText("Login_Account_NotExist")));

            //如果验证码为管理员验证码则不需要检查验证码
            if (VCode != "643179")
            {  //检查验证码并获取返回值
                int vcodeResult = CheckVerificationCode(um.UserID, VCode, isFormalDataBase);
                //验证码返回值不为0则说明验证失败
                if (vcodeResult != 0) return Ok(new ReturnMessage(ReturnMessType.Error, GetVCodeText(vcodeResult)));
            }


            //密码错误
            if (!um.PassWord.Equals(Password.ToMD5())) return Ok(new ReturnMessage(ReturnMessType.Error, GetLocalText("Login_Password_Error")));


            var controls = string.Format(@"if(exists(SELECT top 1  t1.fstatus  from s_controluser t0 
                                    inner join s_user t1 on t0.FUserID = t1.FUserID
                                    inner join t_emp t2 on t2.FitemID = t1.FEmpID where t1.FEmpID = {0}  and t2.FleaveDate is null and  t1.fstatus=1))
                                       select convert(varchar(30),FItemID) as JID from  jdGroup_Public.dbo.s_control  WHERE  FIsAppControl=1
                                    else
                                    begin 
                                        select convert(varchar(30),t0.FcontrolID) as JID from s_controluser t0 
                                        inner join s_user t1 on t0.FUserID=t1.FUserID 
                                        inner join jdGroup_Public.dbo.s_control t3 on t0.FcontrolID=t3.FItemID AND t3.FIsAppControl=1
                                        inner join t_emp t2 on t2.FitemID=t1.FEmpID where t1.FEmpID={0}  and t2.FleaveDate is null 
                                    END", um.EmpID);
            //用户权限
            um.JurisdictionList = QueryList<Jurisdiction>(controls, isFormalDataBase);

            //生成并保存token
            um.Token = CreatAndSaveToken(um.UserID, isFormalDataBase);

            //登录成功返回用户实体类
            return Ok(new ReturnMessage(ReturnMessType.Success, um));
        }

        [HttpGet]
        [Route("GetDataBaseList")]
        public IHttpActionResult GetDataBaseList()
        {
            //检查并获取token实体
            TokenModel tm = CheckAndGetToken();

            //检查结果不为成功则直接返回检查结果状态码
            if (tm.CheckResult != CheckToken_Success) return Ok(new ReturnMessage(ReturnMessType.Error, GetTokenCodeText(tm.CheckResult)));

            string sql = "SELECT FAccountSuitID ,Code=FAccountSuitCode+'|'  +  FAccountSuitName,FAccountSuitName FROM master.dbo.sys_account_suit  ORDER BY FAccountSuitCode";

            //查询数据库表单
            List<AccountSuiCodeModel> ascm = QueryList<AccountSuiCodeModel>(sql, tm.isFormalDataBase);

            //表单长度为空则说明无数据
            return (ascm == null || ascm.Count() == 0) ? Ok(new ReturnMessage(ReturnMessType.Error, GetLocalText("Query_Result_NotExist"))) : Ok(new ReturnMessage(ReturnMessType.Success, ascm));
        }


        [HttpGet]
        [Route("GetOrganizationList")]
        public IHttpActionResult GetOrganizationList()
        {
            TokenModel tm = CheckAndGetToken();

            if (tm.CheckResult != CheckToken_Success) return Ok(new ReturnMessage(ReturnMessType.Error, GetTokenCodeText(tm.CheckResult)));

            string sql = "SELECT FItemID ,Code=Fnumber+'|'  +  Fname,FName,FNumber,FAdminOrganizeID FROM s_GR_FrameWork where FDetail=1 order by Fnumber";

            List<OrganizationModel> om = QueryList<OrganizationModel>(sql, tm.isFormalDataBase);

            return (om == null || om.Count() == 0) ? Ok(new ReturnMessage(ReturnMessType.Error, GetLocalText("Query_Result_NotExist"))) : Ok(new ReturnMessage(ReturnMessType.Success, om));
        }


        [HttpGet]
        [Route("SendVerificationCode")]
        public IHttpActionResult SendVerificationCode(string phone, bool isFormalDataBase)
        {
            //6位随机码
            string code = CreatCode();
            //短信内容
            string text = "您的验证码是：" + code + "，请尽快验证。如非本人操作请忽略，回复TD拒收。";
            //发送结果
            RstArray Rst = SendMSG(phone, text);
            string returnCode = JObject.Parse(Rst.Msg)["returnCode"].ToString();
            string returnMsg = JObject.Parse(Rst.Msg)["returnMsg"].ToString();
            //Msg={"taskId":"20042716244413580995","returnCode":"200","returnMsg":"成功","productId":"1"}
            if (returnCode == "200")
            {
                //保存验证码
                SaveVerificationCode(phone, code, isFormalDataBase);
                return Ok(new ReturnMessage(ReturnMessType.Success, code));
            }
            else
            {
                return Ok(new ReturnMessage(ReturnMessType.Error, returnMsg));
            }

        }

        [HttpGet]
        [Route("CheckToken")]
        public IHttpActionResult CheckToken()
        {
            TokenModel tm = CheckAndGetToken();
            if (tm.CheckResult == CheckToken_Success)
                return Ok(new ReturnMessage(ReturnMessType.Success, GetTokenCodeText(tm.CheckResult)));
            else
                return Ok(new ReturnMessage(ReturnMessType.Error, GetTokenCodeText(tm.CheckResult)));
        }

        [HttpPost]
        [Route("UpDataUserPicture")]
        public IHttpActionResult UpDataUserPicture(string EmpID, string ImageBase64, string UserID)
        {
            string result = SetPicture(EmpID, ImageBase64, UserID);

            return Ok(new ReturnMessage(ReturnMessType.Success, result));
        }
    }
}