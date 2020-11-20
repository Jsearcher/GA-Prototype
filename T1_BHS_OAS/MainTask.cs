using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using T1_BHS_OAS.DBLib;

namespace T1_BHS_OAS
{
    public class MainTask
    {
        #region =====[Private] Event=====



        #endregion

        #region =====[Public] Default Event Body=====



        #endregion

        #region =====[Private] Variable=====

        /// <summary>
        /// T1目前可使用之轉盤列表(LATERAL_ID僅留數值)
        /// </summary>
        private List<int> m_CarouselList = null;

        /// <summary>
        /// T1轉盤屬性設定值List
        /// </summary>
        private List<object> m_CarouselProperty = null;

        /// <summary>
        /// T1航班行李報到櫃檯屬性設定值List
        /// </summary>
        private List<object> m_CounterProperty = null;

        /// <summary>
        /// 基因演算限制條件開關(自定義)字典
        /// </summary>
        private Dictionary<string, bool> m_ConstraintsSW = null;

        /// <summary>
        /// 基因演算限制條件(函數之參數矩陣)字典
        /// </summary>
        private Dictionary<string, Array> m_ConstraintsGA = null;

        /// <summary>
        /// 基因演算選項參數字典
        /// </summary>
        private Dictionary<string, object> m_Options = null;

        /// <summary>
        /// 預排選擇日期
        /// </summary>
        private string m_sDate = string.Empty;

        /// <summary>
        /// 預計開櫃時間
        /// </summary>
        private string m_sTotalTime_EOT = string.Empty;

        /// <summary>
        /// 預計關櫃時間
        /// </summary>
        private string m_sTotalTime_ECT = string.Empty;

        /// <summary>
        /// 匯入的Excel檔名
        /// </summary>
        private string m_sFileName = string.Empty;

        /// <summary>
        /// 匯出的CSV檔名
        /// </summary>
        private string m_sSaveFileName = string.Empty;

        /// <summary>
        /// 匯入的Excel資料夾位置
        /// </summary>
        private string m_sFileDirectory = string.Empty;

        /// <summary>
        /// 匯出存檔的CSV資料夾位置
        /// </summary>
        private string m_sSaveFileDirectory = string.Empty;

        /// <summary>
        /// 匯入的Excel檔案路徑
        /// </summary>
        private string m_sFilePath = string.Empty;

        /// <summary>
        /// 匯出存檔的CSV檔案路徑
        /// </summary>
        private string m_sSaveFilePath = string.Empty;

        /// <summary>
        /// T1_BHS_WEB_DB的連線字串
        /// </summary>
        private readonly string m_sT1_BHS_WEB_DB = Properties.Settings.Default.T1_BHS_WEB_DB;

        /// <summary>
        /// Read Excel file的連線字串(.xlsx)
        /// </summary>
        private readonly string m_sRead_xlsx = Properties.Settings.Default.Read_xlsx;

        /// <summary>
        /// 設定檔資料表"ALL_EXE_CONF"中航班行李預排程式的ID
        /// </summary>
        private readonly string m_sOAS_App = Properties.Settings.Default.OAS_App;

        /// <summary>
        /// 設定檔資料表"ALL_EXE_CONF"中航班行李預排程式演算法設定值的ID
        /// </summary>
        private readonly string m_sGA_Config = Properties.Settings.Default.GA_Config;

        /// <summary>
        /// 同步資源鎖定
        /// </summary>
        private readonly static object m_oMainTaskLock = new object();

        /// <summary>
        /// 固定間隔時間的執行緒
        /// </summary>
        private Thread m_ScheduleThread = null;

        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        #endregion

        #region =====[Public] Property=====

        /// <summary>
        /// 資料集物件(航班行李之轉盤配置預排資料集)
        /// </summary>
        public DataPool DataPoolSet { get; private set; }

        /// <summary>
        /// 目前預排演算進度
        /// </summary>
        public double ProgressRate { get; private set; }

        /// <summary>
        /// 執行緒開關
        /// </summary>
        public bool IsThreadRunning = false;

        /// <summary>
        /// MainTask航班預排最佳化演算完成與否
        /// </summary>
        public bool OAS_Finished { get; private set; }

        /// <summary>
        /// MainTask執行緒過程是否發生錯誤
        /// </summary>
        public bool OAS_Error { get; private set; }

        #endregion

        #region =====[Public] Constructor & Destructor=====

