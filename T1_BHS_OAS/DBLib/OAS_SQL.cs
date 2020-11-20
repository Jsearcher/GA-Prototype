using System;
using System.Collections.Generic;
using System.Data;

namespace T1_BHS_OAS.DBLib
{
    /// <summary>
    /// SQL碼查找類別
    /// </summary>
    public class OAS_SQL : DBRecord
    {
        #region =====[Public] Class=====

        /// <summary>
        /// 資料庫表格欄位物件
        /// </summary>
        public class Row : AbstractRow
        {
            public string CODE_ID { get; set; }
            public string SQL { get; set; }
            public string CONDITION { get; set; }
            public string OTHER { get; set; }
            public string DESCRIPTION { get; set; }
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
        /// OAS_SQL類別建構子
        /// </summary>
        /// <param name="pConn">資料庫連接物件</param>
        public OAS_SQL(IDbConnection pConn) : base(pConn)
        {
            try
            {
                m_sTableName = Properties.Settings.Default.TableName_OAS_SQL;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// OAS_SQL類別解構子
        /// </summary>
        ~OAS_SQL()
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
        /// 擷取一筆資料表當前的目標列資料，使用前請將AbstractRow重作新的物件個體Row
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

        #region =====[Public] Method (Extended)=====

        /// <summary>
        /// 資料表物件建立並依CODE_ID取得對應的SQL指令字串資料列
        /// </summary>
        /// <param name="pID">CODE_ID</param>
        /// <returns>對應CODE_ID的SQL指令字串資料列</returns>
        public object GetSQLByID(string pID)
        {
            object pSQLRow = null;

            // 資料表物件建立與取得目標資料
            try
            {
                m_sSQLStr = string.Format(Properties.Settings.Default.SQLStr_OSA_SQL, m_sTableName, pID);
                if (QuerybySQL(m_sSQLStr) < 0)
                {
                    TxtLog.Error(string.Format("查無 CODE_ID: [{0}] 對應的SQL指令字串資料列！", pID));
                }
                else
                {
                    pSQLRow = (Row)RecordList[0];
                }
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }

            return pSQLRow;
        }

        #endregion
    }
}
