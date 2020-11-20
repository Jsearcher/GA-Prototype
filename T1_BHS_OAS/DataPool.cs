using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using T1_BHS_OAS.DBLib;

namespace T1_BHS_OAS
{
    /// <summary>
    /// "DataPool" 資料集物件(航班行李之轉盤配置預排資料集)
    /// </summary>
    public class DataPool
    {
        #region =====[Public] Class=====

        /// <summary>
        /// 航班行李預排結果類別
        /// </summary>
        public class OASResult
        {
            /// <summary>
            /// FlightData資料序號
            /// </summary>
            public List<int> SERIAL_NO { get; set; }

            /// <summary>
            /// 轉盤號碼List
            /// </summary>
            public List<int> CAROUSEL_NO { get; set; }

            /// <summary>
            /// 航班List
            /// </summary>
            public List<string> FLIGHT_NO { get; set; }

            /// <summary>
            /// 航班日期List
            /// </summary>
            public List<string> FDATE { get; set; }

            /// <summary>
            /// STD List
            /// </summary>
            public List<string> STD { get; set; }

            /// <summary>
            /// 預計開櫃List
            /// </summary>
            public List<string> EOT { get; set; }

            /// <summary>
            /// 預計關櫃List
            /// </summary>
            public List<string> ECT { get; set; }

            /// <summary>
            /// 目的地簡碼List
            /// </summary>
            public List<string> DESTINATION { get; set; }

            /// <summary>
            /// 地勤List
            /// </summary>
            public List<string> GROUND { get; set; }

            /// <summary>
            /// 航班大小List
            /// </summary>
            public List<string> FLIGHT_SIZE { get; set; }

            /// <summary>
            /// 櫃檯號碼List
            /// </summary>
            public List<int> COUNTER_NO { get; set; }

            /// <summary>
            /// 預計行李數List
            /// </summary>
            public List<double> PREDICT_NO { get; set; }
        }

        #endregion

        #region =====[Private] Variable=====

        /// <summary>
        /// 匯入資料"FlightData"的欄位名稱List
        /// </summary>
        private List<string> m_TitleList;

        /// <summary>
        /// 匯入資料"FlightData"的欄位資料List
        /// </summary>
        private List<List<string>> m_FlightDataList;

        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        #endregion

        #region =====[Public] Property=====

        /// <summary>
        /// T1轉盤目前可使用之轉盤列表(LATERAL_ID僅留數值)
        /// </summary>
        public List<int> CarouselList { get; }

        /// <summary>
        /// T1轉盤屬性設定值List
        /// </summary>
        public List<GA_CAROUSEL_TABLE_T1.Row> CarouselProperty { get; }

        /// <summary>
        /// T1航班行李報到櫃檯屬性設定值List
        /// </summary>
        public List<GA_COUNTER_TABLE_T1.Row> CounterProperty { get; }

        /// <summary>
        /// 航班行李預排日期前與設定的開櫃時間內的航班行李轉盤配置List
        /// </summary>
        public List<MAKEUP_ALLOC.Row> LastCarouselList { get; private set; }

        //public List<>

        /// <summary>
        /// 航班行李預排結果物件
        /// </summary>
        public OASResult OASResultSet { get; private set; }

        /// <summary>
        /// 預排選擇日期
        /// </summary>
        public string SDATE { get; }

        /// <summary>
        /// 預計開櫃時間(min)
        /// </summary>
        public int TotalTime_EOT { get; private set; }

        /// <summary>
        /// 預計關櫃時間(min)
        /// </summary>
        public int TotalTime_ECT { get; private set; }

        /// <summary>
        /// 最佳演算重疊時間
        /// </summary>
        public int BestOverlapTime { get; set; }

        /// <summary>
        /// 演算花費時間
        /// </summary>
        public int Proc_TimeCost { get; set; }

        /// <summary>
        /// 基因演算迭代次數
        /// </summary>
        public int Iteration { get; set; }

        #endregion

        #region =====[Public] Constructor & Destructor=====