        /// <summary>
        /// MainTask類別建構子
        /// </summary>
        public MainTask()
        {
            try
            {
                DefaultSettings();
                OAS_Finished = false;
                OAS_Error = false;
                m_ConstraintsGA = new Dictionary<string, Array>();
                m_Options = new Dictionary<string, object>();
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        /// <summary>
        /// MainTask類別解構子
        /// </summary>
        ~MainTask()
        {
            try
            {
                m_ConstraintsSW = null;
                m_ConstraintsGA = null;
                m_Options = null;
                DataPoolSet = null;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        #endregion

        #region =====[Private] Thread=====

        /// <summary>
        /// 固定間隔時間的執行緒(啟動航班行李轉盤預排演算)
        /// </summary>
        private void CarouselSchedule()
        {
            lock (m_oMainTaskLock)
            {
                while (IsThreadRunning)
                {
                    try
                    {
                        // 基因演算法開始時間點
                        DateTime TimeStart = DateTime.Now;

                        // 匯入選擇日期之預排配置輸入資料 FlightData.xlsx (Import Excel File)
                        // FlightData List includes ------------------------------------------- //
                        // SERIAL_NO
                        // CAROUSEL_NO
                        // FLIGHT_NO
                        // FDATE
                        // STD
                        // EOT
                        // ECT
                        // DESTINATION
                        // GROUND (TIAS or EGAS)
                        // FLIGHT_SIZE (L, M, or S)
                        // COUNTER_NO
                        // PREDICT_NO
                        // -------------------------------------------------------------------- //
                        if (DataPoolSet.ImportFlightData(m_sRead_xlsx, m_sFilePath) == 0)
                        {
                            TxtLog.Info(string.Format("成功匯入航班行李之轉盤配置預排輸入資料，筆數: [{0}]筆，位置: [{1}]", DataPoolSet.OASResultSet.FLIGHT_NO.Count, m_sFilePath));
                            Console.WriteLine(string.Format("[{0}]|成功匯入航班行李之轉盤配置預排輸入資料，筆數: [{1}]筆，位置: [{2}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), DataPoolSet.OASResultSet.FLIGHT_NO.Count, m_sFilePath));
                            if (DataPoolSet.OASResultSet.FLIGHT_NO.Count == 0)
                            {
                                OAS_Error = true;
                                StopThread();
                                break;
                            }
                        }
                        else
                        {
                            OAS_Error = true;
                            StopThread();
                            break;
                        }

                        // 讀取選擇日期前與設定的開櫃時間內的航班行李轉盤配置資料
                        if (DataPoolSet.GetLastCarouselList() == 0)
                        {
                            TxtLog.Info(string.Format("成功讀取選擇日期前與設定的開櫃時間內的航班行李轉盤配置資料，筆數: [{0}]筆", DataPoolSet.LastCarouselList.Count));
                            Console.WriteLine(string.Format("[{0}]|成功讀取選擇日期前與設定的開櫃時間內的航班行李轉盤配置資料，筆數: [{1}]筆", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), DataPoolSet.LastCarouselList.Count));
                            //if (DataPoolSet.LastCarouselList.Count == 0)
                            //{
                            //    OAS_Error = true;
                            //    StopThread();
                            //    break;
                            //}
                        }
                        else
                        {
                            OAS_Error = true;
                            StopThread();
                            break;
                        }

                        // 基因演算選項參數字典值設定，包含 演算法限制條件(計算適應值函數之參數矩陣 + 自定義限制條件所需之相關參數 + 自定義限制條件開關) 與 演算法參數
                        if (SetOptionsValue() != 0)
                        {
                            OAS_Error = true;
                            StopThread();
                            break;
                        }

                        // 基因演算法解航班行李之轉盤配置最佳化
                        GA.Genetic ga = new GA.Genetic();
                        try
                        {
                            // 初始化並儲存演算進度 0 至ALL_EXE_CONF資料表
                            ProgressRate = 0;
                            UpdateAllExeConf(ALL_EXE_CONF.oOASApp.Progress_Rate, ProgressRate.ToString());

                            ga.SetGAOptions(m_Options, DataPoolSet.OASResultSet.FLIGHT_NO.Count + DataPoolSet.LastCarouselList.Count);
                            ga.Start(GA.Genetic.CustomFitnessFcn, DataPoolSet.OASResultSet.FLIGHT_NO.Count);

                            // 等待基因演算執行緒Genetic.mGeneticThread執行結束
                            while (!ga.CheckStatus())
                            {
                                // 儲存演算進度至ALL_EXE_CONF資料表
                                ProgressRate = ga.ProgressRate;
                                UpdateAllExeConf(ALL_EXE_CONF.oOASApp.Progress_Rate, ProgressRate.ToString());
                                Thread.Sleep(1000); // 每隔一秒檢查基因演算執行緒是否停止
                            }
                        }
                        catch (Exception ex)
                        {
                            TxtLog.Error(ex);
                            ga.Stop();
                        }

                        // 基因演算法執行結束後取得 最佳基因組合 最小適應值 及 演算迭代次數
                        for (int i = 0; i < DataPoolSet.OASResultSet.CAROUSEL_NO.Count; i++)
                        {
                            DataPoolSet.OASResultSet.CAROUSEL_NO[i] = DataPoolSet.OASResultSet.CAROUSEL_NO[i] == 0 ? Convert.ToInt32(ga.BestChromosome[i]) : DataPoolSet.OASResultSet.CAROUSEL_NO[i];
                        }
                        DataPoolSet.BestOverlapTime = (int)ga.BestFitnessValue;
                        DataPoolSet.Iteration = ga.Iteration;

                        // 依轉盤號碼將預排結果所需資料排序
                        DataPoolSet.OrderByCarouselNo();

                        DateTime TimeStop = DateTime.Now;   // Time count stop
                        UpdateAllExeConf(ALL_EXE_CONF.oOASApp.Last_Iteration, DataPoolSet.Iteration.ToString());
                        DataPoolSet.Proc_TimeCost = (int)(Convert.ToDouble((TimeStop - TimeStart).TotalMilliseconds.ToString()) / 1000);
                        Console.WriteLine(string.Format("[{0}]|最佳演算重疊時間: {1} 分鐘", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), DataPoolSet.BestOverlapTime));
                        Console.WriteLine(string.Format("[{0}]|最佳演算花費時間: {1} 秒鐘", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), DataPoolSet.Proc_TimeCost));
                        TxtLog.Info(string.Format("最佳演算重疊時間: {0} 分鐘", DataPoolSet.BestOverlapTime));
                        TxtLog.Info(string.Format("最佳演算花費時間: {0} 秒鐘", DataPoolSet.Proc_TimeCost));

                        // 匯出航班行李之轉盤配置預排結果資料 FlightResult.csv (Export CSV File)
                        if (DataPoolSet.ExportFlightResult(m_sSaveFilePath) == 0)
                        {
                            Console.WriteLine(string.Format("[{0}]|成功匯出航班行李之轉盤配置預排結果資料，筆數: [{1}]筆，位置: [{2}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), DataPoolSet.OASResultSet.FLIGHT_NO.Count, m_sSaveFilePath));
                            TxtLog.Info(string.Format("成功匯出航班行李之轉盤配置預排結果資料，筆數: [{0}]筆，位置: [{1}]", DataPoolSet.OASResultSet.FLIGHT_NO.Count, m_sSaveFilePath));
                        }

                        // 航班行李之轉盤配置預排完成之設定
                        OAS_Finished = true;

                        // 等待關閉Thread
                        Thread.Sleep(10000);
                    }
                    catch (Exception ex)
                    {
                        TxtLog.Error(ex);
                        OAS_Error = true;
                        StopThread();
                        break;
                    }
                }
            }
        }

        #endregion

        #region =====[Private] Function=====

        #region DefaultSettings()//void
        /// <summary>
        /// 預設的變數設定(預排選擇日期、匯入資料路徑及匯出資料路徑)
        /// </summary>
        private void DefaultSettings()
        {
            Properties.Settings.Default.Reset();
            m_sTotalTime_EOT = SelectAllExeConf(Properties.Settings.Default.GA_Config, ALL_EXE_CONF.oGAConfig.EstimatedOpenTime);
            m_sTotalTime_ECT = SelectAllExeConf(Properties.Settings.Default.GA_Config, ALL_EXE_CONF.oGAConfig.EstimatedCloseTime);
            m_CarouselList = GetCarouselList();
            m_CarouselProperty = GetCarouselProperty();
            m_CounterProperty = GetCounterProperty();
            m_sFileName = string.IsNullOrEmpty(m_sFileName) ? Properties.Settings.Default.ReadFileName : m_sFileName;
            m_sFileDirectory = string.IsNullOrEmpty(m_sFileDirectory) ? Properties.Settings.Default.ReadFileDirectory : m_sFileDirectory;
            m_sSaveFileDirectory = string.IsNullOrEmpty(m_sSaveFileDirectory) ? Properties.Settings.Default.SaveFileDirectory : m_sSaveFileDirectory;
            m_sFilePath = Path.Combine(m_sFileDirectory, m_sFileName);

            // 設定基因演算限制條件開關(自定義 from App.config) - 預設
            m_ConstraintsSW = new Dictionary<string, bool>
            {
                { GA.Constraints.cSW_ACC, Properties.Settings.Default.SW_ACC }, // 允許航班行李指定配置轉盤之限制開關
                { GA.Constraints.cSW_RC, Properties.Settings.Default.SW_RC },   // 允許區分航班尖峰與離峰之轉盤共用限制開關
                { GA.Constraints.cSW_GDC, Properties.Settings.Default.SW_GDC }, // 允許航班所屬地勤不同不共用配置轉盤之限制開關
                { GA.Constraints.cSW_DSC, Properties.Settings.Default.SW_DSC }, // 允許航班目的地相似不共用配置轉盤之限制開關
                { GA.Constraints.cSW_FSC, Properties.Settings.Default.SW_FSC }, // 允許航班大小偏大不共用配置轉盤之限制開關
                //{ GA.Constraints.cSW_CCC, Properties.Settings.Default.SW_CCC }  // 允許所屬報到櫃檯需對應特定轉盤之限制開關
            };  
        }
        #endregion

        #region GetCarouselList()//List<string>
        /// <summary>
        /// 取出T1目前可使用之轉盤列表
        /// </summary>
        /// <returns>T1目前可使用之轉盤列表</returns>
        private List<int> GetCarouselList()
        {
            Database m_dbT1BHSWEB = new Database(null);
            Database m_dbT1BHS = new Database(null);
            OAS_SQL m_tbOasSql = null;
            LATERAL_CONF m_tbLateralConf = null;

            try
            {
                T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter m_Adapter_OasSql = new T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter();
                T1_BHSDataSetTableAdapters.LATERAL_CONFTableAdapter m_Adapter_LateralConf = new T1_BHSDataSetTableAdapters.LATERAL_CONFTableAdapter();

                // 資料庫物件建立與連線
                m_dbT1BHSWEB = new Database(m_Adapter_OasSql.Connection);
                m_dbT1BHS = new Database(m_Adapter_LateralConf.Connection);
                if (m_dbT1BHSWEB.Connect() < 0 || m_dbT1BHS.Connect() < 0)
                {
                    TxtLog.Error("DB: [T1_BHS_web] 或 [T1_BHS] 連線失敗！");
                    return null;
                }
                else
                {
                    //=== 取得SQL指令 ===//
                    // 資料表物件建立
                    m_tbOasSql = new OAS_SQL(m_Adapter_OasSql.Connection);
                    // 取得目標資料
                    OAS_SQL.Row pSqlRow_s = (OAS_SQL.Row)m_tbOasSql.GetSQLByID(Properties.Settings.Default.S_LateralConf_1);

                    //=== 取得行李轉盤屬性設定List ===//
                    m_tbLateralConf = new LATERAL_CONF(m_Adapter_LateralConf.Connection);
                    int pCount = m_tbLateralConf.SelectBySQL(pSqlRow_s.SQL + pSqlRow_s.CONDITION + pSqlRow_s.OTHER);
                    if (pCount > 0)
                    {
                        List<int> slistRtn = new List<int>();
                        for (int i = 0; i < pCount; i++)
                        {
                            slistRtn.Add(int.Parse(Regex.Replace(((LATERAL_CONF.Row)m_tbLateralConf.RecordList[i]).LATERAL_ID, @"[^\d]", string.Empty)));
                        }
                        return slistRtn;
                    }
                    else
                    {
                        TxtLog.Error("查詢T1目前使用之轉盤列表的SQL指令有問題！");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return null;
            }
            finally
            {
                if (m_dbT1BHSWEB != null)
                {
                    m_dbT1BHSWEB.Close();
                    m_dbT1BHSWEB = null;
                }
                if (m_dbT1BHS != null)
                {
                    m_dbT1BHS.Close();
                    m_dbT1BHS = null;
                }

                m_tbOasSql = null;
                m_tbLateralConf = null;
            }
        }
        #endregion

        #region GetCarouselProperty()//List<List<string>>
        /// <summary>
        /// 取出T1行李轉盤屬性設定列表
        /// </summary>
        /// <returns>T1轉盤屬性設定值List</returns>
        private List<object> GetCarouselProperty()
        {
            Database m_dbT1BHSWEB = new Database(null);
            OAS_SQL m_tbOasSql = null;
            GA_CAROUSEL_TABLE_T1 m_tbGaCarouselTableT1 = null;

            try
            {
                T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter m_Adapter_OasSql = new T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter();

                // 資料庫物件建立與連線
                m_dbT1BHSWEB = new Database(m_Adapter_OasSql.Connection);
                if (m_dbT1BHSWEB.Connect() < 0)
                {
                    TxtLog.Error("DB: [T1_BHS_web] 連線失敗！");
                    return null;
                }
                else
                {
                    //=== 取得SQL指令 ===//
                    // 資料表物件建立
                    m_tbOasSql = new OAS_SQL(m_Adapter_OasSql.Connection);
                    // 取得目標資料
                    OAS_SQL.Row pSqlRow_s = (OAS_SQL.Row)m_tbOasSql.GetSQLByID(Properties.Settings.Default.S_GACarouselTableT1_1);

                    //=== 取得行李轉盤屬性設定List ===//
                    m_tbGaCarouselTableT1 = new GA_CAROUSEL_TABLE_T1(m_Adapter_OasSql.Connection);
                    if (m_tbGaCarouselTableT1.SelectBySQL(pSqlRow_s.SQL + pSqlRow_s.CONDITION + pSqlRow_s.OTHER) > 0)
                    {
                        return m_tbGaCarouselTableT1.RecordList;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return null;
            }
            finally
            {
                if (m_dbT1BHSWEB != null)
                {
                    m_dbT1BHSWEB.Close();
                    m_dbT1BHSWEB = null;
                }

                m_tbOasSql = null;
                m_tbGaCarouselTableT1 = null;
            }
        }
        #endregion

        #region GetCounterProperty()//List<List<string>>
        /// <summary>
        /// 取出T1航班行李報到櫃檯屬性設定列表
        /// </summary>
        /// <returns>T1轉盤屬性設定值List</returns>
        private List<object> GetCounterProperty()
        {
            Database m_dbT1BHSWEB = new Database(null);
            OAS_SQL m_tbOasSql = null;
            GA_COUNTER_TABLE_T1 m_tbGaCounterTableT1 = null;

            try
            {
                T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter m_Adapter_OasSql = new T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter();

                // 資料庫物件建立與連線
                m_dbT1BHSWEB = new Database(m_Adapter_OasSql.Connection);
                if (m_dbT1BHSWEB.Connect() < 0)
                {
                    TxtLog.Error("DB: [T1_BHS_web] 連線失敗！");
                    return null;
                }
                else
                {
                    //=== 取得SQL指令 ===//
                    // 資料表物件建立
                    m_tbOasSql = new OAS_SQL(m_Adapter_OasSql.Connection);
                    // 取得目標資料
                    OAS_SQL.Row pSqlRow_s = (OAS_SQL.Row)m_tbOasSql.GetSQLByID(Properties.Settings.Default.S_GACounterTableT1_1);

                    //=== 取得行李報到櫃檯屬性設定List ===//
                    m_tbGaCounterTableT1 = new GA_COUNTER_TABLE_T1(m_Adapter_OasSql.Connection);
                    if (m_tbGaCounterTableT1.SelectBySQL(pSqlRow_s.SQL + pSqlRow_s.CONDITION + pSqlRow_s.OTHER) > 0)
                    {
                        return m_tbGaCounterTableT1.RecordList;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return null;
            }
            finally
            {
                if (m_dbT1BHSWEB != null)
                {
                    m_dbT1BHSWEB.Close();
                    m_dbT1BHSWEB = null;
                }

                m_tbOasSql = null;
                m_tbGaCounterTableT1 = null;
            }
        }
        #endregion

        #region SelectAllExeConf(string pID, string pKey)//string
        /// <summary>
        /// 選擇航班行李之轉盤配置預排應用程式設定值
        /// </summary>
        /// <param name="pID">"CONFIG_ID"篩選條件的ID</param>
        /// <param name="pKey">"CONF_KEY"篩選條件的KEY</param>
        /// <returns>"CONF_VALUE"對應的設定值</returns>
        private string SelectAllExeConf(string pID, string pKey)
        {
            Database m_dbT1BHSWEB = new Database(null);
            OAS_SQL m_tbOasSql = null;
            ALL_EXE_CONF m_tbAllExeConf = null;

            try
            {
                T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter m_Adapter_OasSql = new T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter();

                // 資料庫物件建立與連線
                m_dbT1BHSWEB = new Database(m_Adapter_OasSql.Connection);
                if (m_dbT1BHSWEB.Connect() < 0)
                {
                    TxtLog.Error("DB: [T1_BHS_web] 連線失敗！");
                    return null;
                }
                else
                {
                    //=== 取得SQL指令 ===//
                    // 資料表物件建立
                    m_tbOasSql = new OAS_SQL(m_Adapter_OasSql.Connection);
                    m_tbAllExeConf = new ALL_EXE_CONF(m_Adapter_OasSql.Connection);
                    // 取得目標資料
                    OAS_SQL.Row pSqlRow_s = (OAS_SQL.Row)m_tbOasSql.GetSQLByID(Properties.Settings.Default.S_AllExeConf_1);

                    //=== 依SQL指令取得目標參數值 ===//
                    if (m_tbAllExeConf.SelectBySQL(pSqlRow_s.SQL + string.Format(pSqlRow_s.CONDITION, pID, pKey) + pSqlRow_s.OTHER) > 0)
                    {
                        ALL_EXE_CONF.Row pRow = (ALL_EXE_CONF.Row)m_tbAllExeConf.RecordList[0];
                        return pRow.CONF_VALUE;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return null;
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
        }
        #endregion

        #region UpdateAllExeConf(string pKey, string pValue)//int
        /// <summary>
        /// 更新在"ALL_EXE_CONF"資料表中的航班行李之轉盤配置預排應用程式設定值
        /// </summary>
        /// <param name="pKey">"CONF_KEY"篩選條件的KEY</param>
        /// <param name="pValue">"CONF_VALUE"對應的設定值</param>
        /// <returns>
        /// <para>大於0: 成功設定</para>
        /// <para> 0: 設定錯誤Rollback Transaction</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: ALL_EXE_CONF資料表中沒有指定的KEY</para>
        /// <para>-3: 資料庫連線失敗</para>
        /// </returns>
        private int UpdateAllExeConf(string pKey, string pValue)
        {
            UtyTransaction m_oTransaction = new UtyTransaction();
            Database m_dbT1BHSWEB = new Database(null);
            OAS_SQL m_tbOasSql = null;
            ALL_EXE_CONF m_tbAllExeConf = null;

            try
            {
                T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter m_Adapter_OasSql = new T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter();

                // 資料庫物件建立與連線
                m_dbT1BHSWEB = new Database(m_Adapter_OasSql.Connection);
                if (m_dbT1BHSWEB.Connect() < 0)
                {
                    Console.WriteLine(string.Format("[{0}]|DB: [T1_BHS_web] 連線失敗！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-3]|DB: [T1_BHS_web] 連線失敗！");
                    return -3;
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
                    if (m_tbAllExeConf.SelectBySQL(pSqlRow_s.SQL + string.Format(pSqlRow_s.CONDITION, Properties.Settings.Default.OAS_App, pKey) + pSqlRow_s.OTHER) > 0)
                    {
                        // 變更資料
                        ALL_EXE_CONF.Row pRow = (ALL_EXE_CONF.Row)m_tbAllExeConf.RecordList[0];
                        m_oTransaction.BeginTransaction(m_tbAllExeConf);
                        int pResult_Update = m_tbAllExeConf.UpdateBySQL(string.Format(pSqlRow_u.SQL, pValue) + string.Format(pSqlRow_u.CONDITION, pRow.CONFIG_ID, pRow.CONF_KEY) + pSqlRow_u.OTHER);
                        m_oTransaction.SetTransactionResult(m_tbAllExeConf, (pResult_Update < 0) ? -1 : 1);
                        if (!m_oTransaction.EndTransaction())
                        {
                            Console.WriteLine(string.Format("[{0}]|變更CONFIG_ID: [{1}], CONF_KEY: [{2}], CONF_VALUE: [{3}]時之變更交易回捲！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Properties.Settings.Default.OAS_App, pKey, pValue));
                            TxtLog.Error(string.Format("CODE[0]|變更CONFIG_ID: [{0}], CONF_KEY: [{1}], CONF_VALUE: [{2}]時之變更交易回捲！", Properties.Settings.Default.OAS_App, pKey, pValue));
                            return 0;
                        }
                        else
                        {
                            return pResult_Update;
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("[{0}]|資料庫中找不到CONFIG_ID: [{1}]; CONF_KEY: [{2}]的設定值！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Properties.Settings.Default.OAS_App, ALL_EXE_CONF.oOASApp.Update_Time));
                        TxtLog.Error(string.Format("CODE[-2]|資料庫中找不到CONFIG_ID: [{0}]; CONF_KEY: [{1}]的設定值！", Properties.Settings.Default.OAS_App, ALL_EXE_CONF.oOASApp.Update_Time));
                        return -2;
                    }
                }
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
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
        }
        #endregion

        #region SetOptionsValue()//int
        /// <summary>
        /// 基因演算選項參數字典值設定
        /// </summary>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para></returns>
        private int SetOptionsValue()
        {
            try
            {
                // 設定基因演算限制條件開關(自定義 from DB_T1_BHS_web) - 變更
                m_ConstraintsSW[GA.Constraints.cSW_ACC] = SelectAllExeConf(Properties.Settings.Default.OAS_App, ALL_EXE_CONF.oOASApp.LaterAttr) == "1";
                m_ConstraintsSW[GA.Constraints.cSW_RC] = SelectAllExeConf(Properties.Settings.Default.OAS_App, ALL_EXE_CONF.oOASApp.OtherAttr) == "1";
                m_ConstraintsSW[GA.Constraints.cSW_GDC] = SelectAllExeConf(Properties.Settings.Default.OAS_App, ALL_EXE_CONF.oOASApp.GroundAttr) == "1";
                m_ConstraintsSW[GA.Constraints.cSW_DSC] = SelectAllExeConf(Properties.Settings.Default.OAS_App, ALL_EXE_CONF.oOASApp.DestinationAttr) == "1";
                m_ConstraintsSW[GA.Constraints.cSW_FSC] = SelectAllExeConf(Properties.Settings.Default.OAS_App, ALL_EXE_CONF.oOASApp.FlightAttr) == "1";
                //m_ConstraintsSW[GA.Constraints.cSW_CCC] = SelectAllExeConf(Properties.Settings.Default.OAS_App, ALL_EXE_CONF.oOASApp.CounterAttr) == "1";

                // 設定基因演算法限制條件(函數之參數矩陣)
                m_ConstraintsGA.Add(GA.Constraints.cLB, ArrayInSameElement<double>(DataPoolSet.OASResultSet.FLIGHT_NO.Count + DataPoolSet.LastCarouselList.Count, 0));   // 基因變數下限值之向量矩陣(預設為-Inf)
                m_ConstraintsGA.Add(GA.Constraints.cUB, ArrayInSameElement<double>(DataPoolSet.OASResultSet.FLIGHT_NO.Count + DataPoolSet.LastCarouselList.Count, m_CarouselList.Count));    // 基因變數上限值之向量矩陣(預設為Inf)
                m_ConstraintsGA.Add(GA.Constraints.cintcon, Enumerable.Range(0, DataPoolSet.OASResultSet.FLIGHT_NO.Count + DataPoolSet.LastCarouselList.Count).ToArray());   // 基因變數為整數之序號向量

                // 設定基因演算選項參數(包含函數之參數矩陣)
                m_Options.Add("PopulationSize", int.Parse(SelectAllExeConf(Properties.Settings.Default.GA_Config, ALL_EXE_CONF.oGAConfig.PopulationSize))); // From "ALL_EXE_CONF" table
                m_Options.Add("Generations", int.Parse(SelectAllExeConf(Properties.Settings.Default.GA_Config, ALL_EXE_CONF.oGAConfig.Generations)));       // From "ALL_EXE_CONF" table
                m_Options.Add("StallGenLimit", int.Parse(SelectAllExeConf(Properties.Settings.Default.GA_Config, ALL_EXE_CONF.oGAConfig.StallGenLimit)));   // From "ALL_EXE_CONF" table
                m_Options.Add("FitnessLimit", double.Parse(SelectAllExeConf(Properties.Settings.Default.GA_Config, ALL_EXE_CONF.oGAConfig.FitnessLimit)));  // From "ALL_EXE_CONF" table
                m_Options.Add("CrossRate", double.Parse(SelectAllExeConf(Properties.Settings.Default.GA_Config, ALL_EXE_CONF.oGAConfig.CrossRate)));        // From "ALL_EXE_CONF" table
                m_Options.Add("MutationRate", double.Parse(SelectAllExeConf(Properties.Settings.Default.GA_Config, ALL_EXE_CONF.oGAConfig.MutationRate)));  // From "ALL_EXE_CONF" table
                m_Options.Add("SelectFcn", SelectAllExeConf(Properties.Settings.Default.GA_Config, ALL_EXE_CONF.oGAConfig.SelectFcn));                      // From "ALL_EXE_CONF" table
                m_Options.Add("CrossoverFcn", SelectAllExeConf(Properties.Settings.Default.GA_Config, ALL_EXE_CONF.oGAConfig.CrossoverFcn));                // From "ALL_EXE_CONF" table
                m_Options.Add("MutationFcn", SelectAllExeConf(Properties.Settings.Default.GA_Config, ALL_EXE_CONF.oGAConfig.MutationFcn));                  // From "ALL_EXE_CONF" table
                m_Options.Add("LastIteration", int.Parse(SelectAllExeConf(Properties.Settings.Default.OAS_App, ALL_EXE_CONF.oOASApp.Last_Iteration)));      // From "ALL_EXE_CONF" table
                m_Options.Add("Custom", DataPoolSet);
                m_Options.Add("ConstraintGA", m_ConstraintsGA);
                m_Options.Add("SW_Custom", m_ConstraintsSW);

                return 0;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }
        #endregion

        #region ArrayInSameElement(int nvars, double element)//Array
        /// <summary>
        /// 指定長度且變數相同的一維陣列
        /// </summary>
        /// <typeparam name="T">陣列內容的資料型態</typeparam>
        /// <param name="nvars">陣列長度</param>
        /// <param name="element">相同的陣列元素</param>
        /// <returns>Array of type <T></returns>
        private Array ArrayInSameElement<T>(int nvars, T element)
        {
            T[] dArray = new T[nvars];
            for (int i = 0; i < dArray.Length; i++)
            {
                dArray[i] = element;
            }
            return dArray;
        }
        #endregion

        #endregion

        #region =====[Public] Method=====

        #region RunThread()//void
        /// <summary>
        /// 啟動執行緒(主執行緒：航班行李轉盤預排演算)
        /// </summary>
        public void RunThread()
        {
            // 依預排選擇日期設定參數
            m_sDate = Properties.Settings.Default.Selected_Date;
            m_sSaveFileName = string.IsNullOrEmpty(m_sSaveFileName) ? string.Format(Properties.Settings.Default.SaveFileName, m_sDate) : m_sSaveFileName;
            m_sSaveFilePath = Path.Combine(m_sSaveFileDirectory, m_sSaveFileName);
            DataPoolSet = new DataPool(m_sDate, m_sTotalTime_EOT, m_sTotalTime_ECT, m_CarouselList, m_CarouselProperty, m_CounterProperty);

            // 將Run_Type設定為"1"
            if (UpdateAllExeConf(ALL_EXE_CONF.oOASApp.Run_Type, "1") > 0)
            {
                TxtLog.Info("ALL_EXE_CONF中ID為oas_app的Run_Type欄位更新為'1'");
            }

            try
            {
                Console.WriteLine(string.Format("[{0}]|航班行李之轉盤配置預排程式啟動！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                TxtLog.Info("航班行李之轉盤配置預排程式啟動！");
                IsThreadRunning = true;
                m_ScheduleThread = new Thread(new ThreadStart(CarouselSchedule))
                {
                    IsBackground = true
                };
                m_ScheduleThread.Start();
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
            // 將Run_Type更改為0
            if (UpdateAllExeConf(ALL_EXE_CONF.oOASApp.Run_Type, "0") > 0)
            {
                TxtLog.Info("ALL_EXE_CONF中ID為oas_app的Run_Type欄位更新為'0'");
            }
            // 將Execute_Type設定為"0"
            if (UpdateAllExeConf(ALL_EXE_CONF.oOASApp.Execute_Type, "0") > 0)
            {
                TxtLog.Info("ALL_EXE_CONF中ID為oas_app的Execute_Type欄位更新為'0'");
            }

            try
            {
                IsThreadRunning = false;
                Thread.Sleep(1000);

                if (m_ScheduleThread != null && m_ScheduleThread.IsAlive)
                {
                    m_ScheduleThread.Abort();
                    m_ScheduleThread = null;
                }

                Console.WriteLine(string.Format("[{0}]|航班行李之轉盤配置預排程式關閉！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                TxtLog.Info(string.Format("航班行李之轉盤配置預排程式關閉！"));
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }
        #endregion

        #region ErrorStop()//void
        /// <summary>
        /// 執行緒發生錯誤時將執行緒執行狀態設為關閉
        /// </summary>
        public void ErrorStop()
        {
            // 將Run_Type更改為-1
            if (UpdateAllExeConf(ALL_EXE_CONF.oOASApp.Run_Type, "-1") > 0)
            {
                TxtLog.Info("ALL_EXE_CONF中ID為oas_app的Run_Type欄位更新為'-1'");
            }
            // 將Execute_Type設定為"0"
            if (UpdateAllExeConf(ALL_EXE_CONF.oOASApp.Execute_Type, "0") > 0)
            {
                TxtLog.Info("ALL_EXE_CONF中ID為oas_app的Execute_Type欄位更新為'0'");
            }

            try
            {
                IsThreadRunning = false;
                Thread.Sleep(1000);

                if (m_ScheduleThread != null && m_ScheduleThread.IsAlive)
                {
                    m_ScheduleThread.Abort();
                    m_ScheduleThread = null;
                }

                Console.WriteLine(string.Format("[{0}]|航班行李之轉盤配置預排程式關閉！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                TxtLog.Info("航班行李之轉盤配置預排程式關閉！");
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
