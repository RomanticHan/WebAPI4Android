using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI4Android.Models;
using WebAPI4Android.Tools;

namespace WebAPI4Android.Dll
{
    public class ScWorkCard
    {
        public static string GetWorkCardCombinedSizeListSql(string Filter)
        {
            try
            {
                string WorkCardCombinedSizeSql = @"SELECT  t.FInterID ,
                    State = ( case when t0.Fstate =0 then '未派工' else '已派工' end+''+
                              case when t0.FCheckerID<>0 then ',已审核'  else ',未审核' end+''+
                              case when t0.FCloserID<>0 then ',已关闭'  else ',未关闭' end+''+ 
                              CASE WHEN t0.FReporterID<>0 THEN ',已汇报' ELSE ',未汇报' END+
                              CASE WHEN t0.FWorkRecorderID<>0 THEN ',已记工' ELSE ',未记工' END+
                              CASE WHEN t0.FFinsher<>0 THEN ',已结案' ELSE ',未结案' END  ) ,
                    BillType = ( CASE WHEN t.Fbillstyle = 0 THEN '正单'
                                  WHEN t.Fbillstyle = 1 THEN '差补'
                             END ) ,
                    PlanBill = ( t.FMtoNo ) ,
                    OrderBill = ( t0.FNumber ) ,
                    OrderDate = ( t0.Fdate ) ,
                    FGroup = ( t5.FName ) ,
                    PlanStartTime = ( t.FPlanBeginDate ) ,
                    PlanEndTime = ( t.FPlanEndDate ) ,
                    PlantBody = ( t2.FName ) ,
                    MaterialCode = ( t6.FNumber ) ,
                    MaterialName = ( t6.FName ) ,
                    Unit = ( t7.FName ) ,
                    t0.FBillerID,
                    FBiller = ( t11.Fname ) ,
                    FChecker = ( t12.Fname ) ,
                    FBillDate = ( t0.FBillDate ) ,
                    FCheckDate = ( t0.FCheckDate ) ,
                    FEntryId = ( t.FEntryID ) ,
                    FProcessFlowID = (t0.FProcessFlowID ),
                    --t0.FCanReportByNoStockIn,--允许未扫描入库先报工
		            Convert(int,t0.FCanReportByNoStockIn) as FCanReportByNoStockIn,--允许未扫描入库先报工
				   WorkNumberTotal = (t.FRequireQty) ,--派工数量合计
                   b7.FBillNo as  RouteBillNumber,
                   --CONVERT(decimal(18,3),ISNULL(t.FQtyPass,0)) as ReportedNumber ,-- 已汇报数量
				   --ReportedUnscheduled = CONVERT(decimal(18,3),(ISNULL(t.FQtyPass,0) - ISNULL(t.FQtyProcessPass,0) )) ,--已汇报未计工数
				   --ReportedUnentered = CONVERT(decimal(18,3),(ISNULL(t.FQtyPass,0) - ISNULL(t.FStockInQty,0) )),  --已汇报未入库数
                   CONVERT(decimal(18,3),dbo.fun_FlatProductionWorkOrder(t.FInterID,t.FEntryID,1)) as ReportedNumber ,-- 已汇报数量
				   ReportedUnscheduled = CONVERT(decimal(18,3),dbo.fun_FlatProductionWorkOrder(t.FInterID,t.FEntryID,2)) ,--已汇报未计工数
				   ReportedUnentered = CONVERT(decimal(18,3),dbo.fun_FlatProductionWorkOrder(t.FInterID,t.FEntryID,3)),  --已汇报未入库数
                   CONVERT(decimal(18,3),ISNULL(t.FStockInQty,0)) as StockInQty,--已入库数
				   dbo.getWorkCardCombinedSize(t.FInterID,t.FEntryID) as Size
            FROM    Sc_WorkCard t0
                    LEFT OUTER JOIN Sc_WorkCardEntry t ON t.FInterID = t0.FInterID
                    LEFT OUTER JOIN T_PRD_MO tpm ON tpm.FInterID = t.FSourceInterId 
		            LEFT JOIN T_PRD_MOENTRY tpme ON tpme.FInterID = t.FSourceInterId AND tpme.FEntryID = t.FSourceEntryID
                   -- LEFT OUTER JOIN Sc_WorkCardSize sw ON sw.FInterID = t.FInterID AND sw.FEntryID = t.FEntryID
                    LEFT OUTER JOIN Sc_MO t1 ON t.FSrcICMOInterID = t1.FInterID
                    LEFT OUTER JOIN t_ICItem t2 ON t.FProductID = t2.FItemID
                    LEFT OUTER JOIN t_ProcessFlow t3 ON t.FProcessFlowID = t3.FItemID
                    LEFT OUTER JOIN t_SizeFmt t4 ON t0.FPmFmtID = t4.FitemID
                    LEFT OUTER JOIN t_SizeFmt tt4 ON t.FPmFmtID = tt4.FitemID
                    LEFT OUTER JOIN t_Department t5 ON t0.FDeptID = t5.FItemID
                    LEFT OUTER JOIN t_ICItem t6 ON t.FItemID = t6.FItemID
                    LEFT OUTER JOIN t_MeasureUnit t7 ON t6.FUnitID = t7.FitemID
                    LEFT OUTER JOIN s_user t11 ON t0.FBillerID = t11.FUserID
                    LEFT OUTER JOIN s_user t12 ON t0.FCheckerID = t12.FUserID
                    LEFT OUTER JOIN s_user t13 ON t0.FCloserID = t13.FUserID
                    LEFT OUTER JOIN s_GR_FrameWork s1 ON s1.FitemID = t0.FOrganizeID
	
	            LEFT JOIN T_Sfc_OperPlanning t14 ON t14.FMOID =tpm.FInterID
                LEFT OUTER JOIN  T_Prd_RouteSubtotal b7 on  t.FRouteInterFID=b7.FID and b7.FProcessFlowID=t.FProcessFlowID -- and (b7.FIsGroupWork=0 or b7.FIsGroupWork is null ) 
                WHERE  t0.FCheckerID<>0 and {0} 
                ORDER BY t0.Fdate DESC,
                        t0.FNumber DESC,
                        t.FInterID ASC,
                        t.FEntryID ASC";
                return string.Format(WorkCardCombinedSizeSql, Filter);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}