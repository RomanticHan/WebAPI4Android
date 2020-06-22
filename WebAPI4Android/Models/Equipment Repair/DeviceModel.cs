using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI4Android.Models
{
    public class DeviceModel
    {
        public int DeviceID { get; set; }   //设备ID
        public string DeviceNo { get; set; }    //设备编号
        public string DeviceName { get; set; }  //设备名称
        public string Model { get; set; }   //规格型号
        public int CustodianID { get; set; } //使用人ID（需要维修时正在使用的人 默认情况下为设备保管人）
        public string CustodianCode { get; set; } //使用人工号（需要维修时正在使用的人）
        public string CustodianName { get; set; }   //使用人名称
        public string CustodianDept { get; set; }   //使用人所在单位
        public string CustodianTel { get; set; }    //使用人联系电话
    }
}