using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace T1_BHS_OAS.DBLib
{
    /// <summary>
    /// 航班行李預排資料"FlightData.xlsx" Excel表格類別
    /// </summary>
    public class XLSX_FlightDataPool : DBRecord
    {
        #region =====[Public] Class=====

        /// <summary>
        /// Excel表格欄位物件
        /// </summary>
        public class Row : AbstractRow
        {
            public string FLIGHT_NO { get; set; }
            public string FDATE { get; set; }
            public string STD { get; set; }
            public string DESTINATION { get; set; }
            public string PLANE { get; set; }
            public string TERMINAL { get; set; }
            public string CAROUSEL_NO { get; set; }
            public string GROUND { get; set; }
            public string FLIGHT_SIZE { get; set; }
            public string COUNTER_NO { get; set; }
            public string PREDICT_NO { get; set; }
            public string EOT { get; set; }
            public string ECT { get; set; }
        }

        #endregion

        #region =====[Private] Variable=====

        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        /// <summary>
        /// 是否讀取Excel資料表記錄的欄位名稱
        /// </summary>
        private bool m_IsFirstRow;

        /// <summary>
        /// 欄位名稱List
        /// </summary>
        private List<string> m_TitleList;

        #endregion

        #region =====[Public] Property=====

        /// <summary>
        /// 欄位名稱List
        /// </summary>
        public List<string> TITLEList { get { return m_TitleList; } }

        #endregion

        #region =====[Public] Constructor & Destructor=====

        /// <summary>
        /// 航班行李預排資料"FlightData.xlsx"Excel表格類別建構子(資料表名稱)
        /// </summary>
        /// <param name="pConn">ProjectExcel.Connect連接資料來源物件</param>
        public XLSX_FlightDataPool(IDbConnection pConn) : base(pConn)
        {
            try
            {
                m_sTableName = Properties.Settings.Default.TableName_XLSX_FlightData;
                m_IsFirstRow = true;
                //mFieldName = new string[] { "FLIGHT_NO", "FDATE", "STD", "DESTINATION", "PLANE", "TERMINAL", "CAROUSEL_NO",
                //                            "GROUND", "FLIGHT_SIZE", "COUNTER_NO", "PREDICT_NO", "EOT", "ECT" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 航班行李預排資料"FlightData.xlsx"Excel表格類別解構子
        /// </summary>
        ~XLSX_FlightDataPool()
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region =====[Protected] Function=====

        /// <summary>
        /// 擷取一筆"FlightData.xlsx"的Excel資料表的目標列資料，使用前請將AbstractRow重作新的物件個體為Row
        /// </summary>
        /// <param name="pRs">擷取資料使用之執行個體命令物件</param>
        /// <returns>資料庫表格之列資料欄位物件</returns>
        protected override object FetchRecord(IDataReader pRs)
        {
            Row pRow = new Row();

            try
            {
                if (m_IsFirstRow)
                {
                    // The first row is the fields of the table in Excel file, "FlightData.xlsx"
                    GetFieldName(pRs);
                    m_IsFirstRow = false;
                    return null;
                }
                else
                {
                    List<System.Reflection.PropertyInfo> props = new List<System.Reflection.PropertyInfo>(pRow.GetType().GetProperties());
                    for (int i = 0; i < pRs.FieldCount; i++)
                    {
                        string readerName = m_TitleList[i];
                        foreach (System.Reflection.PropertyInfo prop in props)
                        {
                            if (readerName == prop.Name)
                            {
                                if (prop.PropertyType == typeof(string))
                                {
                                    prop.SetValue(pRow, GetValueOrDefault<string>(pRs, i));
                                }
                                else
                                {
                                    prop.SetValue(pRow, GetValueOrDefault<object>(pRs, i));
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pRow;
        }

        /// <summary>
        /// 設定輸入一筆"FlightData.xlsx"資料庫表格記錄
        /// </summary>
        /// <param name="pSqlStr">"INSERT" SQL字串</param>
        /// <param name="pObj">資料庫表格欄位物件</param>
        /// <returns>
        /// <para> n: 新增的資料筆數</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        protected int SetRecord(string pSqlStr, object pObj)
        {
            return -1;
        }

        #endregion

        #region =====[Private] Function=====

        #region GetFieldName(IDataReader pRs)//int
        /// <summary>
        /// 取得"FlightData.xlsx"的Excel資料表欄位名稱
        /// </summary>
        /// <param name="pRs">IDataReader資料擷取物件</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 沒有取得欄位名稱</para>
        /// </returns>
        private int GetFieldName(IDataReader pRs)
        {
            m_TitleList = new List<string>();

            try
            {
                for (int i = 0; i < pRs.FieldCount; i++)
                {
                    m_TitleList.Add(pRs[i].ToString());
                }

                return 0;
            }
            catch
            {
                return -1;
            }
        }
        #endregion

        #endregion

        #region =====[Public] Method=====

        /// <summary>
        /// 依SQL指令字串取得對應的資料列(航班行李預排資料"FlightData.xlsx"整個的Excel資料表記錄)
        /// </summary>
        /// <param name="pSQLStr">SQL指令字串(SELECT)</param>
        /// <returns>
        /// <para> n: 讀取的資料列筆數</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 非"SELECT"之SQL指令字串</para></returns>
        public int SelectBySQL(string pSQLStr)
        {
            if (!pSQLStr.Contains("SELECT"))
            {
                TxtLog.Error(string.Format("SQL string: [{0}] 非'SELECT'之SQL指令字串！", pSQLStr));
                return -2;
            }
            m_sSQLStr = pSQLStr;

            try
            {
                return QuerybySQL(m_sSQLStr);
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }

        /// <summary>
        /// 依SQL指令字串取得航班行李預排資料"FlightData.xlsx"的Excel資料表記錄(List)
        /// </summary>
        /// <param name="pSQLStr">SQL指令字串(SELECT)</param>
        /// <returns>航班行李預排資料"FlightData.xlsx"資料List</returns>
        public List<List<string>> GetAllTableList(string pSQLStr)
        {
            List<string> tempList = null;
            List<List<string>> slistRtn = new List<List<string>>();
            int pCount = SelectBySQL(pSQLStr);

            if (pCount > 0)
            {
                Row pRow = null;
                for (int i = 0; i < pCount; i++)
                {
                    pRow = (Row)RecordList[i];
                    tempList = new List<string>
                    {
                        pRow.FLIGHT_NO,
                        pRow.FDATE,
                        pRow.STD,
                        pRow.DESTINATION,
                        pRow.PLANE,
                        pRow.TERMINAL,
                        pRow.CAROUSEL_NO,
                        pRow.GROUND,
                        pRow.FLIGHT_SIZE,
                        pRow.COUNTER_NO,
                        pRow.PREDICT_NO,
                        pRow.EOT,
                        pRow.ECT
                    };
                    slistRtn.Add(tempList);
                }

                // 依FDATE=>STD=>FLIGHT_NO作升冪排序
                slistRtn = slistRtn.OrderBy(list => list[1]).ThenBy(list => list[2]).ThenBy(list => list[0]).ToList();
            }

            return slistRtn;
        }

        #endregion
    }
}
