﻿using System;
using System.Collections.Generic;
using System.Data;

namespace T1_BHS_OAS.DBLib
{
    public class LATERAL_CONF : DBRecord
    {
        #region =====[Public] Class=====

        /// <summary>
        /// 資料庫表格欄位物件
        /// </summary>
        public class Row : AbstractRow
        {
            public string LATERAL_ID { get; set; }
            public string TYPE { get; set; }
            public string AREA { get; set; }
            public string CHINESE { get; set; }
            public string ALIAS { get; set; }
            public string REMARK { get; set; }
            public string IN_USE { get; set; }
            public string USER_ID { get; set; }
            public string DATE_TIME { get; set; }
            public string SORT_CHUTE { get; set; }
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
        /// LATERAL_CONF類別建構子
        /// </summary>
        public LATERAL_CONF(IDbConnection pConn) : base(pConn)
        {
            try
            {
                m_sTableName = Properties.Settings.Default.TableName_LATERAL_CONF;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// LATERAL_CONF類別解構子
        /// </summary>
        ~LATERAL_CONF()
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

        #endregion
    }
}
