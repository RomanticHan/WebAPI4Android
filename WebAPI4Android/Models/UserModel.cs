using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI4Android.Models
{
    public class UserModel
    {
        public string Token { get; set; }           //登录Token
        public int DepartmentID { get; set; }       //部门ID
        public int DutyID { get; set; }             //职务ID
        public int EmpID { get; set; }              //员工ID
        public string Number { get; set; }          //员工工号
        public int OrganizeID { get; set; }         //组织ID
        public string Name { get; set; }            //姓名
        public string Sex { get; set; }             //性别
        public int UserID { get; set; }             //用户ID
        public string DepartmentName { get; set; }  //部门名称
        public string Position { get; set; }        //职称
        public string Factory { get; set; }         //厂区
        public int DefaultStockID { get; set; }     //默认仓库ID
        public int DiningRoomID { get; set; }       //默认就餐食堂ID
        public string PicUrl { get; set; }          //照片路径
        public string PassWord { get; set; }        //密码
        public int QuickLoginType { get; set; }     //快速登录类型
        public int IsAppAutoLock { get; set; }      //APP自动上锁
        public string SAPRole { get; set; }         //SAP角色
        public string SAPLineNumber { get; set; }   //SAP线别编号
        public string SAPFactory { get; set; }      //SAP厂区
        public List<Jurisdiction> JurisdictionList { get; set; }    //用户权限列表
    
    }
    public class Jurisdiction
    {
        public string JID { get; set; }
    }
}