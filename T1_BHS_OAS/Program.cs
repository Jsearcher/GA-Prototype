using System;
using System.Threading;


namespace T1_BHS_OAS
{
    class Program
    {
        #region =====[Private] Variable=====

        /// <summary>
        /// 讓Program Main程式主要進入點保持運作之變數
        /// </summary>
        private static bool m_IsProgramMainHold = true;

        /// <summary>
        /// "MSG_SelectedDateRS"回應訊息之處理結果
        /// </summary>
        private static int m_iMSGRS_Status = 0;

        /// <summary>
        /// "MSG_SelectedDateRQ"詢問要求訊息重新傳送次數限制(Default = 10)
        /// </summary>
        private static uint m_iMSGRQ_Retry = 10;

        /// <summary>
        /// "MSG_ProgressRate"目前預排演算進度發送時間
        /// </summary>
        private static DateTime m_LastSendTime = new DateTime();

        /// <summary>
        /// WebSocket建立與訊息監聽物件
        /// </summary>
        private static WebSocketTool m_oWSktTool = null;

        /// <summary>
        /// 程式運作回報BeatThread執行緒
        /// </summary>
        private static HealthTask m_BeatThread = null;

        /// <summary>
        /// 航班行李轉盤預排最佳化程式MainThread執行緒
        /// </summary>
        private static MainTask m_MainThread = null;

        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        #endregion

        static void Main(string[] args)
        {
            int pCount = 0; // 重新傳送詢問要求訊息次數

            while (m_IsProgramMainHold)
            {
                try
                {
                    // 啟動程式運作回報(更新目前時間)BeatThread執行緒(預設間隔60秒)
                    if (m_BeatThread == null)
                    {
                        m_BeatThread = new HealthTask(Properties.Settings.Default.HealthCheck_Time);
                        m_BeatThread.RunThread();
                    }

                    // 建立航班行李轉盤預排最佳化程式MainThread物件(須等待航班行李預排日期設定完成)
                    if (m_MainThread == null)
                    {
                        m_MainThread = new MainTask();
                    }

                    // 建立WebSocket以監聽訊息之傳遞
                    //if (m_oWSktTool == null)
                    //{
                    //    m_oWSktTool = new WebSocketTool();
                    //    m_oWSktTool.WSktManager();
                    //    Console.WriteLine(string.Format("[{0}]|第一次連線WebSocket：[{1}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Properties.Settings.Default.WebSocket_IP));
                    //}
                    //else
                    //{
                    //    // 確認WebSocket是否發生錯誤或關閉，若發生錯誤或關閉則須重新建立WebSocket與訊息監聽
                    //    if (m_oWSktTool.IsWebSocket_Close || m_oWSktTool.IsWebSocket_Error) // IsWebSocket_Close要改成true
                    //    {
                    //        if (m_oWSktTool.IsRestartAfterCloseOrError)
                    //        {
                    //            m_oWSktTool.WSktManager();
                    //            Console.WriteLine(string.Format("[{0}]|重複連線WebSocket：[{1}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Properties.Settings.Default.WebSocket_IP));
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (!m_oWSktTool.IsSelectedDateRS_Receive)
                    //        {
                    //            //m_iMSGRS_Status = 1;    // For Test
                    //            switch (m_iMSGRS_Status)
                    //            {
                    //                case 0:
                    //                    // 初次提出航班行李預排日期之要求訊息
                    //                    m_iMSGRS_Status = m_oWSktTool.SendMSGSelectedDateRQ(0);
                    //                    break;
                    //                default:
                    //                    pCount++;
                    //                    // 超過預設之重新傳送訊息次數則關閉程式
                    //                    m_iMSGRQ_Retry = Properties.Settings.Default.MsgRQ_Retry;

                    //                    if (pCount <= m_iMSGRQ_Retry)
                    //                    {
                    //                        // 程式處理發生問題，重新傳送詢問要求
                    //                        m_iMSGRS_Status = m_oWSktTool.SendMSGSelectedDateRQ(-1);
                    //                    }
                    //                    else
                    //                    {
                    //                        m_IsProgramMainHold = false;
                    //                    }

                    //                    break;
                    //            }
                    //        }
                    //        else
                    //        {
                                if (!m_MainThread.IsThreadRunning)
                                {
                                    // 航班行李預排日期設定完成後，才啟動航班行李轉盤預排最佳化程式MainThread執行緒
                                    m_MainThread.RunThread();
                                    //if (!m_oWSktTool.IsWebSocket_Close)
                                    //{
                                    //    m_oWSktTool.SendMSGProgressRate(0);
                                    //}
                                    //else
                                    //{

                                    //}
                                    Console.WriteLine("傳送行李轉盤預排演算進度(0)");
                                    m_LastSendTime = DateTime.Now;
                                }
                                else
                                {
                                    // 間隔5秒(預設)傳送目前預排演算進度
                                    if ((DateTime.Now - m_LastSendTime).TotalMilliseconds >= Properties.Settings.Default.Timeout_PR)
                                    {
                                        // 傳送目前預排演算進度回報之訊息
                                        //m_oWSktTool.SendMSGProgressRate(m_MainThread.ProgressRate);
                                        Console.WriteLine(string.Format("傳送行李轉盤預排演算進度({0})", m_MainThread.ProgressRate));
                                                    m_LastSendTime = DateTime.Now;
                                    }
                                }
                    //        }
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    TxtLog.Error(ex);
                    if (m_BeatThread != null)
                    {
                        m_BeatThread.StopThread();
                        m_BeatThread = null;
                    }
                    if (m_MainThread != null)
                    {
                        m_MainThread.StopThread();
                        m_MainThread = null;
                    }
                    if (m_oWSktTool !=null)
                    {
                        m_oWSktTool.CloseWebSocket();
                        m_oWSktTool = null;
                    }

                    m_IsProgramMainHold = false;
                }

                // 確認航班行李預排最佳化演算是否成功，以及各個執行緒執行是否異常，若未完成且無異常則主程式進入點保持運作，反之停止所有執行緒
                if (!m_MainThread.OAS_Finished && !m_MainThread.OAS_Error && !m_BeatThread.Health_Error)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    if (m_MainThread.OAS_Error || m_BeatThread.Health_Error)
                    {
                        // 非正常停止情況
                        if (m_BeatThread != null)
                        {
                            m_BeatThread.StopThread();
                            m_BeatThread = null;
                        }
                        if (m_MainThread != null)
                        {
                            m_MainThread.StopThread();
                            m_MainThread = null;
                        }
                        if (m_oWSktTool != null)
                        {
                            m_oWSktTool.CloseWebSocket();
                            m_oWSktTool = null;
                        }
                    }
                    else
                    {
                        // 一般正常完成演算停止情況
                        m_oWSktTool.SendMSGScheduleResult(m_MainThread.DataPoolSet.OASResultSet);   // 傳送預排結果訊息
                        if (m_BeatThread != null)
                        {
                            m_BeatThread.StopThread();
                            m_BeatThread = null;
                        }
                        if (m_MainThread != null)
                        {
                            m_MainThread.StopThread();
                            m_MainThread = null;
                        }
                        if (m_oWSktTool != null)
                        {
                            m_oWSktTool.CloseWebSocket();
                            m_oWSktTool = null;
                        }
                    }

                    m_IsProgramMainHold = false;
                }
            }
        }
    }
}
