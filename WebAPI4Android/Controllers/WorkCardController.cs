using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using WebAPI4Android.Models;
using System.Web.Http;
using WebAPI4Android.Dll;
using Ge.DynamicPublic;
using Newtonsoft.Json;

namespace WebAPI4Android.Controllers
{
    [RoutePrefix("api/WorkCard")]
    public class WorkCardController : BaseAndroidController
    {
        [HttpGet]
        [Route("GetWorkCardCombinedSizeList")]
        public IHttpActionResult GetWorkCardCombinedSizeList(string StartDate, string EndDate, long FDeptID, bool IsClose)
        {
            string Filter;
            if (IsClose)
                Filter = string.Format("  t0.FCloserID<>0 and (t0.Fdate between  '{0} 00:00:00' and '{1} 23:59:59' or t0.FDayWorkDate between  '{0} 00:00:00' and '{1} 23:59:59' ) {2}", StartDate, EndDate, FDeptID.ToString().IsNotEmpty() ? " and FDeptID= " + FDeptID : "");
            else//找未关闭的
                Filter = string.Format("  (t0.FCloserID=0 or t0.FCloserID is null) and  (t0.Fdate between  '{0} 00:00:00' and '{1} 23:59:59' or t0.FDayWorkDate between  '{0} 00:00:00' and '{1} 23:59:59' ) {2}", StartDate, EndDate, FDeptID.ToString().IsNotEmpty() ? " and FDeptID= " + FDeptID : "");
            var sqlstr = ScWorkCard.GetWorkCardCombinedSizeListSql(Filter);
            List<object> result = QueryList<object>(sqlstr);
            List<WorkCardCombinedSize> list = JsonConvert.DeserializeObject<List<WorkCardCombinedSize>>(JsonConvert.SerializeObject(result).Replace("\"[", "[").Replace("]\"", "]").Replace("\\\"", "\""));
            return list.Count > 0 ? Ok(new ReturnMessage(ReturnMessType.Success, list)) : Ok(new ReturnMessage(ReturnMessType.Error, GetLocalText("Query_Result_NotExist")));
        }
    }
}
