using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI4Android.Models
{
    public class SubmitRecordModel
    {
        
        public int DeviceID { get; set; }    //更换配件ID（对应t_FACard表FInterID）
        
        public string MaintenanceUnit { get; set; }    //维修单位

        public string RepairParts { get; set; }    //检修部位

        public string InspectionTime { get; set; }    //报修时间

        public List<RepairOrderEntryModel> RepairEntryData { get; set; }    //分录数据

        public string FaultDescription { get; set; }    //故障说明

        public string RepairTime { get; set; }    //修复时间

        public string IssueCause { get; set; }    //故障原因判断

        public string AssessmentPrevention { get; set; }    //维修后评估及预防

        public int CustodianID { get; set; } //使用人（需要维修时正在使用的人）
    }
}