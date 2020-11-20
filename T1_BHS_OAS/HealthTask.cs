using System;
using System.Threading;
using T1_BHS_OAS.DBLib;


namespace T1_BHS_OAS
{
    public class HealthTask
    {
        #region =====[Private] Event=====



        #endregion


        #region =====[Protected] Event Raise=====



        #endregion


        #region =====[Public] Default Event Body=====



        #endregion


        #region =====[Private] Variable=====

        /// <summary>
        /// T1_BHS_DB的連線字串
        /// </summary>
        private readonly string m_sT1_BHS_DB = Properties.Settings.Default.T1_BHS_DB;

        /// <summary>
        /// T1_BHS_WEB_DB的連線字串
        /// </summary>
        private readonly string m_sT1_BHS_WEB_DB = Properties.Settings.Default.T1_BHS_WEB_DB;
        
        /// <summary>
        /// 設定檔資料表"ALL_EXE_CONF"中航班行李欲排程式的ID
        /// </summary>
        private readonly string m_sOAS_App = Properties.Settings.Default.OAS_App;

        /// <summary>
        /// 同步資源鎖定
        /// </summary>
        private readonly static object m_oHealthTaskLock = new object();

        /// <summary>
        /// 目前時間(yyyyMMddHHmmss.fff)
        /// </summary>
        private string m_sDateTimeNow = string.Empty;

        /// <summary>
        /// 執行緒處理間隔(1分鐘)
        /// </summary>
        private readonly int m_iThreadTimer = 60000;

        /// <summary>
        /// 執行緒開關
        /// </summary>
        private bool m_IsThreadRunning = false;

        /// <summary>
        /// 固定間隔時間的執行緒
        /// </summary>
        private Thread m_HealthCheckThread = null;

        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        #endregion


        #region =====[Public] Property=====

        /// <summary>
        /// HealthTask執行緒過程是否發生錯誤
        /// </summary>
        public bool Health_Error { get; private set; }

        #endregion


        #region =====[Public] Contructor & Destructor=====

        /// <summary>
        /// HealthTask類別建構子
        /// </summary>
        /// <param name="timer"></param>
        public HealthTask(int timer)
        {
            try
            {
                m_iThreadTimer = timer;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        /// <summary>
        /// HealthTask類別解構子
        /// </summary>
        ~HealthTask()
        {
            try
            {
                m_IsThreadRunning = false;
                m_HealthCheckThread = null;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        #endregion


        #region =====[Private] Thread=====

        /// <summary>
        /// 固定間隔時間的執行緒(更新目前時間至"ALL_EXE_CONF"為oas_app的Update_Time)
        /// </summary>
        private void HealthCheck()
        {
            lock (m_oHealthTaskLock)
            {
                while (m_IsThreadRunning)
                {
                    m_sDateTimeNow = DateTime.Now.ToString("yyyyMMddHHmmss.fff");
                    UtyTransaction m_oTransaction = new UtyTransaction();
                    Database m_dbT1BHSWEB = new Database(null);
                    OAS_SQL m_tbOasSql = null;
                    ALL_EXE_CONF m_tbAllExeConf = null;

                    try
                    {
                        T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter m_Adapter_OasSql = new T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter();
                        T1_BHS_WEB_DataSetTableAdapters.ALL_EXE_CONFTableAdapter m_Adapter_AllExeConf = new T1_BHS_WEB_DataSetTableAdapters.ALL_EXE_CONFTableAdapter();

                        // 資料庫物件建立與連線
                        m_dbT1BHSWEB = new Database(m_Adapter_OasSql.Connection);
                        if (m_dbT1BHSWEB.Connect() < 0)
                        {
                            TxtLog.Error("DB: [T1_BHS_web] 連線失敗！");
                        }
                        else
                        {
                            //=== 取得SQL指令 ===//
                            // 資料表物件建立
                            m_tbOasSql = new OAS_SQL(m_Adapter_OasSql.Connection);
                            // 取得目標資料
                            OAS_SQL.Row pSqlRow_u = (OAS_SQL.Row)m_tbOasSql.GetSQLByID(Properties.Settings.Default.U_AllExeConf_1);
                            OAS_SQL.Row pSqlRow_s = (OAS_SQL.Row)m_tbOasSql.GetSQLByID(Properties.Settings.Default.S_AllExeConf_1);

                            //=== 檢查變更目標後變更資料 ===//
                            // 資料表物件建立
                            m_tbAllExeConf = new ALL_EXE_CONF(m_Adapter_OasSql.Connection);
                            // 檢查欲變更的資料
                            if (m_tbAllExeConf.SelectBySQL(pSqlRow_s.SQL + string.Format(pSqlRow_s.CONDITION, Properties.Settings.Default.OAS_App, ALL_EXE_CONF.oOASApp.Update_Time) + pSqlRow_s.OTHER) > 0)
                            {
                                // 變更資料
                                ALL_EXE_CONF.Row pRow = (ALL_EXE_CONF.Row)m_tbAllExeConf.RecordList[0];
                                m_oTransaction.BeginTransaction(m_tbAllExeConf);
                                int pResult_Update = m_tbAllExeConf.UpdateBySQL(string.Format(pSqlRow_u.SQL, m_sDateTimeNow) + string.Format(pSqlRow_u.CONDITION, pRow.CONFIG_ID, pRow.CONF_KEY) + pSqlRow_u.OTHER);
                                m_oTransaction.SetTransactionResult(m_tbAllExeConf, (pResult_Update < 0) ? -1 : 1);
                                if (!m_oTransaction.EndTransaction())
                                {
                                    TxtLog.Info(string.Format("HealthCheck更新目前時間失敗！"));
                                }
                            }
                            else
                            {
                                TxtLog.Error(string.Format("資料庫中找不到CONFIG_ID: [{0}]; CONF_KEY: [{1}]的設定", Properties.Settings.Default.OAS_App, ALL_EXE_CONF.oOASApp.Update_Time));
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        TxtLog.Error(ex);
                        Health_Error = true;
                        StopThread();
                    }
                    finally
                    {
                        if (m_dbT1BHSWEB != null)
                        {
                            m_dbT1BHSWEB.Close();
                            m_dbT1BHSWEB = null;
                        }

                        m_tbOasSql = null;
                        m_tbAllExeConf = null;
                    }

                    Thread.Sleep(m_iThreadTimer);
                }
            }
        }

        #endregion


        #region =====[Private] Function=====



        #endregion


        #region =====[Public] Method=====

        #region RunThread()//void
        /// <summary>
        /// 啟動執行緒(T1_BHS_OAS應用程式狀態回報)
        /// </summary>
        public void RunThread()
        {
            try
            {
                m_IsThreadRunning = true;
                m_HealthCheckThread = new Thread(new ThreadStart(HealthCheck))
                {
                    IsBackground = true
                };
                m_HealthCheckThread.Start();
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                StopThread();
            }
        }
        #endregion

        #region StopThread()//void
        /// <summary>
        /// 執行緒啟動錯誤時將執行緒執行狀態設為關閉並等待1秒鐘
        /// </summary>
        public void StopThread()
        {
            try
            {
                m_IsThreadRunning = false;
                Thread.Sleep(1000);
                m_HealthCheckThread = null;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }
        #endregion

        #endregion
    }
}
