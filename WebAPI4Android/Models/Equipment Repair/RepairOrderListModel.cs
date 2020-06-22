using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI4Android.Models
{
    public class RepairOrderListModel
    {
        public int FInterID { get; set; }   //单据ID
        public string FNumber { get; set; } //单据编号
        public string DeviceNo { get; set; }    //设备编号
        public string DeviceName { get; set; }  //设备名称
        public string Custodian { get; set; }   //保管人
        public string RepairUnit { get; set; }  //维修单位
        public string Biller { get; set; }  //制单人
        public string BillDate { get; set; }    //制单时间
    }
}