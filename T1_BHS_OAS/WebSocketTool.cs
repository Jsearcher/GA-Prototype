using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Fleck;

namespace T1_BHS_OAS
{
    public class WebSocketTool
    {
        #region =====[Public] Class=====

        /// <summary>
        /// WebSocket訊息傳送之Prefix Fields (Header)
        /// </summary>
        public class Header
        {
            public string MessageID { get; set; }
            public string SendTime { get; set; }
            public string Sender { get; set; }
            public string Receiver { get; set; }
        }

        /// <summary>
        /// 向T1_BHS_WEB提出航班行李預排日期之詢問要求訊息
        /// </summary>
        public class MSG_SelectedDateRQ : Header
        {
            public string Selected_Date { get; set; }
            public int RQ_Code { get; set; }
        }

        /// <summary>
        /// 由T1_BHS_WEB對MSG_SelectedDateRQ要求訊息之回應訊息(航班行李預排日期)
        /// </summary>
        public class MSG_SelectedDateRS : Header
        {
            public string Selected_Date { get; set; }
        }

        /// <summary>
        /// 向T1_BHS_WEB以固定時間間隔(預設5 sec)回報目前預排演算進度訊息
        /// </summary>
        public class MSG_ProgressRate : Header
        {
            public double Progress_Rate { get; set; }
        }
        
        /// <summary>
        /// 向T1_BHS_WEB傳送預排演算結果
        /// </summary>
        public class MSG_ScheduleResult : Header
        {
            /// <summary>
            /// 航班日期List
            /// </summary>
            public List<string> FIDS_DATE { get; set; }

            /// <summary>
            /// 航班List
            /// </summary>
            public List<string> FLIGHT_NO { get; set; }

            /// <summary>
            /// STD List
            /// </summary>
            public List<string> STD { get; set; }

            /// <summary>
            /// 航班行李轉盤配置優先次序
            /// </summary>
            public List<int> PRIORITY { get; set; }

            /// <summary>
            /// 目的地簡碼List
            /// </summary>
            public List<string> DESTINATION { get; set; }

            /// <summary>
            /// 轉盤號碼List
            /// </summary>
            public List<int> CAROUSEL_NO { get; set; }
        }

        #endregion

        #region =====[Private] Variable=====

        /// <summary>
        /// OAS與T1_BHS_WEB間的WebSocket連線IP
        /// </summary>
        private readonly string m_sWebSocket_IP = Properties.Settings.Default.WebSocket_IP;

        /// <summary>
        /// 等待回應訊息超時(預設10秒)
        /// </summary>
        private readonly int m_iTimeOut_RS = 10000;

        /// <summary>
        /// WebSocket連線之Server物件
        /// </summary>
        private WebSocketServer m_WSktServer = null;

        /// <summary>
        /// 所有建立連線的WebSocket物件
        /// </summary>
        private Dictionary<string, IWebSocketConnection> m_AllWSktDict = null;
        
        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        #endregion

        #region =====[Public] Property=====

        /// <summary>
        /// 是否為第一次建立WebSocket連線
        /// </summary>
        public bool IsRestartAfterCloseOrError { get; private set; }

        /// <summary>
        /// "MSG_SelectedDateRS"訊息是否已接收到並完成相關觸發動作
        /// </summary>
        public bool IsSelectedDateRS_Receive { get; private set; }

        /// <summary>
        /// 所接收之"MSG_SelectedDateRS"中的預排選擇日期是否成功設定儲存至App.config
        /// </summary>
        public bool IsSelectedDate_Set { get; private set; }

        /// <summary>
        /// WebSocket連線是否發生中斷(須重新建立WebSocketTool物件)
        /// </summary>
        public bool IsWebSocket_Close { get; private set; }

        /// <summary>
        /// WebSocket執行過程是否發生錯誤
        /// </summary>
        public bool IsWebSocket_Error { get; private set; }

        #endregion

        #region =====[Public] Constructor & Destructor=====

