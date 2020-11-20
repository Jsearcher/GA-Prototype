using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace T1_BHS_OAS.DBLib
{
    /// <summary>
    /// 資料庫連線類別(使用資料庫之資料表Adapter類別的方式)
    /// </summary>
    public class Database : IDisposable
    {
        #region =====[Private] Variable=====

        /// <summary>
        /// 釋放資源處置旗標
        /// </summary>
        private bool m_IsDisposed = false;

        #endregion

        #region =====[Protected] Variable=====

        /// <summary>
        /// 資料庫連接物件
        /// </summary>
        protected IDbConnection m_oConn = null;

        #endregion

        #region =====[Public] Property=====

        /// <summary>
        /// 資料庫連接物件(由資料庫之資料表Adapter類別所屬)
        /// </summary>
        public IDbConnection Conn => m_oConn;

        #endregion

        #region =====[Public] Contructor & Destructor=====

        /// <summary>
        /// Database類別建構子
        /// </summary>
        /// <param name="pConn">資料庫連接物件</param>
        public Database(IDbConnection pConn)
        {
            try
            {
                m_oConn = pConn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Database類別解構子
        /// </summary>
        ~Database()
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

        #region =====[Public] Method=====

        /// <summary>
        /// 關閉連線並釋放所使用的非受控資源
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (!m_IsDisposed)
                {
                    m_IsDisposed = true;
                    m_oConn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 連接資料庫
        /// </summary>
        /// <returns>
        /// <para>-1: 例外錯誤</para>
        /// <para> 1: 連接成功</para>
        /// </returns>
        public int Connect()
        {
            int ret = -1;
            if (m_oConn == null)
            {
                return ret;
            }

            try
            {
                m_oConn.Open();
                ret = 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        /// <summary>
        /// 連接資料庫
        /// </summary>
        /// <param name="pConnStr">連線字串</param>
        /// <returns>
        /// <para>-1: 例外錯誤</para>
        /// <para> 1: 連接成功</para>
        /// </returns>
        public int Connect(string pConnStr)
        {
            m_oConn.ConnectionString = pConnStr;
            int ret = -1;
            try
            {
                m_oConn.Open();
                ret = 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        /// <summary>
        /// 終止連接資料庫
        /// </summary>
        public void Close()
        {
            try
            {
                if (m_oConn != null && !IsClosed())
                {
                    m_oConn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 判斷連接資料庫是否中斷
        /// </summary>
        /// <returns>
        /// <para> true: 資料庫連接已中斷</para>
        /// <para>false: 資料庫連接未中斷</para>
        /// </returns>
        public bool IsClosed()
        {
            bool ret = false;
            try
            {
                if (m_oConn.State == ConnectionState.Closed || m_oConn.State == ConnectionState.Broken)
                {
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        #endregion

    }

    /// <summary>
    /// Excel連線類別(原方式)
    /// </summary>
    public class ProjectExcel
    {
        #region =====[Private] Parameter=====

        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        #endregion

        #region =====[Private]=====

        /// <summary>
        /// 在連結資料來源前先測試連線字串中該路徑的檔案是否存在
        /// </summary>
        /// <param name="pConnect">連線字串</param>
        /// <param name="pParaName">資料來源參數名稱</param>
        /// <returns> true: 連線字串中該路徑的檔案存在</returns>
        /// <returns>false: 連線字串中該路徑的檔案不存在</returns>
        private static bool CheckBeforeConnect(string pConnect, out string pParaName)
        {
            string[] sSettings = null;
            string[] sParas = null;
            string sFilePath = string.Empty;
            string sIMEX = string.Empty;
            pParaName = string.Empty;

            try
            {
                // Check it exists or not first
                sSettings = ClassLibrary.FX.Text.Parsing.String.Split_static(pConnect.Replace("'", ""), ";");
                if (sSettings != null && sSettings.Length > 0)
                {
                    foreach (string sPara in sSettings)
                    {
                        sParas = ClassLibrary.FX.Text.Parsing.String.Split_static(sPara, "=");
                        if (sParas != null && sParas.Length == 2)
                        {
                            if (sParas[0].ToLower() == "data source")
                            {
                                pParaName = sParas[0].ToLower();
                                sFilePath = sParas[1];
                            }
                            else if (sParas[0].ToLower() == "imex")
                            {
                                sIMEX = sParas[1];
                            }
                        }
                    }
                }

                if (!File.Exists(sFilePath) && sIMEX != "0")
                {
                    TxtLog.Error(string.Format("檔案路徑: [{0}]不存在! ConnectionString = {1}", sFilePath, pConnect));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return false;
            }
        }

        #endregion

        #region =====[Public] Method=====

        /// <summary>
        /// 依連線字串的路徑連結資料來源
        /// </summary>
        /// <param name="pConnect">連線字串</param>
        /// <param name="pFilePath">資料來源路徑</param>
        /// <returns>資料來源物件</returns>
        public static Database Connect(string pConnect, string pFilePath)
        {
            Database m_dbExcel = null;
            string sParaName = string.Empty;
            pConnect = pConnect.Replace("filepath", pFilePath);

            try
            {
                if (CheckBeforeConnect(pConnect, out sParaName))
                {
                    if (sParaName == "data source")
                    {
                        m_dbExcel = new Database(new OleDbConnection());
                    }

                    if (m_dbExcel.Connect(pConnect) > 0)
                    {
                        return m_dbExcel;
                    }
                }
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }

            return new Database(null);
        }

        #endregion
    }

    /// <summary>
    /// 資料表查找與更動類別
    /// </summary>
    public abstract class DBRecord : IDisposable
    {
        /// <summary>
        /// 資料庫表格之列資料欄位類別(須覆寫各自列資料欄位的類別)
        /// </summary>
        public class AbstractRow { };

        #region =====[Private] Parameter=====

        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        /// <summary>
        /// 釋放資源處置旗標
        /// </summary>
        private bool m_IsDisposed = false;

        #endregion

        #region =====[Protected] Variable=====

        /// <summary>
        /// SQL指令字串
        /// </summary>
        protected string m_sSQLStr = string.Empty;

        /// <summary>
        /// 完整資料表名稱(包含登入使用者之預設結構描述)
        /// </summary>
        protected string m_sTableName = string.Empty;

        /// <summary>
        /// 擷取資料使用之執行個體命令物件
        /// </summary>
        protected IDataReader m_oResultSet = null;

        #endregion

        #region =====[Public] Property=====

        /// <summary>
        /// 資料庫表格記錄集合物件
        /// </summary>
        public List<object> RecordList { get; set; } = new List<object>();

        /// <summary>
        /// 完整資料表名稱(包含登入使用者之預設結構描述)
        /// </summary>
        public string TableName { get => m_sTableName; }

        /// <summary>
        /// SQL指令字串
        /// </summary>
        public string SQLStr => m_sSQLStr;

        /// <summary>
        /// 連接至資料來源時的SQL陳述式
        /// </summary>
        public IDbCommand m_oSqlCmd = null;

        #endregion

        #region =====Constructor & Destructor=====

        /// <summary>
        /// DBRecord類別建構子
        /// </summary>
        /// <param name="pConn">資料庫連接物件</param>
        public DBRecord(IDbConnection pConn)
        {
            try
            {
                if (pConn != null)
                {
                    m_oSqlCmd = pConn.CreateCommand();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// DBRecord類別解構子
        /// </summary>
        ~DBRecord()
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
        /// 擷取一筆資料表當前的目標列資料，使用前請將AbstractRow重作新的物件個體為Row
        /// </summary>
        /// <param name="pRs">擷取資料使用之執行個體命令物件</param>
        /// <returns>資料庫表格之列資料欄位物件</returns>
        protected abstract object FetchRecord(IDataReader pRs);

        /// <summary>
        /// 終止使用資料擷取執行個體命令物件並清空
        /// </summary>
        protected void CloseQuery()
        {
            if (m_oResultSet != null)
            {
                m_oResultSet.Close();
                m_oResultSet = null;
            }
        }

        /// <summary>
        /// 執行非擷取命令
        /// </summary>
        /// <returns>
        /// <para> n: 執行SQL指令成功受影響的資料列筆數</para>
        /// <para>-1: 執行失敗</para>
        /// </returns>
        protected int Execute()
        {
            int ret = -1;
            try
            {
                m_oSqlCmd.CommandText = m_sSQLStr;
                ret = m_oSqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        /// <summary>
        /// 執行擷取命令
        /// </summary>
        /// <returns>
        /// <para> true: 擷取成功</para>
        /// <para>false: 擷取失敗</para>
        /// </returns>
        protected bool ExecuteQuery()
        {
            bool ret = false;
            CloseQuery();
            try
            {
                m_oSqlCmd.CommandText = m_sSQLStr;
                m_oResultSet = m_oSqlCmd.ExecuteReader();
                ret = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        /// <summary>
        /// 讀取目前資料列並移到下一資料列
        /// </summary>
        /// <returns>
        /// <para> true: 尚有資料列需讀取</para>
        /// <para>false: 此為最後一資料列</para>
        /// </returns>
        protected bool NextRecord()
        {
            return m_oResultSet.Read();
        }

        /// <summary>
        /// 依輸入的"欄位名稱"讀取資料表列的欄位值
        /// </summary>
        /// <typeparam name="T">欄位的資料型態</typeparam>
        /// <param name="pRs">目前讀取的資料表列</param>
        /// <param name="pFieldName">欄位名稱</param>
        /// <returns>對應"欄位名稱"之資料表列的欄位值</returns>
        /// <returns>欄位的資料型態為"String"且為空值，則回傳空字串(string.Empty)</returns>
        protected T GetValueOrDefault<T>(IDataReader pRs, string pFieldName)
        {
            int ordinal = pRs.GetOrdinal(pFieldName);
            return GetValueOrDefault<T>(pRs, ordinal);
        }

        /// <summary>
        /// 依輸入的"欄位索引值"讀取資料表列的欄位值
        /// </summary>
        /// <typeparam name="T">欄位的資料型態</typeparam>
        /// <param name="pRs">目前讀取的資料表列</param>
        /// <param name="ordinal">欄位索引值</param>
        /// <returns>對應"欄位索引值"之資料表列的欄位值</returns>
        /// <returns>欄位的資料型態為"string"且為空值，則回傳空字串(string.Empty)</returns>
        protected T GetValueOrDefault<T>(IDataReader pRs, int ordinal)
        {
            if (typeof(T) == typeof(string))
            {
                object rtn = (pRs.IsDBNull(ordinal) ? string.Empty : pRs.GetValue(ordinal)).ToString();
                return (T)rtn;
            }
            return (T)(pRs.IsDBNull(ordinal) ? default(T) : pRs.GetValue(ordinal));
        }

        #endregion

        #region =====[Public] Method=====

        /// <summary>
        /// 停止擷取資料並釋放所使用的非受控資源
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (!m_IsDisposed)
                {
                    CloseQuery();
                    m_IsDisposed = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 依SQL指令擷取表格記錄
        /// </summary>
        /// <param name="pSQLStr">SQL指令字串(SELECT)</param>
        /// <returns>
        /// <para> n: 讀取的資料列筆數</para>
        /// <para>-1: 無SQL指令字串之輸入</para>
        /// </returns>
        public virtual int QuerybySQL(string pSQLStr)
        {
            if (string.IsNullOrEmpty(pSQLStr))
            {
                TxtLog.Info(string.Format("SQL string: [{0}]", pSQLStr));
                return -1;
            }

            RecordList.Clear();
            m_sSQLStr = pSQLStr;
            TxtLog.Info(string.Format("SQL string: [{0}]", pSQLStr));

            try
            {
                if (ExecuteQuery())
                {
                    while (NextRecord())
                    {
                        object pRow = FetchRecord(m_oResultSet);
                        if (pRow != null)
                        {
                            RecordList.Add(pRow);
                        }
                    }

                    CloseQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RecordList.Count;
        }

        /// <summary>
        /// 依SQL指令(非擷取)更動資料表格或資料列，如資料的新增、變更、移除或資料表的創建、刪除，須使用Transaction
        /// </summary>
        /// <param name="pSQLStr">SQL指令字串(INSERT INTO、UPDATE、DELETE FROM、CREATE、DROP...)</param>
        /// <returns>
        /// <para> n: 執行SQL指令成功受影響的資料列筆數</para>
        /// <para>-1: 無SQL指令字串之輸入</para>
        /// </returns>
        public virtual int ExecuteBySQL(string pSQLStr)
        {
            if (string.IsNullOrEmpty(pSQLStr))
            {
                TxtLog.Info(string.Format("SQL string: [{0}]", pSQLStr));
                return -1;
            }

            m_sSQLStr = pSQLStr;
            TxtLog.Info(string.Format("SQL string: [{0}]", pSQLStr));

            try
            {
                return Execute();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }

    /// <summary>
    /// 資料庫交易類別
    /// </summary>
    public class UtyTransaction
    {
        private class TransactionUnit
        {
            public IDbTransaction m_oTransaction;
            public int m_iResult;
        }

        #region =====[Private] Parameter=====

        private Dictionary<IDbConnection, TransactionUnit> mRegistry = new Dictionary<IDbConnection, TransactionUnit>();

        #endregion

        #region =====[Public] Method=====

        /// <summary>
        /// 資料庫交易開始
        /// </summary>
        /// <param name="pDataTable">資料表查找與更動之物件</param>
        public void BeginTransaction(DBRecord pDataTable)
        {
            if (!mRegistry.ContainsKey(pDataTable.m_oSqlCmd.Connection))
            {
                TransactionUnit pTranUnit = new TransactionUnit
                {
                    m_oTransaction = pDataTable.m_oSqlCmd.Connection.BeginTransaction(),
                    m_iResult = 0
                };
                pDataTable.m_oSqlCmd.Transaction = pTranUnit.m_oTransaction;

                mRegistry.Add(pDataTable.m_oSqlCmd.Connection, pTranUnit);
            }
            else
            {
                TransactionUnit pTranUnit = mRegistry[pDataTable.m_oSqlCmd.Connection];
            }
        }

        /// <summary>
        /// 資料庫交易執行成功筆數設定
        /// </summary>
        /// <param name="pDataTable">資料表查找與更動之物件</param>
        /// <param name="pResult">執行SQL指令成功受影響的資料列筆數</param>
        public void SetTransactionResult(DBRecord pDataTable, int pResult)
        {
            if (pResult < 0)
            {
                TransactionUnit pTranUnit = mRegistry[pDataTable.m_oSqlCmd.Connection];
                pTranUnit.m_iResult = pResult;
            }
        }

        /// <summary>
        /// 資料庫交易結束
        /// </summary>
        /// <returns>
        /// <para> true: 資料庫交易成功</para>
        /// <para>false: 資料庫交易失敗並將SQL指令執行回捲</para>
        /// </returns>
        public bool EndTransaction()
        {
            bool endResult = true;
            foreach (IDbConnection pKey in mRegistry.Keys)
            {
                TransactionUnit pTranUnit = mRegistry[pKey];
                if (pTranUnit.m_iResult < 0)
                {
                    endResult = false;
                    break;
                }
            }

            foreach (IDbConnection pKey in mRegistry.Keys)
            {
                TransactionUnit pTranUnit = mRegistry[pKey];

                if (endResult == true)
                {
                    pTranUnit.m_oTransaction.Commit();
                }
                else
                {
                    pTranUnit.m_oTransaction.Rollback();
                }
            }

            mRegistry.Clear();
            return endResult;
        }

        #endregion
    }
}
