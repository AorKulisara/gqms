//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.OleDb;
using System.Data;

public partial class DAL
{


    #region OGC


    //-- edit 12/07/2018 --
    public DataTable SearchGqmsDailyUpdateALL(String SiteID, String FID, String FromDate = "", String ToDate = "", String orderSQL = "", String OtherCriteria = "", String NgBillRptNo = "")
    {
        String sql = ""; String criteria = "";
        String criteria2 = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "S.SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "D.FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "D.RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            AddCriteriaRange(ref criteria2, "ADATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            //--- 31/07/2018 มีการเพิ่ม function ใน ORACLE -> is_number(), to_number3(), to_number4()
            //-- 23/08/2018 เพิ่ม ALERT_FLAG
            // ไม่ได้ให้เลือก flow จึงตัดบรรทัดนี้ออก" ,CASE WHEN D.FLOW_NAME=S.FLOW_NAME2 THEN F2.VALUE ELSE F1.VALUE END FLOW " +
            sql = " SELECT DY.ADATE, A.* " +
                " FROM O_DIM_DATE DY LEFT OUTER JOIN " +
                " ( " +
                "   SELECT D.*, S.SITE_ID, S.ISO_FLAG, S.H2S_FLAG, S.TOTAL_RUN, S.TOLERANCE_RUN " +
                " ,CASE WHEN is_number(D.C1)=1  AND is_number(D.C2)=1 AND is_number(D.C3)=1 AND is_number(D.IC4)=1 AND is_number(D.NC4)=1  " +
                "   AND is_number(D.IC5)=1 AND is_number(D.NC5)=1 AND is_number(D.C6)=1 AND is_number(D.CO2)=1 AND is_number(D.N2)=1  " +
                "   AND ( S.H2S_FLAG='N' OR (is_number(D.H2S)=1 ) )  " +
                " THEN to_number3(D.C1)+to_number3(D.C2)+to_number3(D.C3) +to_number3(D.IC4)+to_number3(D.NC4) " +
                "  +to_number3(D.IC5) +to_number3(D.NC5)+to_number3(D.C6)+to_number3(D.CO2) +to_number3(D.N2)  " +
                "  +(CASE WHEN S.H2S_FLAG='Y' THEN to_number3(D.H2S) ELSE 0 END)   " +
                " ELSE NULL END AS SUM_COMPO   " +
                " ,S.OMA_NAME1, M1.VALUE AS WC1 ,M1.ALERT_FLAG AS WC1_ALERT, S.OMA_NAME2, M2.VALUE AS WC2,M2.ALERT_FLAG AS WC2_ALERT " +
                " , CASE WHEN D.OMA_NAME=S.OMA_NAME2 THEN M2.ALERT_FLAG ELSE M1.ALERT_FLAG END WC_ALERT " +
                " ,S.FLOW_NAME1, F1.VALUE AS FLOW1,F1.ALERT_FLAG AS FLOW1_ALERT, S.FLOW_NAME2, F2.VALUE AS FLOW2,F2.ALERT_FLAG AS FLOW2_ALERT " +
                " ,F1.VALUE FLOW " +
                " ,B.BTUDATE, B.BTU, B.RUN " +
                " ,S.OGC_NAME1, S.OGC_NAME2, S.OGC_NAME3 ";

            //NgBillRptNo -- กรณีพิจารณาส่งข้อมูลไปยัง NGBILL_DAILY_UPDATE
            if ( NgBillRptNo== "")
                sql += " , null AS NGDATE, null AS NGMODIFIED_DATE ";
            else
                sql += " , NG.RDATE AS NGDATE, NG.MODIFIED_DATE AS NGMODIFIED_DATE  ";

            sql += " FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID " +
                "   LEFT OUTER JOIN CHONBURI_VW_ARCH_DAY_MOISTURE M1 ON S.OMA_NAME1 = M1.NAME AND D.RDATE = (M1.RDATE-1) " +
                "   LEFT OUTER JOIN CHONBURI_VW_ARCH_DAY_MOISTURE M2 ON S.OMA_NAME2 = M2.NAME AND D.RDATE = (M2.RDATE-1) " +
                "   LEFT OUTER JOIN CHONBURI_VW_ARCH_DAY_FLOWRATE F1 ON S.FLOW_NAME1 = F1.NAME AND D.RDATE = (F1.RDATE-1) " +
                "   LEFT OUTER JOIN CHONBURI_VW_ARCH_DAY_FLOWRATE F2 ON S.FLOW_NAME2 = F2.NAME AND D.RDATE = (F2.RDATE-1) " +
                "   LEFT OUTER JOIN O_OGC_DAILY_BTU B ON D.FID=B.FID AND D.RDATE= (B.BTUDATE-1) ";

            if (NgBillRptNo != "")
                sql += " LEFT OUTER JOIN NGBILL_DAILY_UPDATE NG ON D.RDATE=NG.RDATE AND NG.FID='" + NgBillRptNo +"' ";


            if (criteria != "") { sql += " WHERE " + criteria; }

            sql += ") A  ON DY.ADATE=A.RDATE  ";
            if (criteria2 != "") { sql += " WHERE " + criteria2; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY DY.ADATE ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

 
    //-- edit 19/07/2018 --
    public DataTable SearchGqmsNgbillDailyUpdate(String FID, String NgBillRptNo, String FromDate = "", String ToDate = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "D.FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "D.RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            //--- 31/07/2018 มีการเพิ่ม function ใน ORACLE -> is_number(), to_number3(), to_number4()
            sql = " SELECT D.*  " +
                " ,CASE WHEN is_number(D.C1)=1 AND is_number(D.C2)=1 AND is_number(D.C3)=1 AND is_number(D.IC4)=1 AND is_number(D.NC4)=1 " +
                "   AND is_number(D.IC5)=1 AND is_number(D.NC5)=1 AND is_number(D.C6)=1 AND is_number(D.CO2)=1 AND is_number(D.N2)=1  " +
                "   AND ( S.H2S_FLAG='N' OR (is_number(D.H2S)=1 AND NVL(D.H2S,0)<>'-')   )  " +
                " THEN to_number3(D.C1)+to_number3(D.C2)+to_number3(D.C3) +to_number3(D.IC4)+to_number3(D.NC4) " +
                "  +to_number3(D.IC5) +to_number3(D.NC5)+to_number3(D.C6)+to_number3(D.CO2)+to_number3(D.N2)  " +
                "  +(CASE WHEN S.H2S_FLAG='Y' THEN to_number3(D.H2S) ELSE 0 END)  " +
                " ELSE NULL END AS SUM_COMPO   " +
                " ,NG.ID AS NGID, NG.RDATE AS NGDATE, NG.MODIFIED_DATE AS NGMODIFIED_DATE " +
                " FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID " +
                "  LEFT OUTER JOIN NGBILL_DAILY_UPDATE NG ON D.RDATE=NG.RDATE AND NG.FID='" + NgBillRptNo + "' ";
            
            if (criteria != "") { sql += " WHERE " + criteria; }
 
            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY D.RDATE ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 13/07/2018 --
    public DataTable SearchGqmsDailyUpdateAVG(String SiteID, String FID, String FromDate = "", String ToDate = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "D.FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            //--- 31/07/2018 มีการเพิ่ม function ใน ORACLE -> is_number(), to_number3(), to_number4(), to_numbernull3(), to_numbernull4()

            sql = "SELECT S.SITE_ID, D.FID, " +
                "   ROUND(AVG(to_numbernull3(C1)),3) C1,  ROUND(AVG(to_numbernull3(C2)),3) C2,   ROUND(AVG(to_numbernull3(C3)),3) C3, " +
                "   ROUND(AVG(to_numbernull3(IC4)),3) IC4,  ROUND(AVG(to_numbernull3(NC4)),3) NC4,  ROUND(AVG(to_numbernull3(IC5)),3) IC5, " +
                "   ROUND(AVG(to_numbernull3(NC5)),3) NC5,  ROUND(AVG(to_numbernull3(C6)),3) C6,    ROUND(AVG(to_numbernull3(N2)),3) N2, " +
                "   ROUND(AVG(to_numbernull3(CO2)),3) CO2,  ROUND(AVG(to_numbernull4(SG)),4) SG,   ROUND(AVG(to_numbernull3(GHV)),3) GHV, " +
                "   ROUND(AVG(to_numbernull3(NHV)),3) NHV,  ROUND(AVG(to_numbernull3(WC)),2) WC, ROUND(AVG(to_numbernull3(UNNORMALIZED)),3) UNNORMALIZED, " +
                "   ROUND(AVG(to_numbernull3(UNNORMMIN)),3) UNNORMMIN,  ROUND(AVG(to_numbernull3(UNNORMMAX)),3) UNNORMMAX,  ROUND(AVG(to_numbernull3(WB)),3) WB, " +
                "   ROUND(AVG(to_numbernull3(H2S)),3) H2S, " +
                " ROUND0(AVG(to_numbernull3(C1)),3)+ ROUND0(AVG(to_numbernull3(C2)),3)+ ROUND0(AVG(to_numbernull3(C3)),3)+ ROUND0(AVG(to_numbernull3(IC4)),3)+ ROUND0(AVG(to_numbernull3(NC4)),3) " +
                "+ ROUND0(AVG(to_numbernull3(IC5)),3)+ ROUND0(AVG(to_numbernull3(NC5)),3)+ ROUND0(AVG(to_numbernull3(C6)),3)+ ROUND0(AVG(to_numbernull3(CO2)),3) " +
                "+ ROUND0(AVG(to_numbernull3(N2)),3)+(CASE WHEN MAX(S.H2S_FLAG)='Y' THEN ROUND0(AVG(to_numbernull3(H2S)),3) ELSE 0 END )  AS SUM_COMPO  " +
                " FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            sql += " GROUP BY S.SITE_ID, D.FID ";


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 16/07/2018 --
    public DataTable SearchGqmsDailyUpdateMIN(String SiteID, String FID, String FromDate = "", String ToDate = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "D.FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            //--- 31/07/2018 มีการเพิ่ม function ใน ORACLE -> is_number(), to_number3(), to_number4(), to_numbernull3(), to_numbernull4()

            sql = "SELECT S.SITE_ID, D.FID, " +
                "   ROUND(MIN(to_numbernull3(C1)),3) C1,  ROUND(MIN(to_numbernull3(C2)),3) C2,   ROUND(MIN(to_numbernull3(C3)),3) C3, " +
                "   ROUND(MIN(to_numbernull3(IC4)),3) IC4,  ROUND(MIN(to_numbernull3(NC4)),3) NC4,  ROUND(MIN(to_numbernull3(IC5)),3) IC5, " +
                "   ROUND(MIN(to_numbernull3(NC5)),3) NC5,  ROUND(MIN(to_numbernull3(C6)),3) C6,    ROUND(MIN(to_numbernull3(N2)),3) N2, " +
                "   ROUND(MIN(to_numbernull3(CO2)),3) CO2,  ROUND(MIN(to_numbernull4(SG)),4) SG,   ROUND(MIN(to_numbernull3(GHV)),3) GHV, " +
                "   ROUND(MIN(to_numbernull3(NHV)),3) NHV,  ROUND(MIN(to_numbernull3(WC)),2) WC, ROUND(MIN(to_numbernull3(UNNORMALIZED)),3) UNNORMALIZED, " +
                "   ROUND(MIN(to_numbernull3(UNNORMMIN)),3) UNNORMMIN,  ROUND(MIN(to_numbernull3(UNNORMMAX)),3) UNNORMMAX,  ROUND(MIN(to_numbernull3(WB)),3) WB, " +
                "   ROUND(MIN(to_numbernull3(H2S)),3) H2S, " +
                "   NULL AS SUM_COMPO   " +
                " FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            sql += " GROUP BY S.SITE_ID, D.FID ";


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 16/07/2018 --
    public DataTable SearchGqmsDailyUpdateMAX(String SiteID, String FID, String FromDate = "", String ToDate = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "D.FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            sql = "SELECT S.SITE_ID, D.FID, " +
                "   ROUND(MAX(to_numbernull3(C1)),3) C1,  ROUND(MAX(to_numbernull3(C2)),3) C2,   ROUND(MAX(to_numbernull3(C3)),3) C3, " +
                "   ROUND(MAX(to_numbernull3(IC4)),3) IC4,  ROUND(MAX(to_numbernull3(NC4)),3) NC4,  ROUND(MAX(to_numbernull3(IC5)),3) IC5, " +
                "   ROUND(MAX(to_numbernull3(NC5)),3) NC5,  ROUND(MAX(to_numbernull3(C6)),3) C6,    ROUND(MAX(to_numbernull3(N2)),3) N2, " +
                "   ROUND(MAX(to_numbernull3(CO2)),3) CO2,  ROUND(MAX(to_numbernull4(SG)),4) SG,   ROUND(MAX(to_numbernull3(GHV)),3) GHV, " +
                "   ROUND(MAX(to_numbernull3(NHV)),3) NHV,  ROUND(MAX(to_numbernull3(WC)),2) WC, ROUND(MAX(to_numbernull3(UNNORMALIZED)),3) UNNORMALIZED, " +
                "   ROUND(MAX(to_numbernull3(UNNORMMIN)),3) UNNORMMIN,  ROUND(MAX(to_numbernull3(UNNORMMAX)),3) UNNORMMAX,  ROUND(MAX(to_numbernull3(WB)),3) WB, " +
                "   ROUND(MAX(to_numbernull3(H2S)),3) H2S, " +
                "   NULL AS SUM_COMPO   " +
                " FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            sql += " GROUP BY S.SITE_ID, D.FID ";


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 17/07/2018 --
    public String UpdateWCGqmsDailyUpdate(String SiteID, String FID, String MM = "", String YY = ""  )
    {
        String sql = ""; String criteria1 = ""; String criteria2 = "";
        String result = "";
        try
        {
            //criteria1 = " AND S.SITE_ID=999 AND S.FID='DDD' ";
            AddCriteria(ref criteria1, "S.SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria1, "S.FID", FID, DBUTIL.FieldTypes.ftText);


            //criteria2 = "  AND D.RDATE IN ( SELECT RDATE FROM GQMS_DAILY_UPDATE WHERE OMA_NAME IS NULL AND RDATE >=TO_DATE('01072018','DDMMYYYY') AND RDATE <TO_DATE('01082018','DDMMYYYY') ) ";
            AddCriteria(ref criteria2, "FID", FID, DBUTIL.FieldTypes.ftText);
            if ( MM != "" && YY != "")
            {
                string MM2 = "", YY2 = "";
                if ( Utility.ToInt(MM) == 12 )
                {
                    YY2 = (Utility.ToInt(YY) + 1).ToString();
                    MM2 = "1";
                }
                else
                {
                    YY2 = YY;
                    MM2 = (Utility.ToInt(MM) + 1).ToString();
                }

                if (criteria2 != "") criteria2 += " AND ";
                criteria2 += " RDATE >=TO_DATE('01"+ MM.PadLeft(2, '0') + YY + "','DDMMYYYY') AND RDATE <TO_DATE('01" + MM2.PadLeft(2, '0') + YY2 + "','DDMMYYYY') ";

                //-- EDIT 13/01/2020 --  ต้องดึงข้อมูลวันที่มากกว่า 1 วัน
                if (criteria1 != "") criteria1 += " AND ";
                criteria1 += " RDATE >=TO_DATE('01" + MM.PadLeft(2, '0') + YY + "','DDMMYYYY') AND RDATE <TO_DATE('03" + MM2.PadLeft(2, '0') + YY2 + "','DDMMYYYY') ";

            }

            if ( criteria2 != "")
                criteria2 = " D.RDATE IN ( SELECT RDATE FROM GQMS_DAILY_UPDATE WHERE OMA_NAME IS NULL AND " + criteria2 + "  ) ";
            //-- EDIT 13/01/2020 -- 
            if (FID != "") criteria2 += " AND D.FID='" + FID +"' ";

            if (criteria1 != "" && criteria2 != "")
            {
                sql = " MERGE INTO GQMS_DAILY_UPDATE D " +
                   " USING  ( " +
                   "  SELECT S.FID, S.OMA_NAME1, M1.VALUE, M1.RDATE " +
                   "  FROM O_SITE_FID S INNER JOIN CHONBURI_VW_ARCH_DAY_MOISTURE M1 ON S.OMA_NAME1 = M1.NAME  ";
                sql += " AND " + criteria1;
                sql += "  ) ta ON (ta.FID = D.FID AND ta.RDATE-1 = D.RDATE  ";   
                sql += " AND " + criteria2;
                sql += " ) " +
                    " WHEN MATCHED THEN UPDATE " +
                    "    SET D.OMA_NAME=ta.OMA_NAME1, D.WC=ta.VALUE ";
               
                ExecuteSQL(sql);
            }


            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    //___________________________________________________________________________________________________________________________________________

    //-- edit 16/07/2018 --
    //-- FID ใน NGBILL_DAILY_UPDATE คือ NGBILL_RPT_NO
    public DataTable SearchNgbillDailyUpdate(String NgBillRptNo, String FromDate = "", String ToDate = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "FID", NgBillRptNo, DBUTIL.FieldTypes.ftText); //-- FID ใน NGBILL_DAILY_UPDATE คือ NGBILL_RPT_NO
            AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            sql = " SELECT * FROM NGBILL_DAILY_UPDATE ";
            
            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY FID, RDATE ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 17/07/2018 --
    public void MngNgbillDailyUpdate(int op, string ID, string rDate, string NgBillRptNo, object C1 = null, object C2 = null, object C3 = null, object IC4 = null, object NC4 = null, object IC5 = null, object NC5 = null, object C6 = null, object N2 = null
                                   , object CO2 = null, object SG = null, object GHV = null, object NHV = null, object WC = null, object Unnorm = null, object UnnormMin = null, object UnnormMax = null, object WB = null, object H2S = null, object SumCompo = null)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        string nID = "";
        try
        {
            //-- EDIT 22/10/2020 -- ถ้าไม่มีข้อมูล ก็ไม่ต้องบันทึก 
            if ( !( op == DBUTIL.opINSERT && Utility.ToNum(SumCompo) == 0) )
            {

                SQL = ""; SQL1 = ""; SQL2 = "";
                if (op != DBUTIL.opINSERT)
                {
                    AddCriteria(ref Criteria, "ID", ID, DBUTIL.FieldTypes.ftNumeric);
                    AddCriteria(ref Criteria, "FID", NgBillRptNo, DBUTIL.FieldTypes.ftText);
                    AddCriteria(ref Criteria, "RDATE", Utility.AppDateValue(rDate), DBUTIL.FieldTypes.ftDate);
                }
                if (op != DBUTIL.opDELETE)
                {
                    if (op == DBUTIL.opINSERT)
                    {
                        nID = GenerateID("NGBILL_DAILY_UPDATE", "ID").ToString();
                        AddSQL(op, ref SQL1, ref SQL2, "ID", nID, DBUTIL.FieldTypes.ftNumeric);
                        AddSQL(op, ref SQL1, ref SQL2, "FID", NgBillRptNo, DBUTIL.FieldTypes.ftText);
                        AddSQL(op, ref SQL1, ref SQL2, "RDATE", Utility.AppDateValue(rDate), DBUTIL.FieldTypes.ftDate);
                        AddSQL(op, ref SQL1, ref SQL2, "CREATED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                        AddSQL(op, ref SQL1, ref SQL2, "CREATED_BY", System.Web.HttpContext.Current.Session["USER_NAME"] + "", DBUTIL.FieldTypes.ftText);

                    }
                    else
                    {
                        op = DBUTIL.opUPDATE;
                        //-- ต้องเก็บข้อมูลเก่าไว้ที่ฟิลด์ Lxxx
                        SQL1 += " LC1=C1, LC2=C2,LC3=C3,LIC4=IC4,LNC4=NC4,LIC5=IC5,LNC5=NC5,LC6=C6,LC7=C7,LC8=C8,LC9=C9,LC10=C10,LCO2=CO2,LN2=N2,LWC=WC,LGHV=GHV ";
                    }

                    AddSQL2(op, ref SQL1, ref SQL2, "HOUR", "0", DBUTIL.FieldTypes.ftNumeric);
                    String BeginMonth = "01/" + rDate.Split('/')[1] + "/" + rDate.Split('/')[2]; //-- จะบันทึกเป็นวันที่ 1 ของเดือน

                    AddSQL2(op, ref SQL1, ref SQL2, "BMONTH", Utility.AppDateValue(BeginMonth), DBUTIL.FieldTypes.ftDate);
                    AddSQL2(op, ref SQL1, ref SQL2, "DAY_ORDER", Convert.ToDateTime(Utility.AppDateValue(rDate)).Day, DBUTIL.FieldTypes.ftNumeric);

                    if (Utility.IsNumeric(C1)) C1 = Utility.FormatNum(C1, 3);
                    if (Utility.IsNumeric(C2)) C2 = Utility.FormatNum(C2, 3);
                    if (Utility.IsNumeric(C3)) C3 = Utility.FormatNum(C3, 3);
                    if (Utility.IsNumeric(IC4)) IC4 = Utility.FormatNum(IC4, 3);
                    if (Utility.IsNumeric(NC4)) NC4 = Utility.FormatNum(NC4, 3);
                    if (Utility.IsNumeric(IC5)) IC5 = Utility.FormatNum(IC5, 3);
                    if (Utility.IsNumeric(NC5)) NC5 = Utility.FormatNum(NC5, 3);
                    if (Utility.IsNumeric(C6)) C6 = Utility.FormatNum(C6, 3);
                    if (Utility.IsNumeric(N2)) N2 = Utility.FormatNum(N2, 3);
                    if (Utility.IsNumeric(CO2)) CO2 = Utility.FormatNum(CO2, 3);
                    if (Utility.IsNumeric(SG)) SG = Utility.FormatNum(SG, 4);
                    if (Utility.IsNumeric(GHV)) GHV = Utility.FormatNum(GHV, 3);
                    if (Utility.IsNumeric(NHV)) NHV = Utility.FormatNum(NHV, 3);
                    if (Utility.IsNumeric(WC)) WC = Utility.FormatNum(WC, 2); //-- H20
                    if (Utility.IsNumeric(Unnorm)) Unnorm = Utility.FormatNum(Unnorm, 3);
                    if (Utility.IsNumeric(UnnormMin)) UnnormMin = Utility.FormatNum(UnnormMin, 3);
                    if (Utility.IsNumeric(UnnormMax)) UnnormMax = Utility.FormatNum(UnnormMax, 3);
                    if (Utility.IsNumeric(WB)) WB = Utility.FormatNum(WB, 3);
                    if (Utility.IsNumeric(H2S)) H2S = Utility.FormatNum(H2S, 3);
                    if (Utility.IsNumeric(SumCompo)) SumCompo = Utility.FormatNum(SumCompo, 3);

                    AddSQL2(op, ref SQL1, ref SQL2, "C1", C1, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "C2", C2, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "C3", C3, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "IC4", IC4, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "NC4", NC4, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "IC5", IC5, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "NC5", NC5, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "C6", C6, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "N2", N2, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "CO2", CO2, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "SG", SG, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "GHV", GHV, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "NHV", NHV, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "WC", WC, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "UNNORMALIZED", Unnorm, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "UNNORMMIN", UnnormMin, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "UNNORMMAX", UnnormMax, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "WB", WB, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "H2S", H2S, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "SUM_COMPO", SumCompo, DBUTIL.FieldTypes.ftText);
                    AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                    AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_BY", System.Web.HttpContext.Current.Session["USER_NAME"] + "", DBUTIL.FieldTypes.ftText);

                }

                if (op != DBUTIL.opINSERT && Criteria == "")
                {
                    throw new Exception("Insufficient data!");
                }
                else
                {
                    SQL = CombineSQL(op, ref SQL1, ref SQL2, "NGBILL_DAILY_UPDATE", Criteria, timeStamp: false);
                    ExecuteSQL(SQL);
                }

            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    //-- aor edit 17/07/2018 --
    public void MngTmpDailyBtu(int op, string FID, string BtuDate, string Btu, string Run)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "FID", FID, DBUTIL.FieldTypes.ftText);
                AddCriteria(ref Criteria, "BTUDATE", Utility.AppDateValue(BtuDate), DBUTIL.FieldTypes.ftDate);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "FID", FID.Trim(), DBUTIL.FieldTypes.ftText);//-- EDIT 18/10/2019 -- FID.Trim()
                    AddSQL(op, ref SQL1, ref SQL2, "BTUDATE", Utility.AppDateValue(BtuDate), DBUTIL.FieldTypes.ftDateTime);
                }
                else {
                    op = DBUTIL.opUPDATE;
                }
                
                AddSQL2(op, ref SQL1, ref SQL2, "BTU", Utility.FormatNumNoComma(Btu,3), DBUTIL.FieldTypes.ftNumeric);
                AddSQL2(op, ref SQL1, ref SQL2, "RUN", Run, DBUTIL.FieldTypes.ftNumeric);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_TMP_DAILY_BTU", Criteria);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 17/07/2018 --
    public void MngTmp2OgcDailyBtu(string FID)
    {
        string SQL;
        try
        {
            if ( FID != "")
            {
                //-- ลบข้อมูลเก่าออกก่อน ย้อนหลัง 1 เดือน
                SQL = "DELETE FROM O_OGC_DAILY_BTU  " +
                     " WHERE FID = '" + FID + "' " +
                     "   AND TO_CHAR(BTUDATE,'YYYYMM') = (SELECT TO_CHAR(MAX(T.BTUDATE), 'YYYYMM') FROM O_TMP_DAILY_BTU T INNER JOIN O_OGC_DAILY_BTU O " +
                     "   ON T.FID = O.FID AND T.BTUDATE = O.BTUDATE  WHERE T.FID = '" + FID + "') ";
                ExecuteSQL(SQL);


                SQL = " INSERT INTO  O_OGC_DAILY_BTU " +
                    " (FID, BTUDATE, BTU, RUN, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY) " +
                    " (SELECT distinct T.FID, T.BTUDATE, T.BTU, T.RUN, T.CREATED_DATE, T.CREATED_BY, T.MODIFIED_DATE, T.MODIFIED_BY " +
                    "  FROM O_TMP_DAILY_BTU T LEFT OUTER JOIN O_OGC_DAILY_BTU O ON T.FID = O.FID AND T.BTUDATE = O.BTUDATE " +
                    "  WHERE T.FID = '" + FID + "' AND O.FID IS NULL " +
                    "   AND T.BTUDATE IN ( SELECT MIN(BTUDATE) FROM O_TMP_DAILY_BTU WHERE FID = '" + FID + "' GROUP BY TO_CHAR(BTUDATE,'YYYYMMDD') )  )";

                ExecuteSQL(SQL);

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    //-- aor edit 20/07/2018 --
    //-- edit 15/08/2018 --
    public void MngTmpDailyGqms(int op, string ID, string rDate, string FID, object C1 = null, object C2 = null, object C3 = null, object IC4 = null, object NC4 = null, object IC5 = null, object NC5 = null, object C6 = null, object N2 = null
                                   , object CO2 = null, object SG = null, object GHV = null, object NHV = null, object WC = null, object Unnorm = null, object UnnormMin = null, object UnnormMax = null, object WB = null, object H2S = null, object GHVFC = null
                                   , object OMA_NAME = null, object OGC_NAME = null, string userName = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";

            if (userName == "") userName = Utility.ToString(System.Web.HttpContext.Current.Session["USER_NAME"]);

            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "ID", ID, DBUTIL.FieldTypes.ftNumeric);
                AddCriteria(ref Criteria, "FID", FID, DBUTIL.FieldTypes.ftText);
                AddCriteria(ref Criteria, "RDATE", Utility.AppDateValue(rDate), DBUTIL.FieldTypes.ftDate);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "FID", FID.Trim(), DBUTIL.FieldTypes.ftText);    //-- EDIT 18/10/2019 -- FID.Trim()
                    AddSQL(op, ref SQL1, ref SQL2, "RDATE", Utility.AppDateValue(rDate), DBUTIL.FieldTypes.ftDate);
                    AddSQL(op, ref SQL1, ref SQL2, "CREATED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                    AddSQL(op, ref SQL1, ref SQL2, "CREATED_BY", userName, DBUTIL.FieldTypes.ftText);

                }
                else {
                    op = DBUTIL.opUPDATE;
                }
                

                AddSQL2(op, ref SQL1, ref SQL2, "C1", Utility.FormatNumNoComma(C1, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C2", Utility.FormatNumNoComma(C2, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C3", Utility.FormatNumNoComma(C3, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "IC4", Utility.FormatNumNoComma(IC4, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "NC4", Utility.FormatNumNoComma(NC4, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "IC5", Utility.FormatNumNoComma(IC5, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "NC5", Utility.FormatNumNoComma(NC5, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C6", Utility.FormatNumNoComma(C6, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "N2", Utility.FormatNumNoComma(N2, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "CO2", Utility.FormatNumNoComma(CO2, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "SG", Utility.FormatNumNoComma(SG, 4), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "GHV", Utility.FormatNumNoComma(GHV, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "NHV", Utility.FormatNumNoComma(NHV, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "WC", Utility.FormatNumNoComma(WC, 2), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "UNNORMALIZED", Utility.FormatNumNoComma(Unnorm, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "UNNORMMIN", Utility.FormatNumNoComma(UnnormMin, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "UNNORMMAX", Utility.FormatNumNoComma(UnnormMax, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "WB", Utility.FormatNumNoComma(WB, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "H2S", Utility.FormatNumNoComma(H2S, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "GHVFC", Utility.FormatNumNoComma(GHVFC, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_BY", userName , DBUTIL.FieldTypes.ftText);
                

                AddSQL2(op, ref SQL1, ref SQL2, "OMA_NAME", OMA_NAME, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "OGC_NAME", OGC_NAME, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_TMP_DAILY_GQMS", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 20/07/2018 --
    //-- edit 15/08/2018 -- เพิ่ม OMA_NAME, FLOW_NAME
    //-- EDIT 03/02/2020 -- เพิ่ม flag ให้ update ค่าน้ำ WC  , Boolean WCFlag = false
    public void MngTmp2GqmsDailyUpdate(string FID, Boolean WCFlag = false )
    {
        string SQL;
        string nID = "";
        try
        {
            if (FID != "")
            {
                //-- ต้องตรวจสอบว่ามีข้อมูลใน GQMS_DAILY_UPDATE หรือยัง
                // ถ้ามีต้อง update แล้วย้ายข้อมูลเดิมไปไว้ที่ฟิลด์ Lxxxx สำหรับครั้งแรกเท่านั้น 
                // ถ้าไม่มีให้ insert 


                ////SQL = " MERGE INTO GQMS_DAILY_UPDATE A " +
                ////    "   USING (SELECT * FROM O_TMP_DAILY_GQMS WHERE FID = '" + FID + "') B " +
                ////    "  ON (A.FID = B.FID AND A.RDATE = B.RDATE ) " +
                ////    "   WHEN MATCHED THEN UPDATE SET " +
                ////    " A.LC1 = A.C1, A.LC2 = A.C2, A.LC3 = A.C3, A.LIC4 = A.IC4, A.LNC4 = A.NC4,  " +
                ////    " A.LIC5 = A.IC5, A.LNC5 = A.NC5, A.LC6 = A.C6, A.LN2 = A.N2, A.LCO2 = A.CO2,  " +
                ////    " A.LSG = A.SG, A.LGHV = A.GHV, A.LNHV = A.NHV, A.LWC = A.WC, A.LUNNORMALIZED = A.UNNORMALIZED,  " +
                ////    " A.LWB = A.WB, A.LH2S = A.H2S, A.LC7 = A.C7, A.LC8 = A.C8, A.LUNNORMMIN = A.UNNORMMIN, A.LUNNORMMAX = A.UNNORMMAX,  " +
                ////    " A.LGHVFC = A.GHVFC, A.C1 = B.C1, A.C2 = B.C2, A.C3 = B.C3, A.IC4 = B.IC4, A.NC4 = B.NC4, A.IC5 = B.IC5, A.NC5 = B.NC5,  " +
                ////    " A.C6 = B.C6, A.N2 = B.N2, A.CO2 = B.CO2, A.SG = B.SG, A.GHV = B.GHV, A.NHV = B.NHV, A.WC = B.WC,  " +
                ////    " A.UNNORMALIZED = B.UNNORMALIZED, A.WB = B.WB, A.H2S = B.H2S, A.C7 = B.C7, A.C8 = B.C8, A.UNNORMMIN = B.UNNORMMIN,  " +
                ////    " A.UNNORMMAX = B.UNNORMMAX, A.GHVFC = B.GHVFC, A.MODIFIED_BY = B.MODIFIED_BY, A.MODIFIED_DATE = B.MODIFIED_DATE  ";

                //-- ถ้ามีต้อง update แล้วย้ายข้อมูลเดิมไปไว้ที่ฟิลด์ Lxxxx สำหรับครั้งแรกเท่านั้น 
                SQL = " MERGE INTO GQMS_DAILY_UPDATE A " +
                    "   USING (SELECT * FROM O_TMP_DAILY_GQMS WHERE FID = '" + FID + "') B " +
                    "  ON (A.FID = B.FID AND A.RDATE = B.RDATE AND A.CREATED_DATE=A.MODIFIED_DATE) " +
                    "   WHEN MATCHED THEN UPDATE SET " +
                    " A.LC1 = A.C1, A.LC2 = A.C2, A.LC3 = A.C3, A.LIC4 = A.IC4, A.LNC4 = A.NC4,  " +
                    " A.LIC5 = A.IC5, A.LNC5 = A.NC5, A.LC6 = A.C6, A.LN2 = A.N2, A.LCO2 = A.CO2,  " +
                    " A.LSG = A.SG, A.LGHV = A.GHV, A.LNHV = A.NHV, A.LWC = A.WC, A.LUNNORMALIZED = A.UNNORMALIZED,  " +
                    " A.LWB = A.WB, A.LH2S = A.H2S, A.LC7 = A.C7, A.LC8 = A.C8, A.LUNNORMMIN = A.UNNORMMIN, A.LUNNORMMAX = A.UNNORMMAX,  " +
                    " A.LGHVFC = A.GHVFC ";
                ExecuteSQL(SQL);

                //-- edit 13/01/2020 -- ไม่ต้อง update ค่าน้ำ WC
                //-- ถ้ามีต้อง update  
                SQL = " MERGE INTO GQMS_DAILY_UPDATE A " +
                "   USING (SELECT * FROM O_TMP_DAILY_GQMS WHERE FID = '" + FID + "') B " +
                "  ON (A.FID = B.FID AND A.RDATE = B.RDATE ) " +
                "   WHEN MATCHED THEN UPDATE SET " +
                " A.C1 = B.C1, A.C2 = B.C2, A.C3 = B.C3, A.IC4 = B.IC4, A.NC4 = B.NC4, A.IC5 = B.IC5, A.NC5 = B.NC5,  " +
                " A.C6 = B.C6, A.N2 = B.N2, A.CO2 = B.CO2, A.SG = B.SG, A.GHV = B.GHV, A.NHV = B.NHV, ";    // A.WC = B.WC,  ไม่ต้อง update ค่าน้ำ WC

                if ( WCFlag == true )
                {
                    SQL += " A.WC = B.WC, "; //ต้อง update ค่าน้ำ WC เพราะบางที import จาก excel 
                }

                SQL +=" A.UNNORMALIZED = B.UNNORMALIZED, A.WB = B.WB, A.H2S = B.H2S, A.C7 = B.C7, A.C8 = B.C8, A.UNNORMMIN = B.UNNORMMIN,  " +
                " A.UNNORMMAX = B.UNNORMMAX, A.GHVFC = B.GHVFC, A.MODIFIED_BY = B.MODIFIED_BY, A.MODIFIED_DATE = B.MODIFIED_DATE ";
                ExecuteSQL(SQL);

                //-- edit 13/01/2020 -- ต้อง update ค่าน้ำ WC , A.WC = B.WC
                //-- ต้อง update OMA_NAME, OGC_NAME เฉพาะมีข้อมูลที่ส่งมาเท่านั้น
                SQL = " MERGE INTO GQMS_DAILY_UPDATE A " +
               "   USING (SELECT * FROM O_TMP_DAILY_GQMS WHERE FID = '" + FID + "') B " +
               "  ON (A.FID = B.FID AND A.RDATE = B.RDATE AND NOT B.OMA_NAME IS NULL ) " +
               "   WHEN MATCHED THEN UPDATE SET " +
               " A.OMA_NAME=B.OMA_NAME, A.OGC_NAME=B.OGC_NAME ";

                if (WCFlag == false)
                {
                    SQL += ", A.WC = B.WC ";  
                }

                 ExecuteSQL(SQL);

                //-- insert data ที่ไม่มีใน GQMS_DAILY_UPDATE
                nID = GenerateID("GQMS_DAILY_UPDATE", "ID").ToString();

                SQL = "INSERT INTO GQMS_DAILY_UPDATE " +
                    " (ID, RDATE, FID, C1, C2, C3, IC4, NC4, IC5, NC5, C6, N2, CO2, SG, GHV, GHVFC " +
                    "  , NHV, WC, UNNORMALIZED, WB, H2S, C7, C8, UNNORMMIN, UNNORMMAX " +
                    "  , CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, OMA_NAME, OGC_NAME ) " +
                    " (SELECT " + nID + "+ROW_NUMBER() OVER(ORDER BY RDATE ASC), RDATE, FID, C1, C2, C3, IC4, NC4, IC5, NC5, C6, N2, CO2, SG, GHV, GHVFC " +
                    "  , NHV, WC, UNNORMALIZED, WB, H2S, C7, C8, UNNORMMIN, UNNORMMAX " +
                    "  , CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, OMA_NAME, OGC_NAME " +
                    " FROM O_TMP_DAILY_GQMS WHERE FID = '" + FID + "' " +
                    " AND NOT RDATE IN (SELECT DISTINCT RDATE FROM GQMS_DAILY_UPDATE WHERE FID = '" + FID + "') ) ";
                ExecuteSQL(SQL);  


            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }
    }


    //-- aor edit 26/07/2018 --
    public void TransferTmp2GqmsDailyUpdate(string FID)
    {
        string SQL;
        string nID = "";
        try
        {
            if (FID != "")
            {
                //-- ต้องตรวจสอบว่ามีข้อมูลใน GQMS_DAILY_UPDATE หรือยัง
                // ถ้ามีต้อง เปลี่ยน FID ให้เป็นชื่อ xxx_BACKUP แล้วจึงค่อย insert 
                // ถ้าไม่มีให้ insert 
                SQL = "DELETE FROM GQMS_DAILY_UPDATE " +
                    " WHERE FID = '" + FID + "_BACKUP' " +
                    "   AND RDATE IN (SELECT A.RDATE  FROM GQMS_DAILY_UPDATE A INNER JOIN O_TMP_DAILY_GQMS B ON A.FID = B.FID AND A.RDATE = B.RDATE " +
                    "       WHERE A.FID = '" + FID + "_BACKUP') ";
                ExecuteSQL(SQL);

                SQL = " UPDATE GQMS_DAILY_UPDATE SET FID = FID || '_BACKUP' " +
                    " WHERE FID = '" + FID + "' AND RDATE IN (SELECT RDATE FROM O_TMP_DAILY_GQMS WHERE FID = '" + FID + "')  ";
                ExecuteSQL(SQL);


                //-- EDIT 15/01/2020 --- ต้องดูค่า WC, OMA_NAME ด้วย เพราะข้อมูลเก่าอาจจะกำหนดค่า WC แล้ว จึงต้องนำมาด้วย
                SQL = " MERGE INTO O_TMP_DAILY_GQMS A  " +
                    "   USING  ( SELECT T.FID FID_N, K.FID,K.RDATE,MAX(K.WC) WC , MAX(K.OMA_NAME) OMA_NAME   " +
                    "                FROM O_TMP_DAILY_GQMS T INNER JOIN GQMS_DAILY_UPDATE K  " +
                    "                ON T.FID|| '_BACKUP' = K.FID AND T.FID = '" + FID + "' AND T.RDATE = K.RDATE AND NOT K.OMA_NAME IS NULL  " +
                    "                GROUP BY T.FID, K.FID,K.RDATE  ) B   " +
                    "   ON (A.FID = B.FID_N AND A.RDATE = B.RDATE  )   " +
                    "    WHEN MATCHED THEN UPDATE SET    " +
                    "   A.OMA_NAME=B.OMA_NAME, A.WC = B.WC    ";
                ExecuteSQL(SQL);


                //-- insert data ที่ไม่มีใน GQMS_DAILY_UPDATE
                nID = GenerateID("GQMS_DAILY_UPDATE", "ID").ToString();


                //-- EDIT 15/01/2020 -- เพิ่ม OMA_NAME
                //-- EDIT 18/04/2022 -- RDATE>SYSDATE-365 AND
                SQL = "INSERT INTO GQMS_DAILY_UPDATE " +
                    " (ID, RDATE, FID, C1, C2, C3, IC4, NC4, IC5, NC5, C6, N2, CO2, SG, GHV, GHVFC " +
                    "  , NHV, WC, UNNORMALIZED, WB, H2S, C7, C8, UNNORMMIN, UNNORMMAX " +
                    "  , CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, OMA_NAME ) " +
                    " (SELECT " + nID + "+ROW_NUMBER() OVER(ORDER BY RDATE ASC), RDATE, FID, C1, C2, C3, IC4, NC4, IC5, NC5, C6, N2, CO2, SG, GHV, GHVFC " +
                    "  , NHV, WC, UNNORMALIZED, WB, H2S, C7, C8, UNNORMMIN, UNNORMMAX " +
                    "  , CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, OMA_NAME " +
                    " FROM O_TMP_DAILY_GQMS WHERE FID = '" + FID + "' " +
                    " AND NOT RDATE IN (SELECT DISTINCT RDATE FROM GQMS_DAILY_UPDATE WHERE RDATE>SYSDATE-365 AND FID = '" + FID + "') ) ";
                ExecuteSQL(SQL);

                //-- EDIT 18/04/2022 -- ถ้าบันทึกสำเร็จ ให้ลบข้อมูลที่ O_TMP_DAILY_GQMS เลย เพื่อจะได้ run อีกรอบกรณีที่เกิด error 
                SQL = "DELETE FROM O_TMP_DAILY_GQMS " +
                    " WHERE FID = '" + FID + "' ";
                ExecuteSQL(SQL);

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }
    }


    //-- edit 23/07/2019 -- กรณีวันที่ผ่านมาไม่มีข้อมูล ก็จะไม่มี record จึงทำให้ไม่แสดงข้อมูลอื่นๆที่เกี่ยวข้อง จึงต้องใส่ null record 
    //ถ้า site หลักไม่มีข้อมูลแล้ว เลือก site สำรอง  ข้อมูลของ site สำรอง ยังไม่แสดง จนกว่าจะ save แล้วเลือกใหม่ แต่ถ้า site หลักมีข้อมูล แล้วเลือกสำรอง จะแสดงข้อมูลเลย
    //ค่า moisture (H2O) WC, ข้อมูล H2O มาจากแหล่งอื่นที่ไม่ใช่แแหล่งเดียวกับ GC ซึ่งบางวันมีข้อมูล H2O แต่ไม่มี GC เลยไม่มี record ทำให้ค่า H2O ไม่แสดง

    public void InsertNULLGqmsDailyUpdate(String FID, String MM = "", String YY = "")
    {
        string user = System.Web.HttpContext.Current.Session["USER_NAME"] + "";
        string SQL;
        string nID = "";
        string YMD1 = "", YMD2 = "";
        try
        {
            if ( FID != "" && MM!= "" && YY != "")
            {
                string MM2 = "", YY2 = "";
                if (Utility.ToInt(MM) == 12)
                {
                    YY2 = (Utility.ToInt(YY) + 1).ToString();
                    MM2 = "1";
                }
                else
                {
                    YY2 = YY;
                    MM2 = (Utility.ToInt(MM) + 1).ToString();
                }

                YMD1 = YY + MM.PadLeft(2, '0') + "01";
                YMD2 = YY2 + MM2.PadLeft(2, '0') + "01";


                //-- insert data ที่ไม่มีใน GQMS_DAILY_UPDATE
                nID = GenerateID("GQMS_DAILY_UPDATE", "ID").ToString();

                SQL = "INSERT INTO GQMS_DAILY_UPDATE  " +
                " (ID, RDATE, FID,  CREATED_BY, CREATED_DATE  )   " +
                " (SELECT " + nID + "+ROW_NUMBER() OVER(ORDER BY ADATE ASC)  , DY.ADATE, '" + FID + "' FID, '" + user + "', SYSDATE " +
                "  FROM O_DIM_DATE DY LEFT OUTER JOIN  " +
                "  (SELECT * FROM GQMS_DAILY_UPDATE D " +
                "  WHERE FID='" + FID + "' AND (D.RDATE>=TO_DATE('" + YMD1 + "','YYYYMMDD') AND D.RDATE<TO_DATE('" + YMD2 + "','YYYYMMDD')) ) A " +
                " ON DY.ADATE=A.RDATE    " +
                "  WHERE (ADATE>=TO_DATE('" + YMD1 + "','YYYYMMDD') AND ADATE<TO_DATE('" + YMD2 + "','YYYYMMDD'))  AND ADATE<SYSDATE-1 " +
                " AND A.ID IS NULL ) ";
                ExecuteSQL(SQL);
            }

 
  


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- EDIT 04/10/2022 -- กรณีที่ไม่มีข้อมูลเข้า ให้ใส่ null
    public void InsertNULLGqmsDailyUpdateYMD(String YMD)
    {
        string user = "GQMS_SYSTEM";
        string SQL;
        string nID = "";
        try
        {
            if (YMD != "")
            {

                //-- insert data ที่ไม่มีใน GQMS_DAILY_UPDATE
                nID = GenerateID("GQMS_DAILY_UPDATE", "ID").ToString();

                SQL = "INSERT INTO GQMS_DAILY_UPDATE   " +
                " (ID, RDATE, FID,  CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE  )    " +
                " (SELECT " + nID + "+ROW_NUMBER() OVER(ORDER BY D1.FID ASC) , TO_DATE('" + YMD + "','YYYYMMDD'),D1.FID,'" + user + "',SYSDATE,'" + user + "',SYSDATE " + //--เดิมใส่ DD EDIT 220/03/2023
                " FROM GQMS_DAILY_UPDATE D1 LEFT OUTER JOIN GQMS_DAILY_UPDATE D2  ON D1.FID=D2.FID AND D1.RDATE=D2.RDATE-1 " +
                " WHERE  D1.RDATE=TO_DATE('" + YMD + "','YYYYMMDD')-1 AND D2.RDATE IS NULL  " +
                "  AND NOT D1.FID LIKE '%_BACKUP' ) "; //-- EDIT 22/03/2023 ไม่เอา _BACKUP

                ExecuteSQL(SQL);
            }





        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    // --------  AUTO REPLACE OMA  -----------------------------------------------------------------------------------------------------
    //-- FromYMD = YYYYMMDD
    //-- ค้นหา edit 22/08/2022 --
    //  -- edit 23/03/2025 -- 
    //    ต้องการปรับปรุงให้ตรวจสอบ column UnnormMin, UnnormMax ด้วยครับ โดยมีเงื่อนไขการตรวจสอบดังนี้
    //1. ถ้า UnnormMin< 98 ให้ระบบแจ้ง invalid Unnorm และมีการแทนค่า
    //2. ถ้า UnnormMax > 102 ให้ระบบแจ้ง invalid Unnorm และมีการแทนค่า
    //สำหรับเงื่อนไขเดิมที่ตรวจสอบ Column Unnormalize ให้ยกเลิกการตรวจสอบ เนื่องจากเงื่อนไขทั้ง 2 ข้อด้านบนครอบคลุมค่าของ Column Unnormalize แล้วครับ
    //  1. Unnorm อยู่นอกช่วง 100±2  (คือ นอกช่วง 98-102) --->  ใช้ UnnormMin, UnnormMax
    public DataTable SearchFID_InvalidUnNorm(String FromYMD, String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            if (FromYMD != "")
            {
                if (criteria != "") criteria += " AND ";
                criteria += " RDATE > TO_DATE('" + FromYMD + "','YYYYMMDD') ";

               //  sql = "SELECT ID, RDATE, FID, UNNORMALIZED, UNNORMMIN, UNNORMMAX FROM GQMS_DAILY_UPDATE " +
               //  " WHERE (UNNORMALIZED < 98 OR UNNORMALIZED > 102) AND UNNORMALIZED> 0 AND AUTO_CAUSE IS NULL ";
               //-- edit 30/03/2025 --
                sql = "SELECT ID, RDATE, FID, UNNORMALIZED, UNNORMMIN, UNNORMMAX FROM GQMS_DAILY_UPDATE " +
               " WHERE (UNNORMMIN < 98 OR UNNORMMAX > 102 OR UNNORMALIZED < 98 OR UNNORMALIZED > 102) AND UNNORMALIZED> 0 AND AUTO_CAUSE IS NULL ";

                if (criteria != "") { sql += " AND " + criteria; }

                if (orderSQL != "")
                {
                    sql += " ORDER BY " + orderSQL;
                }
                else
                {
                    sql += " ORDER BY FID, RDATE ";
                }

            }



            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- ค้นหา edit 22/08/2022 -- ไม่มีข้อมูลมา 
    public DataTable SearchFID_InvalidNA(String FromYMD ,String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            if (FromYMD != "")
            {
                if (criteria != "") criteria += " AND ";
                criteria += " RDATE > TO_DATE('" + FromYMD + "','YYYYMMDD') ";
              

                sql = " SELECT ID, RDATE, FID, C1 FROM GQMS_DAILY_UPDATE   " +
                " WHERE C1 IS NULL AND AUTO_CAUSE IS NULL ";


                if (criteria != "") { sql += " AND " + criteria; }

                if (orderSQL != "")
                {
                    sql += " ORDER BY " + orderSQL;
                }
                else
                {
                    sql += " ORDER BY FID, RDATE ";
                }

            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    //-- edit 22/08/2022 --
    public void UpdateAutoGqmsDailyUpdate(String ID, String FID, String RDATE_YMD = "", String AutoCause = null, String AutoOGCName = null, String AutoDate = null)
    {
        int op = DBUTIL.opUPDATE;
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            Project.dal.AddCriteria(ref Criteria, "ID", ID, DBUTIL.FieldTypes.ftNumeric);
            Project.dal.AddCriteria(ref Criteria, "FID", FID, DBUTIL.FieldTypes.ftText);
            if (RDATE_YMD != "")
            {
                if (Criteria != "") Criteria += " AND ";
                Criteria += " RDATE > TO_DATE('" + RDATE_YMD + "','YYYYMMDD') ";
            }

            Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "AUTO_CAUSE", AutoCause, DBUTIL.FieldTypes.ftText);
            Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "AUTO_OGC_NAME", AutoOGCName, DBUTIL.FieldTypes.ftText);
            if (AutoDate != null) Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "AUTO_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);


            if (Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "GQMS_DAILY_UPDATE", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    //-- edit 22/08/2022 --
    public void AutoReplaceGqmsDailyUpdateByFID(String destID, String srcFID)
    {
        string SQL = "";
        try
        {

            if (destID != "" && srcFID != "")
            {
                SQL = "MERGE INTO GQMS_DAILY_UPDATE D " +
               "    USING GQMS_DAILY_UPDATE S " +
               "    ON (D.RDATE=S.RDATE AND D.ID=" + destID + " AND S.FID='" + srcFID + "') " +
               "  WHEN MATCHED THEN " +
               "    UPDATE SET D.C1=S.C1, D.C2=S.C2, D.C3=S.C3, D.IC4=S.IC4, D.NC4=S.NC4, D.IC5=S.IC5, D.NC5=S.NC5 " +
               "    , D.C6=S.C6, D.C7=S.C7, D.C8=S.C8, D.N2=S.N2, D.CO2=S.CO2, D.SG=S.SG, D.GHV=S.GHV, D.NHV=S.NHV " +
               "    , D.WC=S.WC, D.GHVFC=S.GHVFC, D.WB=S.WB, D.H2S=S.H2S " +
               "    , D.UNNORMALIZED=S.UNNORMALIZED, D.UNNORMMIN=S.UNNORMMIN, D.UNNORMMAX=S.UNNORMMAX " +
               "    , D.OGC_NAME='" + srcFID + "', D.AUTO_OGC_NAME='" + srcFID + "', D.AUTO_DATE=SYSDATE";

                ExecuteSQL(SQL);

            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    //-- edit 22/08/2022 --
    public void AutoReplaceGqmsDailyUpdateByLastGood(String destID, String srcFID)
    {
        string SQL = "";
        try
        {

            if (destID != "" && srcFID != "")
            {
                //-- ค้นหา LAST GOOD --
                String srcID = "";
                //String SQLmax = " SELECT MAX(ID) MX_ID FROM GQMS_DAILY_UPDATE  WHERE FID='" + srcFID + "' AND  ID <" + destID + " AND AUTO_CAUSE IS NULL AND NOT C1 IS NULL"; //-- EDIT 23/03/2023
                //-- edit 26/05/2025 -- มีกรณีที่ข้อมูลวันที่ก่อนหน้า มาที่หลัง (ดูจาก ID) ?????
                // ID       RDATE               FID
                //899140	22-MAY-25 00:00:00 GSRC 12
                //899119	23-MAY-25 00:00:00 GSRC 12

                String SQLmax = " SELECT MAX(ID) MX_ID FROM GQMS_DAILY_UPDATE  WHERE FID='" + srcFID + "' AND  ID <" + destID + " AND AUTO_CAUSE IS NULL AND NOT C1 IS NULL" +
                   "   AND RDATE = ( SELECT MAX(RDATE) FROM GQMS_DAILY_UPDATE  WHERE FID='" + srcFID + "' AND  ID <" + destID + " AND AUTO_CAUSE IS NULL AND NOT C1 IS NULL) ";

                srcID = GetSQLValue(SQLmax);

                if (srcID != "")
                {
                    SQL = "MERGE INTO GQMS_DAILY_UPDATE D " +
              "    USING GQMS_DAILY_UPDATE S " +
              "    ON (D.FID=S.FID AND D.ID=" + destID + " AND S.ID=" + srcID + ") " +
              "  WHEN MATCHED THEN " +
              "    UPDATE SET D.C1=S.C1, D.C2=S.C2, D.C3=S.C3, D.IC4=S.IC4, D.NC4=S.NC4, D.IC5=S.IC5, D.NC5=S.NC5 " +
              "    , D.C6=S.C6, D.C7=S.C7, D.C8=S.C8, D.N2=S.N2, D.CO2=S.CO2, D.SG=S.SG, D.GHV=S.GHV, D.NHV=S.NHV " +
              "    , D.WC=S.WC, D.GHVFC=S.GHVFC, D.WB=S.WB, D.H2S=S.H2S " +
              "    , D.UNNORMALIZED=S.UNNORMALIZED, D.UNNORMMIN=S.UNNORMMIN, D.UNNORMMAX=S.UNNORMMAX " +
              "    ,  D.OGC_NAME='LAST_GOOD',D.AUTO_OGC_NAME='LAST_GOOD', D.AUTO_DATE=SYSDATE";

                    ExecuteSQL(SQL);
                }


               
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }



    // -----------------------------------------------------------------------------------------------------------------------------------

    //-- edit 21/06/2019 --
    //-- edit 26/07/2019 -- เพิ่ม row_no
    public void MngTmpOgcH2S(int op, string sName, string sDate, object ROWNO = null, object sampleNo = null, object SULFUR = null, object H2S = null, object COS = null, object CH3SH = null, object C2H5SH = null, object DMS = null, object LSH = null, object C3H7SH = null
                            , string userName = "", string OtherCri = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";

        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";

            if (userName == "") userName = Utility.ToString(System.Web.HttpContext.Current.Session["USER_NAME"]);

            if (op != DBUTIL.opINSERT)
            {
                Criteria = OtherCri;
                AddCriteria(ref Criteria, "H2S_NAME", sName, DBUTIL.FieldTypes.ftText);
                AddCriteria(ref Criteria, "SDATE", Utility.AppDateValue(sDate), DBUTIL.FieldTypes.ftDate); //ค้นหาเฉพาะวันที่
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "H2S_NAME", sName, DBUTIL.FieldTypes.ftText);
                    AddSQL(op, ref SQL1, ref SQL2, "SDATE", Utility.AppDateValue(sDate), DBUTIL.FieldTypes.ftDateTime);
                    AddSQL(op, ref SQL1, ref SQL2, "CREATED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                    AddSQL(op, ref SQL1, ref SQL2, "CREATED_BY", userName, DBUTIL.FieldTypes.ftText);

                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "SAMPLE_NO", sampleNo, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "SULFUR", Utility.FormatNumNoComma(SULFUR, 2), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "H2S", Utility.FormatNumNoComma(H2S, 2), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "COS", Utility.FormatNumNoComma(COS, 2), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "CH3SH", Utility.FormatNumNoComma(CH3SH, 2), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C2H5SH", Utility.FormatNumNoComma(C2H5SH, 2), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "DMS", Utility.FormatNumNoComma(DMS, 2), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "LSH", Utility.FormatNumNoComma(LSH, 2), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C3H7SH", Utility.FormatNumNoComma(C3H7SH, 2), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_BY", userName, DBUTIL.FieldTypes.ftText);

                if ( Utility.IsNumeric(ROWNO)) AddSQL2(op, ref SQL1, ref SQL2, "ROW_NO", ROWNO, DBUTIL.FieldTypes.ftNumeric);
            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_TMP_H2S", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 22/06/2019 --  
    //-- edit 26/07/2019 -- เพิ่ม row_no
    public void MngTmp2OgcH2S(string YYYYMM)
    {
        string SQL;
        try
        {
            if (YYYYMM != "")
            {
                //-- ลบข้อมูลเก่าออกก่อน 
                SQL = "DELETE FROM O_OGC_H2S  " +
                     " WHERE TO_CHAR(SDATE,'YYYYMM') = '" + YYYYMM +"' ";
                ExecuteSQL(SQL);


                SQL = " INSERT INTO  O_OGC_H2S " +
                    " (H2S_NAME,SDATE,SAMPLE_NO,SULFUR,H2S,COS,CH3SH,C2H5SH,DMS,LSH,C3H7SH,  CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY,ROW_NO) " +
                    " (SELECT H2S_NAME,SDATE,SAMPLE_NO,SULFUR,H2S,COS,CH3SH,C2H5SH,DMS,LSH,C3H7SH,  CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY,ROW_NO " +
                    "  FROM O_TMP_H2S " +
                    " WHERE TO_CHAR(SDATE,'YYYYMM') = '" + YYYYMM + "') ";
                ExecuteSQL(SQL);

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    //-- edit 22/06/2019 --
    //-- edit 26/07/2019 -- เพิ่ม row_no
    public void MngTmpOgcHC(int op, string sName, string sDate, object ROWNO = null, object HC = null, string userName = "", string OtherCri = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";

        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";

            if (userName == "") userName = Utility.ToString(System.Web.HttpContext.Current.Session["USER_NAME"]);

            if (op != DBUTIL.opINSERT)
            {
                Criteria = OtherCri;
                AddCriteria(ref Criteria, "HC_NAME", sName, DBUTIL.FieldTypes.ftText);
                AddCriteria(ref Criteria, "SDATE", Utility.AppDateValue(sDate), DBUTIL.FieldTypes.ftDate); //ค้นหาเฉพาะวันที่
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "HC_NAME", sName, DBUTIL.FieldTypes.ftText);
                    AddSQL(op, ref SQL1, ref SQL2, "SDATE", Utility.AppDateValue(sDate), DBUTIL.FieldTypes.ftDateTime);
                    AddSQL(op, ref SQL1, ref SQL2, "CREATED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                    AddSQL(op, ref SQL1, ref SQL2, "CREATED_BY", userName, DBUTIL.FieldTypes.ftText);

                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }


                AddSQL2(op, ref SQL1, ref SQL2, "HC", Utility.FormatNumNoComma(HC, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_BY", userName, DBUTIL.FieldTypes.ftText);
                if (Utility.IsNumeric(ROWNO)) AddSQL2(op, ref SQL1, ref SQL2, "ROW_NO", ROWNO, DBUTIL.FieldTypes.ftNumeric);
            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_TMP_HC", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 22/06/2019 --  
    //-- edit 26/07/2019 -- เพิ่ม row_no
    public void MngTmp2OgcHC(string YYYYMM)
    {
        string SQL;
        try
        {
            if (YYYYMM != "")
            {
                //-- ลบข้อมูลเก่าออกก่อน 
                SQL = "DELETE FROM O_OGC_HC  " +
                     " WHERE TO_CHAR(SDATE,'YYYYMM') = '" + YYYYMM + "' ";
                ExecuteSQL(SQL);


                SQL = " INSERT INTO  O_OGC_HC " +
                    " (HC_NAME,SDATE,HC, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY,ROW_NO) " +
                    " (SELECT HC_NAME,SDATE,HC, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY,ROW_NO " +
                    "  FROM O_TMP_HC " +
                    " WHERE TO_CHAR(SDATE,'YYYYMM') = '" + YYYYMM + "') ";
                ExecuteSQL(SQL);

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    //-- edit 22/06/2019 --
    //-- edit 26/07/2019 -- เพิ่ม row_no
    public void MngTmpOgcHG(int op, string sName, string sDate, object ROWNO = null, object sampleNo = null, object HG = null, object VOL = null, string userName = "", string OtherCri = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";

        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";

            if (userName == "") userName = Utility.ToString(System.Web.HttpContext.Current.Session["USER_NAME"]);

            if (op != DBUTIL.opINSERT)
            {
                Criteria = OtherCri;
                AddCriteria(ref Criteria, "HG_NAME", sName, DBUTIL.FieldTypes.ftText);
                AddCriteria(ref Criteria, "SDATE", Utility.AppDateValue(sDate), DBUTIL.FieldTypes.ftDate); //ค้นหาเฉพาะวันที่
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "HG_NAME", sName, DBUTIL.FieldTypes.ftText);
                    AddSQL(op, ref SQL1, ref SQL2, "SDATE", Utility.AppDateValue(sDate), DBUTIL.FieldTypes.ftDateTime);
                    AddSQL(op, ref SQL1, ref SQL2, "CREATED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                    AddSQL(op, ref SQL1, ref SQL2, "CREATED_BY", userName, DBUTIL.FieldTypes.ftText);

                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "SAMPLE_NO", sampleNo, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "HG", Utility.FormatNumNoComma(HG, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "VOL", Utility.FormatNumNoComma(VOL, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_BY", userName, DBUTIL.FieldTypes.ftText);
                if (Utility.IsNumeric(ROWNO)) AddSQL2(op, ref SQL1, ref SQL2, "ROW_NO", ROWNO, DBUTIL.FieldTypes.ftNumeric);
            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_TMP_HG", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 22/06/2019 --  
    //-- edit 26/07/2019 -- เพิ่ม row_no
    public void MngTmp2OgcHG(string YYYYMM)
    {
        string SQL;
        try
        {
            if (YYYYMM != "")
            {
                //-- ลบข้อมูลเก่าออกก่อน 
                SQL = "DELETE FROM O_OGC_HG  " +
                     " WHERE TO_CHAR(SDATE,'YYYYMM') = '" + YYYYMM + "' ";
                ExecuteSQL(SQL);


                SQL = " INSERT INTO  O_OGC_HG " +
                    " (HG_NAME,SDATE,SAMPLE_NO,HG,VOL, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY,ROW_NO) " +
                    " (SELECT HG_NAME,SDATE,SAMPLE_NO,HG,VOL, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY,ROW_NO " +
                    "  FROM O_TMP_HG " +
                    " WHERE TO_CHAR(SDATE,'YYYYMM') = '" + YYYYMM + "') ";
                ExecuteSQL(SQL);

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 22/06/2019 --
    //-- edit 26/07/2019 -- เพิ่ม row_no
    public void MngTmpOgcO2(int op, string sName, string sDate, object ROWNO = null, object sampleNo = null, object O2 = null, string userName = "", string OtherCri = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";

        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";

            if (userName == "") userName = Utility.ToString(System.Web.HttpContext.Current.Session["USER_NAME"]);

            if (op != DBUTIL.opINSERT)
            {
                Criteria = OtherCri;
                AddCriteria(ref Criteria, "O2_NAME", sName, DBUTIL.FieldTypes.ftText);
                AddCriteria(ref Criteria, "SDATE", Utility.AppDateValue(sDate), DBUTIL.FieldTypes.ftDate); //ค้นหาเฉพาะวันที่
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "O2_NAME", sName, DBUTIL.FieldTypes.ftText);
                    AddSQL(op, ref SQL1, ref SQL2, "SDATE", Utility.AppDateValue(sDate), DBUTIL.FieldTypes.ftDateTime);
                    AddSQL(op, ref SQL1, ref SQL2, "CREATED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                    AddSQL(op, ref SQL1, ref SQL2, "CREATED_BY", userName, DBUTIL.FieldTypes.ftText);

                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "SAMPLE_NO", sampleNo, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "O2", Utility.FormatNumNoComma(O2, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_BY", userName, DBUTIL.FieldTypes.ftText);
                if (Utility.IsNumeric(ROWNO)) AddSQL2(op, ref SQL1, ref SQL2, "ROW_NO", ROWNO, DBUTIL.FieldTypes.ftNumeric);
            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_TMP_O2", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 22/06/2019 --  
    //-- edit 26/07/2019 -- เพิ่ม row_no
    public void MngTmp2OgcO2(string YYYYMM)
    {
        string SQL;
        try
        {
            if (YYYYMM != "")
            {
                //-- ลบข้อมูลเก่าออกก่อน 
                SQL = "DELETE FROM O_OGC_O2  " +
                     " WHERE TO_CHAR(SDATE,'YYYYMM') = '" + YYYYMM + "' ";
                ExecuteSQL(SQL);


                SQL = " INSERT INTO  O_OGC_O2 " +
                    " (O2_NAME,SDATE,SAMPLE_NO,O2, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY,ROW_NO) " +
                    " (SELECT O2_NAME,SDATE,SAMPLE_NO,O2, CREATED_DATE, CREATED_BY, MODIFIED_DATE, MODIFIED_BY,ROW_NO " +
                    "  FROM O_TMP_O2 " +
                    " WHERE TO_CHAR(SDATE,'YYYYMM') = '" + YYYYMM + "') ";
                ExecuteSQL(SQL);

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }




    // -----------------------------------------------------------------------------------------------------------------------------------

    //-- edit 25/06/2019 --
    public DataTable SearchOgcH2S(string sName, string FromDate = "", string ToDate = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "H2S_NAME", sName, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "SDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);


            sql = " SELECT *  FROM O_OGC_H2S ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY ROW_NO, H2S_NAME, SDATE ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 25/06/2019 --
    public DataTable SearchOgcHC(string sName, string FromDate="",  string ToDate="", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "HC_NAME", sName, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "SDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);


            sql = " SELECT *  FROM O_OGC_HC ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY ROW_NO,HC_NAME, SDATE ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 25/06/2019 --
    public DataTable SearchOgcHG(string sName, string FromDate = "", string ToDate = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "HG_NAME", sName, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "SDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);


            sql = " SELECT *  FROM O_OGC_HG ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY ROW_NO,HG_NAME, SDATE ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 25/06/2019 --
    public DataTable SearchOgcO2(string sName, string FromDate = "", string ToDate = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "O2_NAME", sName, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "SDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);


            sql = " SELECT *  FROM O_OGC_O2 ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY ROW_NO,O2_NAME, SDATE ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 26/06/2019 ---
    public void MngTmpOffshoreDaily(int op, string ID, string rDate, string FID, object C1 = null, object C2 = null, object C3 = null, object IC4 = null, object NC4 = null, object IC5 = null, object NC5 = null, object C6 = null, object C7 = null
                                 , object CO2 = null, object N2 = null, object GHV = null, object SG = null, object H2O = null, object HG = null, object H2S = null, string userName = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";

        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";

            if (userName == "") userName = Utility.ToString(System.Web.HttpContext.Current.Session["USER_NAME"]);

            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "ID", ID, DBUTIL.FieldTypes.ftNumeric);
                AddCriteria(ref Criteria, "FID", FID, DBUTIL.FieldTypes.ftText);
                AddCriteria(ref Criteria, "RDATE", Utility.AppDateValue(rDate), DBUTIL.FieldTypes.ftDate);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "FID", FID.Trim(), DBUTIL.FieldTypes.ftText);//-- EDIT 18/10/2019 -- FID.Trim()
                    AddSQL(op, ref SQL1, ref SQL2, "RDATE", Utility.AppDateValue(rDate), DBUTIL.FieldTypes.ftDate);
                    AddSQL(op, ref SQL1, ref SQL2, "CREATED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                    AddSQL(op, ref SQL1, ref SQL2, "CREATED_BY", userName, DBUTIL.FieldTypes.ftText);

                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }


                AddSQL2(op, ref SQL1, ref SQL2, "C1", Utility.FormatNumNoComma(C1, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C2", Utility.FormatNumNoComma(C2, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C3", Utility.FormatNumNoComma(C3, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "IC4", Utility.FormatNumNoComma(IC4, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "NC4", Utility.FormatNumNoComma(NC4, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "IC5", Utility.FormatNumNoComma(IC5, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "NC5", Utility.FormatNumNoComma(NC5, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C6", Utility.FormatNumNoComma(C6, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C7", Utility.FormatNumNoComma(C7, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "CO2", Utility.FormatNumNoComma(CO2, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "N2", Utility.FormatNumNoComma(N2, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "GHV", Utility.FormatNumNoComma(GHV, 3), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "SG", Utility.FormatNumNoComma(SG, 4), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "H2O", Utility.FormatNumNoComma(H2O, 4), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "HG", Utility.FormatNumNoComma(HG, 4), DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "H2S", Utility.FormatNumNoComma(H2S, 4), DBUTIL.FieldTypes.ftText);
 
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_BY", userName, DBUTIL.FieldTypes.ftText);


            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_TMP_OFFSHORE_DAILY ", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 26/06/2019 ---
    public void MngTmp2OffshoreDailyUpdate(string FID, string YYMM)
    {
        string SQL;
        string nID = "";
        try
        {
            if (FID != "" & YYMM != "")
            {
                //-- delete ข้อมูลเก่า
                SQL = "DELETE FROM OFFSHORE_DAILY_UPDATE " +
                " WHERE FID='" + FID +"' AND TO_CHAR(RDATE,'YYYYMM')= '"+ YYMM+"' ";
                ExecuteSQL(SQL);


                //-- insert data ใน OFFSHORE_DAILY_UPDATE
                nID = GenerateID("OFFSHORE_DAILY_UPDATE", "ID").ToString();

                SQL = "INSERT INTO OFFSHORE_DAILY_UPDATE " +
                    " (ID, RDATE, FID, C1, C2, C3, IC4, NC4, IC5, NC5, C6, C7, CO2, N2, GHV, SG, H2O, HG, H2S " +
                    "  , CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE  ) " +
                    " (SELECT " + nID + "+ROW_NUMBER() OVER(ORDER BY RDATE ASC), RDATE, FID, C1, C2, C3, IC4, NC4, IC5, NC5, C6, C7, CO2, N2, GHV, SG, H2O, HG, H2S " +
                    "  , CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE " +
                    " FROM O_TMP_OFFSHORE_DAILY WHERE FID = '" + FID + "'  ) ";
                ExecuteSQL(SQL);


            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }
    }


    // -----------------------------------------------------------------------------------------------------------------------------------


    // -- Add 24/04/2562
    public DataTable SearchOffshoreDailyUpdate(String SiteID, String FID, String FromDate = "", String ToDate = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        String criteria2 = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "S.SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "D.FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "D.RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            AddCriteriaRange(ref criteria2, "ADATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            sql = " SELECT DY.ADATE, A.* " +
               " FROM O_DIM_DATE DY LEFT OUTER JOIN " +
               " ( " +
               "   SELECT D.*, S.SITE_ID  " +
               " ,CASE WHEN is_number(D.C1)=1  AND is_number(D.C2)=1 AND is_number(D.C3)=1 AND is_number(D.IC4)=1 AND is_number(D.NC4)=1  " +
               "   AND is_number(D.IC5)=1 AND is_number(D.NC5)=1 AND is_number(D.C6)=1 AND is_number(D.C7)=1 AND is_number(D.CO2)=1 AND is_number(D.N2)=1  " +
               " THEN to_number3(D.C1)+to_number3(D.C2)+to_number3(D.C3) +to_number3(D.IC4)+to_number3(D.NC4) " +
               "  +to_number3(D.IC5) +to_number3(D.NC5)+to_number3(D.C6)+to_number3(D.C7)+to_number3(D.CO2) +to_number3(D.N2)  " +
               " ELSE NULL END AS SUM_COMPO   ";

            sql += " FROM OFFSHORE_DAILY_UPDATE D INNER JOIN O_OFFSHORE_FID S ON D.FID = S.FID ";


            if (criteria != "") { sql += " WHERE " + criteria; }

            sql += ") A  ON DY.ADATE=A.RDATE  ";
            if (criteria2 != "") { sql += " WHERE " + criteria2; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY DY.ADATE ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 27/06/2019 --
    public DataTable SearchOffshoreDailyUpdateAVG(String SiteID, String FID, String FromDate = "", String ToDate = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "D.FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            //--- 31/07/2018 มีการเพิ่ม function ใน ORACLE -> is_number(), to_number3(), to_number4(), to_numbernull3(), to_numbernull4()

            sql = "SELECT S.SITE_ID, D.FID,   " +
             "   ROUND(AVG(to_numbernull3(C1)), 3) C1,  ROUND(AVG(to_numbernull3(C2)), 3) C2,   ROUND(AVG(to_numbernull3(C3)), 3) C3,   " +
             "   ROUND(AVG(to_numbernull3(IC4)), 3) IC4,  ROUND(AVG(to_numbernull3(NC4)), 3) NC4,  ROUND(AVG(to_numbernull3(IC5)), 3) IC5,   " +
             "   ROUND(AVG(to_numbernull3(NC5)), 3) NC5,  ROUND(AVG(to_numbernull3(C6)), 3) C6,   ROUND(AVG(to_numbernull3(C7)), 3) C7,   " +
             "   ROUND(AVG(to_numbernull3(CO2)), 3) CO2, ROUND(AVG(to_numbernull3(N2)), 3) N2,   ROUND(AVG(to_numbernull3(GHV)), 3) GHV,  " +
             "   ROUND(AVG(to_numbernull4(SG)), 4) SG, ROUND(AVG(to_numbernull4(H2O)), 4) H2O,  ROUND(AVG(to_numbernull4(HG)), 4) HG,  " +
             "   ROUND(AVG(to_numbernull4(H2S)), 4) H2S,  " +
             " ROUND0(AVG(to_numbernull3(C1)), 3) + ROUND0(AVG(to_numbernull3(C2)), 3) + ROUND0(AVG(to_numbernull3(C3)), 3) + ROUND0(AVG(to_numbernull3(IC4)), 3) + ROUND0(AVG(to_numbernull3(NC4)), 3) " +
             " + ROUND0(AVG(to_numbernull3(IC5)), 3) + ROUND0(AVG(to_numbernull3(NC5)), 3) + ROUND0(AVG(to_numbernull3(C6)), 3) + ROUND0(AVG(to_numbernull3(C7)), 3) + ROUND0(AVG(to_numbernull3(CO2)), 3) " +
             " + ROUND0(AVG(to_numbernull3(N2)), 3)  AS SUM_COMPO " +
             " FROM OFFSHORE_DAILY_UPDATE D INNER JOIN O_OFFSHORE_FID S ON D.FID = S.FID ";


            if (criteria != "") { sql += " WHERE " + criteria; }

            sql += " GROUP BY S.SITE_ID, D.FID ";


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 27/06/2019 --
    public DataTable SearchOffshoreDailyUpdateMIN(String SiteID, String FID, String FromDate = "", String ToDate = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "D.FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            //--- 31/07/2018 มีการเพิ่ม function ใน ORACLE -> is_number(), to_number3(), to_number4(), to_numbernull3(), to_numbernull4()

            sql = "SELECT S.SITE_ID, D.FID,   " +
             "   ROUND(MIN(to_numbernull3(C1)), 3) C1,  ROUND(MIN(to_numbernull3(C2)), 3) C2,   ROUND(MIN(to_numbernull3(C3)), 3) C3,   " +
             "   ROUND(MIN(to_numbernull3(IC4)), 3) IC4,  ROUND(MIN(to_numbernull3(NC4)), 3) NC4,  ROUND(MIN(to_numbernull3(IC5)), 3) IC5,   " +
             "   ROUND(MIN(to_numbernull3(NC5)), 3) NC5,  ROUND(MIN(to_numbernull3(C6)), 3) C6,   ROUND(MIN(to_numbernull3(C7)), 3) C7,   " +
             "   ROUND(MIN(to_numbernull3(CO2)), 3) CO2, ROUND(MIN(to_numbernull3(N2)), 3) N2,   ROUND(MIN(to_numbernull3(GHV)), 3) GHV,  " +
             "   ROUND(MIN(to_numbernull4(SG)), 4) SG, ROUND(MIN(to_numbernull4(H2O)), 4) H2O,  ROUND(MIN(to_numbernull4(HG)), 4) HG,  " +
             "   ROUND(MIN(to_numbernull4(H2S)), 4) H2S,  " +
             " NULL  AS SUM_COMPO " +
             " FROM OFFSHORE_DAILY_UPDATE D INNER JOIN O_OFFSHORE_FID S ON D.FID = S.FID ";


            if (criteria != "") { sql += " WHERE " + criteria; }

            sql += " GROUP BY S.SITE_ID, D.FID ";


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 27/06/2019 --
    public DataTable SearchOffshoreDailyUpdateMAX(String SiteID, String FID, String FromDate = "", String ToDate = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "D.FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            //--- 31/07/2018 มีการเพิ่ม function ใน ORACLE -> is_number(), to_number3(), to_number4(), to_numbernull3(), to_numbernull4()

            sql = "SELECT S.SITE_ID, D.FID,   " +
             "   ROUND(MAX(to_numbernull3(C1)), 3) C1,  ROUND(MAX(to_numbernull3(C2)), 3) C2,   ROUND(MAX(to_numbernull3(C3)), 3) C3,   " +
             "   ROUND(MAX(to_numbernull3(IC4)), 3) IC4,  ROUND(MAX(to_numbernull3(NC4)), 3) NC4,  ROUND(MAX(to_numbernull3(IC5)), 3) IC5,   " +
             "   ROUND(MAX(to_numbernull3(NC5)), 3) NC5,  ROUND(MAX(to_numbernull3(C6)), 3) C6,   ROUND(MAX(to_numbernull3(C7)), 3) C7,   " +
             "   ROUND(MAX(to_numbernull3(CO2)), 3) CO2, ROUND(MAX(to_numbernull3(N2)), 3) N2,   ROUND(MAX(to_numbernull3(GHV)), 3) GHV,  " +
             "   ROUND(MAX(to_numbernull4(SG)), 4) SG, ROUND(MAX(to_numbernull4(H2O)), 4) H2O,  ROUND(MAX(to_numbernull4(HG)), 4) HG,  " +
             "   ROUND(MAX(to_numbernull4(H2S)), 4) H2S,  " +
             " NULL  AS SUM_COMPO " +
             " FROM OFFSHORE_DAILY_UPDATE D INNER JOIN O_OFFSHORE_FID S ON D.FID = S.FID ";


            if (criteria != "") { sql += " WHERE " + criteria; }

            sql += " GROUP BY S.SITE_ID, D.FID ";


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    // -----------------------------------------------------------------------------------------------------------------------------------

    //-- EDIT 11/08/2023 --
    public void TransferTmp2Customer()
    {
        string SQL;
        try
        {

            SQL = "MERGE INTO O_CUSTOMER A " +
" USING  O_TMP_CUSTOMER  B " +
" ON (A.PERMANENT_CODE=B.PERMANENT_CODE) " +
" WHEN MATCHED THEN  " +
"   UPDATE SET A.STATUS_CL=B.STATUS_CL, A.NAME_ABBR=B.NAME_ABBR, A.NAME_FULL=B.NAME_FULL, " +
"   A.REGION=B.REGION, A.TYPE=B.TYPE, A.SUB_TYPE=B.SUB_TYPE, A.BV_ZONE=B.BV_ZONE, A.BV_VALVE=B.BV_VALVE, " +
"   A.MODIFIED_DATE=B.CREATED_DATE,A.MODIFIED_BY=B.CREATED_BY " +
"   WHERE ( A.STATUS_CL<>B.STATUS_CL OR A.NAME_ABBR<>B.NAME_ABBR OR A.NAME_FULL<>B.NAME_FULL OR " +
"   A.REGION<>B.REGION OR A.TYPE<>B.TYPE OR A.SUB_TYPE<>B.SUB_TYPE OR A.BV_ZONE<>B.BV_ZONE OR A.BV_VALVE<>B.BV_VALVE ) " +
" WHEN NOT MATCHED THEN " +
"   INSERT (A.PERMANENT_CODE,A.STATUS_CL,A.NAME_ABBR,A.NAME_FULL,A.REGION,A.TYPE,A.SUB_TYPE,A.BV_ZONE,A.BV_VALVE,A.CREATED_DATE,A.CREATED_BY,A.MODIFIED_DATE,A.MODIFIED_BY) " +
"   VALUES (B.PERMANENT_CODE,B.STATUS_CL,B.NAME_ABBR,B.NAME_FULL,B.REGION,B.TYPE,B.SUB_TYPE,B.BV_ZONE,B.BV_VALVE,B.CREATED_DATE,B.CREATED_BY,B.CREATED_DATE,B.CREATED_BY) ";
            ExecuteSQL(SQL);


        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }
    }

    //---------------------------------------------------------------------------------------------
    #endregion


    // ================================================================================================================================
    #region PMISHS

    //-- aor edit 25/07/2018 --
    public void MngVwArchDayMoisture(int op, object RDate, object Name, object TagName, object Value, object Unit, string AlertFlag=null, string userName = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            if (userName == "") userName = Utility.ToString(System.Web.HttpContext.Current.Session["USER_NAME"]);


            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "RDATE", RDate, DBUTIL.FieldTypes.ftDate);
                Project.dal.AddCriteria(ref Criteria, "TAGNAME", TagName, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "RDATE", RDate, DBUTIL.FieldTypes.ftDate);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "TAGNAME", Utility.ToString(TagName).Trim(), DBUTIL.FieldTypes.ftText);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "CREATED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "CREATED_BY", userName, DBUTIL.FieldTypes.ftText);

                }
                else {
                    op = DBUTIL.opUPDATE;
                }

                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "NAME", Utility.ToString(Name).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "VALUE", Utility.FormatNumNoComma(Value,2), DBUTIL.FieldTypes.ftNumeric);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "UNIT", Unit, DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ALERT_FLAG", AlertFlag, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "CHONBURI_VW_ARCH_DAY_MOISTURE", Criteria, timeStamp: false);

                try
                {
                    ExecuteSQL(SQL);
                }
                catch
                {
                    //--- 13/08/2018 --- เกิด error เพราะมี date, name ซ้ำกัน 
                    string uSQL = " UPDATE CHONBURI_VW_ARCH_DAY_MOISTURE SET VALUE=" + Value + " WHERE TAGNAME='" + TagName + "' AND RDATE = TO_DATE('" + Utility.FormatDate(Convert.ToDateTime(RDate), "YYYYMMDD") + "','YYYYMMDD')";
                    ExecuteSQL(uSQL);
                }


            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 23/08/2018 --
    public void MngVwArchDayMoistureAlertFlag( object RDate, object TagName, string AlertFlag = null)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        int op = DBUTIL.opUPDATE;
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
  
                Project.dal.AddCriteria(ref Criteria, "RDATE", RDate, DBUTIL.FieldTypes.ftDate);
                Project.dal.AddCriteria(ref Criteria, "TAGNAME", TagName, DBUTIL.FieldTypes.ftText);
         
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ALERT_FLAG", AlertFlag, DBUTIL.FieldTypes.ftText);

        

            if ( Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "CHONBURI_VW_ARCH_DAY_MOISTURE", Criteria, timeStamp: false);

                try
                {
                    ExecuteSQL(SQL);
                }
                catch
                {
 
                }


            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 25/07/2018 --
    public void MngVwArchHourMoisture(int op, object RDate, object Name, object TagName, object Value, object Unit, string userName = "")
    {
        string SQL = "";
        string SQL1, SQL2, Criteria = "";
        try
        {
            if (userName == "") userName = Utility.ToString(System.Web.HttpContext.Current.Session["USER_NAME"]);

            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "RDATE", RDate, DBUTIL.FieldTypes.ftDateTime);
                Project.dal.AddCriteria(ref Criteria, "TAGNAME", TagName, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "RDATE", RDate, DBUTIL.FieldTypes.ftDateTime);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "TAGNAME", Utility.ToString(TagName).Trim(), DBUTIL.FieldTypes.ftText);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "CREATED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "CREATED_BY", userName , DBUTIL.FieldTypes.ftText);

                }
                else {
                    op = DBUTIL.opUPDATE;
                }

                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "NAME", Utility.ToString(Name).Trim(), DBUTIL.FieldTypes.ftText);
               // Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "VALUE", Utility.FormatNumNoComma(Value,2), DBUTIL.FieldTypes.ftNumeric);
               //-- EDIT 04/10/2022 AOR เปลี่ยนเป็นทศนิยม 6 ตำแหน่งตามที่แสดงในหน้าจอ (เดิมใช้ 2 ตำแหน่งมาตั้งแต่ปี 2018) 
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "VALUE", Utility.FormatNumNoComma(Value, 6), DBUTIL.FieldTypes.ftNumeric);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "UNIT", Unit, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "CHONBURI_VW_ARCH_HOUR_MOISTURE", Criteria, timeStamp: false);

                try
                {
                    ExecuteSQL(SQL);
                }
                catch
                {
                    //--- 13/08/2018 --- เกิด error เพราะมี date, name ซ้ำกัน 
                    string uSQL = " UPDATE CHONBURI_VW_ARCH_HOUR_MOISTURE SET VALUE=" + Value + " WHERE TAGNAME='" + TagName + "' AND RDATE = TO_DATE('" + Utility.FormatDate(Convert.ToDateTime(RDate), "YYYYMMDD HH:MI") + "','YYYYMMDD HH24:MI')";
                    ExecuteSQL(uSQL);
                }

                
            }


        }
        catch (Exception ex)
        {
            throw ex;

        }
    }



    public DataTable SearchVwArchHourMoisture(String Name = "", String FromDate = "", String ToDate = "", String orderSQL = "", String OtherCriteria = "")
    {                
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "NAME", Name, DBUTIL.FieldTypes.ftText);
            //--- time --- //-- EDIT 18/09/2019 --->? Value = 10-มี.ค.-19 มีความยาว 11
            if (ToDate.Length > 11) //-- พิจารณาเวลาด้วย
            {
                AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDateTime);
            }
            else
            {
                //-- ระบบนี้การแสดงข้อมูลรายชั่วโมงของวัน จะเริ่มจากเวลา 1:00 ไปถึง 0:00 ของวันถัดไป เนื่องจากเป็นการเก็บข้อมูลแบบ accumulate
                //-- ส่งเงื่อนไขมาเป็น วันที่ 12/07/2018
                //-- เช่น ต้องการดูข้อมูลวันที่ 12/07/2018 จะต้องดึงข้อมูล 12/07/2018 1:00 ถึง 13/07/2018 0:00
                DateTime FromDateTime = Convert.ToDateTime(Utility.AppDateValue(FromDate)).AddMinutes(1);
                DateTime ToDateTime = Convert.ToDateTime(Utility.AppDateValue(ToDate)).AddDays(1);
                
                AddCriteriaRange(ref criteria, "RDATE", FromDateTime, ToDateTime, DBUTIL.FieldTypes.ftDateTime);
            }

            sql = "SELECT * FROM CHONBURI_VW_ARCH_HOUR_MOISTURE ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY NAME, RDATE";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 12/07/2018 --
    public String GetAverageVwArchHourMoisture(String Name = "", String FromDate = "", String ToDate = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        DataTable DT = null;
        String result = "";

        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "NAME", Name, DBUTIL.FieldTypes.ftText);
            //--- time --- //-- EDIT 18/09/2019 --->? Value = 10-มี.ค.-19 มีความยาว 11
            if (ToDate.Length > 11) //-- พิจารณาเวลาด้วย
            {
                AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDateTime);
            }
            else
            {
                //-- ระบบนี้การแสดงข้อมูลรายชั่วโมงของวัน จะเริ่มจากเวลา 1:00 ไปถึง 0:00 ของวันถัดไป เนื่องจากเป็นการเก็บข้อมูลแบบ accumulate
                //-- ส่งเงื่อนไขมาเป็น วันที่ 12/07/2018
                //-- เช่น ต้องการดูข้อมูลวันที่ 12/07/2018 จะต้องดึงข้อมูล 12/07/2018 1:00 ถึง 13/07/2018 0:00
                DateTime FromDateTime = Convert.ToDateTime(Utility.AppDateValue(FromDate)).AddMinutes(1);
                DateTime ToDateTime = Convert.ToDateTime(Utility.AppDateValue(ToDate)).AddDays(1);
                AddCriteriaRange(ref criteria, "RDATE", FromDateTime, ToDateTime, DBUTIL.FieldTypes.ftDateTime);
            }
            

            sql = "SELECT ROUND(NVL(AVG(VALUE),0),6) AS AVG_VALUE FROM CHONBURI_VW_ARCH_HOUR_MOISTURE " +
                " WHERE VALUE > 0 ";   //-- คำนวณค่าเฉลี่ย โดยตัด 0 กับ -1 ออกก่อน
            if (criteria != "") { sql += " AND " + criteria; }


            DT = QueryData(sql);
            if (DT.Rows.Count > 0) result = Utility.ToString(DT.Rows[0]["AVG_VALUE"]);

            return result;

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            Utility.ClearObject(ref DT);
        }
    }


    //-- aor edit 17/07/2018 --
    public String GetAlertVwArchHourMoisture(String Name = "", String FromDate = "", String ToDate = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        DataTable DT = null;
        String result = "";
        int cnt = 0;
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "NAME", Name, DBUTIL.FieldTypes.ftText);
            ////-- การค้นหา จะค้นหาย้อนหลังไป 3 ชั่วโมง เผื่อกรณี 3 ชั่วโมงก่อนวันที่เลือกเป็น ค่าซ้ำ แล้ว ชั่วโมงสุดท้ายข้ามวันเป็น ค่าซ้ำ ครบ 4 ครั้งพอดี
            //DateTime fDate = Convert.ToDateTime(Utility.AppDateValue(FromDate)).AddHours(-3);
            //FromDate = Utility.AppFormatDateTime(fDate);
            //-- edit 03/07/2023 -- ให้พิจารณาเฉพาะวันที่กำหนด ไม่ต้องดูย้อนหลังของวันก่อนหน้า 

            //--- time --- //-- EDIT 18/09/2019 --->? Value = 10-มี.ค.-19 มีความยาว 11
            if (ToDate.Length > 11) //-- พิจารณาเวลาด้วย
            {
                AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDateTime);
            }
            else
            {
                //-- ระบบนี้การแสดงข้อมูลรายชั่วโมงของวัน จะเริ่มจากเวลา 1:00 ไปถึง 0:00 ของวันถัดไป เนื่องจากเป็นการเก็บข้อมูลแบบ accumulate
                //-- ส่งเงื่อนไขมาเป็น วันที่ 12/07/2018
                //-- เช่น ต้องการดูข้อมูลวันที่ 12/07/2018 จะต้องดึงข้อมูล 12/07/2018 1:00 ถึง 13/07/2018 0:00
                DateTime FromDateTime = Convert.ToDateTime(Utility.AppDateValue(FromDate)).AddMinutes(1);
                DateTime ToDateTime = Convert.ToDateTime(Utility.AppDateValue(ToDate)).AddDays(1);
                AddCriteriaRange(ref criteria, "RDATE", FromDateTime, ToDateTime, DBUTIL.FieldTypes.ftDateTime);
            }
                

            //ให้มีการแจ้ง alert เงื่อนไข คือ 0, -1 หรือว่าค่าซ้ำกันเกิน 3 ชั่วโมง หรือว่าเกิน 7 lb
            sql = "SELECT COUNT(*) AS CNT FROM CHONBURI_VW_ARCH_HOUR_MOISTURE " +
                   " WHERE ( VALUE<=0 OR VALUE >7) ";  
            if (criteria != "") { sql += " AND " + criteria; }

            DT = QueryData(sql);
            if (DT.Rows.Count > 0) cnt = Utility.ToInt(DT.Rows[0]["CNT"]);
            if (cnt > 0)
            {
                result = "Y";
            }
            else
            {
                //ค้นหาว่าค่าซ้ำกันเกิน 3 ชั่วโมง  
                sql = " SELECT VALUE, COUNT(*) AS CNT FROM CHONBURI_VW_ARCH_HOUR_MOISTURE ";
                if (criteria != "") { sql += " WHERE " + criteria; }
                sql += " GROUP BY VALUE HAVING COUNT(*)>3 ";
                DataTable DTc = QueryData(sql);
                if (DTc.Rows.Count > 0) cnt = Utility.ToInt(DTc.Rows[0]["CNT"]);
                if ( cnt > 3 )
                {
                    //-- ตรวจสอบว่ามีค่าซ้ำกัน ติดกัน > 3 รายการหรือไม่

                    String PrevData1 = "A", PrevData2 = "B", PrevData3 = "C", CurrentData="D";
                    DataTable DTh = SearchVwArchHourMoisture(Name, FromDate, ToDate);
                    foreach (DataRow DRh in DTh.Rows)
                    {
                        CurrentData = Utility.ToString(DRh["VALUE"]);
                        if (CurrentData == PrevData3 && CurrentData == PrevData2 && CurrentData == PrevData1)
                        {
                            result = "Y";
                            break;
                        }
                        else
                        {
                            PrevData1 = PrevData2; PrevData2 = PrevData3; PrevData3 = CurrentData;
                        }
                    }
                }
                Utility.ClearObject(ref DTc);
            }
 

            return result;

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            Utility.ClearObject(ref DT);
        }
    }


    //-- aor edit 25/07/2018 --
    public void MngVwArchDayFlowrate(int op, object RDate, object Name, object TagName, object Value, object Unit, string AlertFlag=null, string userName = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            if (userName == "") userName = Utility.ToString(System.Web.HttpContext.Current.Session["USER_NAME"]);

            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "RDATE", RDate, DBUTIL.FieldTypes.ftDate);
                Project.dal.AddCriteria(ref Criteria, "TAGNAME", TagName, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "RDATE", RDate, DBUTIL.FieldTypes.ftDate);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "TAGNAME", Utility.ToString(TagName).Trim(), DBUTIL.FieldTypes.ftText);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "CREATED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "CREATED_BY", userName, DBUTIL.FieldTypes.ftText);

                }
                else {
                    op = DBUTIL.opUPDATE;
                }

                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "NAME", Utility.ToString(Name).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "VALUE", Utility.FormatNumNoComma(Value,3), DBUTIL.FieldTypes.ftNumeric);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "UNIT", Unit, DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ALERT_FLAG", AlertFlag, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "CHONBURI_VW_ARCH_DAY_FLOWRATE", Criteria, timeStamp: false);

                try
                {
                    ExecuteSQL(SQL);
                }
                catch
                {
                    //--- 13/08/2018 --- เกิด error เพราะมี date, name ซ้ำกัน 
                    string uSQL = " UPDATE CHONBURI_VW_ARCH_DAY_FLOWRATE SET VALUE=" + Value + " WHERE TAGNAME='" + TagName + "' AND RDATE = TO_DATE('" + Utility.FormatDate(Convert.ToDateTime(RDate), "YYYYMMDD") + "','YYYYMMDD')";
                    ExecuteSQL(uSQL);
                }


            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 23/08/2018 --
    public void MngVwArchDayFlowrateAlertFlag( object RDate, object TagName, string AlertFlag = null)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        int op = DBUTIL.opUPDATE;
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
  
                Project.dal.AddCriteria(ref Criteria, "RDATE", RDate, DBUTIL.FieldTypes.ftDate);
                Project.dal.AddCriteria(ref Criteria, "TAGNAME", TagName, DBUTIL.FieldTypes.ftText);
 
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ALERT_FLAG", AlertFlag, DBUTIL.FieldTypes.ftText);


            if (Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "CHONBURI_VW_ARCH_DAY_FLOWRATE", Criteria, timeStamp: false);

                try
                {
                    ExecuteSQL(SQL);
                }
                catch
                {
                    
                }


            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 25/07/2018 --
    public void MngVwArchHourFlowrate(int op, object RDate, object Name, object TagName, object Value, object Unit, string userName = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            if (userName == "") userName = Utility.ToString(System.Web.HttpContext.Current.Session["USER_NAME"]);

            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "RDATE", RDate, DBUTIL.FieldTypes.ftDateTime);
                Project.dal.AddCriteria(ref Criteria, "TAGNAME", TagName, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "RDATE", RDate, DBUTIL.FieldTypes.ftDateTime);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "TAGNAME", Utility.ToString(TagName).Trim(), DBUTIL.FieldTypes.ftText);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "CREATED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "CREATED_BY", userName, DBUTIL.FieldTypes.ftText);

                }
                else {
                    op = DBUTIL.opUPDATE;
                }

                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "NAME", Utility.ToString(Name).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "VALUE", Utility.FormatNumNoComma(Value,3), DBUTIL.FieldTypes.ftNumeric);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "UNIT", Unit, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "CHONBURI_VW_ARCH_HOUR_FLOWRATE", Criteria, timeStamp: false);

                try
                {
                    ExecuteSQL(SQL);
                }
                catch
                {
                    //--- 13/08/2018 --- เกิด error เพราะมี date, name ซ้ำกัน 
                    string uSQL = " UPDATE CHONBURI_VW_ARCH_HOUR_FLOWRATE SET VALUE=" + Value + " WHERE TAGNAME='" + TagName + "' AND RDATE = TO_DATE('" + Utility.FormatDate(Convert.ToDateTime(RDate), "YYYYMMDD HH:MI") + "','YYYYMMDD HH24:MI')";
                    ExecuteSQL(uSQL);
                }


            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable SearchVwArchHourFlowrate(String Name = "", String FromDate = "", String ToDate = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "NAME", Name, DBUTIL.FieldTypes.ftText);
            //--- time --- //-- EDIT 18/09/2019 --->? Value = 10-มี.ค.-19 มีความยาว 11
            if (ToDate.Length > 11) //-- พิจารณาเวลาด้วย
            {
                AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDateTime);
            }
            else
            {
                //-- ระบบนี้การแสดงข้อมูลรายชั่วโมงของวัน จะเริ่มจากเวลา 1:00 ไปถึง 0:00 ของวันถัดไป เนื่องจากเป็นการเก็บข้อมูลแบบ accumulate
                //-- ส่งเงื่อนไขมาเป็น วันที่ 12/07/2018
                //-- เช่น ต้องการดูข้อมูลวันที่ 12/07/2018 จะต้องดึงข้อมูล 12/07/2018 1:00 ถึง 13/07/2018 0:00
                DateTime FromDateTime = Convert.ToDateTime(Utility.AppDateValue(FromDate)).AddMinutes(1);
                DateTime ToDateTime = Convert.ToDateTime(Utility.AppDateValue(ToDate)).AddDays(1);
                AddCriteriaRange(ref criteria, "RDATE", FromDateTime, ToDateTime, DBUTIL.FieldTypes.ftDateTime);
            }

            sql = "SELECT * FROM CHONBURI_VW_ARCH_HOUR_FLOWRATE ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY NAME, RDATE";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 12/07/2018 --
    public String GetSumVwArchHourFlowrate(String Name = "", String FromDate = "", String ToDate = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        DataTable DT = null;
        String result = "";

        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "NAME", Name, DBUTIL.FieldTypes.ftText);
            //--- time --- //-- EDIT 18/09/2019 --->? Value = 10-มี.ค.-19 มีความยาว 11
            if (ToDate.Length > 11) //-- พิจารณาเวลาด้วย
            {
                AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDateTime);
            }
            else
            {
                //-- ระบบนี้การแสดงข้อมูลรายชั่วโมงของวัน จะเริ่มจากเวลา 1:00 ไปถึง 0:00 ของวันถัดไป เนื่องจากเป็นการเก็บข้อมูลแบบ accumulate
                //-- ส่งเงื่อนไขมาเป็น วันที่ 12/07/2018
                //-- เช่น ต้องการดูข้อมูลวันที่ 12/07/2018 จะต้องดึงข้อมูล 12/07/2018 1:00 ถึง 13/07/2018 0:00
                DateTime FromDateTime = Convert.ToDateTime(Utility.AppDateValue(FromDate)).AddMinutes(1);
                DateTime ToDateTime = Convert.ToDateTime(Utility.AppDateValue(ToDate)).AddDays(1);
                AddCriteriaRange(ref criteria, "RDATE", FromDateTime, ToDateTime, DBUTIL.FieldTypes.ftDateTime);
            }
            

            sql = "SELECT NVL(SUM(VALUE),0) AS SUM_VALUE FROM CHONBURI_VW_ARCH_HOUR_FLOWRATE " +
                " WHERE VALUE > 0 ";   //-- คำนวณค่าเฉลี่ย โดยตัด 0 กับ -1 ออกก่อน
            if (criteria != "") { sql += " AND " + criteria; }


            DT = QueryData(sql);
            if (DT.Rows.Count > 0) result = Utility.ToString(DT.Rows[0]["SUM_VALUE"]);

            return result;

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            Utility.ClearObject(ref DT);
        }
    }


    //-- aor edit 17/07/2018 --
    //-- edit 02/07/2023 -- ส่งวันที่ ของ DAY_FLOWRATE-1 ไป
    public String GetAlertVwArchHourFlowrate(String Name = "", String FromDate = "", String ToDate = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        DataTable DT = null;
        String result = "";
        int cnt = 0;
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "NAME", Name, DBUTIL.FieldTypes.ftText);
            ////-- การค้นหา จะค้นหาย้อนหลังไป 5 ชั่วโมง เผื่อกรณี 5 ชั่วโมงก่อนวันที่เลือกเป็น 0 แล้ว ชั่วโมงสุดท้ายข้ามวันเป็น 0 ครบ 6 ครั้งพอดี
            //DateTime fDate = Convert.ToDateTime(Utility.AppDateValue(FromDate)).AddHours(-5);
            //FromDate = Utility.AppFormatDateTime(fDate);
            //-- edit 03/07/2023 -- ให้พิจารณาเฉพาะวันที่กำหนด ไม่ต้องดูย้อนหลังของวันก่อนหน้า 

            //--- time --- //-- EDIT 18/09/2019 --->? Value = 10-มี.ค.-19 มีความยาว 11
            if (ToDate.Length > 11) //-- พิจารณาเวลาด้วย
            {
                AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDateTime);
            }
            else
            {
                //-- ระบบนี้การแสดงข้อมูลรายชั่วโมงของวัน จะเริ่มจากเวลา 1:00 ไปถึง 0:00 ของวันถัดไป เนื่องจากเป็นการเก็บข้อมูลแบบ accumulate
                //-- ส่งเงื่อนไขมาเป็น วันที่ 12/07/2018 
                //-- เช่น ต้องการดูข้อมูลวันที่ 12/07/2018 จะต้องดึงข้อมูล 12/07/2018 1:00 ถึง 13/07/2018 0:00
                DateTime FromDateTime = Convert.ToDateTime(Utility.AppDateValue(FromDate)).AddMinutes(1);
                DateTime ToDateTime = Convert.ToDateTime(Utility.AppDateValue(ToDate)).AddDays(1);
                AddCriteriaRange(ref criteria, "RDATE", FromDateTime, ToDateTime, DBUTIL.FieldTypes.ftDateTime);
            }

            //--- ถ้าเป็น 0 ติดต่อกันเกิน >= 6 ชั่วโมง ก็จะไม่ใช้ flow นี้ (มี Alert)

            sql = "SELECT COUNT(*) AS CNT FROM CHONBURI_VW_ARCH_HOUR_FLOWRATE " +
                " WHERE VALUE=0 ";   //-- นับก่อนว่า วันนี้ มี 0 มากกว่าหรือเท่ากับ 6 ครั้งหรือไม่
            if (criteria != "") { sql += " AND " + criteria; }
            
            DT = QueryData(sql);
            if (DT.Rows.Count > 0) cnt = Utility.ToInt(DT.Rows[0]["CNT"]);
            if ( cnt >= 6 )
            {
                //-- ตรวจสอบว่ามี 0 ติดกัน >= 6 รายการหรือไม่

                String PrevData1 = "", PrevData2 = "", PrevData3 = "", PrevData4 = "", PrevData5 = "", CurrentData;
                DataTable DTh = SearchVwArchHourFlowrate(Name, FromDate, ToDate);
                foreach ( DataRow DRh in DTh.Rows)
                {
                    CurrentData = Utility.ToString(DRh["VALUE"]);
                    if (CurrentData == "0" && PrevData5 == "0" && PrevData4 == "0" && PrevData3 == "0" && PrevData2 == "0" && PrevData1 == "0")
                    {
                        result = "Y";
                        break;
                    }
                    else
                    {
                        PrevData1 = PrevData2; PrevData2 = PrevData3; PrevData3 = PrevData4;
                        PrevData4 = PrevData5; PrevData5 = CurrentData;
                    }
                }
                Utility.ClearObject(ref DTh);
            }


            return result;

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            Utility.ClearObject(ref DT);
        }
    }



    #endregion

    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region REPORT

    //=====  Monthly Report ===========================
    //-- edit 15/08/2018 --
    //*** ต้องใส่ FID เสมอ
    public DataTable SearchGqmsDailyUpdateReport(String SiteID, String FID, String FromDate = "", String ToDate = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        String criteria2 = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "S.SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "D.FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "D.RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            AddCriteriaRange(ref criteria2, "ADATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            //--- 31/07/2018 มีการเพิ่ม function ใน ORACLE -> is_number(), to_number3(), to_number4()
            //-- 23/08/2018 เพิ่ม ALERT_FLAG
            //-- 03/09/2018 ตัดบรรทัด " ,CASE WHEN D.FLOW_NAME=S.FLOW_NAME2 THEN F2.VALUE ELSE F1.VALUE END FLOW " +
            //--ตัดบรรทัด " ,CASE WHEN D.FLOW_NAME=S.FLOW_NAME2 THEN F2.ALERT_FLAG ELSE F1.ALERT_FLAG END FLOW_ALERT "+
            sql = " SELECT DY.ADATE, A.*, R.SHOW_FLAG " +
                " FROM O_DIM_DATE DY LEFT OUTER JOIN " +
                " ( " +
                "   SELECT D.*, S.SITE_ID, S.ISO_FLAG, S.H2S_FLAG, S.TOTAL_RUN, S.TOLERANCE_RUN " +
                " ,S.FLOW_NAME1, F1.VALUE AS FLOW1, S.FLOW_NAME2, F2.VALUE AS FLOW2 " +
                " ,F1.VALUE FLOW " +
                " ,B.BTUDATE, B.BTU, B.RUN " +
                " , M1.ALERT_FLAG WC_ALERT "+
                " , F1.ALERT_FLAG FLOW_ALERT "+
                " FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID " +
                "   LEFT OUTER JOIN CHONBURI_VW_ARCH_DAY_FLOWRATE F1 ON S.FLOW_NAME1 = F1.NAME AND D.RDATE = (F1.RDATE-1) " +
                "   LEFT OUTER JOIN CHONBURI_VW_ARCH_DAY_FLOWRATE F2 ON S.FLOW_NAME2 = F2.NAME AND D.RDATE = (F2.RDATE-1) " +
                "   LEFT OUTER JOIN CHONBURI_VW_ARCH_DAY_MOISTURE M1 ON D.OMA_NAME = M1.NAME AND D.RDATE = (M1.RDATE-1) " +
                "   LEFT OUTER JOIN O_OGC_DAILY_BTU B ON D.FID=B.FID AND D.RDATE= (B.BTUDATE-1) ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            sql += ") A  ON DY.ADATE=A.RDATE  ";
            sql += " LEFT OUTER JOIN O_RPT_MONTHLY_DATE R ON DY.ADATE=R.RDATE AND R.FID='" + FID + "' ";

            if (criteria2 != "") { sql += " WHERE " + criteria2; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY DY.ADATE ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 15/08/2018 --
    public DataTable SearchRptMonthlyDate(String FID, String FromDate = "", String ToDate = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            sql = "SELECT * FROM O_RPT_MONTHLY_DATE ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY FID, RDATE";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 15/08/2018 --
    //-- showflag เช่น ที่ไม่แสดง = N
    public void MngRptMonthlyDate(int op,  String FID = "", String Rdate = "", String ShowFlag = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "FID", FID, DBUTIL.FieldTypes.ftText);
                AddCriteria(ref Criteria, "RDATE", Utility.AppDateValue(Rdate), DBUTIL.FieldTypes.ftDate);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "FID", FID.Trim(), DBUTIL.FieldTypes.ftText); //-- EDIT 18/10/2019 -- FID.Trim()
                    AddSQL(op, ref SQL1, ref SQL2, "RDATE", Utility.AppDateValue(Rdate), DBUTIL.FieldTypes.ftDate);
                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "SHOW_FLAG", ShowFlag, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_RPT_MONTHLY_DATE", Criteria);
                ExecuteSQL(SQL);

            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    //-- aor edit 15/08/2018 --
    public DataTable SearchRptMonthly(String ID, String FID, String MM, String YY, String rptType, String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "ID", ID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "MM", MM, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "YY", YY, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "RPT_TYPE", rptType, DBUTIL.FieldTypes.ftText);

            sql = "SELECT O.*, NVL(REMARK1,'')||NVL(REMARK2,'') AS REMARK " +
                " , U1.USER_DESC AS REPORT_BY_NAME, U1.SIGN_FILENAME AS REPORT_BY_SIGN " +
                " , U2.USER_DESC AS APPROVE_BY_NAME, U2.SIGN_FILENAME AS APPROVE_BY_SIGN " +
                "FROM O_RPT_MONTHLY O LEFT OUTER JOIN O_SYS_USERS U1 ON O.REPORT_BY=U1.USER_NAME " +
                "  LEFT OUTER JOIN O_SYS_USERS U2 ON O.APPROVE_BY = U2.USER_NAME";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY FID, YY, MM, RPT_TYPE";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    //-- aor edit 15/08/2018 --
    //-- EDIT 22/07/2019 ---  , String SignedFlag = "", String isoMinMax = ""
    //-- EDIT 30/07/2019 -- , String Remark2 = ""
    public void MngRptMonthly(int op, ref String ID, String FID = "", String MM = "", String YY = "", String rptType = "", String Remark1 = "", String isoAccredit = "", String ReportBy = "", String ApproveBy = "", String SignedFlag = "", String isoMinMax = "", String Remark2 = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        string remark1 = "", remark2 = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "ID", ID, DBUTIL.FieldTypes.ftNumeric);
                AddCriteria(ref Criteria, "FID", FID, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    ID = GenerateID("O_RPT_MONTHLY", "ID").ToString();
                    AddSQL(op, ref SQL1, ref SQL2, "ID", ID, DBUTIL.FieldTypes.ftNumeric);

                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "FID", FID, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "MM", MM, DBUTIL.FieldTypes.ftNumeric);
                AddSQL2(op, ref SQL1, ref SQL2, "YY", YY, DBUTIL.FieldTypes.ftNumeric);
                AddSQL2(op, ref SQL1, ref SQL2, "RPT_TYPE", rptType, DBUTIL.FieldTypes.ftText);

                //if ( Remark.Length > 3995 )
                //{
                //    remark1 = Utility.Left(Remark, 3995);
                //    remark2 = Utility.Mid(Remark, 3995);
                //}
                //else
                //{
                //    remark1 = Remark;
                //}

                //AddSQL2(op, ref SQL1, ref SQL2, "REMARK1", remark1 , DBUTIL.FieldTypes.ftText);
                //AddSQL2(op, ref SQL1, ref SQL2, "REMARK2", remark2, DBUTIL.FieldTypes.ftText);

                AddSQL2(op, ref SQL1, ref SQL2, "REMARK1", Remark1 , DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "REMARK2", Remark2, DBUTIL.FieldTypes.ftText);

                AddSQL2(op, ref SQL1, ref SQL2, "ISO_ACCREDIT", isoAccredit, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "REPORT_BY", ReportBy, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "APPROVE_BY", ApproveBy, DBUTIL.FieldTypes.ftText);

                AddSQL2(op, ref SQL1, ref SQL2, "SIGNED_FLAG", SignedFlag, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "ISO_MINMAX", isoMinMax, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_RPT_MONTHLY", Criteria);
                ExecuteSQL(SQL);

            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }



    //-- aor edit 16/08/2018 --
    //-- EDIT 27/6/2019 -- เพิ่ม String TTYPE  (template type : 1=Onshore, 2=Offshore, 3= Spot)
    public DataTable SearchRptFidTemplate(String TTYPE, String TID, String TName, String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";

        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "T_TYPE", TTYPE, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "TID", TID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "UPPER(T_NAME)", TName.ToUpper(), DBUTIL.FieldTypes.ftText);


            sql = "SELECT * FROM O_RPT_FID_TEMPLATE ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY T_TYPE, T_NAME";
            }


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 15/08/2018 --
    //-- EDIT 27/6/2019 -- เพิ่ม String TTYPE  (template type : 1=Onshore, 2=Offshore, 3= Spot)
    public void MngRptFidTemplate(int op, ref String TID, String TName = "",String TTYPE="")
    {
        string SQL, SQL1, SQL2, Criteria = "";

        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "TID", TID, DBUTIL.FieldTypes.ftNumeric);

            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    TID = GenerateID("O_RPT_FID_TEMPLATE", "TID").ToString();
                    AddSQL(op, ref SQL1, ref SQL2, "TID", TID, DBUTIL.FieldTypes.ftNumeric);
                    AddSQL(op, ref SQL1, ref SQL2, "T_TYPE", TTYPE, DBUTIL.FieldTypes.ftNumeric);
                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "T_NAME", TName, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_RPT_FID_TEMPLATE", Criteria);
                ExecuteSQL(SQL);

                if ( op == DBUTIL.opDELETE)
                {
                    SQL = "DELETE FROM O_RPT_FID_DETAIL WHERE TID=" + TID +" ";
                    ExecuteSQL(SQL);

                }

            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    //-- aor edit 16/08/2018 --
    public DataTable SearchRptFidDetail(String TID, String FID, String Seq, String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";

        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "TID", TID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "SEQ", Seq, DBUTIL.FieldTypes.ftNumeric);

            sql = "SELECT * FROM O_RPT_FID_DETAIL ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY TID, SEQ, FID";
            }


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 15/08/2018 --
    public void MngRptFidDetail(int op, ref String TID, String FID, String Seq)
    {
        string SQL, SQL1, SQL2, Criteria = "";

        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "TID", TID, DBUTIL.FieldTypes.ftNumeric);
                AddCriteria(ref Criteria, "FID", FID, DBUTIL.FieldTypes.ftText);

            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "TID", TID, DBUTIL.FieldTypes.ftNumeric);
                    AddSQL(op, ref SQL1, ref SQL2, "FID", FID.Trim(), DBUTIL.FieldTypes.ftText); //-- EDIT 18/10/2019 -- FID.Trim()

                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "SEQ", Seq, DBUTIL.FieldTypes.ftNumeric);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_RPT_FID_DETAIL", Criteria);
                ExecuteSQL(SQL);

            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    //-- edit 01/07/2019 --
    //-- EDIT 19/07/2019 --
    //-- EDIT 17/09/2019 -- 
    //-- edit 25/09/2019 --
    //-- EDIT 14/10/2019 -- ในหน้า Report Onshore summary ให้นำข้อมูลที่ผ่านการ confirm แล้วเท่านั้นมาแสดงครับ 
    //-- edit 27/05/2024 เพิ่ม "1DAY"

    public DataTable SearchRptOnshoreSummary(String PeriodType, String FromDate , String ToDate , String TemplateList="", String SIDList="", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = "";
        String criteria = "", selectDay = "";
        String criteria2 = "", selectDay2 = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }

            AddCriteriaRange(ref criteria, "D.RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);
            AddCriteriaRange(ref criteria2, "ADATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            if (TemplateList != "")
            {
                criteria += " AND S.SITE_ID IN (SELECT DISTINCT S.SITE_ID FROM O_RPT_FID_DETAIL T INNER JOIN O_SITE_FID S ON T.FID = S.FID WHERE TID IN (" + TemplateList + ")) ";
                criteria2 += " AND S.SITE_ID IN (SELECT DISTINCT S.SITE_ID FROM O_RPT_FID_DETAIL T INNER JOIN O_SITE_FID S ON T.FID = S.FID WHERE TID IN (" + TemplateList + ")) ";
            }
            else
            {
                if (SIDList != "")
                {
                    criteria += "  AND S.SITE_ID IN ("+ SIDList+" ) ";
                    criteria2 += "  AND S.SITE_ID IN (" + SIDList + " ) ";
                }
            }
            
            switch (PeriodType) //pType ="10DAY","15DAY","ENDMTH" //-- edit 27/05/2024 เพิ่ม "1DAY"
            {
                case "1DAY":  
                    selectDay = " 'D'||TO_CHAR(D.RDATE,'DD') "; 
                    selectDay2 = " 'D'||TO_CHAR(ADATE,'DD') ";
                    break;
                case "10DAY":
                    selectDay = "CASE WHEN TO_CHAR(D.RDATE,'DD')<'11' THEN 'D1' WHEN TO_CHAR(D.RDATE,'DD')<'21' THEN 'D2' ELSE 'D3' END ";
                    selectDay2 = "CASE WHEN TO_CHAR(ADATE,'DD')<'11' THEN 'D1' WHEN TO_CHAR(ADATE,'DD')<'21' THEN 'D2' ELSE 'D3' END ";
                    break;
                case "15DAY":
                    selectDay = "CASE WHEN TO_CHAR(D.RDATE,'DD')<'16' THEN 'D4' ELSE 'D5' END ";
                    selectDay2 = "CASE WHEN TO_CHAR(ADATE,'DD')<'16' THEN 'D4' ELSE 'D5' END ";
                    break;
                case "ENDMTH":
                    selectDay = " 'D6' ";
                    selectDay2 = " 'D6' ";
                    break;
            }


            //sql = " SELECT S.SITE_ID, D.FID, " +
            //" TO_CHAR(D.RDATE,'YYYY') YY, TO_CHAR(D.RDATE,'MM') MM , " + selectDay + " DDAY," +
            //"    ROUND(AVG(to_numberv3(C1)),3) C1,  ROUND(AVG(to_numberv3(C2)),3) C2,   ROUND(AVG(to_numberv3(C3)),3) C3,   " +
            //"     ROUND(AVG(to_numberv3(IC4)),3) IC4,  ROUND(AVG(to_numberv3(NC4)),3) NC4,  ROUND(AVG(to_numberv3(IC5)),3) IC5,  " +
            //"     ROUND(AVG(to_numberv3(NC5)),3) NC5,  ROUND(AVG(to_numberv3(C6)),3) C6,    ROUND(AVG(to_numberv3(N2)),3) N2,   " +
            //"     ROUND(AVG(to_numberv3(CO2)),3) CO2,  ROUND(AVG(to_numbernull4(SG)),4) SG,   ROUND(AVG(to_numberv3(GHV)),3) GHV,  " +
            //"     ROUND(AVG(to_numberv3(NHV)),3) NHV,  ROUND(AVG(to_numberv3(WC)),2) WC, ROUND(AVG(to_numberv3(WB)),3) WB, ROUND(AVG(to_numberv3(H2S)),3) H2S,   " +
            //"   ROUND0(AVG(to_numberv3(C2)),3)+ ROUND0(AVG(to_numberv3(C3)),3)+ ROUND0(AVG(to_numberv3(IC4)),3)+ ROUND0(AVG(to_numberv3(NC4)),3)   " +
            //"  + ROUND0(AVG(to_numberv3(IC5)),3)+ ROUND0(AVG(to_numberv3(NC5)),3)+ ROUND0(AVG(to_numberv3(C6)),3)  AS SUM_C2 , " +
            //"  ROUND0(AVG(to_numberv3(CO2)),3) + ROUND0(AVG(to_numberv3(N2)),3)  AS CO2_N2    " +
            //"  , MAX(P.HG_NAME) HG_NAME, ROUND(AVG(to_numberv3(H.HG)),3) HG  " +
            //"   FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID " +
            //"   LEFT OUTER JOIN O_SPOT_UPDATE P ON D.FID=P.FID AND TO_CHAR(D.RDATE,'YYYYMM')=TO_CHAR(P.RDATE,'YYYYMM') " +
            //"   LEFT OUTER JOIN O_OGC_HG H ON P.HG_NAME = H.HG_NAME AND TO_CHAR(P.RDATE,'YYYYMM')= TO_CHAR(H.SDATE, 'YYYYMM') ";
            //if (criteria != "") { sql += " WHERE " + criteria; }
            //sql += " GROUP BY S.SITE_ID, D.FID, TO_CHAR(D.RDATE,'YYYY'),TO_CHAR(D.RDATE,'MM'), " + selectDay;
            //sql += " ORDER BY S.SITE_ID, D.FID, TO_CHAR(D.RDATE,'YYYY'), TO_CHAR(D.RDATE,'MM'), " + selectDay;

            ////-- edit 25/09/2019 -- เพิ่ม calc_wi => WI = (GHVsat/0.9826) / sqt(SG)
            //sql = "SELECT AA.SITE_ID, AA.FID,AA.YY, AA.MM,AA.DDAY " +
            //" ,C1, C2, C3, IC4, NC4, IC5, NC5, C6, N2, CO2, SG, GHV, NHV, WC, WB, H2S, SUM_C2 , CO2_N2, HG_NAME, HG, CALC_WI " +
            //" FROM " +
            //" (" +
            //" SELECT S.SITE_ID, S.FID, TO_CHAR(ADATE, 'YYYY') YY, TO_CHAR(ADATE, 'MM') MM, " + selectDay2 + "  DDAY " +
            //" FROM O_DIM_DATE DD, O_SITE_FID S ";
            //if (criteria2 != "") { sql += " WHERE " + criteria2; }
            //sql += " GROUP BY S.SITE_ID, S.FID, TO_CHAR(ADATE, 'YYYY'), TO_CHAR(ADATE, 'MM'), " + selectDay2 + " " +
            // " ) AA " +
            // " LEFT OUTER JOIN " +
            // " (" +
            // "  SELECT S.SITE_ID, S.FID, TO_CHAR(D.RDATE,'YYYY') YY, TO_CHAR(D.RDATE, 'MM') MM , " + selectDay + " DDAY " +
            // "       , ROUND(AVG(to_numberv3(C1)), 3) C1,  ROUND(AVG(to_numberv3(C2)), 3) C2,   ROUND(AVG(to_numberv3(C3)), 3) C3,        ROUND(AVG(to_numberv3(IC4)), 3) IC4,  ROUND(AVG(to_numberv3(NC4)), 3) NC4,  ROUND(AVG(to_numberv3(IC5)), 3) IC5,       ROUND(AVG(to_numberv3(NC5)), 3) NC5,  ROUND(AVG(to_numberv3(C6)), 3) C6 " +
            // "       , ROUND(AVG(to_numberv3(N2)), 3) N2,  ROUND(AVG(to_numberv3(CO2)), 3) CO2,  ROUND(AVG(to_numbernull4(SG)), 4) SG,   ROUND(AVG(to_numberv3(GHV)), 3) GHV,       ROUND(AVG(to_numberv3(NHV)), 3) NHV,  ROUND(AVG(to_numberv3(WC)), 2) WC, ROUND(AVG(to_numberv3(WB)), 3) WB " +
            // " , ROUND(AVG(to_numberv3(HS.H2S)), 3) H2S  " +
            // "       , ROUND0(AVG(to_numberv3(C2)), 3) + ROUND0(AVG(to_numberv3(C3)), 3) + ROUND0(AVG(to_numberv3(IC4)), 3) + ROUND0(AVG(to_numberv3(NC4)), 3) + ROUND0(AVG(to_numberv3(IC5)), 3) + ROUND0(AVG(to_numberv3(NC5)), 3) " +
            // "                    + ROUND0(AVG(to_numberv3(C6)), 3)  AS SUM_C2, ROUND0(AVG(to_numberv3(CO2)), 3) +ROUND0(AVG(to_numberv3(N2)), 3)  AS CO2_N2, MAX(P.HG_NAME) HG_NAME, ROUND(AVG(to_numberv3(H.HG)), 3) HG " +
            // "       , ROUND( (AVG(to_numberv3(GHV))/0.9826) /  SQRT(AVG(to_numbernull4(SG))),3)  CALC_WI" +
            // "  FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID " +
            // "  LEFT OUTER JOIN O_SPOT_UPDATE P ON D.FID = P.FID AND TO_CHAR(D.RDATE,'YYYYMM')= TO_CHAR(P.RDATE, 'YYYYMM') " +
            // "  LEFT OUTER JOIN O_OGC_HG H ON P.HG_NAME = H.HG_NAME AND TO_CHAR(P.RDATE,'YYYYMM')= TO_CHAR(H.SDATE, 'YYYYMM') " +
            // "  LEFT OUTER JOIN O_OGC_H2S HS ON P.H2S_NAME = HS.H2S_NAME AND TO_CHAR(P.RDATE,'YYYYMM')= TO_CHAR(HS.SDATE, 'YYYYMM') ";

            //if (criteria != "") { sql += " WHERE " + criteria; }
            //sql += "  GROUP BY S.SITE_ID, S.FID, TO_CHAR(D.RDATE, 'YYYY'),TO_CHAR(D.RDATE, 'MM'), " + selectDay + " " +
            //" ) BB ON AA.SITE_ID = BB.SITE_ID AND AA.YY = BB.YY AND AA.MM = BB.MM AND AA.DDAY = BB.DDAY " +
            //" ORDER BY AA.SITE_ID, AA.FID,AA.YY, AA.MM,AA.DDAY ";

            ////-- EDIT 14/10/2019 -- ในหน้า Report Onshore summary ให้นำข้อมูลที่ผ่านการ confirm แล้วเท่านั้นมาแสดงครับ 
            //sql = " SELECT AA.SITE_ID, AA.FID,AA.YY, AA.MM,AA.DDAY  " +
            //" ,DECODE(NGMODIFIED_DATE,NULL,NULL,C1) C1 ,DECODE(NGMODIFIED_DATE,NULL,NULL,C2) C2 ,DECODE(NGMODIFIED_DATE,NULL,NULL,C3) C3  " +
            //" ,DECODE(NGMODIFIED_DATE,NULL,NULL,IC4) IC4    ,DECODE(NGMODIFIED_DATE,NULL,NULL,NC4) NC4  ,DECODE(NGMODIFIED_DATE,NULL,NULL,IC5) IC5  " +
            //" ,DECODE(NGMODIFIED_DATE,NULL,NULL,NC5) NC5  ,DECODE(NGMODIFIED_DATE,NULL,NULL,C6) C6 ,DECODE(NGMODIFIED_DATE,NULL,NULL,N2) N2  " +
            //" ,DECODE(NGMODIFIED_DATE,NULL,NULL,CO2) CO2 ,DECODE(NGMODIFIED_DATE,NULL,NULL,SG) SG ,DECODE(NGMODIFIED_DATE,NULL,NULL,GHV) GHV  " +
            //" ,DECODE(NGMODIFIED_DATE,NULL,NULL,NHV) NHV ,DECODE(NGMODIFIED_DATE,NULL,NULL,WC) WC ,DECODE(NGMODIFIED_DATE,NULL,NULL,WB) WB  " +
            //" ,DECODE(NGMODIFIED_DATE,NULL,NULL,H2S) H2S  ,DECODE(NGMODIFIED_DATE,NULL,NULL,SUM_C2) SUM_C2 ,DECODE(NGMODIFIED_DATE,NULL,NULL,CO2_N2) CO2_N2  " +
            //" ,DECODE(NGMODIFIED_DATE,NULL,NULL,HG) HG ,DECODE(NGMODIFIED_DATE,NULL,NULL,CALC_WI) CALC_WI  ,HG_NAME  " +
            //" FROM    " +
            //" ( SELECT S.SITE_ID, S.FID, TO_CHAR(ADATE, 'YYYY') YY, TO_CHAR(ADATE, 'MM') MM, " + selectDay2 + "   DDAY    " +
            //" FROM O_DIM_DATE DD, O_SITE_FID S ";
            // if (criteria2 != "") { sql += " WHERE " + criteria2; }
            //sql += "  GROUP BY S.SITE_ID, S.FID, TO_CHAR(ADATE, 'YYYY'), TO_CHAR(ADATE, 'MM'), " + selectDay2 + "  " +
            //" ) AA    " +
            //" LEFT OUTER JOIN   " +
            //" (  SELECT S.SITE_ID, S.FID, TO_CHAR(D.RDATE,'YYYY') YY, TO_CHAR(D.RDATE, 'MM') MM , " + selectDay + "  DDAY " +
            //" , ROUND(AVG(to_numberv3(D.C1)), 3)  C1, ROUND(AVG(to_numberv3(D.C2)), 3) C2,   ROUND(AVG(to_numberv3(D.C3)), 3) C3,  ROUND(AVG(to_numberv3(D.IC4)), 3) IC4,  ROUND(AVG(to_numberv3(D.NC4)), 3) NC4,  ROUND(AVG(to_numberv3(D.IC5)), 3) IC5  " +
            //" , ROUND(AVG(to_numberv3(D.NC5)), 3) NC5,  ROUND(AVG(to_numberv3(D.C6)), 3) C6  , ROUND(AVG(to_numberv3(D.N2)), 3) N2,   ROUND(AVG(to_numberv3(D.CO2)), 3) CO2,  ROUND(AVG(to_numbernull4(D.SG)), 4) SG,   ROUND(AVG(to_numberv3(D.GHV)), 3) GHV  " +
            //" , ROUND(AVG(to_numberv3(D.NHV)), 3) NHV,  ROUND(AVG(to_numberv3(D.WC)), 2) WC, ROUND(AVG(to_numberv3(D.WB)), 3) WB , ROUND(AVG(to_numberv3(HS.H2S)), 3) H2S  " +
            //" , ROUND0(AVG(to_numberv3(D.C2)), 3) + ROUND0(AVG(to_numberv3(D.C3)), 3) + ROUND0(AVG(to_numberv3(D.IC4)), 3) + ROUND0(AVG(to_numberv3(D.NC4)), 3) + ROUND0(AVG(to_numberv3(D.IC5)), 3) + ROUND0(AVG(to_numberv3(D.NC5)), 3)  " +
            //"  + ROUND0(AVG(to_numberv3(D.C6)), 3)  AS SUM_C2, ROUND0(AVG(to_numberv3(D.CO2)), 3) +ROUND0(AVG(to_numberv3(D.N2)), 3)  AS CO2_N2  " +
            //" , MAX(P.HG_NAME) HG_NAME, ROUND(AVG(to_numberv3(H.HG)), 3) HG , ROUND( (AVG(to_numberv3(D.GHV))/0.9826) /  SQRT(AVG(to_numbernull4(D.SG))),3)  CALC_WI   " +
            //" , MAX(NG.MODIFIED_DATE) AS NGMODIFIED_DATE  " +
            //" FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID     " +
            //" LEFT OUTER JOIN O_SPOT_UPDATE P ON D.FID = P.FID AND TO_CHAR(D.RDATE,'YYYYMM')= TO_CHAR(P.RDATE, 'YYYYMM')     " +
            //" LEFT OUTER JOIN O_OGC_HG H ON P.HG_NAME = H.HG_NAME AND TO_CHAR(P.RDATE,'YYYYMM')= TO_CHAR(H.SDATE, 'YYYYMM')    " +
            //" LEFT OUTER JOIN O_OGC_H2S HS ON P.H2S_NAME = HS.H2S_NAME AND TO_CHAR(P.RDATE,'YYYYMM')= TO_CHAR(HS.SDATE, 'YYYYMM')    " +
            //" LEFT OUTER JOIN O_SITE_REPORT R ON D.FID=R.FID AND R.RPT_TYPE='DAILY'  " +
            //" LEFT OUTER JOIN NGBILL_DAILY_UPDATE NG ON D.RDATE=NG.RDATE AND NG.FID=R.NGBILL_RPT_NO  ";
            //if (criteria != "") { sql += " WHERE " + criteria; }
            //sql += "  GROUP BY S.SITE_ID, S.FID, TO_CHAR(D.RDATE, 'YYYY'),TO_CHAR(D.RDATE, 'MM'), " + selectDay + " " +
            //" ) BB ON AA.SITE_ID = BB.SITE_ID AND AA.YY = BB.YY AND AA.MM = BB.MM AND AA.DDAY = BB.DDAY    " +
            //" ORDER BY AA.SITE_ID, AA.FID,AA.YY, AA.MM,AA.DDAY  ";


            //-- EDIT 17/10/2019 -- ในหน้า Report Onshore summary ให้นำข้อมูลที่ผ่านการ confirm แล้วเท่านั้นมาแสดงครับ ยกเว้น H2S,HG ที่ใช้ SPOT 
            sql = " SELECT AA.SITE_ID, AA.FID,AA.YY, AA.MM,AA.DDAY  " +
            " ,DECODE(NGMODIFIED_DATE,NULL,NULL,C1) C1 ,DECODE(NGMODIFIED_DATE,NULL,NULL,C2) C2 ,DECODE(NGMODIFIED_DATE,NULL,NULL,C3) C3  " +
            " ,DECODE(NGMODIFIED_DATE,NULL,NULL,IC4) IC4 ,DECODE(NGMODIFIED_DATE,NULL,NULL,NC4) NC4  ,DECODE(NGMODIFIED_DATE,NULL,NULL,IC5) IC5  " +
            " ,DECODE(NGMODIFIED_DATE,NULL,NULL,NC5) NC5 ,DECODE(NGMODIFIED_DATE,NULL,NULL,C6) C6 ,DECODE(NGMODIFIED_DATE,NULL,NULL,N2) N2  " +
            " ,DECODE(NGMODIFIED_DATE,NULL,NULL,CO2) CO2 ,DECODE(NGMODIFIED_DATE,NULL,NULL,SG) SG ,DECODE(NGMODIFIED_DATE,NULL,NULL,GHV) GHV  " +
            " ,DECODE(NGMODIFIED_DATE,NULL,NULL,NHV) NHV ,DECODE(NGMODIFIED_DATE,NULL,NULL,WC) WC ,DECODE(NGMODIFIED_DATE,NULL,NULL,WB) WB  " +
            " ,DECODE(NGMODIFIED_DATE,NULL,NULL,SUM_C2) SUM_C2 ,DECODE(NGMODIFIED_DATE,NULL,NULL,CO2_N2) CO2_N2  " +
            " ,DECODE(NGMODIFIED_DATE,NULL,NULL,CALC_WI) CALC_WI    " +
            " ,HG, H2S ,HG_NAME, H2S_NAME" +
            " FROM    " +
            " ( SELECT S.SITE_ID, S.FID, TO_CHAR(ADATE, 'YYYY') YY, TO_CHAR(ADATE, 'MM') MM, " + selectDay2 + "   DDAY    " +
            " FROM O_DIM_DATE DD, O_SITE_FID S ";
            if (criteria2 != "") { sql += " WHERE " + criteria2; }
            sql += "  GROUP BY S.SITE_ID, S.FID, TO_CHAR(ADATE, 'YYYY'), TO_CHAR(ADATE, 'MM'), " + selectDay2 + "  " +
            " ) AA    " +
            " LEFT OUTER JOIN   " +
            " (  SELECT S.SITE_ID, S.FID, TO_CHAR(D.RDATE,'YYYY') YY, TO_CHAR(D.RDATE, 'MM') MM , " + selectDay + "  DDAY " +
            " , ROUND(AVG(to_numberv3(D.C1)), 3)  C1, ROUND(AVG(to_numberv3(D.C2)), 3) C2,   ROUND(AVG(to_numberv3(D.C3)), 3) C3,  ROUND(AVG(to_numberv3(D.IC4)), 3) IC4,  ROUND(AVG(to_numberv3(D.NC4)), 3) NC4,  ROUND(AVG(to_numberv3(D.IC5)), 3) IC5  " +
            " , ROUND(AVG(to_numberv3(D.NC5)), 3) NC5,  ROUND(AVG(to_numberv3(D.C6)), 3) C6  , ROUND(AVG(to_numberv3(D.N2)), 3) N2,   ROUND(AVG(to_numberv3(D.CO2)), 3) CO2,  ROUND(AVG(to_numbernull4(D.SG)), 4) SG,   ROUND(AVG(to_numberv3(D.GHV)), 3) GHV  " +
            " , ROUND(AVG(to_numberv3(D.NHV)), 3) NHV,  ROUND(AVG(to_numberv3(D.WC)), 2) WC, ROUND(AVG(to_numberv3(D.WB)), 3) WB  " +
            " , ROUND0(AVG(to_numberv3(D.C2)), 3) + ROUND0(AVG(to_numberv3(D.C3)), 3) + ROUND0(AVG(to_numberv3(D.IC4)), 3) + ROUND0(AVG(to_numberv3(D.NC4)), 3) + ROUND0(AVG(to_numberv3(D.IC5)), 3) + ROUND0(AVG(to_numberv3(D.NC5)), 3)  " +
            "  + ROUND0(AVG(to_numberv3(D.C6)), 3)  AS SUM_C2, ROUND0(AVG(to_numberv3(D.CO2)), 3) +ROUND0(AVG(to_numberv3(D.N2)), 3)  AS CO2_N2  " +
            " , ROUND( (AVG(to_numberv3(D.GHV))/0.9826) /  SQRT(AVG(to_numbernull4(D.SG))),3)  CALC_WI   " +
            " , MAX(NG.MODIFIED_DATE) AS NGMODIFIED_DATE  " +
            " , ROUND(AVG(to_numberv3(NVL(H.HG,H_1.HG))), 3) HG , ROUND(AVG(to_numberv3(NVL(HS.H2S,HS_1.H2S))), 3) H2S  " +
            " , MAX(NVL(H.HG_NAME,H_1.HG_NAME)) HG_NAME , MAX(NVL(HS.H2S_NAME,HS_1.H2S_NAME)) H2S_NAME " +
            " FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID     " +
            " LEFT OUTER JOIN O_SPOT_UPDATE P ON D.FID = P.FID AND TO_CHAR(D.RDATE,'YYYYMM')= TO_CHAR(P.RDATE, 'YYYYMM')     " +
            " LEFT OUTER JOIN O_OGC_HG H ON P.HG_NAME = H.HG_NAME AND TO_CHAR(P.RDATE,'YYYYMM')= TO_CHAR(H.SDATE, 'YYYYMM')    " +
            " LEFT OUTER JOIN O_OGC_H2S HS ON P.H2S_NAME = HS.H2S_NAME AND TO_CHAR(P.RDATE,'YYYYMM')= TO_CHAR(HS.SDATE, 'YYYYMM')    " +
            " LEFT OUTER JOIN O_OGC_HG H_1 ON S.HG_NAME1 = H_1.HG_NAME AND TO_CHAR(D.RDATE,'YYYYMM')= TO_CHAR(H_1.SDATE, 'YYYYMM')  " +
            " LEFT OUTER JOIN O_OGC_H2S HS_1 ON S.H2S_NAME1 = HS_1.H2S_NAME AND TO_CHAR(D.RDATE,'YYYYMM')= TO_CHAR(HS_1.SDATE, 'YYYYMM')  " +
            " LEFT OUTER JOIN O_SITE_REPORT R ON D.FID=R.FID AND R.RPT_TYPE='DAILY'  " +
            " LEFT OUTER JOIN NGBILL_DAILY_UPDATE NG ON D.RDATE=NG.RDATE AND NG.FID=R.NGBILL_RPT_NO  ";
            if (criteria != "") { sql += " WHERE " + criteria; }
            sql += "  GROUP BY S.SITE_ID, S.FID, TO_CHAR(D.RDATE, 'YYYY'),TO_CHAR(D.RDATE, 'MM'), " + selectDay + " " +
            " ) BB ON AA.SITE_ID = BB.SITE_ID AND AA.YY = BB.YY AND AA.MM = BB.MM AND AA.DDAY = BB.DDAY    " +
            " ORDER BY AA.SITE_ID, AA.FID,AA.YY, AA.MM,AA.DDAY  ";

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 02/07/2019 --
    //-- edit 17/09/2019 --
    //-- edit 27/05/2024 เพิ่ม "1DAY"
    public DataTable SearchRptOffshoreSummary(String PeriodType, String FromDate, String ToDate, String TemplateList = "", String SIDList = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = "";
        String criteria = "", selectDay = "";
        String criteria2 = "", selectDay2 = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteriaRange(ref criteria, "RDATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);
            AddCriteriaRange(ref criteria2, "ADATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);

            if (TemplateList != "")
            {
                criteria += " AND S.SITE_ID IN (SELECT DISTINCT S.SITE_ID FROM O_RPT_FID_DETAIL T INNER JOIN O_OFFSHORE_FID S ON T.FID = S.FID WHERE TID IN (" + TemplateList + ")) ";
                criteria2 += " AND S.SITE_ID IN (SELECT DISTINCT S.SITE_ID FROM O_RPT_FID_DETAIL T INNER JOIN O_OFFSHORE_FID S ON T.FID = S.FID WHERE TID IN (" + TemplateList + ")) ";
            }
            else
            {
                if (SIDList != "")
                {
                    criteria += "  AND S.SITE_ID IN (" + SIDList + " ) ";
                    criteria2 += "  AND S.SITE_ID IN (" + SIDList + " ) ";
                }
            }

            switch (PeriodType) //pType ="10DAY","15DAY","ENDMTH" //-- edit 27/05/2024 เพิ่ม "1DAY"
            {
                case "1DAY":  
                    selectDay = " 'D'||TO_CHAR(RDATE,'DD') ";
                    selectDay2 = " 'D'||TO_CHAR(ADATE,'DD') ";
                    break;
                case "10DAY":
                    selectDay = "CASE WHEN TO_CHAR(RDATE,'DD')<'11' THEN 'D1' WHEN TO_CHAR(RDATE,'DD')<'21' THEN 'D2' ELSE 'D3' END ";
                    selectDay2 = "CASE WHEN TO_CHAR(ADATE,'DD')<'11' THEN 'D1' WHEN TO_CHAR(ADATE,'DD')<'21' THEN 'D2' ELSE 'D3' END ";
                    break;
                case "15DAY":
                    selectDay = "CASE WHEN TO_CHAR(RDATE,'DD')<'16' THEN 'D4' ELSE 'D5' END ";
                    selectDay2 = "CASE WHEN TO_CHAR(ADATE,'DD')<'16' THEN 'D4' ELSE 'D5' END ";
                    break;
                case "ENDMTH":
                    selectDay = " 'D6' ";
                    selectDay2 = " 'D6' ";
                    break;
            }


            //sql = " SELECT S.SITE_ID, D.FID, " +
            //" TO_CHAR(RDATE,'YYYY') YY, TO_CHAR(RDATE,'MM') MM , " + selectDay + " DDAY," +
            //"    ROUND(AVG(to_numberv3(C1)),3) C1,  ROUND(AVG(to_numberv3(C2)),3) C2,   ROUND(AVG(to_numberv3(C3)),3) C3,   " +
            //"     ROUND(AVG(to_numberv3(IC4)),3) IC4,  ROUND(AVG(to_numberv3(NC4)),3) NC4,  ROUND(AVG(to_numberv3(IC5)),3) IC5,  " +
            //"     ROUND(AVG(to_numberv3(NC5)),3) NC5,  ROUND(AVG(to_numberv3(C6)),3) C6,  ROUND(AVG(to_numberv3(C7)),3) C7,  ROUND(AVG(to_numberv3(N2)),3) N2,   " +
            //"     ROUND(AVG(to_numberv3(CO2)),3) CO2,  ROUND(AVG(to_numbernull4(SG)),4) SG,   ROUND(AVG(to_numberv3(GHV)),3) GHV,  " +
            //"      ROUND(AVG(to_numberv3(H2O)),2) H2O,  ROUND(AVG(to_numberv3(HG)),2) HG, ROUND(AVG(to_numberv3(H2S)),3) H2S,    " +
            //"   ROUND0(AVG(to_numberv3(C2)),3)+ ROUND0(AVG(to_numberv3(C3)),3)+ ROUND0(AVG(to_numberv3(IC4)),3)+ ROUND0(AVG(to_numberv3(NC4)),3)   " +
            //"  + ROUND0(AVG(to_numberv3(IC5)),3)+ ROUND0(AVG(to_numberv3(NC5)),3)+ ROUND0(AVG(to_numberv3(C6)),3) + ROUND0(AVG(to_numberv3(C7)),3)  AS SUM_C2 , " +
            //"  ROUND0(AVG(to_numberv3(CO2)),3) + ROUND0(AVG(to_numberv3(N2)),3)  AS CO2_N2    " +
            //"   FROM OFFSHORE_DAILY_UPDATE D INNER JOIN O_OFFSHORE_FID S ON D.FID = S.FID ";
            //if (criteria != "") { sql += " WHERE " + criteria; }
            //sql += " GROUP BY S.SITE_ID, D.FID, TO_CHAR(RDATE,'YYYY'),TO_CHAR(RDATE,'MM'), " + selectDay;
            //sql += " ORDER BY S.SITE_ID, D.FID, TO_CHAR(RDATE,'YYYY'), TO_CHAR(RDATE,'MM'), " + selectDay;

            sql = " SELECT AA.SITE_ID, AA.FID,AA.YY, AA.MM,AA.DDAY " +
            "  , C1,  C2,  C3,  IC4,  NC4,  IC5 , NC5,  C6,  C7,  N2,  CO2,  SG , GHV,  H2O,  HG, H2S, SUM_C2 ,  CO2_N2 " +
            " FROM " +
            " (" +
            " SELECT S.SITE_ID, S.FID, TO_CHAR(ADATE, 'YYYY') YY, TO_CHAR(ADATE, 'MM') MM, " + selectDay2 + " DDAY " +
            " FROM O_DIM_DATE DD, O_OFFSHORE_FID S ";
            if (criteria2 != "") { sql += " WHERE " + criteria2; }
            sql += " GROUP BY S.SITE_ID, S.FID, TO_CHAR(ADATE, 'YYYY'), TO_CHAR(ADATE, 'MM'), " + selectDay2 + " " +
            " ) AA " +
            " LEFT OUTER JOIN " +
            " ( " +
            "  SELECT S.SITE_ID, D.FID, TO_CHAR(RDATE,'YYYY') YY, TO_CHAR(RDATE, 'MM') MM , " + selectDay + " DDAY " +
            "      , ROUND(AVG(to_numberv3(C1)), 3) C1,  ROUND(AVG(to_numberv3(C2)), 3) C2,   ROUND(AVG(to_numberv3(C3)), 3) C3,        ROUND(AVG(to_numberv3(IC4)), 3) IC4,  ROUND(AVG(to_numberv3(NC4)), 3) NC4,  ROUND(AVG(to_numberv3(IC5)), 3) IC5 " +
            "  ,       ROUND(AVG(to_numberv3(NC5)), 3) NC5,  ROUND(AVG(to_numberv3(C6)), 3) C6,  ROUND(AVG(to_numberv3(C7)), 3) C7,  ROUND(AVG(to_numberv3(N2)), 3) N2,        ROUND(AVG(to_numberv3(CO2)), 3) CO2,  ROUND(AVG(to_numbernull4(SG)), 4) SG " +
            "  ,   ROUND(AVG(to_numberv3(GHV)), 3) GHV,        ROUND(AVG(to_numberv3(H2O)), 2) H2O,  ROUND(AVG(to_numberv3(HG)), 2) HG, ROUND(AVG(to_numberv3(H2S)), 3) H2S " +
            "  ,       ROUND0(AVG(to_numberv3(C2)), 3) + ROUND0(AVG(to_numberv3(C3)), 3) + ROUND0(AVG(to_numberv3(IC4)), 3) + ROUND0(AVG(to_numberv3(NC4)), 3) + ROUND0(AVG(to_numberv3(IC5)), 3) + ROUND0(AVG(to_numberv3(NC5)), 3) + ROUND0(AVG(to_numberv3(C6)), 3) + ROUND0(AVG(to_numberv3(C7)), 3)  AS SUM_C2, ROUND0(AVG(to_numberv3(CO2)), 3) +ROUND0(AVG(to_numberv3(N2)), 3)  AS CO2_N2 " +
            "  FROM OFFSHORE_DAILY_UPDATE D INNER JOIN O_OFFSHORE_FID S ON D.FID = S.FID ";
            if (criteria != "") { sql += " WHERE " + criteria; }
            sql += "  GROUP BY S.SITE_ID, D.FID, TO_CHAR(RDATE, 'YYYY'),TO_CHAR(RDATE, 'MM') , " + selectDay + " " +
            "  ) BB ON AA.SITE_ID = BB.SITE_ID AND AA.YY = BB.YY AND AA.MM = BB.MM AND AA.DDAY = BB.DDAY " +
            " ORDER BY AA.SITE_ID, AA.FID,AA.YY, AA.MM,AA.DDAY ";


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    #endregion

    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region DBSyncs

    //-- aor edit 28/09/2018 --
    public DataTable SearchDBSyncs(String tName, String syncFlag, String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";

        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "TNAME", tName , DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "SYNC_FLAG", syncFlag, DBUTIL.FieldTypes.ftText);

            sql = "SELECT * FROM O_DB_SYNCS ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY TNAME";
            }


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 28/09/2018 --
    public void UpdateDBSyncs(string tName, object lastDate)
    {
        string SQL, SQL1, SQL2, criteria = "";
        try
        {
            if (tName != "")
            {
                SQL = ""; SQL1 = ""; SQL2 = "";

                AddCriteria(ref criteria, "TNAME", tName, DBUTIL.FieldTypes.ftText);

                //-- EDIT 23/09/2019 --
                if (tName == "GQMS_MAP_GCDS") //MODIFIED_DATE เป็น varchar เลยให้ backup ทั้ง table
                    AddSQL(DBUTIL.opUPDATE, ref SQL1, ref SQL2, "LAST_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                else
                    AddSQL(DBUTIL.opUPDATE, ref SQL1, ref SQL2, "LAST_DATE", lastDate, DBUTIL.FieldTypes.ftDateTime);


                AddSQL(DBUTIL.opUPDATE, ref SQL1, ref SQL2, "MODIFIED_BY", "GQMS_SYSTEM", DBUTIL.FieldTypes.ftText);
                AddSQL(DBUTIL.opUPDATE, ref SQL1, ref SQL2, "MODIFIED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);

                SQL = CombineSQL(DBUTIL.opUPDATE, ref SQL1, ref SQL2, "O_DB_SYNCS", criteria, false);
                ExecuteSQL(SQL);

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    #endregion

    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    #region PROCEDURE

    //-- aor edit 31/05/2017 --
    public String impSAP_MASTER()
    {
        String result = "";
        try
        {
            //DB.AddParam("NAME", "VALUE", DBUTIL.FieldTypes.ftText);

            DB.ExecProc("impSAP_MASTER",null);

            return result;
        } 
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
        }
    }

    #endregion

    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    #region SENDMAIL

    //-- AOR EDIT 05/06/2017  
    public DataTable SearchEmailLog(String ItemID, String DateF, String DateT, String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            criteria = OtherCriteria;
            AddCriteria(ref criteria, "ALERT_EMAIL_ITEM_ID", ItemID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "SEND_DATE", Utility.AppDateValue(DateF), Utility.AppDateValue(DateT), DBUTIL.FieldTypes.ftDate);

            sql = " SELECT * FROM O_EMAIL_LOG ";
            if (criteria != "") { sql += " WHERE " + criteria; }

            sql += " ORDER BY TRANS_ID ";


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 23/08/2018 --
    public void InsertEmailLog(string MailItemID, string Subject, string Status, string MailTO, string MailCC = "", string MailBCC = "", string ErrMsg = "", string Message = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";

            string TransId = GenerateID("O_EMAIL_LOG", "TRANS_ID").ToString();
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "TRANS_ID", TransId, DBUTIL.FieldTypes.ftNumeric);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "SEND_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "ETEMPLATE_ID", MailItemID, DBUTIL.FieldTypes.ftText);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "MAIL_TO", MailTO, DBUTIL.FieldTypes.ftText);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "MAIL_CC", MailCC, DBUTIL.FieldTypes.ftText);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "MAIL_BCC", MailBCC, DBUTIL.FieldTypes.ftText);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "SUBJECT", Subject, DBUTIL.FieldTypes.ftText);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "STATUS", Status, DBUTIL.FieldTypes.ftNumeric);
            if (ErrMsg.Length > 4000) ErrMsg = ErrMsg.Substring(0, 4000);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "ERR_MSG", ErrMsg, DBUTIL.FieldTypes.ftText);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "MESSAGE", Message, DBUTIL.FieldTypes.ftText);

            if (System.Web.HttpContext.Current.Session["USER_NAME"] + "" == "") System.Web.HttpContext.Current.Session["USER_NAME"] = "GQMS_SYSTEM";

            SQL = CombineSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "O_EMAIL_LOG", Criteria, true);
            ExecuteSQL(SQL);


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 01/06/2017 --
    public void UpdateEmailLog(string TransID, string Status, string ErrMsg = "")
    {
        string SQL, SQL1, SQL2, criteria = "";
        try
        {
            if (TransID != "")
            {
                SQL = ""; SQL1 = ""; SQL2 = "";

                AddCriteria(ref criteria, "TRANS_ID", TransID, DBUTIL.FieldTypes.ftNumeric);

                if (ErrMsg.Length > 5000) ErrMsg = ErrMsg.Substring(0, 5000);

                AddSQL(DBUTIL.opUPDATE, ref SQL1, ref SQL2, "SEND_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                AddSQL(DBUTIL.opUPDATE, ref SQL1, ref SQL2, "STATUS", Status, DBUTIL.FieldTypes.ftNumeric);
                AddSQL(DBUTIL.opUPDATE, ref SQL1, ref SQL2, "ERR_MSG", ErrMsg, DBUTIL.FieldTypes.ftText);

                if (System.Web.HttpContext.Current.Session["USER_NAME"] + "" == "") System.Web.HttpContext.Current.Session["USER_NAME"] = "GQMS_SYSTEM";

                SQL = CombineSQL(DBUTIL.opUPDATE, ref SQL1, ref SQL2, "O_EMAIL_LOG", criteria, true);
                ExecuteSQL(SQL);

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    #endregion
}