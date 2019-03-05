using CommonHelper;
using Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ExcelBLL
    {
        public static string BuildExcel1(String[] columnName, DataTable table)
        {
            MemoryStream ms = BuildExcel2(columnName, table);
            string fileName = "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd")))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd"));
            }
            var f = File.Create(System.AppDomain.CurrentDomain.BaseDirectory + fileName);
            ms.WriteTo(f);
            ms.Close();
            f.Close();
            return fileName;
        }
        private static MemoryStream BuildExcel2(String[] columnName, DataTable table)
        {
            MemoryStream ms = new MemoryStream();
            using (table)
            {
                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                IRow headerRow = sheet.CreateRow(0);
                foreach (DataColumn column in table.Columns)
                {
                    headerRow.CreateCell(column.Ordinal).SetCellValue(columnName[column.Ordinal]);
                }
                int rowIndex = 1;
                foreach (DataRow row in table.Rows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in table.Columns)
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }
                    rowIndex++;
                }
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
            }
            return ms;
        }
        #region 表头
        /// <summary>
        /// 获取学生导入表格表头
        /// </summary>
        /// <param name="eInfo"></param>
        /// <returns></returns>
        public static List<string> GetStuColumn(E_Info eInfo)
        {
            List<string> data = new List<string>() { "学校代号", "学校简称", "考号", "班级", "姓名" };
            foreach (var item in eInfo.sbs)
            {
                data.Add(item.sbnm + "试室号");
                data.Add(item.sbnm + "座位号");
                data.Add(item.sbnm + "教师");
            }
            data.Add("身份证号");
            data.Add("考生类别");
            return data;
        }
        public static List<string> GetTchColumn()
        {
            List<string> data = new List<string>() { "学校代号", "学校简称", "科目", "姓名", "性别", "职称", "手机" };
            return data;
        }
        public static List<string> GetScoreColumn(E_Info eInfo)
        {

            List<string> data = new List<string>() { "考号", "班级", "姓名", "身份证号" };
            foreach (var item in eInfo.sbs)
            {
                data.Add(item.sbnm);
            }
            data.Add("总分");
            return data;
        }
        public static List<string> GetSchColumn()
        {
            List<string> data = new List<string>() { "学校代号", "学校简称", "所属区", "联系人姓名", "联系电话", "邮箱地址", "QQ号码" };
            return data;
        }
        public static List<string> GetAreaColumn()
        {
            List<string> data = new List<string>() { "账号", "所属区", "教研员姓名", "联系电话", "邮箱地址", "QQ号码" };
            return data;
        }
        public static List<string> GetPaperColumn(E_Info eInfo)
        {
            List<string> data = new List<string>() { "学校代号", "学校简称", "总数" };
            foreach (var item in eInfo.sbs)
            {
                data.Add(item.sbnm);
            }
            return data;
        }
        public static List<string> GetPaperColumn1(E_Info eInfo)
        {
            List<string> data = new List<string>() { "学校代号", "学校简称" };
            foreach (var item in eInfo.sbs)
            {
                data.Add(item.sbnm + "应考");
                data.Add(item.sbnm + "实考");
                data.Add(item.sbnm + "缺考");
            }
            return data;
        } 
        #endregion
        /// <summary>
        /// 生成学生导入表格
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string BuildStuExcel(List<string> columnName, List<string> col)
        {
            MemoryStream ms = new MemoryStream();
            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < columnName.Count; i++)
            {
                headerRow.CreateCell(i).SetCellValue(columnName[i]);
            }

            //设置生成下拉框的行和列
            var cellRegions = new CellRangeAddressList(1, 65535, columnName.Count - 1, columnName.Count - 1);

            //设置 下拉框内容
            DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(col.ToArray());

            //绑定下拉框和作用区域，并设置错误提示信息
            HSSFDataValidation dataValidate = new HSSFDataValidation(cellRegions, constraint);
            dataValidate.CreateErrorBox("输入不合法", "请输入或选择下拉列表中的值。");
            dataValidate.ShowPromptBox = true;
            sheet.AddValidationData(dataValidate);

            workbook.Write(ms);
            string fileName = "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" +"学生" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd")))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd"));
            }
            var f = File.Create(System.AppDomain.CurrentDomain.BaseDirectory + fileName);
            ms.WriteTo(f);
            ms.Close();
            f.Close();
            return fileName;
        }
        /// <summary>
        /// 生成老师导入表格
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string BuildTchExcel(List<string> columnName, List<string> col)
        {
            MemoryStream ms = new MemoryStream();
            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < columnName.Count; i++)
            {
                headerRow.CreateCell(i).SetCellValue(columnName[i]);
            }
            //设置生成下拉框的行和列
            var cellRegions = new CellRangeAddressList(1, 65535, 2, 2);
            //设置科目 下拉框内容
            DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(col.ToArray());
            //绑定下拉框和作用区域，并设置错误提示信息
            HSSFDataValidation dataValidate = new HSSFDataValidation(cellRegions, constraint);
            dataValidate.CreateErrorBox("输入不合法", "请输入或选择下拉列表中的值。");
            dataValidate.ShowPromptBox = true;
            sheet.AddValidationData(dataValidate);
            //设置生成性别下拉框的行和列
            var cellRegions1 = new CellRangeAddressList(1, 65535, 4, 4);
            //设置 下拉框内容
            DVConstraint constraint1 = DVConstraint.CreateExplicitListConstraint(new String[] { "男", "女" });
            //绑定下拉框和作用区域，并设置错误提示信息
            HSSFDataValidation dataValidate1 = new HSSFDataValidation(cellRegions1, constraint1);
            dataValidate1.CreateErrorBox("输入不合法", "请输入或选择下拉列表中的值。");
            dataValidate1.ShowPromptBox = true;
            sheet.AddValidationData(dataValidate1);

            //设置生成职称下拉框的行和列
            var cellRegions2 = new CellRangeAddressList(1, 65535, 5, 5);
            //设置 下拉框内容
            DVConstraint constraint2 = DVConstraint.CreateExplicitListConstraint(new String[] { "正高级教师", "高级教师", "一级教师", "二级教师", "三级教师" });
            //绑定下拉框和作用区域，并设置错误提示信息
            HSSFDataValidation dataValidate2 = new HSSFDataValidation(cellRegions2, constraint2);
            dataValidate2.CreateErrorBox("输入不合法", "请输入或选择下拉列表中的值。");
            dataValidate2.ShowPromptBox = true;
            sheet.AddValidationData(dataValidate2);


            workbook.Write(ms);
            string fileName = "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" +"教师"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd")))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd"));
            }
            var f = File.Create(System.AppDomain.CurrentDomain.BaseDirectory + fileName);
            ms.WriteTo(f);
            ms.Close();
            f.Close();
            return fileName;
        }

        public static string ImportStuExcel(string path, E_Info exam,string SchoolID,out int errnum)
        {
            HSSFWorkbook hssfworkbook;
            #region//初始化信息
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(fs);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            #endregion
            NPOI.SS.UserModel.ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            DataTable dt = new DataTable();
            rows.MoveNext();
            HSSFRow row = (HSSFRow)rows.Current;
            List<string> cols = new List<string>();
            for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)
            {
                cols.Add(row.GetCell(j).ToString().Trim());
                dt.Columns.Add(row.GetCell(j).ToString());
            }
            var colHead = GetStuColumn(exam);
            if (!(colHead.All(cols.Contains) && cols.All(colHead.Contains)))
            {
                errnum = 0;
                return "表格格式错误，请重新下载模板";
            }
            cols.Add("错误原因");
            dt.Columns.Add("错误原因");
            while (rows.MoveNext())
            {
                row = (HSSFRow)rows.Current;
                St_Info st = new St_Info();
                if (!IsNull(row, 0) || !IsNull(row, 1) || !IsNull(row, 2))
                {
                    continue;
                }
                    var stInfo = MongoDbHelper.QueryOne<St_Info>(DbName.St_Info, w => w.eid == exam._id && (w.idcd == row.GetCell(row.LastCellNum - 2).ToString() || w.stid == row.GetCell(2).ToString()));
                var school = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info,w=>w._id== row.GetCell(0).ToString());
                var ifTrue = false;
                if (SchoolID != "SchoolID"&& school!=null)
                {
                    ifTrue =!(SchoolID == school._id);
                }
                if (school==null|| ifTrue || stInfo != null || !IsNull(row,0) || !IsNull(row, 1) || !IsNull(row, 2) || !IsNull(row, 3) || !IsNull(row, 4) || !IsNull(row, row.LastCellNum - 2) || !IsNull(row, row.LastCellNum - 1) || !CommonHelper.Function.MathIdCard(row.GetCell(row.LastCellNum - 2).ToString()))
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < (sheet.GetRow(0).LastCellNum); i++)
                    {
                        NPOI.SS.UserModel.ICell cell = row.GetCell(i);
                        if (cell == null)
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            dr[i] = cell.ToString();
                        }
                    }
                    if (school == null)
                    {
                        dr[sheet.GetRow(0).LastCellNum] += "学校不存在;";
                    }
                    if (ifTrue)
                    {
                        dr[sheet.GetRow(0).LastCellNum] += "学校代号错误;";
                    }
                    if (stInfo != null)
                    {
                        dr[sheet.GetRow(0).LastCellNum] += "考号或身份证号已存在;";
                    }
                    if (!CommonHelper.Function.MathIdCard(row.GetCell(row.LastCellNum - 2).ToString()))
                    {
                        dr[sheet.GetRow(0).LastCellNum] += "身份证号不正确;";
                    }
                    dt.Rows.Add(dr);
                    continue;
                }
                st.eid = exam._id;
                st.stid = row.GetCell(2).ToString();
                st.sid = row.GetCell(0).ToString();
                st.snm = row.GetCell(1).ToString();
                st.cls = row.GetCell(3).ToString();
                st.nm = row.GetCell(4).ToString();
                st.idcd = row.GetCell(row.LastCellNum - 2).ToString();
                st.tp = row.GetCell(row.LastCellNum - 1).ToString();
                var schPp = MongoDbHelper.QueryOne<Pp_Nm>(DbName.Pp_Nm, w => w.sid == st.sid && w.eid == exam._id);//查询该学校这次考试试卷数量
                int firstpp = 0;//0代表不是新建
                if (schPp == null)
                {
                    firstpp = 1;
                    schPp = new Pp_Nm();
                    schPp.sid = st.sid;
                    schPp.snm = st.snm;
                    schPp.eid = exam._id;
                    schPp.ct = 0;
                }
                schPp.ct++;//学校考试人数加1
                for (int i = 0; i < exam.sbs.Count; i++)
                {
                    if (IsNull(row, 5 + i * 3) && IsNull(row, 6 + i * 3) && IsNull(row, 7 + i * 3))
                    {
                        SubE sube = new SubE();
                        sube.sbid = exam.sbs[i]._id;
                        sube.sbnm = exam.sbs[i].sbnm;
                        sube.sbrm = row.GetCell(5 + i * 3).ToString();
                        sube.sbst = row.GetCell(6 + i * 3).ToString();
                        sube.sbtch = row.GetCell(7 + i * 3).ToString();
                        st.subEs.Add(sube);
                        exam.sbs[i].stct++;//考试人数加1
                        var Sbnm = schPp.sbnms.Where(w => w.sbid == sube.sbid).FirstOrDefault();
                        if (Sbnm == null)
                        {
                            Sbnm = new Sbnm();
                            Sbnm.sbid = sube.sbid;
                            Sbnm.sbnm = sube.sbnm;
                            Sbnm.sct = 1;
                            Sbnm.ac = 0;
                            schPp.sbnms.Add(Sbnm);
                        }
                        else
                        {
                            Sbnm.sct++;
                        }
                    }
                }
                if (firstpp == 0)
                {
                    MongoDbHelper.ReplaceOne(schPp._id.ToString(), schPp, DbName.Pp_Nm);
                }
                else
                {
                    MongoDbHelper.Insert(schPp, DbName.Pp_Nm);
                }
                MongoDbHelper.Insert(st, DbName.St_Info);
            }
            MongoDbHelper.ReplaceOne(exam._id.ToString(), exam, DbName.E_Info);
            errnum = dt.Rows.Count;
            if (dt.Rows.Count > 0)
            {
                return BuildExcel1(cols.ToArray(), dt);
            }
            return "";
        }
        /// <summary>
        /// 导入老师表格
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exam"></param>
        /// <returns></returns>
        public static string ImportTchExcel(string path, E_Info exam,string SchoolID, out int errnum)
        {
            HSSFWorkbook hssfworkbook;
            #region//初始化信息
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(fs);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            #endregion
            NPOI.SS.UserModel.ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            DataTable dt = new DataTable();
            rows.MoveNext();
            HSSFRow row = (HSSFRow)rows.Current;
            List<string> cols = new List<string>();
            for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)
            {
                cols.Add(row.GetCell(j).ToString().Trim());
                dt.Columns.Add(row.GetCell(j).ToString());
            }
            var colHead = GetTchColumn();
            if (!(colHead.All(cols.Contains) && cols.All(colHead.Contains)))
            {
                errnum = 0;
                return "表格格式错误，请重新下载模板";
            }
            cols.Add("错误原因");
            dt.Columns.Add("错误原因");
            while (rows.MoveNext())
            {
                row = (HSSFRow)rows.Current;
                if (!IsNull(row, 0) || !IsNull(row, 1) || !IsNull(row, 2))
                {
                    continue;
                }
                var school = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == row.GetCell(0).ToString());
                var ifTrue = false;
                if (SchoolID != "SchoolID" && school != null)
                {
                    ifTrue = !(SchoolID == school._id);
                }
                var sbsnm = exam.sbs.Select(s => s.sbnm).ToList();               
                if (!sbsnm.Contains(row.GetCell(2).ToString())||school ==null || ifTrue || !IsNull(row, 4) || !IsNull(row, 5) || !IsNull(row, 6)||!Function.MathPhone(row.GetCell(6).ToString()))
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < (sheet.GetRow(0).LastCellNum); i++)
                    {
                        NPOI.SS.UserModel.ICell cell = row.GetCell(i);
                        if (cell == null)
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            dr[i] = cell.ToString();
                        }
                    }
                    if (school == null)
                    {
                        dr[sheet.GetRow(0).LastCellNum] += "学校不存在;";
                    }
                    if (ifTrue)
                    {
                        dr[sheet.GetRow(0).LastCellNum] += "学校代号错误;";
                    }
                    if (!sbsnm.Contains(row.GetCell(2).ToString()))
                    {
                        dr[sheet.GetRow(0).LastCellNum] += "学科不存在;";
                    }
                    if (!Function.MathPhone(row.GetCell(6).ToString()))
                    {
                        dr[sheet.GetRow(0).LastCellNum] += "手机号不正确;";
                    }
                    dt.Rows.Add(dr);
                    continue;
                }

                Tch_Info tch = new Tch_Info();
                tch.eid = exam._id;
                tch.sid = row.GetCell(0).ToString();
                tch.snm = school.snm;
                tch.sb = row.GetCell(2).ToString().Replace(" ", "");
                tch.nm = row.GetCell(3).ToString();
                tch.sx = row.GetCell(4).ToString() == "男" ? 1 : 0;
                tch.zc = row.GetCell(5).ToString();
                tch.ph = Convert.ToInt64(row.GetCell(6).ToString());
                var sbinfo = exam.sbs.Where(w => w.sbnm == tch.sb).FirstOrDefault();
                if (sbinfo != null)
                    sbinfo.tchct++;
                MongoDbHelper.Insert(tch, DbName.Tch_Info);
            }
            MongoDbHelper.ReplaceOne(exam._id.ToString(), exam, DbName.E_Info);
            errnum = dt.Rows.Count;
            if (dt.Rows.Count > 0)
            {
                return BuildExcel1(cols.ToArray(), dt);
            }
            return "";
        }
        public static string ImportScoreExcel(string path, E_Info exam,out int errnum)
        {
            HSSFWorkbook hssfworkbook;
            #region//初始化信息
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(fs);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            #endregion
            NPOI.SS.UserModel.ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            DataTable dt = new DataTable();
            rows.MoveNext();
            HSSFRow row = (HSSFRow)rows.Current;
            List<string> cols = new List<string>();
            for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)
            {
                cols.Add(row.GetCell(j).ToString());
                dt.Columns.Add(row.GetCell(j).ToString());
            }
            cols.Add("错误原因");
            dt.Columns.Add("错误原因");
            while (rows.MoveNext())
            {
                row = (HSSFRow)rows.Current;
                if (!IsNull(row, 0) || !IsNull(row, 1) || !IsNull(row, 2))
                {
                    continue;
                }
                St_Sc stsc = new St_Sc();
                stsc.eid = exam._id;
                stsc.stid = row.GetCell(0).ToString();
                var st = MongoDbHelper.QueryOne<St_Info>(DbName.St_Info, w => w.eid == stsc.eid && w.stid == stsc.stid);
                if (st == null || !IsNull(row, 0) || !IsNull(row, 1) || !IsNull(row, 2) || !IsNull(row, 3))
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < (sheet.GetRow(0).LastCellNum); i++)
                    {
                        NPOI.SS.UserModel.ICell cell = row.GetCell(i);
                        if (cell == null)
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            dr[i] = cell.ToString();
                        }
                    }
                    if (st == null)
                    {
                        dr[sheet.GetRow(0).LastCellNum] += "考号不存在;";
                    }
                    else
                    {
                        dr[sheet.GetRow(0).LastCellNum] += "考生信息不能为空;";
                    }
                    dt.Rows.Add(dr);
                    continue;
                }
                stsc.cls = st.cls;
                stsc.nm = st.nm;
                stsc.idcd = st.idcd;
                stsc.sid = st.sid;
                stsc.s1 = IsNull(row, 3) ? Convert.ToDouble(row.GetCell(3).ToString()) : -1;//语文
                stsc.s2 = IsNull(row, 4) ? Convert.ToDouble(row.GetCell(4).ToString()) : -1;//文科数学
                stsc.s3 = IsNull(row, 5) ? Convert.ToDouble(row.GetCell(5).ToString()) : -1;//理科数学
                stsc.s4 = IsNull(row, 6) ? Convert.ToDouble(row.GetCell(6).ToString()) : -1;
                stsc.s5 = IsNull(row, 7) ? Convert.ToDouble(row.GetCell(7).ToString()) : -1;
                stsc.s6 = IsNull(row, 8) ? Convert.ToDouble(row.GetCell(8).ToString()) : -1;
                stsc.s7 = IsNull(row, 9) ? Convert.ToDouble(row.GetCell(9).ToString()) : -1;
                stsc.s8 = IsNull(row, 10) ? Convert.ToDouble(row.GetCell(10).ToString()) : -1;
                stsc.s9 = IsNull(row, 11) ? Convert.ToDouble(row.GetCell(11).ToString()) : -1;
                stsc.s10 = IsNull(row, 12) ? Convert.ToDouble(row.GetCell(12).ToString()) : -1;
                stsc.sc = IsNull(row, 13) ? Convert.ToDouble(row.GetCell(13).ToString()) : 0;
                MongoDbHelper.Insert(stsc, DbName.St_Sc);
            }
            var stus = MongoDbHelper.QueryBy<St_Info>(DbName.St_Info, w => w.eid == exam._id);
            var stscs = MongoDbHelper.QueryBy<St_Sc>(DbName.St_Sc, w => w.eid == exam._id).Select(s => s.stid).ToList();
            var errStu = stus.Where(w => !stscs.Contains(w.stid)).ToList();
            for (int st = 0; st < errStu.Count; st++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = errStu[st].stid;
                dr[1] = errStu[st].cls;
                dr[2] = errStu[st].nm;
                for (int i = 3; i < (sheet.GetRow(0).LastCellNum); i++)
                {
                    dr[i] = "0";
                }
                dr[sheet.GetRow(0).LastCellNum] += "缺少该考生成绩;";
                dt.Rows.Add(dr);
            }
            errnum = dt.Rows.Count;
            if (dt.Rows.Count > 0)
            {
                return BuildExcel1(cols.ToArray(), dt);
            }           
            return "";
        }
        public static string BuildPaperExcel(String[] columnName, DataTable table)
        {
            MemoryStream ms = new MemoryStream();
            using (table)
            {
                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                ICellStyle style = workbook.CreateCellStyle();
                //设置单元格的样式：水平对齐居中
                style.Alignment = HorizontalAlignment.Center;

                //新建一个字体样式对象
                IFont font = workbook.CreateFont();
                //设置字体加粗样式
                font.Boldweight = short.MaxValue;
                //使用SetFont方法将字体样式添加到单元格样式中 
                style.SetFont(font);
                //在工作表中：建立行，参数为行号，从0计
                IRow row = sheet.CreateRow(0);
                IRow row1 = sheet.CreateRow(1);
                //在行中：建立单元格，参数为列号，从0计
                ICell cell0 = row.CreateCell(0);
                //设置单元格内容
                cell0.SetCellValue("学校代号");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                //设置单元格内容
                cell1.SetCellValue("学校简称");
                cell1.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(0, 1, 0, 0));
                sheet.AddMergedRegion(new CellRangeAddress(0, 1, 1, 1));
                for (int i = 0; i < columnName.Length; i++)
                {
                    ICell celli = row.CreateCell(i * 3 + 2);
                    celli.SetCellValue(columnName[i]);
                    celli.CellStyle = style;
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, i * 3 + 2, i * 3 + 4));
                    ICell celli1 = row1.CreateCell(i * 3 + 2);
                    celli1.SetCellValue("应考");
                    ICell celli2 = row1.CreateCell(i * 3 + 3);
                    celli2.SetCellValue("实考");
                    ICell celli3 = row1.CreateCell(i * 3 + 4);
                    celli3.SetCellValue("缺考");
                }
                //IRow headerRow = sheet.CreateRow(0);
                //foreach (DataColumn column in table.Columns)
                //{
                //    headerRow.CreateCell(column.Ordinal).SetCellValue(columnName[column.Ordinal]);
                //}
                int rowIndex = 2;
                foreach (DataRow r in table.Rows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in table.Columns)
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(r[column].ToString());
                    }
                    rowIndex++;
                }
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
            }
            string fileName = "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd")))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd"));
            }
            var f = File.Create(System.AppDomain.CurrentDomain.BaseDirectory + fileName);
            ms.WriteTo(f);
            ms.Close();
            f.Close();
            return fileName;
        }

        public static bool IsNull(HSSFRow row, int i)
        {
            if (row.GetCell(i) != null && row.GetCell(i).ToString() != "")
            {
                return true;
            }
            return false;
        }
    }
}
