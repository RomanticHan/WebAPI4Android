using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI4Android.Models
{
    public class RepairOrderEntryModel
    {
        public string AccessoriesName { get; set; }    //更换配件名称
        public string Manufacturer { get; set; }    //厂商/品牌
        public string Specification { get; set; }   //规格
        public double Quantity { get; set; }    //数量
        public string Unit { get; set; }    //单位
        public double Budget { get; set; }  //预算
        public string Remarks { get; set; } //备注
    }
}