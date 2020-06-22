using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI4Android.Models
{
    public class RepairOrderModel
    {
        public int FInterID { get; set; }   //单据ID
        public string FNumber { get; set; } //单据编号
        public int DeviceID { get; set; }   //设备ID
        public string DeviceNo { get; set; }    //设备编号
        public string DeviceName { get; set; }  //设备名称
        public string Model { get; set; }   //规格型号
        public int CustodianID { get; set; } //使用人ID（需要维修时正在使用的人）
        public string CustodianCode { get; set; } //使用人工号（需要维修时正在使用的人）
        public string Custodian { get; set; }   //保管人
        public string CustodianDept { get; set; }   //保管单位
        public string CustodianTel { get; set; }    //保管人联系电话
        public string MaintenanceUnit { get; set; } //维修单位
        public string RepairParts { get; set; } //检修部位
        public DateTime InspectionTime { get; set; }  //报修时间
        public List<RepairOrderEntryModel> RepairEntryData { get; set; }    //分录数据
        public string FaultDescription { get; set; }    //故障说明
        public DateTime RepairTime { get; set; }  //修复时间
        public string IssueCause { get; set; }  //故障原因判断
        public string AssessmentPrevention { get; set; }    //维修后评估及预防
    }
}