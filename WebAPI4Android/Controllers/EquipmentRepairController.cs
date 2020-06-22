using Ge;
using Ge.DynamicPublic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI4Android.Models;

namespace WebAPI4Android.Controllers
{
    [RoutePrefix("api/EquipmentRepair")]
    public class EquipmentRepairController : BaseAndroidController
    {
        [HttpGet]
        [Route("GetDeviceInformation")]
        public IHttpActionResult GetDeviceInformation(bool isInterID, string Device)
        {
            try
            {
                var sqlWhere = string.Empty;
                if (isInterID)
                    sqlWhere = string.Format("where t0.FinterID = @Device");
                else
                    sqlWhere = string.Format("where t0.FNumber = @Device");
                var sql = string.Format(@"select top 1
                                t0.FinterID as DeviceID,
                                t0.FNumber as DeviceNo,
                                t0.FName as DeviceName,
                                t0.FModel as Model,
								t0.FKeepEmpID as CustodianID,
                                t1.Fnumber as CustodianCode,
                                t1.Fname as CustodianName,
                                t2.Fname as CustodianDept,
                                t1.FPhone as CustodianTel
                                from t_FACard t0 
                                left outer join t_emp t1 on t1.FleaveDate is null And t0.FKeepEmpID=t1.FitemID
                                left outer join t_Department t2 on t1.FDeptmentID=t2.FItemID 
                                {0}", sqlWhere);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Device", Device);
                DeviceModel deviceModel = Query<DeviceModel>(sql, dic);
                if (deviceModel == null) return Ok(new ReturnMessage(ReturnMessType.Error, GetLocalText("Query_Result_NotExist")));
                return Ok(new ReturnMessage(ReturnMessType.Success, new Dictionary<string, object>() {
                        { "DeviceMessage", deviceModel },
                        { "RepairOrder", null }
                    }));
            }
            catch
            {
                //return Ok(new ReturnMessage(ReturnMessType.Error, e.Message));
                return Ok(new ReturnMessage(ReturnMessType.Error, GetLocalText("DataBase_Error")));
            }
        }

        [HttpGet]
        [Route("GetRepairOrderList")]
        public IHttpActionResult GetRepairOrderList(string StartDate, string EndDate)
        {
            try
            {
                var sql = string.Format(@"select
                                t.FInterID as FInterID,
                                t.FNumber as FNumber,
                                t0.FNumber as DeviceNo,
                                t0.FName as DeviceName,
                                t1.Fnumber+'('+t1.Fname+')' as Custodian,
                                t.FRepairUnit as RepairUnit,
                                t2.Fname as Biller,
                                t.FBillDate as BillDate
                                from t_EquipmentRepair t 
                                inner join t_FACard t0 on t.FACardID=t0.FinterID
                                left outer join t_emp t1 on t1.FleaveDate is null And t.FUserID=t1.FitemID
                                left outer join s_user t2 on t.FBillerID=t2.FUserID
                                where CONVERT(varchar(30),t.FBillDate,23) between '{0}' and '{1}'", StartDate, EndDate);
                List<RepairOrderListModel> repairOrderListModels = QueryList<RepairOrderListModel>(sql);
                if (repairOrderListModels != null && repairOrderListModels.Count > 0)
                {
                    return Ok(new ReturnMessage(ReturnMessType.Success, repairOrderListModels));
                }
                else
                    return Ok(new ReturnMessage(ReturnMessType.Error, GetLocalText("Query_Result_NotExist")));
            }
            catch
            {
                return Ok(new ReturnMessage(ReturnMessType.Error, GetLocalText("DataBase_Error")));
            }
        }
    }
}