        /// <summary>
        /// DataPool類別建構子
        /// </summary>
        /// <param name="pSDATE">預排選擇日期</param>
        public DataPool(string pSDATE)
        {
            try
            {
                SDATE = pSDATE;
                OASResultSet = new OASResult();
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        /// <summary>
        /// DataPool類別建構子
        /// </summary>
        /// <param name="pSDATE">預排選擇日期</param>
        /// <param name="pCarouselProperty">T1轉盤屬性設定值List</param>
        /// <param name="pCounterProperty">T1航班行李報到櫃檯屬性設定值List</param>
        public DataPool(string pSDATE, List<object> pCarouselProperty, List<object> pCounterProperty)
        {
            try
            {
                SDATE = pSDATE;
                CarouselProperty = pCarouselProperty.Select(cp => (GA_CAROUSEL_TABLE_T1.Row)cp).ToList();
                CounterProperty = pCounterProperty.Select(cp => (GA_COUNTER_TABLE_T1.Row)cp).ToList();
                OASResultSet = new OASResult();
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        /// <summary>
        /// DataPool類別建構子
        /// </summary>
        /// <param name="pSDATE">預排選擇日期</param>
        /// <param name="pEOT">預計開櫃時間</param>
        /// <param name="pECT">預計關櫃時間</param>
        /// <param name="pCarouselList">T1轉盤目前可使用之轉盤列表(LATERAL_ID僅留數值)</param>
        /// <param name="pCarouselProperty">T1轉盤屬性設定值List</param>
        /// <param name="pCounterProperty">T1航班行李報到櫃檯屬性設定值List</param>
        public DataPool(string pSDATE, string pEOT, string pECT, List<int> pCarouselList, List<object> pCarouselProperty, List<object> pCounterProperty)
        {
            try
            {
                SDATE = pSDATE;
                TotalTime_EOT = int.Parse((double.Parse(pEOT) * 60).ToString());
                TotalTime_ECT = int.Parse((double.Parse(pECT) * 60).ToString());
                CarouselList = pCarouselList;
                CarouselProperty = pCarouselProperty.Select(cp => (GA_CAROUSEL_TABLE_T1.Row)cp).ToList();
                CounterProperty = pCounterProperty.Select(cp => (GA_COUNTER_TABLE_T1.Row)cp).ToList();
                OASResultSet = new OASResult();
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        /// <summary>
        /// DataPool類別解構子
        /// </summary>
        ~DataPool()
        {
            try
            {
                OASResultSet = null;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        #endregion

        #region =====[Private] Thread=====



        #endregion

        #region =====[Private] Function=====

        #region ParseFlightDataList()//int
        /// <summary>
        /// 剖析FlightData資料List
        /// </summary>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        private int ParseFlightDataList()
        {
            int ns = m_FlightDataList.Count;
            OASResultSet.SERIAL_NO = new List<int>(ns);
            OASResultSet.CAROUSEL_NO = new List<int>(ns);
            OASResultSet.FLIGHT_NO = new List<string>(ns);
            OASResultSet.FDATE = new List<string>(ns);
            OASResultSet.STD = new List<string>(ns);
            OASResultSet.EOT = new List<string>(ns);
            OASResultSet.ECT = new List<string>(ns);
            OASResultSet.DESTINATION = new List<string>(ns);
            OASResultSet.GROUND = new List<string>(ns);
            OASResultSet.FLIGHT_SIZE = new List<string>(ns);
            OASResultSet.COUNTER_NO = new List<int>(ns);
            OASResultSet.PREDICT_NO = new List<double>(ns);

            try
            {
                for (int i = 0; i < ns; i++)
                {
                    OASResultSet.SERIAL_NO.Add(i);
                    OASResultSet.CAROUSEL_NO.Add(int.Parse(m_FlightDataList[i][m_TitleList.IndexOf("CAROUSEL_NO")]));
                    OASResultSet.FLIGHT_NO.Add(m_FlightDataList[i][m_TitleList.IndexOf("FLIGHT_NO")]);
                    OASResultSet.FDATE.Add(m_FlightDataList[i][m_TitleList.IndexOf("FDATE")]);
                    OASResultSet.STD.Add(m_FlightDataList[i][m_TitleList.IndexOf("STD")]);
                    OASResultSet.EOT.Add(m_FlightDataList[i][m_TitleList.IndexOf("EOT")]);
                    OASResultSet.ECT.Add(m_FlightDataList[i][m_TitleList.IndexOf("ECT")]);
                    OASResultSet.DESTINATION.Add(m_FlightDataList[i][m_TitleList.IndexOf("DESTINATION")]);
                    OASResultSet.GROUND.Add(m_FlightDataList[i][m_TitleList.IndexOf("GROUND")]);
                    OASResultSet.FLIGHT_SIZE.Add(m_FlightDataList[i][m_TitleList.IndexOf("FLIGHT_SIZE")]);
                    OASResultSet.COUNTER_NO.Add(int.Parse(m_FlightDataList[i][m_TitleList.IndexOf("COUNTER_NO")]));
                    OASResultSet.PREDICT_NO.Add(double.Parse(m_FlightDataList[i][m_TitleList.IndexOf("PREDICT_NO")]));
                }

                return 0;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }
        #endregion

        #region ComposeFlightDataList()//int
        /// <summary>
        /// 重組FlightData資料List
        /// </summary>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        private int ComposeFlightDataList()
        {
            m_FlightDataList = new List<List<string>>();
            m_TitleList = (new string[] { "CAROUSEL_NO", "FLIGHT_NO", "FDATE", "STD", "EOT", "ECT",
                                            "DESTINATION", "GROUND", "FLIGHT_SIZE", "COUNTER_NO", "PREDICT_NO" }).ToList();

            try
            {
                for (int i = 0; i < OASResultSet.FLIGHT_NO.Count; i++)
                {
                    List<string> tempList = new List<string>
                    {
                        OASResultSet.CAROUSEL_NO[i].ToString(),
                        OASResultSet.FLIGHT_NO[i],
                        OASResultSet.FDATE[i],
                        OASResultSet.STD[i],
                        OASResultSet.EOT[i],
                        OASResultSet.ECT[i],
                        OASResultSet.DESTINATION[i],
                        OASResultSet.GROUND[i],
                        OASResultSet.FLIGHT_SIZE[i],
                        OASResultSet.COUNTER_NO[i].ToString(),
                        OASResultSet.PREDICT_NO[i].ToString()
                    };
                    m_FlightDataList.Add(tempList);
                }

                return 0;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }
        #endregion

        #endregion

        #region =====[Public] Method=====

        #region GetLastCarouselList()//int
        /// <summary>
        /// 讀取選擇日期前與設定的(開櫃時間+關櫃時間)內的航班行李轉盤配置資料
        /// </summary>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 資料庫連線失敗</para>
        /// <para>-3: 取得前一天目標資料發生錯誤</para>
        /// </returns>
        public int GetLastCarouselList()
        {
            Database m_dbT1BHSWEB = new Database(null);
            OAS_SQL m_tbOasSql = null;
            Database m_dbT1BHS = new Database(null);
            MAKEUP_ALLOC m_tbMakeupAlloc = null;

            try
            {
                // 計算重疊時間點(Today at 00:00 - EOT + ECT => time point)
                DateTime dt = DateTime.ParseExact(SDATE + "0000", "yyyyMMddHHmm", null).AddMinutes(-TotalTime_EOT + TotalTime_ECT);
                string pDate = dt.ToString("yyyyMMddHHmm").Substring(0, 8);
                string pTimePoint = dt.ToString("yyyyMMddHHmm").Substring(8, 4);

                T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter m_Adapter_OasSql = new T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter();
                T1_BHSDataSetTableAdapters.MAKEUP_ALLOCTableAdapter m_Adapter_MakeupAlloc = new T1_BHSDataSetTableAdapters.MAKEUP_ALLOCTableAdapter();
                // 資料庫物件建立與連線
                m_dbT1BHSWEB = new Database(m_Adapter_OasSql.Connection);
                m_dbT1BHS = new Database(m_Adapter_MakeupAlloc.Connection);
                if (m_dbT1BHSWEB.Connect() < 0 || m_dbT1BHS.Connect() < 0)
                {
                    TxtLog.Error("CODE[-2]|DB: [T1_BHS_web] 或 [T1_BHS] 連線失敗！");
                    return -2;
                }
                else
                {
                    //=== 取得SQL指令 ===//
                    // 資料表物件建立
                    m_tbOasSql = new OAS_SQL(m_Adapter_OasSql.Connection);
                    // 取得目標資料
                    OAS_SQL.Row pSqlRow_s = (OAS_SQL.Row)m_tbOasSql.GetSQLByID(Properties.Settings.Default.S_MakeupAlloc_1);

                    //=== 依SQL指令取得目標配置列表 ===//
                    // 資料表物件建立
                    m_tbMakeupAlloc = new MAKEUP_ALLOC(m_Adapter_MakeupAlloc.Connection);
                    if (m_tbMakeupAlloc.SelectBySQL(pSqlRow_s.SQL + string.Format(pSqlRow_s.CONDITION, pDate, pTimePoint) + pSqlRow_s.OTHER) < 0)
                    {
                        TxtLog.Error("CODE[-3]|取得前一天時間重疊的航班配置資料List發生錯誤！");
                        return -3;
                    }
                    else
                    {
                        LastCarouselList = m_tbMakeupAlloc.RecordList.Select(re => (MAKEUP_ALLOC.Row)re).ToList();
                        return 0;
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
                if (m_dbT1BHS != null)
                {
                    m_dbT1BHS.Close();
                    m_dbT1BHS = null;
                }

                m_tbOasSql = null;
                m_tbMakeupAlloc = null;
            }
        }
        #endregion

        #region ImportFlightData(string pRead_xlsx, string pFlightPath)//int
        /// <summary>
        /// 匯入Excel檔案資料 - FlightData.xlsx
        /// </summary>
        /// /// <param name="pRead_xlsx">Read Excel file的連線字串(.xlsx)</param>
        /// <param name="pFilePath">匯入的Excel檔案路徑</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: Excel檔案連線失敗 或 T1_BHS_web資料庫連線失敗</para>
        /// <para>-3: 取得FlightData資料List發生錯誤</para>
        /// <para>-4: 剖析FlightData資料List失敗</para>
        /// </returns>
        public int ImportFlightData(string pRead_xlsx, string pFilePath)
        {
            Database m_dbT1BHSWEB = new Database(null);
            OAS_SQL m_tbOasSql = null;
            Database m_oExcelRead = new Database(null);
            XLSX_FlightDataPool m_tbFlightDataPool = null;

            try
            {
                T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter m_Adapter_OasSql = new T1_BHS_WEB_DataSetTableAdapters.OAS_SQLTableAdapter();

                // 資料庫物件建立與連線
                m_dbT1BHSWEB = new Database(m_Adapter_OasSql.Connection);
                if (m_dbT1BHSWEB.Connect() < 0)
                {
                    TxtLog.Error("CODE[-2]|DB: [T1_BHS_web] 連線失敗！");
                    return -2;
                }
                else
                {
                    //=== 取得SQL指令 ===//
                    // 資料表物件建立
                    m_tbOasSql = new OAS_SQL(m_Adapter_OasSql.Connection);
                    // 取得目標資料
                    OAS_SQL.Row pSqlRow_s = (OAS_SQL.Row)m_tbOasSql.GetSQLByID(Properties.Settings.Default.S_XLSXFlightData_1);

                    // 連線Excel檔案
                    if ((m_oExcelRead = ProjectExcel.Connect(pRead_xlsx, pFilePath)).Conn == new Database(null).Conn)
                    {
                        Console.WriteLine(string.Format("[{0}]|Excel: [{1}] 連線失敗！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), pFilePath));
                        TxtLog.Error(string.Format("CODE[-2]|Excel: [{0}] 連線失敗！", pFilePath));
                        return -2;
                    }

                    //=== 匯入Excel資料並剖析為List ===//
                    // 資料表物件建立
                    m_tbFlightDataPool = new XLSX_FlightDataPool(m_oExcelRead.Conn);

                    // 取得航班行李之轉盤配置輸入資料(FlightData)List
                    m_FlightDataList = m_tbFlightDataPool.GetAllTableList(pSqlRow_s.SQL);
                    m_TitleList = m_tbFlightDataPool.TITLEList;
                    TxtLog.Info(string.Format("SQL string: \r\n\t[{0}]", m_tbFlightDataPool.SQLStr));

                    if (m_FlightDataList.Count <= 0 || m_TitleList.Count <= 0)
                    {
                        Console.WriteLine(string.Format("[{0}]|取得FlightData資料List發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                        TxtLog.Error("CODE[-3]|取得FlightData資料List發生錯誤！");
                        return -3;
                    }

                    // 剖析FlightData資料List
                    if (ParseFlightDataList() != 0)
                    {
                        Console.WriteLine(string.Format("[{0}]|剖析FlightData資料List失敗！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                        TxtLog.Error("CODE[-4]|剖析FlightData資料List失敗！");
                        return -4;
                    }

                    return 0;
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
                if (m_oExcelRead != null)
                {
                    m_oExcelRead.Close();
                    m_oExcelRead = null;
                }

                m_tbOasSql = null;
                m_tbFlightDataPool = null;
            }
        }
        #endregion

        #region ExportFlightResult()//int
        /// <summary>
        /// 匯出CSV航班行李之轉盤配置預排結果資料 - FlightResult.csv
        /// </summary>
        /// <param name="pFilePath">匯出的CSV檔案路徑</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        public int ExportFlightResult(string pFilePath)
        {
            StreamWriter sw = null;

            try
            {
                // 開啟 CSV 檔案
                sw = new StreamWriter(pFilePath);

                // 修改 CSV 檔案成 "not read only"
                FileInfo FileAttribute = new FileInfo(pFilePath)
                {
                    Attributes = FileAttributes.Normal
                };

                // 寫入資料至 CSV 檔案
                for (int i = 0; i < m_TitleList.Count - 1; i++)
                {
                    sw.Write("{0},", m_TitleList[i]);
                }
                sw.Write("{0}\n", m_TitleList[m_TitleList.Count - 1]);
                for (int i = 0; i < m_FlightDataList.Count; i++)
                {
                    for (int j = 0; j < m_FlightDataList[i].Count - 1; j++)
                    {
                        sw.Write("{0},", m_FlightDataList[i][j]);
                    }
                    sw.Write("{0}\n", m_FlightDataList[i][m_FlightDataList[i].Count - 1]);
                }

                return 0;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                    sw = null;
                }
            }
        }
        #endregion

        #region SendFlightResult()//int
        /// <summary>
        /// 傳送航班行李之轉盤配置預排結果資料至T1_BHS_web(WebSocket)
        /// </summary>
        /// <param name="pWSkt">WebSocket建立與訊息監聽物件</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        public int SendFlightResult(WebSocketTool pWSkt)
        {
            // WebSocket傳送資料
            
            try
            {
                
                return 0;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }
        #endregion

        #region OrderByCarouselNo()//int
        /// <summary>
        /// 將FlightData物件中所需的航班預排輸入參數依轉盤號碼排序
        /// </summary>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 重組FlightData資料List失敗</para>
        /// <para>-3: 剖析FlightData資料List失敗</para>
        /// </returns>
        public int OrderByCarouselNo()
        {
            try
            {
                // 重組FlightData資料List
                if (ComposeFlightDataList() != 0)
                {
                    Console.WriteLine(string.Format("[{0}]: 重組FlightData資料List失敗!", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-2]|重組FlightData資料List失敗!");
                    return -2;
                }

                // 依CAROUSEL_NO=>FDATE=>STD=>FLIGHT_NO作升冪排序
                m_FlightDataList = m_FlightDataList.OrderBy(list => list[0]).
                                                                ThenBy(list => list[2]).
                                                                ThenBy(list => list[3]).
                                                                ThenBy(list => list[1]).ToList();

                // 剖析FlightData資料List
                if (ParseFlightDataList() != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|剖析FlightData資料List失敗!", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-3]|剖析FlightData資料List失敗!");
                    return -3;
                }

                return 0;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }
        #endregion

        #endregion
    }
}
