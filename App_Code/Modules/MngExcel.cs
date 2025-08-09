//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************




using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Data.OleDb;



public class MngExcel
{
    //--  อ่าน excel file ที่ first sheet
    public DataTable ReadWorksheet(string FileName, string WorkSheetName = "")
    {
        OleDbConnection Conn = null;
        OleDbDataAdapter DA = null;
        DataTable DT = new DataTable();
        string ConnStr = "";
        string FileType = null;

        bool foundFlag = true;
        try
        {
            FileType = Utility.GetFileType(FileName).ToLower();
        

            switch (FileType)
            {
                case ".xlsx":
                case ".xls":
                    ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=No;IMEX=1;\"";
                    break;
                //-- 28/06/2016 -- เนื่องจากเครื่องเป็น 64bit จึงไม่ได้ install 32 bit  
                //Case Else 'xls
                //   ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=No;IMEX=1;"""

            }

            if (!string.IsNullOrEmpty(ConnStr))
            {
                ConnStr = string.Format(ConnStr, FileName);
                Conn = new OleDbConnection(ConnStr);
                OleDbCommand cmdExcel = new OleDbCommand();

                cmdExcel.Connection = Conn;

                //Get the name of First Sheet 
                Conn.Open();
                DataTable dtExcelSchema = default(DataTable);
                dtExcelSchema = Conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (WorkSheetName == "")  //ถ้าไม่กำหนด worksheet ให้เอาอันแรก เรียงตามชื่อ
                      WorkSheetName = Utility.ToString(dtExcelSchema.Rows[0]["TABLE_NAME"]); // (เรียงตามชื่อ)
                else
                {
                    WorkSheetName = WorkSheetName + "$";
                    //-- aor edit 20/07/2018 --
                    //-- ถ้าส่ง WorkSheetName มาแล้วไม่มีใน excel ให้ใช้ worksheet อันแรก  (เรียงตามชื่อ)
                    foundFlag = false;
                    for (int w = 0; w <= dtExcelSchema.Rows.Count - 1; w++)
                    {
                        string SheetName = Utility.ToString(dtExcelSchema.Rows[w]["TABLE_NAME"]);
                        if (SheetName.ToUpper() == "" + WorkSheetName.ToUpper() | SheetName.ToUpper() == "'" + WorkSheetName.ToUpper() + "'")
                        {
                            foundFlag = true;
                            break;
                        }
                    }

                    //-- aor edit 21/06/2019 -- ถ้าส่ง WorkSheetName มาแล้วไม่มีใน excel ก็ไม่ต้องอ่านข้อมูล
                    //if ( foundFlag == false ) WorkSheetName = Utility.ToString(dtExcelSchema.Rows[0]["TABLE_NAME"]); // (เรียงตามชื่อ)

                }

                //-- aor edit 21/06/2019 -- ถ้าส่ง WorkSheetName มาแล้วไม่มีใน excel ก็ไม่ต้องอ่านข้อมูล
                if (foundFlag == true)
                {
                    //ตรวจสอบ worksheetname
                    for (int w = 0; w <= dtExcelSchema.Rows.Count - 1; w++)
                    {
                        string SheetName = Utility.ToString(dtExcelSchema.Rows[w]["TABLE_NAME"]);

                        if (SheetName.ToUpper() == "" + WorkSheetName.ToUpper() | SheetName.ToUpper() == "'" + WorkSheetName.ToUpper() + "$'")
                        {
                            try
                            {
                                DA = new OleDbDataAdapter("SELECT * FROM [" + SheetName + "]", Conn);
                                DA.Fill(DT);

                                //-- ลบบรรทัดที่ว่าง -- 21/06/2019 ต้องอ่านจากด้านล่างขึ้นมาจนกว่าจะเจอข้อมูล
                                DataTable DT2 = DT;
                                int colCtn = DT2.Columns.Count;
                                for (int i = DT2.Rows.Count - 1; i > 1; i--)
                                {
                                    DataRow DR = DT2.Rows[i];
                                    //-- 21/06/2019 ลบบรรทัดที่ว่าง ต้องตรวจหลายๆคอลัมน์
                                    if ( colCtn > 3)
                                    {
                                        if (DR[0].ToString().Trim().Length == 0 && DR[1].ToString().Trim().Length == 0 && DR[2].ToString().Trim().Length == 0)
                                        {
                                            DT.Rows[i].Delete();
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (DR[0].ToString().Trim().Length == 0 )
                                        {
                                            DT.Rows[i].Delete();
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    
                                }

                                DT.AcceptChanges();
                                w = dtExcelSchema.Rows.Count;
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }

                }



            }

            return DT;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            try
            {
                DA.Dispose();
            }
            catch { }
            finally { DA = null; }

            try
            {
                Conn.Dispose();
            }
            catch { }
            finally { Conn = null; }

        }
    }


    //-- aor edit 09/01/2017  เขียน excel file
    public string WriteWorksheet(string FileName, string WorkSheetName = "")
    {
        OleDbConnection Conn = null;
        OleDbDataAdapter DA = null;
        DataTable DT = new DataTable();
        string ConnStr = "";
        string FileType = null;


        try
        {
            FileType = Utility.GetFileType(FileName).ToLower();
            switch (FileType)
            {
                case ".xlsx":
                case ".xls":
                    //ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=No;IMEX=1;"""
                    //-- aor edit 09/01/2017 --
                    ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=No;READONLY=FALSE;\"";

                    break;
                    //-- 28/06/2016 -- เนื่องจากเครื่องเป็น 64bit จึงไม่ได้ install 32 bit  
                    //Case Else 'xls
                    //    ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=No;IMEX=1;"""

            }

            if (!string.IsNullOrEmpty(ConnStr))
            {
                ConnStr = string.Format(ConnStr, FileName);
                Conn = new OleDbConnection(ConnStr);

                OleDbCommand cmdExcel = new OleDbCommand();

                cmdExcel.Connection = Conn;

                //Get the name of First Sheet 
                Conn.Open();
                DataTable dtExcelSchema = default(DataTable);
                dtExcelSchema = Conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                
                //ตรวจสอบ worksheetname
                for (int w = 0; w <= dtExcelSchema.Rows.Count - 1; w++)
                {
                    string SheetName = Utility.ToString(dtExcelSchema.Rows[w]["TABLE_NAME"]);
                    if (SheetName == "" + WorkSheetName + "$" | SheetName == "'" + WorkSheetName + "$'")
                    {
                        try
                        {
                            DA = new OleDbDataAdapter("SELECT * FROM [" + SheetName + "]", Conn);
                            DA.Fill(DT);

                            //-- AOR EDIT 09/01/2017 --- ถ้าเป็น .xlsx ใช้ insert / .xls ใช้ update --------------
                            cmdExcel.CommandText = "INSERT INTO [ผลตรวจทั่วไป$](F1,F2,F3,F4,F5,F6,F7,F8,F9,F10,F11) VALUES ('1','บริษัท ทีโอซี ไกลคอล จำกัด','26003029','นาย','ชัยณรงค์','สพกลาง','','','','8/6/2525','35' );";
                            //cmdExcel.CommandText = " UPDATE [ผลตรวจทั่วไป$] SET F2='บริษัท ทีโอซี ไกลคอล จำกัด', F3='26003029', F4='นาย', F5='ชัยณรงค์', F6='สพกลาง' WHERE F1=1"'
                            // cmdExcel.CommandText = " UPDATE [ผลตรวจทั่วไป$A3:F3] SET F1=1, F2='บริษัท ทีโอซี ไกลคอล จำกัด', F3='26003029', F4='นาย', F5='ชัยณรงค์', F6='สพกลาง'"
                            cmdExcel.ExecuteNonQuery();

                            cmdExcel.CommandText = "INSERT INTO [ผลตรวจทั่วไป$](F1,F2,F3,F4,F5,F6,F7,F8,F9,F10,F11) VALUES ('2','บริษัท ทีโอซี ไกลคอล จำกัด','99000052','น.ส.','ปวีณา','อินบัว','Secretary','','','19/12/2498','62' );";
                            //cmdExcel.CommandText = " UPDATE [ผลตรวจทั่วไป$] SET F2='บริษัท ทีโอซี ไกลคอล จำกัด', F3='99000052', F4='น.ส.', F5='ปวีณา', F6='อินบัว' WHERE F1=2"
                            //cmdExcel.CommandText = " UPDATE [ผลตรวจทั่วไป$A4:F4] SET F1=2, F2='บริษัท ทีโอซี ไกลคอล จำกัด', F3='99000052', F4='น.ส.', F5='ปวีณา', F6='อินบัว'"
                            cmdExcel.ExecuteNonQuery();



                            //cmdExcel.CommandText = " UPDATE [ผลตรวจทั่วไป$A5:F5] SET F1=3, F2='บริษัท ทีโอซี ไกลคอล จำกัด', F3='222222', F4='น.ส.', F5='ปวีณา22', F6='อินบั22ว'"
                            //cmdExcel.ExecuteNonQuery()

                            Conn.Close();
                            Conn.Dispose();



                            //DA = New OleDbDataAdapter("SELECT * FROM [" & SheetName & "]", Conn)
                            //DA.Fill(DT)


                            w = dtExcelSchema.Rows.Count;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }


            }

            return "";
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            try
            {
                DA.Dispose();
            }
            catch { }
            finally { DA = null; }

            try
            {
                Conn.Dispose();
            }
            catch { }
            finally { Conn = null; }
        }
    }


    public DataTable GetDataTableFromCSVFile(string csv_file_path, int startRow = 0, int headerRow = -1)
    {
        DataTable csvData = new DataTable();
        DataRow csvRow = null;
        StreamReader StrWer;
        String sLine = "";
        int row = 0;
        int col = 0, maxcol = 0;
        try
        {

            StrWer = File.OpenText(csv_file_path);
            

            while (!StrWer.EndOfStream)
            {
                sLine = StrWer.ReadLine();
                if ( row >= startRow )
                {
                    String[] split = sLine.Split(new Char[] {','});
                    //-- กำหนดคอลัมน์
                    if (row == startRow)
                    {
                        if (headerRow > -1)
                        {
                            if (row == headerRow) //headerRow > -1
                            {
                                foreach (String s in split)
                                {
                                    DataColumn datacol = new DataColumn(s);
                                    datacol.AllowDBNull = true;
                                    csvData.Columns.Add(datacol);
                                    maxcol++;
                                }
                            }
                        }
                        else
                        {
                            for (col = 0; col < split.Length; col++)
                            {
                                DataColumn datacol = new DataColumn("F"+col);
                                datacol.AllowDBNull = true;
                                csvData.Columns.Add(datacol);
                                maxcol++;
                            }
                            //อ่านข้อมูลบรรทัดแรก
                            csvRow = csvData.NewRow();
                            col = 0;
                            foreach (String s in split)
                            {
                                if ( col < maxcol )
                                {
                                    csvRow[col] = s.ToString();
                                    col++;
                                }
                            }
                            csvData.Rows.Add(csvRow);

                        }
                    }
                    else
                    {
                        //อ่านข้อมูลบรรทัดต่อมา
                        csvRow = csvData.NewRow();
                        col = 0;
                        foreach (String s in split)
                        {
                            if (col < maxcol)
                            {
                                csvRow[col] = s.ToString();
                                col++;
                            }
                        }
                        csvData.Rows.Add(csvRow);
                    }
                }


                row++;
            }

            StrWer.Close();

         }
        catch (Exception ex)
        {
            throw ex;
        }

        return csvData;

    }

}

 