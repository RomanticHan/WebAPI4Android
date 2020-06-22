using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI4Android.Models
{
    public class WorkCardCombinedSize
    {
        public long FInterID { get; set; }//FInterID
        public string State { get; set; }//已派工,已审核,未发卡,未关闭
        public string BillType { get; set; }//是否正单
        public string PlanBill { get; set; }//计划跟踪号
        public string FGroup { get; set; }//组别
        public string OrderBill { get; set; }//派工单号
        public string OrderDate { get; set; }//派工日期
        public string PlanStartTime { get; set; }//计划开工日期
        public string PlanEndTime { get; set; }//计划完工日期
        public string PlantBody { get; set; }//
        public string MaterialCode { get; set; }//物料代码
        public string MaterialName { get; set; }//物料名称
        public string Unit { get; set; }//单位
        public int BillerID { get; set; }//制单人ID
        public string FBiller { get; set; }//制单人名称
        public string FChecker { get; set; }//审核人名称
        public string FBillDate { get; set; }//制单日期
        public string FCheckDate { get; set; }//审核日期
        public int FEntryId { get; set; }//分录FEntryID
        public int FProcessFlowID { get; set; }//制程ID
        public int FCanReportByNoStockIn { get; set; }//允许未扫描入库先报工
        //[DataMember]
        //public decimal dispatchingnumber { get; set; }//分录上的派工数量
        public string RouteBillNumber { get; set; }//工艺路线
        public decimal ReportedNumber { get; set; }//以汇报数
        public decimal ReportedUnscheduled { get; set; }//已汇报未计工数
        public decimal ReportedUnentered { get; set; }//已汇报未入库数
        public decimal StockInQty { get; set; }//已入库数
        public List<WorkCardSize> Size { get; set; }//尺码数据
    }

    public class WorkCardSize
    {
        public string Size { get; set; }//尺码
        public decimal Num { get; set; }//派工数量
    }
}