        /// <summary>
        /// WebSocketTool類別建構子
        /// </summary>
        public WebSocketTool()
        {
            try
            {
                m_WSktServer = new WebSocketServer(m_sWebSocket_IP);
                m_AllWSktDict = new Dictionary<string, IWebSocketConnection>();
                IsSelectedDateRS_Receive = false;
                IsWebSocket_Close = true;
                IsWebSocket_Error = false;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        /// <summary>
        /// WebSocketTool類別解構子
        /// </summary>
        ~WebSocketTool()
        {
            try
            {
                CloseWebSocket();
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        #endregion

        #region =====[Private] Function=====

        #region JSONStringToList<T>(string JsonStr)//List<T>
        /// <summary>
        /// 將JSON字串格式轉為List
        /// </summary>
        /// <typeparam name="T">List的物件資料型態</typeparam>
        /// <param name="JsonStr">JSON字串</param>
        /// <returns>List物件</returns>
        private List<T> JSONStringToList<T>(string JsonStr)
        {
            List<T> ListRtn = new List<T>();

            try
            {
                ListRtn = JsonConvert.DeserializeObject<List<T>>(JsonStr);
                return ListRtn;
            }
            catch
            {
                return ListRtn;
            }
        }
        #endregion

        #region OnMSGSelectedDateRQ(IWebSocketConnection pSocket, string pMessage)//void
        /// <summary>
        /// <para>當WebSocket收到T1_BHS_WEB傳送的"MSG_SelectedDateRS"之動作</para>
        /// <para>儲存"MSG_SelectedDateRS中的Selected_Date"資料</para>
        /// </summary>
        /// <param name="pMessage">"MSG_SelectedDateRS"訊息</param>
        private void OnMSGSelectedDateRS(string pMessage)
        {
            TxtLog.Info(string.Format("接收訊息: {0}", pMessage));
            IsSelectedDate_Set = false;

            if (m_AllWSktDict.Count > 0)
            {
                try
                {
                    // 解析"MSG_SelectedDateRQ訊息(JSON => "MSG_SelectedDateRQ" Object)"
                    MSG_SelectedDateRS m_MSG_SelectedDateRS = JsonConvert.DeserializeObject<MSG_SelectedDateRS>(pMessage);

                    // 將訊息中的Selected_Date參數儲存至App.config中
                    Properties.Settings.Default.Selected_Date = m_MSG_SelectedDateRS.Selected_Date;
                    Properties.Settings.Default.Save();
                    IsSelectedDate_Set = true;
                    Console.WriteLine(string.Format("[{0}]|已變更Config檔案之Selected_Date：[{1}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), m_MSG_SelectedDateRS.Selected_Date));
                }
                catch (Exception ex)
                {
                    TxtLog.Error(ex);
                    IsSelectedDate_Set = false;
                }
            }

            // 已接收"MSG_SelectedDateRS"並已完成此觸發事件之動作(不論是否發生錯誤)
            IsSelectedDateRS_Receive = true;
        }
        #endregion

        #endregion

        #region =====[Public] Method=====

        #region WSktManager()//void
        /// <summary>
        /// WebSocket建立與訊息監聽
        /// </summary>
        public void WSktManager()
        {
            // 出錯後進行重啟
            //m_WSktServer.RestartAfterListenError = true;    // 須測試會不會重啟

            try
            {
                m_WSktServer.Start(socket =>
                {
                    Console.WriteLine(string.Format("[{0}]|等待連線中...\n目前OAS所使用的WebSocket連線IP: {1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), m_sWebSocket_IP));
                    TxtLog.Info(string.Format("等待連線中...，目前OAS所使用的WebSocket連線IP: {0}", m_sWebSocket_IP));

                    // 新的WebSocket已連線
                    socket.OnOpen = () =>
                    {
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        m_AllWSktDict.Add(clientUrl, socket);
                        Console.WriteLine(string.Format("[{0}]|目前OAS與Client IP：[{1}]間之連線已建立！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), clientUrl));
                        TxtLog.Info(string.Format("目前OAS與Client IP：[{0}]間之連線已建立！", clientUrl));
                        IsRestartAfterCloseOrError = true;
                        IsWebSocket_Close = false;
                        IsWebSocket_Error = false;
                    };

                    // 此WebSocket已離線
                    socket.OnClose = () =>
                    {
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        if (m_AllWSktDict.ContainsKey(clientUrl))
                        {
                            m_AllWSktDict.Remove(clientUrl);
                        }
                        Console.WriteLine(string.Format("[{0}]|目前OAS與Client IP：[{1}]間之連線已中斷！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), clientUrl));
                        TxtLog.Info(string.Format("目前OAS與Client IP：[{0}]間之連線已中斷！", clientUrl));
                        //CloseWebSocket();
                    };

                    // 當T1_BHS_WEB傳送訊息
                    socket.OnMessage = message =>
                    {
                        if (message.Contains("MSG_SelectedDateRS"))
                        {
                            OnMSGSelectedDateRS(message);
                        }
                        else
                        {
                            Console.WriteLine(string.Format("[{0}]|訊息名稱|[{1}]，未能識別。", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), message));
                            TxtLog.Error(string.Format("訊息名稱|[{0}]，未能識別。", message));
                        }
                    };

                    // 當此WebSocket發生錯誤時
                    socket.OnError = ex =>
                    {
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        Console.WriteLine(string.Format("[{0}]|目前OAS與Client IP：[{1}]間之連線發生錯誤: [{2}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), clientUrl, ex));
                        TxtLog.Error(ex);
                        CloseWebSocket();
                        IsWebSocket_Error = true;
                    };
                });
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
            
        }
        #endregion

        #region SendMSGSelectedDateRQ(int pCode)//int
        /// <summary>
        /// 傳送航班行李預排日期之詢問要求訊息"MSG_SelectedDateRQ"
        /// </summary>
        /// <param name="pCode">
        /// <para>要求指示代碼</para>
        /// <para> 0: 航班行李轉盤預排最佳化程式啟動初次詢問要求</para>
        /// <para>-1: 程式處理發生問題，重新傳送詢問要求</para>
        /// </param>
        /// <returns>
        /// <para> 1: 訊息傳遞成功且已將參數設定完成</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 未有連線的WebSocket物件</para>
        /// <para>-3: 訊息傳遞成功但未將參數設定完成</para>
        /// <para>-4: 等待回應訊息處理超時</para>
        /// <para>-5: 目前無任何Client端OAS與間有連線，無法傳送訊息</para>
        /// </returns>
        public int SendMSGSelectedDateRQ(int pCode)
        {
            if (m_AllWSktDict.Count == 0)
            {
                Console.WriteLine(string.Format("[{0}]|目前無任何Client端OAS與間有連線，無法傳送訊息！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                TxtLog.Error("目前無任何Client端OAS與間有連線，無法傳送訊息！");
                return -5;
            }

            // 初始化"MSG_SelectedDateRQ"之Header
            MSG_SelectedDateRQ m_MSG_SelectedDateRQ = new MSG_SelectedDateRQ
            {
                MessageID = "MSG_SelectedDateRQ",
                SendTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                Sender = "OAS",
                Receiver = "T1_BHS_WEB"
            };

            if (m_AllWSktDict.Count > 0)
            {
                try
                {
                    // "MSG_SelectedDateRQ"的要求指示代碼(0為預排日期初次詢問要求；-1為重新傳送詢問要求)
                    m_MSG_SelectedDateRQ.RQ_Code = pCode;

                    // 傳送預排日期之詢問要求訊息"MSG_SelectedDateRQ"予T1_BHS_WEB
                    foreach (IWebSocketConnection pSocket in m_AllWSktDict.Values)
                    {
                        pSocket.Send(JsonConvert.SerializeObject(m_MSG_SelectedDateRQ, Formatting.Indented));
                        string clientUrl = pSocket.ConnectionInfo.ClientIpAddress + ":" + pSocket.ConnectionInfo.ClientPort;
                        Console.WriteLine(string.Format("[{0}]|傳送預排日期之詢問要求訊息'MSG_SelectedDateRQ'，要求指示代碼：[{1}]，Client IP：[{2}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), m_MSG_SelectedDateRQ.RQ_Code, clientUrl));
                        TxtLog.Info(string.Format("傳送預排日期之詢問要求訊息'MSG_SelectedDateRQ'，要求指示代碼：[{0}]，Client IP：[{1}]", m_MSG_SelectedDateRQ.RQ_Code, clientUrl));
                    }

                    // 設定尚未接收到"MSG_SelectedDateRS"回應訊息
                    IsSelectedDateRS_Receive = false;
                    DateTime pStart = DateTime.Now;
                    // 等待訊息回應及參數設定完成 或 等待回應訊息處理超時(預設10秒)
                    while (!IsSelectedDateRS_Receive)
                    {
                        if ((DateTime.Now - pStart).TotalMilliseconds >= m_iTimeOut_RS)
                        {
                            TxtLog.Error(string.Format("CODE[-4]|未接收'MSG_SelectedDateRS'回應訊息，等待回應訊息處理超時: [{0}]！", m_iTimeOut_RS.ToString()));
                            return -4;
                        }
                    }
                    if (IsSelectedDate_Set)
                    {
                        TxtLog.Info("已接收'MSG_SelectedDateRS'回應訊息，並完成航班行李預排日期'Selected_Date'之參數設定！");
                        return 1;
                    }
                    else
                    {
                        TxtLog.Error("CODE[-3]|已接收'MSG_SelectedDateRS'回應訊息，但未完成航班行李預排日期'Selected_Date'之參數設定！");
                        return -3;
                    }
                }
                catch (Exception ex)
                {
                    TxtLog.Error(ex);
                    return -1;
                }
            }
            else
            {
                TxtLog.Error("CODE[-2]|未有已連線的WebSocket物件");
                return -2;
            }
        }
        #endregion

        #region SendMSGProgressRate(double pRate)//int
        /// <summary>
        /// 使用"MSG_ProgressRate"訊息向T1_BHS_WEB以固定時間間隔(預設5 sec)回報目前預排演算進度
        /// </summary>
        /// <param name="pRate">預排演算進度</param>
        /// <returns>
        /// <para> 1: 訊息傳遞成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 未有連線的WebSocket物件</para>
        /// <para>-3: 目前無任何Client端OAS與間有連線，無法傳送訊息</para>
        /// </returns>
        public int SendMSGProgressRate(double pRate)
        {
            if (m_AllWSktDict.Count == 0)
            {
                Console.WriteLine(string.Format("[{0}]|目前無任何Client端OAS與間有連線，無法傳送訊息！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                TxtLog.Error("目前無任何Client端OAS與間有連線，無法傳送訊息！");
                return -3;
            }

            // 初始化"MSG_ProgressRate"之Header
            MSG_ProgressRate m_MSG_ProgressRate = new MSG_ProgressRate
            {
                MessageID = "MSG_ProgressRate",
                SendTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                Sender = "OAS",
                Receiver = "T1_BHS_WEB"
            };

            if (m_AllWSktDict.Count > 0)
            {
                try
                {
                    // "MSG_ProgressRate"的預排演算進度
                    m_MSG_ProgressRate.Progress_Rate = pRate;

                    // 傳送預排演算進度訊息"MSG_ProgressRate"予T1_BHS_WEB
                    foreach (IWebSocketConnection pSocket in m_AllWSktDict.Values)
                    {
                        pSocket.Send(JsonConvert.SerializeObject(m_MSG_ProgressRate, Formatting.Indented));
                        string clientUrl = pSocket.ConnectionInfo.ClientIpAddress + ":" + pSocket.ConnectionInfo.ClientPort;
                        Console.WriteLine(string.Format("[{0}]|傳送預排日期之詢問要求訊息'MSG_ProgressRate'，目前演算進度：[{1}]，Client IP：[{2}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), m_MSG_ProgressRate.Progress_Rate, clientUrl));
                        TxtLog.Info(string.Format("傳送預排日期之詢問要求訊息'MSG_ProgressRate'，目前演算進度：[{0}]，Client IP：[{1}]", m_MSG_ProgressRate.Progress_Rate, clientUrl));
                    }

                    return 1;
                }
                catch (Exception ex)
                {
                    TxtLog.Error(ex);
                    return -1;
                }
            }
            else
            {
                TxtLog.Error("CODE[-2]|未有已連線的WebSocket物件");
                return -2;
            }
        }
        #endregion

        #region SendMSGScheduleResult(object pResult)//int
        /// <summary>
        /// 使用"MSG_ScheduleResult"訊息向T1_BHS_WEB傳遞預排演算結果
        /// </summary>
        /// <param name="pResult">預排演算結果(資料型態為DataPool.OASResult)</param>
        /// <returns>
        /// <para> 1: 訊息傳遞成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 未有連線的WebSocket物件</para>
        /// <para>-3: 目前無任何Client端OAS與間有連線，無法傳送訊息</para>
        /// </returns>
        public int SendMSGScheduleResult(object pResult)
        {
            if (m_AllWSktDict.Count == 0)
            {
                Console.WriteLine(string.Format("[{0}]|目前無任何Client端OAS與間有連線，無法傳送訊息！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                TxtLog.Error("目前無任何Client端OAS與間有連線，無法傳送訊息！");
                return -3;
            }

            DataPool.OASResult pOASResult = (DataPool.OASResult)pResult;
            // 初始化"MSG_ScheduleResult"之Header
            MSG_ScheduleResult m_MSG_ScheduleResult = new MSG_ScheduleResult
            {
                MessageID = "MSG_ScheduleResult",
                SendTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                Sender = "OAS",
                Receiver = "T1_BHS_WEB"
            };

            if (m_AllWSktDict.Count > 0)
            {
                try
                {
                    // "MSG_ScheduleResult"的預排演算結果
                    m_MSG_ScheduleResult.FIDS_DATE = pOASResult.FDATE;
                    m_MSG_ScheduleResult.FLIGHT_NO = pOASResult.FLIGHT_NO;
                    m_MSG_ScheduleResult.STD = pOASResult.STD;
                    m_MSG_ScheduleResult.PRIORITY = new List<int>();
                    for (int i = 0; i < m_MSG_ScheduleResult.FLIGHT_NO.Count; i++)
                    {
                        m_MSG_ScheduleResult.PRIORITY.Add(0);
                    }
                    m_MSG_ScheduleResult.DESTINATION = pOASResult.DESTINATION;
                    m_MSG_ScheduleResult.CAROUSEL_NO = pOASResult.CAROUSEL_NO;

                    // 傳送預排演算進度訊息"MSG_ScheduleResult"予T1_BHS_WEB
                    foreach (IWebSocketConnection pSocket in m_AllWSktDict.Values)
                    {
                        pSocket.Send(JsonConvert.SerializeObject(m_MSG_ScheduleResult, Formatting.Indented));
                        string clientUrl = pSocket.ConnectionInfo.ClientIpAddress + ":" + pSocket.ConnectionInfo.ClientPort;
                        Console.WriteLine(string.Format("[{0}]|傳送預排日期之詢問要求訊息'MSG_ScheduleResult'，Client IP：[{1}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), clientUrl));
                        TxtLog.Info(string.Format("傳送預排日期之詢問要求訊息'MSG_ScheduleResult'，Client IP：[{0}]", clientUrl));
                    }

                    TxtLog.Info("成功傳遞航班行李轉盤之預排演算結果訊息");
                    return 1;
                }
                catch (Exception ex)
                {
                    TxtLog.Error(ex);
                    return -1;
                }
            }
            else
            {
                TxtLog.Error("未有已連線的WebSocket物件");
                return -2;
            }
        }
        #endregion

        #region CloseWebSocket()//void
        /// <summary>
        /// 關閉WebSocket之建立與訊息監聽
        /// </summary>
        public void CloseWebSocket()
        {
            try
            {
                if (m_WSktServer != null)
                {
                    m_WSktServer.Dispose();
                    m_WSktServer = null;
                }
                m_AllWSktDict = null;
                IsWebSocket_Close = true;
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
