using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace T1_BHS_OAS.DBLib
{
    public class GA_CAROUSEL_TABLE_T1 : DBRecord
    {
        #region =====[Public] Class=====

        /// <summary>
        /// 資料庫表格欄位物件
        /// </summary>
        public class Row : AbstractRow
        {
            public int CAROUSEL_NO { get; set; }
            public string REGION { get; set; }
            public int IS_PROBLEM { get; set; }
            public int IS_USE { get; set; }
            public int UP_LIMIT_RUSH { get; set; }
            public int UP_LIMIT_NORMAL { get; set; }
            public string REMARK { get; set; }
            public string DATE_TIME { get; set; }
        }

        #endregion

        #region =====[Private] Parameter=====

        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        #endregion

        #region =====[Public] Property=====



        #endregion

        #region =====Constructor & Destructor=====

        /// <summary>
        /// GA_CAROUSEL_TABLE_T1類別建構子
        /// </summary>
        public GA_CAROUSEL_TABLE_T1(IDbConnection pConn) : base(pConn)
        {
            try
            {
                m_sTableName = Properties.Settings.Default.TableName_GA_CAROUSEL_TABLE_T1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// GA_CAROUSEL_TABLE_T1類別解構子
        /// </summary>
        ~GA_CAROUSEL_TABLE_T1()
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

        #region =====[Protected] Function (Extended)=====

        /// <summary>
        /// 擷取一筆資料表當前的目標列資料，使用前請將AbstractRow重作新的物件個體為Row
        /// </summary>
        /// <param name="pRs">擷取資料使用之執行個體命令物件</param>
        /// <returns>資料庫表格之列資料欄位物件</returns>
        protected override object FetchRecord(IDataReader pRs)
        {
            Row pRow = new Row();

            try
            {
                List<System.Reflection.PropertyInfo> props = new List<System.Reflection.PropertyInfo>(pRow.GetType().GetProperties());
                for (int i = 0; i < pRs.FieldCount; i++)
                {
                    string readerName = pRs.GetName(i);
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
            catch (Exception ex)
            {
                throw ex;
            }

            return pRow;
        }

        #endregion

        #region =====[Public] Method=====

        /// <summary>
        /// 依SQL指令字串取得對應的資料列
        /// </summary>
        /// <param name="pSQLStr">SQL指令字串(SELECT)</param>
        /// <returns>
        /// <para> n: 讀取的資料列筆數</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 非"SELECT"之SQL指令字串</para>
        /// </returns>
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
        /// 篩選"GA_CAROUSEL_TABLE_T1"全部的資料庫表格記錄
        /// </summary>
        /// <param name="pSQLStr">SQL指令字串(SELECT)</param>
        /// <returns>"Carousel Property Information"資料List</returns>
        public List<List<string>> GetAllTableList(string pSQLStr)
        {
            List<string> tempList = null;
            List<List<string>> slistRtn = new List<List<string>>();
            int iCount = SelectBySQL(pSQLStr);

            try
            {
                if (iCount > 0)
                {
                    Row pRow = null;
                    for (int i = 0; i < iCount; i++)
                    {
                        pRow = (Row)RecordList[i];
                        tempList = new List<string>
                        {
                            pRow.CAROUSEL_NO.ToString(), 
                            pRow.REGION, 
                            pRow.IS_PROBLEM.ToString(), 
                            pRow.IS_USE.ToString(), 
                            pRow.UP_LIMIT_RUSH.ToString(), 
                            pRow.UP_LIMIT_NORMAL.ToString(), 
                            pRow.REMARK
                        };
                        slistRtn.Add(tempList);
                    }

                    // 依CAROUSEL_NO作升冪排序
                    slistRtn = slistRtn.OrderBy(list => list[0]).ToList();
                }
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }

            return slistRtn;
        }

        #endregion
    }
}
