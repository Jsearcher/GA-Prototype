using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using T1_BHS_OAS.DBLib;

namespace T1_BHS_OAS.GA
{
    public class Constraints
    {
        #region =====[Public] Event=====



        #endregion

        #region =====[Protected] Event Raise=====



        #endregion

        #region =====[Public] Default Event Body=====



        #endregion

        #region =====[Private] Class=====

        /// <summary>
        /// T1航班行李報到櫃檯屬性設定值類別
        /// </summary>
        private class Counter_Prop
        {
            /// <summary>
            /// 櫃檯號碼對應Array
            /// </summary>
            public int[] CounterNo = null;

            /// <summary>
            /// 櫃檯對應區域Array
            /// </summary>
            public string[] CounterRegion = null;
        }

        #endregion

        #region =====[Public] Class=====

        /// <summary>
        /// 演算法限制條件(計算適應值函數之參數矩陣)類別(靜態)
        /// </summary>
        public static class ConstraintParam_GA
        {
            /// <summary>
            /// 線性不等式之參數矩陣(A*x s= b 係數項)；s=為小於等於
            /// </summary>
            public static double[] A = null;

            /// <summary>
            /// 線性不等式之向量矩陣(A*x s= b 常數項)；s=為小於等於
            /// </summary>
            public static double[] b = null;

            /// <summary>
            /// 線性等式之參數矩陣(Aeq*x = beq 係數項)
            /// </summary>
            public static double[] Aeq = null;

            /// <summary>
            /// 線性等式之向量矩陣(A*x = beq 常數項)
            /// </summary>
            public static double[] beq = null;

            /// <summary>
            /// 基因變數下限值之向量矩陣(預設為-Inf)
            /// </summary>
            public static double[] LB = null;

            /// <summary>
            /// 基因變數上限值之向量矩陣(預設為Inf)
            /// </summary>
            public static double[] UB = null;

            /// <summary>
            /// 基因變數為整數之序號向量
            /// </summary>
            public static int[] intcon = null;
        }

        /// <summary>
        /// 自定義限制條件開關類別(靜態)
        /// </summary>
        public static class Custom_Switch
        {
            /// <summary>
            /// 自定義限制條件開關 - 目前航班行李允許(Allowed)之轉盤配置(Carousel Configuration)
            /// </summary>
            public static bool SW_ACC { get; set; }

            /// <summary>
            /// 自定義限制條件開關 - 航班位於尖峰時段(Rush-hour)之配置(Configuration)
            /// </summary>
            public static bool SW_RC { get; set; }

            /// <summary>
            /// 自定義限制條件開關 - 航班間時間是否重疊(Time Overlap)之配置(Configuration)
            /// </summary>
            public static bool SW_TOC = true;

            /// <summary>
            /// 自定義限制條件開關 - 目前航班間轉盤重疊共用(Carousel Overlap)之配置(Configuration)
            /// </summary>
            public static bool SW_COC = true;

            /// <summary>
            /// 自定義限制條件開關 - 航班間地勤(Ground)異同(Difference)之配置(Configuration)
            /// </summary>
            public static bool SW_GDC { get; set; }

            /// <summary>
            /// 自定義限制條件開關 - 航班間目的地簡碼(Destination)太相似(Similar)之配置(Configuration)
            /// </summary>
            public static bool SW_DSC { get; set; }

            /// <summary>
            /// 自定義限制條件開關 - 航班間航班大小(Flight Size)過大之配置(Configuration)
            /// </summary>
            public static bool SW_FSC { get; set; }

            /// <summary>
            /// 自定義限制條件開關 - 航班所屬報到櫃檯與轉盤間關係(Counter & Carousel)之配置(Configuration)
            /// </summary>
            public static bool SW_CCC { get; set; }
        }

        /// <summary>
        /// 自定義限制條件配置設定類別(靜態)
        /// </summary>
        public static class Custom_Config
        {
            /// <summary>
            /// 航班指定之轉盤配置List
            /// </summary>
            public static List<int> FlightCarouselNo = null;

            /// <summary>
            /// 航班指定之報到櫃檯List
            /// </summary>
            public static List<int> FlightCounterNo = null;

            /// <summary>
            /// 目前航班行李允許(Allowed)之轉盤配置(Carousel Configuration)
            /// </summary>
            public static int[,] ACC = null;

            /// <summary>
            /// 航班位於尖峰時段(Rush-hour)之配置(Configuration)
            /// </summary>
            public static int[] RC = null;

            /// <summary>
            /// 航班間時間是否重疊(Time Overlap)之配置(Configuration)
            /// </summary>
            public static int[,] TOC = null;

            /// <summary>
            /// 航班間重疊的時間(Overlap Time)之配置(Configuration)
            /// </summary>
            public static int[,] OTC = null;

            /// <summary>
            /// 目前航班間轉盤重疊共用(Carousel Overlap)之配置(Configuration)
            /// </summary>
            public static int[,] COC = null;

            /// <summary>
            /// 航班間地勤(Ground)異同(Difference)之配置(Configuration)
            /// </summary>
            public static int[,] GDC = null;

            /// <summary>
            /// 航班間目的地簡碼(Destination)太相似(Similar)之配置(Configuration)
            /// </summary>
            public static int[,] DSC = null;

            /// <summary>
            /// 航班間航班大小(Flight Size)過大之配置(Configuration)
            /// </summary>
            public static int[,] FSC = null;

            /// <summary>
            /// 共用轉盤時間重疊的航班間地勤相異之限制條件
            /// </summary>
            public static int[,] GroundConflict { get; set; }

            /// <summary>
            /// 共用轉盤時間重疊的航班間目的地太相似之限制條件
            /// </summary>
            public static int[,] DestinationConflict { get; set; }

            /// <summary>
            /// 共用轉盤時間重疊的航班間航班大小配置之限制條件
            /// </summary>
            public static int[,] FlightSizeConflict { get; set; }

            /// <summary>
            /// 各航班共用轉盤時間重疊數量
            /// </summary>
            public static int[] TimeCarouselOverlap { get; set; }
        }

        /// <summary>
        /// T1轉盤屬性設定值類別 [靜態]
        /// </summary>
        public static class Carousel_Prop
        {
            /// <summary>
            /// 轉盤號碼對應Array
            /// </summary>
            public static double[] CarouselNo = null;

            /// <summary>
            /// 尖峰時段轉盤之航班上限值Array
            /// </summary>
            public static int[] UpLimitRush = null;

            /// <summary>
            /// 離峰時段轉盤之航班上限值Array
            /// </summary>
            public static int[] UpLimitNormal = null;

            /// <summary>
            /// 問題轉盤Array
            /// </summary>
            public static int[] IsProblem = null;

            /// <summary>
            /// 轉盤對應區域Array
            /// </summary>
            public static string[] CarouselRegion = null;
        }

        #endregion

        #region =====[Private] Variable=====

        ///// <summary>
        ///// 演算法限制條件(計算適應值函數之參數矩陣)
        ///// </summary>
        //private static ConstraintParam_GA CP_GA = new ConstraintParam_GA();

        ///// <summary>
        ///// 自定義限制條件開關物件
        ///// </summary>
        //private static Custom_Switch SW_Custom = new Custom_Switch();

        ///// <summary>
        ///// 自定義限制條件配置設定物件
        ///// </summary>
        //private static Custom_Config Config_Custom = new Custom_Config();

        ///// <summary>
        ///// T1轉盤屬性設定值物件
        ///// </summary>
        //private static Carousel_Prop CarouselProp = new Carousel_Prop();

        /// <summary>
        /// 前日開櫃時間內的航班數量
        /// </summary>
        private static int m_iRS_fix = default(int);

        /// <summary>
        /// T1轉盤目前可使用之轉盤列表(LATERAL_ID僅留數值)
        /// </summary>
        private static List<int> m_CarouselList = null;

        /// <summary>
        /// 航班行李預排日期前與設定的開櫃時間內的航班行李轉盤配置List
        /// </summary>
        private static List<double> m_LastCarouselList = null;
        
        /// <summary>
        /// T1航班行李報到櫃檯屬性設定值物件
        /// </summary>
        private static Counter_Prop CounterProp = new Counter_Prop();

        /// <summary>
        /// 隨機亂數種子
        /// </summary>
        private static Random ms_rnd = new Random();

        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        #endregion

        #region =====[Public] Property=====

        /// <summary>
        /// 線性不等式之參數矩陣(A*x s= b 係數項)；s=為小於等於
        /// </summary>
        public const string cA = "A";

        /// <summary>
        /// 線性不等式之向量矩陣(A*x s= b 常數項)；s=為小於等於
        /// </summary>
        public const string cb = "b";

        /// <summary>
        /// 線性等式之參數矩陣(Aeq*x = beq 係數項)
        /// </summary>
        public const string cAeq = "Aeq";

        /// <summary>
        /// 線性等式之向量矩陣(A*x = beq 常數項)
        /// </summary>
        public const string cbeq = "beq";

        /// <summary>
        /// 基因變數下限值之向量矩陣(預設為-Inf)
        /// </summary>
        public const string cLB = "LB";

        /// <summary>
        /// 基因變數上限值之向量矩陣(預設為Inf)
        /// </summary>
        public const string cUB = "UB";

        /// <summary>
        /// 基因變數為整數之序號向量
        /// </summary>
        public const string cintcon = "intcon";

        /// <summary>
        /// 自定義限制條件開關 - 目前航班行李允許(Allowed)之轉盤配置(Carousel Configuration)
        /// </summary>
        public const string cSW_ACC = "SW_ACC";

        /// <summary>
        /// 自定義限制條件開關 - 航班位於尖峰時段(Rush-hour)之配置(Configuration)
        /// </summary>
        public const string cSW_RC = "SW_RC";

        /// <summary>
        /// 自定義限制條件開關 - 航班間地勤(Ground)異同(Difference)之配置(Configuration)
        /// </summary>
        public const string cSW_GDC = "SW_GDC";

        /// <summary>
        /// 自定義限制條件開關 - 航班間目的地簡碼(Destination)太相似(Similar)之配置(Configuration)
        /// </summary>
        public const string cSW_DSC = "SW_DSC";

        /// <summary>
        /// 自定義限制條件開關 - 航班間航班大小(Flight Size)過大之配置(Configuration)
        /// </summary>
        public const string cSW_FSC = "SW_FSC";

        /// <summary>
        /// 自定義限制條件開關 - 航班所屬報到櫃檯與轉盤間關係(Counter & Carousel)之配置(Configuration)
        /// </summary>
        public const string cSW_CCC = "SW_CCC";

        #endregion

        #region =====[Private] Function=====

        #region SetACC(DataPool pDataPool)//int
        /// <summary>
        /// 設定基因演算法限制條件(ACC; Allowed Carousel Configuration)
        /// </summary>
        /// <param name="pDataPool">航班行李之轉盤配置預排資料集物件</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        private static int SetACC(DataPool pDataPool)
        {
            try
            {
                int rs = pDataPool.OASResultSet.FLIGHT_NO.Count;
                int cs = pDataPool.CarouselList.Count;
                int ns = pDataPool.CounterProperty.Count;
                Custom_Config.ACC = new int[rs, cs];

                m_LastCarouselList = pDataPool.LastCarouselList.Select(lcl => double.Parse(Regex.Replace(lcl.LATERAL_ID, @"[^\d]", string.Empty))).ToList();

                for (int i = 0; i < rs; i++)
                {
                    int current_assigned_carousel = i < m_iRS_fix ? int.Parse(m_LastCarouselList[i].ToString()) : pDataPool.OASResultSet.CAROUSEL_NO[i - m_iRS_fix];  // 目前各航班(包含前日開櫃時間內)指定的轉盤配置設定
                    int current_counter = i < m_iRS_fix ? 0 : pDataPool.OASResultSet.COUNTER_NO[i - m_iRS_fix];   // 目前各航班(包含前日開櫃時間內)指定的報到櫃檯設定
                    string current_region = null;                                           // 目前航班指定的報到櫃檯對應之轉盤區域設定

                    for (int j = 0; j < ns; j++)
                    {
                        if (current_counter == pDataPool.CounterProperty.Select(cp => cp.COUNTER_NO).ToList()[j])
                        {
                            current_region = pDataPool.CounterProperty.Select(cp => cp.REGION).ToList()[j];
                            break;
                        }
                    }

                    for (int j = 0; j < cs; j++)
                    {
                        Custom_Config.ACC[i, j] = 1;
                        int current_carousel = pDataPool.CarouselList[j];     // 目前轉盤屬性設定之轉盤配置
                        string current_carousel_region = pDataPool.CarouselProperty.Select(cp => cp.REGION).ToList()[j];// 目前轉盤屬性設定之轉盤區域
                        int carousel_is_problem = pDataPool.CarouselProperty.Select(cp => cp.IS_PROBLEM).ToList()[j];   // 目前轉盤屬性設定之問題轉盤
                        int carousel_in_use = pDataPool.CarouselProperty.Select(cp => cp.IS_USE).ToList()[j];           // 目前轉盤屬性設定之轉盤使用狀態

                        // 確認轉盤是否設定使用
                        if (carousel_in_use != 1)
                        {
                            Custom_Config.ACC[i, j] = 0;
                        }
                        else
                        {
                            if (Custom_Switch.SW_ACC || i < m_iRS_fix)
                            {
                                // 確認航班指定之轉盤配置設定
                                if (current_assigned_carousel != 0 && current_assigned_carousel == current_carousel)
                                {
                                    Custom_Config.ACC[i, j] = 1;
                                }
                                else
                                {
                                    Custom_Config.ACC[i, j] = 0;
                                }
                            }
                            else
                            {
                                // 確認非問題轉盤
                                if (carousel_is_problem == 1)
                                {
                                    Custom_Config.ACC[i, j] = 0;
                                }

                                if (Custom_Switch.SW_CCC)
                                {
                                    // 確認航班指定報到櫃檯使用之轉盤區域設定
                                    if (current_region != current_carousel_region)
                                    {
                                        Custom_Config.ACC[i, j] = 0;
                                    }
                                }
                            }
                        }
                    }
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

        #region SetRC(DataPool pDataPool)//int
        /// <summary>
        /// 設定基因演算法限制條件(RC; Rush-hour Configuration)
        /// </summary>
        /// <param name="pDataPool">航班行李之轉盤配置預排資料集物件</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        private static int SetRC(DataPool pDataPool)
        {
            int s, e, count, limit = 0, rush, normal;
            List<int> HalfHourFlight = new List<int>();
            List<string> EOT_fix = new List<string>(m_iRS_fix);
            List<string> ECT_fix = new List<string>(m_iRS_fix);

            for (int i = 0; i < m_iRS_fix; i++)
            {
                int pTime_EOT = int.Parse(pDataPool.LastCarouselList.Select(re => re.STD).ToList()[i]) - pDataPool.TotalTime_EOT;
                int pTime_ECT = int.Parse(pDataPool.LastCarouselList.Select(re => re.STD).ToList()[i]) - pDataPool.TotalTime_ECT;
                EOT_fix[i] = Math.Floor((double)(pTime_EOT / 60)).ToString().PadLeft(2, '0') + (pTime_EOT % 60).ToString().PadLeft(2, '0');
                ECT_fix[i] = Math.Floor((double)(pTime_ECT / 60)).ToString().PadLeft(2, '0') + (pTime_ECT % 60).ToString().PadLeft(2, '0');
            }

            List<int> TotalTime_EOT = TotalTime(pDataPool.LastCarouselList.Select(re => re.FIDS_DATE).ToList().Concat(pDataPool.OASResultSet.FDATE).ToList(), EOT_fix.Concat(pDataPool.OASResultSet.EOT).ToList(), pDataPool.SDATE); // 預計開櫃時間總分鐘數List
            List<int> TotalTime_ECT = TotalTime(pDataPool.LastCarouselList.Select(re => re.FIDS_DATE).ToList().Concat(pDataPool.OASResultSet.FDATE).ToList(), ECT_fix.Concat(pDataPool.OASResultSet.ECT).ToList(), pDataPool.SDATE); // 預計關櫃時間總分鐘數List
            Custom_Config.RC = new int[pDataPool.OASResultSet.FLIGHT_NO.Count + m_iRS_fix];

            try
            {
                // 依轉盤屬性設定計算離峰轉盤配置總數
                for (int i = 0; i < pDataPool.CarouselProperty.Count; i++)
                {
                    // 計算非問題轉盤、可以使用轉盤、離峰時段共用上限不為0轉盤的轉盤數量pDataPool.CarouselList.Contains(current_carousel)
                    if (pDataPool.CarouselProperty.Select(cp => cp.IS_PROBLEM).ToList()[i] != 1 && pDataPool.CarouselList.Contains(pDataPool.CarouselProperty.Select(cp => cp.IS_USE).ToList()[i]) && pDataPool.CarouselProperty.Select(cp => cp.UP_LIMIT_NORMAL).ToList()[i] > 0)
                    {
                        limit++;    // 離峰時可配置轉盤(上限>0)總數
                    }
                }
                limit = limit == 0 ? 1 : limit; // 防止其值為0

                // 統整00:00~23:59間每半小時間隔的航班處理數量
                for (int i = 0; i < 48; i++)
                {
                    s = (30 * i) - pDataPool.TotalTime_EOT; // 總分鐘數起始(00:00 - EOT開始開櫃)
                    e = s + 30;                             // 總分鐘數結束
                    count = 0;
                    for (int j = 0; j < TotalTime_EOT.Count; j++)
                    {
                        if ((s < TotalTime_ECT[j] && TotalTime_ECT[j] < e) ||
                            (TotalTime_EOT[j] < s && e < TotalTime_ECT[j]) ||
                            (s < TotalTime_EOT[j] && TotalTime_EOT[j] < e))
                        {
                            count++;
                        }
                    }
                    HalfHourFlight.Add(((double)count / limit) > 1 ? 1 : 0);    // 精度為半小時間隔的尖峰時段List(1: 尖峰; 0: 離峰)
                }

                // 判斷航班位於尖峰時段之配置(RC)
                for (int i = 0; i < TotalTime_EOT.Count + m_iRS_fix; i++)
                {
                    if (i < m_iRS_fix)
                    {
                        Custom_Config.RC[i] = 0;
                    }
                    else
                    {
                        rush = 0;
                        normal = 0;
                        for (int j = 0; j < HalfHourFlight.Count; j++)
                        {
                            s = (30 * j) - pDataPool.TotalTime_EOT; // 總分鐘數起始(00:00 - EOT開始開櫃)
                            e = s + 30;                             // 總分鐘數結束
                            if ((s < TotalTime_ECT[i] && TotalTime_ECT[i] < e) ||
                                (TotalTime_EOT[i] < s && e < TotalTime_ECT[i]) ||
                                (s < TotalTime_EOT[i] && TotalTime_EOT[i] < e))
                            {
                                if (HalfHourFlight[j] == 1)
                                {
                                    rush++;
                                }
                                else
                                {
                                    normal++;
                                }
                            }
                        }
                        Custom_Config.RC[i] = rush > normal ? 1 : 0;
                    }
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

        #region SetTOCandOTC(DataPool pDataPool)//int
        /// <summary>
        /// 設定基因演算法限制條件(TOC and OTC; Time Overlap Configuration and Overlap Time Configuration)
        /// </summary>
        /// <param name="pDataPool">航班行李之轉盤配置預排資料集物件</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        private static int SetTOCandOTC(DataPool pDataPool)
        {
            List<string> EOT_fix = new List<string>(m_iRS_fix);
            List<string> ECT_fix = new List<string>(m_iRS_fix);
            List<string> EOT_Date = new List<string>(pDataPool.OASResultSet.FLIGHT_NO.Count);
            List<string> ECT_Date = new List<string>(pDataPool.OASResultSet.FLIGHT_NO.Count);

            for (int i = 0; i < m_iRS_fix; i++)
            {
                EOT_fix.Add(DateTime.ParseExact(pDataPool.LastCarouselList.Select(re => re.FIDS_DATE).ToList()[i] + pDataPool.LastCarouselList.Select(re => re.STD).ToList()[i], "yyyyMMddHHmm", null)
                    .AddMinutes(-pDataPool.TotalTime_EOT).ToString("yyyyMMddHHmm").Substring(8, 4));
                ECT_fix.Add(DateTime.ParseExact(pDataPool.LastCarouselList.Select(re => re.FIDS_DATE).ToList()[i] + pDataPool.LastCarouselList.Select(re => re.STD).ToList()[i], "yyyyMMddHHmm", null)
                    .AddMinutes(-pDataPool.TotalTime_ECT).ToString("yyyyMMddHHmm").Substring(8, 4));
            }

            for (int i = 0; i < pDataPool.OASResultSet.FLIGHT_NO.Count; i++)
            {
                int Time_EOT = int.Parse(pDataPool.OASResultSet.EOT[i].Substring(0, 2)) * 60 + int.Parse(pDataPool.OASResultSet.EOT[i].Substring(2, 2));
                int Time_ECT = int.Parse(pDataPool.OASResultSet.ECT[i].Substring(0, 2)) * 60 + int.Parse(pDataPool.OASResultSet.ECT[i].Substring(2, 2));
                int Time_STD = int.Parse(pDataPool.OASResultSet.STD[i].Substring(0, 2)) * 60 + int.Parse(pDataPool.OASResultSet.STD[i].Substring(2, 2));
                if (Time_EOT > Time_STD)
                {
                    EOT_Date.Add(DateTime.ParseExact(pDataPool.OASResultSet.FDATE[i], "yyyyMMdd", null).AddDays(-1).ToString("yyyyMMdd"));
                }
                else
                {
                    EOT_Date.Add(pDataPool.OASResultSet.FDATE[i]);
                }
                if (Time_ECT > Time_STD)
                {
                    ECT_Date.Add(DateTime.ParseExact(pDataPool.OASResultSet.FDATE[i], "yyyyMMdd", null).AddDays(-1).ToString("yyyyMMdd"));
                }
                else
                {
                    ECT_Date.Add(pDataPool.OASResultSet.FDATE[i]);
                }
            }

            List<int> TotalTime_EOT = TotalTime(pDataPool.LastCarouselList.Select(re => re.FIDS_DATE).ToList().Concat(EOT_Date).ToList(), EOT_fix.Concat(pDataPool.OASResultSet.EOT).ToList(), pDataPool.SDATE); // 預計開櫃時間總分鐘數List
            List<int> TotalTime_ECT = TotalTime(pDataPool.LastCarouselList.Select(re => re.FIDS_DATE).ToList().Concat(ECT_Date).ToList(), ECT_fix.Concat(pDataPool.OASResultSet.ECT).ToList(), pDataPool.SDATE); // 預計關櫃時間總分鐘數List
            int ns = pDataPool.OASResultSet.FLIGHT_NO.Count + m_iRS_fix;
            Custom_Config.TOC = new int[ns, ns];
            Custom_Config.OTC = new int[ns, ns];

            try
            {
                for (int i = 0; i < ns; i++)
                {
                    for (int j = 0; j < ns; j++)
                    {
                        int overlap_time = TotalTime_ECT[i] - TotalTime_EOT[j];
                        Custom_Config.TOC[i, j] = overlap_time > 0 ? 1 : 0;
                        Custom_Config.OTC[i, j] = overlap_time > 0 ? overlap_time : 0;
                    }
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

        #region SetGDC(List<string> ground)//int
        /// <summary>
        /// 設定基因演算法限制條件(GDC; Ground Difference Configuration)
        /// </summary>
        /// <param name="ground">航班行李之轉盤配置預排資料集物件之地勤</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        private static int SetGDC(List<string> ground)
        {
            int ns = ground.Count + m_iRS_fix;
            Custom_Config.GDC = new int[ns, ns];

            try
            {
                for (int i = 0; i < ns - 1; i++)
                {
                    for (int j = i + 1; j < ns; j++)
                    {
                        if (i < m_iRS_fix || j < m_iRS_fix)
                        {
                            Custom_Config.GDC[i, j] = 0;
                        }
                        else
                        {
                            Custom_Config.GDC[i, j] = ground[i] != ground[j] ? 1 : 0;
                        }
                    }
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

        #region SetDSC(List<string> destination//int
        /// <summary>
        /// 設定基因演算法限制條件(DSC; Destination Similar Configuration)
        /// </summary>
        /// <param name="destination">航班行李之轉盤配置預排資料集物件之目的地</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        private static int SetDSC(List<string> destination)
        {
            int count;
            int ns = destination.Count + m_iRS_fix;
            Custom_Config.DSC = new int[ns, ns];

            try
            {
                for (int i = 0; i < ns; i++)
                {
                    for (int j = i + 1; j < ns - 1; j++)
                    {
                        if (i < m_iRS_fix || j < m_iRS_fix)
                        {
                            Custom_Config.DSC[i, j] = 0;
                        }
                        else
                        {
                            char[] DES_i = destination[i].ToCharArray();
                            char[] DES_j = destination[j].ToCharArray();
                            count = 0;
                            for (int k = 0; k < DES_i.Length; k++)
                            {
                                if (DES_i[k] == DES_j[k])
                                {
                                    count++;
                                }

                            }
                            Custom_Config.DSC[i, j] = count > 1 ? 1 : 0;
                        }
                    }
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

        #region SetFSC(List<string> flight_size)//int
        /// <summary>
        /// 設定基因演算法限制條件(FSC; Flight Size Configuration)
        /// </summary>
        /// <param name="flight_size">航班行李之轉盤配置預排資料集物件之航班大小</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        private static int SetFSC(List<string> flight_size)
        {
            int ns = flight_size.Count + m_iRS_fix;
            Custom_Config.FSC = new int[ns, ns];

            try
            {
                for (int i = 0; i < ns - 1; i++)
                {
                    for (int j = i + 1; j < ns; j++)
                    {
                        if (i < m_iRS_fix || j < m_iRS_fix)
                        {
                            Custom_Config.FSC[i, j] = 0;
                        }
                        else
                        {
                            Custom_Config.FSC[i, j] = (flight_size[1] == "L" && flight_size[j] == "L" || flight_size[i] == "L" && flight_size[j] == "M" || flight_size[i] == "M" && flight_size[j] == "L") ? 1 : 0;
                        }
                    }
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

        #region SetCarouselNo(List<GA_CAROUSEL_TABLE_T1.Row> carousel_property)//int
        /// <summary>
        /// 設定T1轉盤屬性設定值Array
        /// </summary>
        /// <param name="carousel_property">T1轉盤屬性設定值</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        private static int SetCarouselProperty(List<GA_CAROUSEL_TABLE_T1.Row> carousel_property)
        {
            int ns = carousel_property.Count;
            Carousel_Prop.CarouselNo = new double[ns];
            Carousel_Prop.UpLimitRush = new int[ns];
            Carousel_Prop.UpLimitNormal = new int[ns];
            Carousel_Prop.IsProblem = new int[ns];
            Carousel_Prop.CarouselRegion = new string[ns];
            try
            {
                for (int i = 0; i < ns; i++)
                {
                    Carousel_Prop.CarouselNo[i] = double.Parse(carousel_property[i].CAROUSEL_NO.ToString());
                    Carousel_Prop.CarouselRegion[i] = carousel_property[i].REGION;
                    Carousel_Prop.IsProblem[i] = carousel_property[i].IS_PROBLEM;
                    Carousel_Prop.UpLimitRush[i] = carousel_property[i].UP_LIMIT_RUSH;
                    Carousel_Prop.UpLimitNormal[i] = carousel_property[i].UP_LIMIT_NORMAL;
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

        #region SetCounterProperty(List<GA_COUNTER_TABLE_T1.Row> counter_troperty)//int
        /// <summary>
        /// 設定T1航班行李報到櫃檯屬性設定值Array
        /// </summary>
        /// <param name="counter_property">T1航班行李報到櫃檯屬性設定值</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        private static int SetCounterProperty(List<GA_COUNTER_TABLE_T1.Row> counter_property)
        {
            int ns = counter_property.Count;
            CounterProp.CounterNo = new int[ns];
            CounterProp.CounterRegion = new string[ns];

            try
            {
                for (int i = 0; i < ns; i++)
                {
                    CounterProp.CounterNo[i] = counter_property[i].COUNTER_NO;
                    CounterProp.CounterRegion[i] = counter_property[i].REGION;
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

        #region TotalTime(List<string> pDateTime, string pSDATE)//List<int>
        /// <summary>
        /// 以航班行李預排選擇日期的00:00為基準所計算輸入之時間總分鐘數
        /// </summary>
        /// <param name="pDate">輸入日期(yyyyMMdd)List</param>
        /// <param name="pTime">輸入時間(HHmm)List</param>
        /// <param name="pSDATE">航班預排選擇日期(yyyyMMdd)</param>
        /// <returns>時間總分鐘數List</returns>
        private static List<int> TotalTime(List<string> pDate, List<string> pTime, string pSDATE)
        {
            List<int> TimeList = new List<int>();

            try
            {
                for (int i = 0; i < pTime.Count; i++)
                {
                    int Hour = int.Parse(pTime[i].Substring(0, 2));
                    int Minute = int.Parse(pTime[i].Substring(2, 2));
                    
                    if (DateTime.ParseExact(pDate[i], "yyyyMMdd", null) < DateTime.ParseExact(pSDATE, "yyyyMMdd", null))
                    {
                        // Yesterday
                        if (60 - Minute == 60)
                        {
                            TimeList.Add(-(24 - Hour) * 60);
                        }
                        else
                        {
                            TimeList.Add(-(23 - Hour) * 60 + (60 - Minute));
                        }
                    }
                    else if (DateTime.ParseExact(pDate[i], "yyyyMMdd", null) > DateTime.ParseExact(pSDATE, "yyyyMMdd", null))
                    {
                        // Tomorrow
                        TimeList.Add((Hour + 24) * 60 + Minute);
                    }
                    else
                    {
                        // Today
                        TimeList.Add(Hour * 60 + Minute);
                    }
                }
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }

            return TimeList;
        }
        #endregion

        #endregion

        #region =====[Public] Method=====

        /// <summary>
        /// 依限制條件隨機選擇基因變數值
        /// </summary>
        /// <param name="index">基因變數序號</param>
        /// <returns>基因變數值</returns>
        public static double GetValueByConstraint(int index)
        {
            // 等式及不等式限制條件
            // ...Unfinished...

            return default(double);
        }

        /// <summary>
        /// 依限制條件(隨目前基因組合而修正)決定基因變數值候選列表
        /// </summary>
        /// <param name="chromosome">目前基因組合</param>
        /// <param name="index">基因變數序號</param>
        /// <returns>基因變數值候選列表</returns>
        public static List<double> GetCandidateByConstraint(List<double> chromosome, int index)
        {
            // do something...

            return default(List<double>);
        }

        #region GetValueByConstraintInOrder(List<double> chromosome, int index)//double
        /// <summary>
        /// 依限制條件(隨目前基因組合而修正)及指定方法決定基因變數值
        /// </summary>
        /// <param name="chromosome">目前基因組合</param>
        /// <param name="index">基因變數序號</param>
        /// <returns>基因變數值</returns>
        public static double GetValueByConstraintInOrder(List<double> chromosome, int index)
        {
            // 決定基因變數值候選 -> 依指定方法隨機選擇基因變數值
            int ns = Custom_Config.ACC.GetLength(0);    // 航班數量
            int cs = Custom_Config.ACC.GetLength(1);    // 轉盤數量
            List<double> candidate = new List<double>();    // 基因變數值候選
            List<double> tempList = new List<double>();     // 替換用List
            string current_flight_region = Custom_Switch.SW_ACC ? CounterProp.CounterRegion[Array.IndexOf(CounterProp.CounterNo, Custom_Config.FlightCounterNo[index])] : null; // 目前航班指定之報到櫃檯對應的區域

            double candidateRtn = default(double);

            try
            {
                // 更新目前航班間轉盤重疊共用之配置(COC; Carousel Overlap Configuration)
                chromosome[index] = candidateRtn;
                if (SetCOC(chromosome) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|更新目前航班間轉盤重疊共用之配置COC發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("更新目前航班間轉盤重疊共用之配置COC發生錯誤！");
                }

                // 列出該基因變數序號(index)於允許的轉盤配置(ACC)下之候選
                for (int i = 0; i < cs; i++)
                {
                    if (Custom_Config.ACC[index, i] == 1)
                    {
                        candidate.Add(m_CarouselList[i]);
                        tempList.Add(m_CarouselList[i]);
                    }
                }

                if (Custom_Switch.SW_ACC || index < m_iRS_fix)
                {
                    // 若於允許的轉盤配置(ACC)下之後僅有一個，表示為指定配置，回傳該配置
                    if (candidate.Count == 1)
                    {
                        return candidate[0];
                    }
                }

                // 比對目前基因組合(chromosome)以列出候選(candidate)中沒有超過尖峰離峰之航班上限值、沒有地勤相異(GDC)、沒有目的地太相似(DSC)、沒有航班大小配置問題(FSC)的基因變數值
                for (int i = 0; i < tempList.Count; i++)
                {
                    bool isRemoved = false;                                                             // 是否已移除候選變數值
                    int current_carousel_index = Array.IndexOf(m_CarouselList.ToArray(), tempList[i]);  // 目前候選轉盤序號
                    int rush = 0;                                                                       // 與候選轉盤共用之尖峰時段航班數量
                    int normal = 0;                                                                     // 與候選轉盤共用之離峰時段航班

                    for (int j = 0; j < chromosome.Count; j++)
                    {
                        if (chromosome[j] != 0)
                        {
                            if (chromosome[j] == tempList[i])
                            {
                                // 比對時間重疊且轉盤共用之航班沒有地勤相異(GDC)、沒有目的地太相似(DSC)、沒有航班大小配置問題(FSC)
                                int r = j > index ? index : j;
                                int c = j > index ? j : index;
                                if (Custom_Switch.SW_GDC)
                                {
                                    if (Custom_Config.GroundConflict[r, c] == 1)
                                    {
                                        candidate.Remove(tempList[i]);
                                        isRemoved = true;
                                        break;
                                    }
                                }
                                else if (Custom_Switch.SW_DSC)
                                {
                                    if (Custom_Config.DestinationConflict[r, c] == 1)
                                    {
                                        candidate.Remove(tempList[i]);
                                        isRemoved = true;
                                        break;
                                    }
                                }
                                else if (Custom_Switch.SW_FSC)
                                {
                                    if (Custom_Config.FlightSizeConflict[r, c] == 1)
                                    {
                                        candidate.Remove(tempList[i]);
                                        isRemoved = true;
                                        break;
                                    }
                                }

                                // 計算時間重疊下已配置轉盤之航班的尖峰與離峰數
                                if (Custom_Switch.SW_RC)
                                {
                                    if (Custom_Config.TOC[r, c] == 1)
                                    {
                                        if (Custom_Config.RC[j] == 1)
                                        {
                                            rush++;
                                        }
                                        else
                                        {
                                            normal++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // 若尚未移除候選變數值則以尖峰離峰上限值繼續判斷
                    if (!isRemoved && Custom_Switch.SW_RC)
                    {
                        // 依尖峰數量與離峰數量比例決定參考上限值
                        int UpLimit = rush > normal ? Carousel_Prop.UpLimitRush[current_carousel_index] : Carousel_Prop.UpLimitNormal[current_carousel_index];

                        // 比對時間重疊且轉盤共用之航班數量沒有超過尖峰(離峰)轉盤之航班上限值
                        for (int j = 0; j < Custom_Config.TimeCarouselOverlap.Length; j++)
                        {
                            if (chromosome[j] == tempList[i])
                            {
                                int r = j > index ? index : j;
                                int c = j > index ? j : index;
                                // 與對象航班時間重疊
                                if (Custom_Config.TOC[r, c] == 1)
                                {
                                    // 對象轉盤共用數量超過上限
                                    if (Custom_Config.TimeCarouselOverlap[j] >= UpLimit)
                                    {
                                        candidate.Remove(tempList[i]);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                // 依航班指定之櫃檯對應的區域隨機選擇候選中之基因變數值，若無候選則不配置
                if (candidate.Count > 0)
                {
                    candidateRtn = candidate[ms_rnd.Next(0, candidate.Count)];  // 隨機選擇候選中之基因變數值
                }
                else
                {
                    candidateRtn = 0;
                }

                return candidateRtn;    // 篩選之基因變數值(沒指定預設為第1個轉盤號碼
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return default(double);
            }
        }
        #endregion

        #region SetConstraints(Dictionary<string, object> constraints)//int
        /// <summary>
        /// 設定基因演算法限制條件 [函數問題]
        /// </summary>
        /// <param name="constraints">限制條件字典</param>
        /// <param name="nvars">基因組中基因變數個數(number of design variables)</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 限制條件設定值個數與 nvars 不相符</para>
        /// </returns>
        public static int SetConstraints(Dictionary<string, Array> constraints, int nvars)
        {
            try
            {
                foreach (KeyValuePair<string, Array> constraint in constraints)
                {
                    if (constraint.Key != cintcon && constraint.Value.Length != nvars)
                    {
                        Console.WriteLine(string.Format("[{0}]|Key: [{1}] 的限制條件設定值個數與 nvars: [{2}] 不相符", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), constraint.Key, nvars));
                        TxtLog.Error(string.Format("CODE[-2]|Key: [{0}] 的限制條件設定值個數與 nvars: [{1}] 不相符", constraint.Key, nvars));
                        return -2;
                    }

                    switch (constraint.Key)
                    {
                        case cA:
                            ConstraintParam_GA.A = (double[])constraint.Value;
                            break;
                        case cb:
                            ConstraintParam_GA.b = (double[])constraint.Value;
                            break;
                        case cAeq:
                            ConstraintParam_GA.Aeq = (double[])constraint.Value;
                            break;
                        case cbeq:
                            ConstraintParam_GA.beq = (double[])constraint.Value;
                            break;
                        case cLB:
                            ConstraintParam_GA.LB = (double[])constraint.Value;
                            break;
                        case cUB:
                            ConstraintParam_GA.UB = (double[])constraint.Value;
                            break;
                        case cintcon:
                            ConstraintParam_GA.intcon = (int[])constraint.Value;
                            break;
                        default:
                            Console.WriteLine(string.Format("[{0}]|無法設定 Key: [{1}] 作為基因演算法之限制條件參數", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), constraint.Key));
                            TxtLog.Error(string.Format("無法設定 Key: [{0}] 作為基因演算法之限制條件參數", constraint.Key));
                            break;
                    }
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

        #region SetConstraints(object pObj)//int
        /// <summary>
        /// 設定基因演算法限制條件與對應的條件開關 [自定義]
        /// </summary>
        /// <param name="pObj">限制條件物件</param>
        /// <param name="pSWs">限制條件開關物件</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 設定ACC發生錯誤</para>
        /// <para>-3: 設定RC發生錯誤</para>
        /// <para>-4: 設定TOC及OTC發生錯誤</para>
        /// <para>-5: 設定GDC發生錯誤</para>
        /// <para>-6: 設定DSC發生錯誤</para>
        /// <para>-7: 設定FSC發生錯誤</para>
        /// <para>-8: 設定COC發生錯誤</para>
        /// <para>-9: 設定CarouselProperty發生錯誤</para>
        /// <para>-10: 設定CounterProperty發生錯誤</para>
        /// </returns>
        public static int SetConstraints(object pObj, Dictionary<string, bool> pSWs)
        {
            try
            {
                DataPool oDataPool = (DataPool)pObj;
                // 設定航班指定之轉盤配置List(自定義)
                Custom_Config.FlightCarouselNo = oDataPool.OASResultSet.CAROUSEL_NO;
                // 設定航班指定之報到櫃檯List(自定義)
                Custom_Config.FlightCounterNo = oDataPool.OASResultSet.COUNTER_NO;
                // 設定T1轉盤目前可使用之轉盤列表(LATERAL_ID僅留數值)至此Constraints類別
                m_CarouselList = oDataPool.CarouselList;

                m_iRS_fix = oDataPool.LastCarouselList.Count;

                // 設定基因演算法限制條件(自定義)的開關
                foreach (KeyValuePair<string, bool> pSW in pSWs)
                {
                    switch (pSW.Key)
                    {
                        case cSW_ACC:
                            Custom_Switch.SW_ACC = pSW.Value;
                            break;
                        case cSW_RC:
                            Custom_Switch.SW_RC = pSW.Value;
                            break;
                        case cSW_GDC:
                            Custom_Switch.SW_GDC = pSW.Value;
                            break;
                        case cSW_DSC:
                            Custom_Switch.SW_DSC = pSW.Value;
                            break;
                        case cSW_FSC:
                            Custom_Switch.SW_FSC = pSW.Value;
                            break;
                        case cSW_CCC:
                            Custom_Switch.SW_CCC = pSW.Value;
                            break;
                        default:
                            Console.WriteLine(string.Format("[{0}]|無法設定 Key: [{1}] 作為基因演算法之限制條件(自定義)的開關參數", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), pSW.Key));
                            TxtLog.Error(string.Format("無法設定 Key: [{0}] 作為基因演算法之限制條件(自定義)的開關參數", pSW.Key));
                            break;
                    }
                }

                // 設定基因演算法限制條件(ACC; Allowed Carousel Configuration)
                if (SetACC(oDataPool) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|設定ACC發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-2]|設定ACC發生錯誤！");
                    return -2;
                }

                // 設定基因演算法限制條件(RC; Rush-hour Configuration)
                if (Custom_Switch.SW_RC)
                {
                    if (SetRC(oDataPool) != 0)
                    {
                        Console.WriteLine(string.Format("[{0}]|設定RC發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                        TxtLog.Error("CODE[-3]|設定RC發生錯誤！");
                        return -3;
                    }
                }

                // 設定基因演算法限制條件(TOC and OTC; Time Overlap Configuration and Overlap Time Configuration) [必要]
                if (SetTOCandOTC(oDataPool) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|設定TOC及OTC發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-4]|設定TOC及OTC發生錯誤！");
                    return -4;
                }

                // 設定基因演算法限制條件(GDC; Ground Difference Configuration)
                if (Custom_Switch.SW_GDC)
                {
                    if (SetGDC(oDataPool.OASResultSet.GROUND) != 0)
                    {
                        Console.WriteLine(string.Format("[{0}]|設定GDC發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                        TxtLog.Error("CODE[-5]|設定GDC發生錯誤！");
                        return -5;
                    }
                }

                // 設定基因演算法限制條件(DSC; Destination Similar Configuration)
                if (Custom_Switch.SW_DSC)
                {
                    if (SetDSC(oDataPool.OASResultSet.DESTINATION) != 0)
                    {
                        Console.WriteLine(string.Format("[{0}]|設定DSC發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                        TxtLog.Error("CODE[-6]|設定DSC發生錯誤！");
                        return -6;
                    }
                }

                // 設定基因演算法限制條件(FSC; Flight Size Configuration)
                if (Custom_Switch.SW_FSC)
                {
                    if (SetFSC(oDataPool.OASResultSet.FLIGHT_SIZE) != 0)
                    {
                        Console.WriteLine(string.Format("[{0}]|設定FSC發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                        TxtLog.Error("CODE[-7]|設定FSC發生錯誤！");
                        return -7;
                    }
                }

                // 設定基因演算法限制條件(COC; Carousel Overlap Configuration) [必要]
                List<double> carousel_no_lf = oDataPool.OASResultSet.CAROUSEL_NO.Select<int, double>(i => i).ToList();
                if (SetCOC(carousel_no_lf) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|設定COC發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-8]|設定COC發生錯誤！");
                    return -8;
                }

                // 設定轉盤屬性設定List [保留；方法內部作開關判斷]
                if (SetCarouselProperty(oDataPool.CarouselProperty) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|設定CarouselProperty發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-9]|設定CarouselProperty發生錯誤！");
                    return -9;
                }

                // 設定櫃檯屬性設定Array [保留；方法內部作開關判斷]
                if (SetCounterProperty(oDataPool.CounterProperty) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|設定CounterProperty發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-10]|設定CounterProperty發生錯誤！");
                    return -10;
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

        #region SetCOC(List<int> carousel_no)//int
        /// <summary>
        /// 設定基因演算法限制條件(COC; Carousel Overlap Configuration)
        /// </summary>
        /// <param name="carousel_no">航班行李之轉盤配置預排資料集物件之轉盤配置</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        public static int SetCOC(List<double> carousel_no)
        {
            int ns = carousel_no.Count + m_iRS_fix;
            Custom_Config.COC = new int[ns, ns];
            Custom_Config.GroundConflict = Custom_Switch.SW_GDC ? new int[ns, ns] : null;
            Custom_Config.DestinationConflict = Custom_Switch.SW_DSC ? new int[ns, ns] : null;
            Custom_Config.FlightSizeConflict = Custom_Switch.SW_FSC ? new int[ns, ns] : null;
            Custom_Config.TimeCarouselOverlap = new int[ns];
            
            try
            {
                for (int i = 0; i < ns - 1; i++)
                {
                    for (int j = i + 1; j < ns; j++)
                    {
                        double i_carousel_no = i < m_iRS_fix ? m_LastCarouselList[i] : carousel_no[i - m_iRS_fix];
                        double j_carousel_no = j < m_iRS_fix ? m_LastCarouselList[j] : carousel_no[j - m_iRS_fix];
                        
                        int pValue = (i_carousel_no == j_carousel_no && i_carousel_no * j_carousel_no != 0) ? 1 : 0;
                        Custom_Config.COC[i, j] = pValue;
                        if (i < m_iRS_fix || j < m_iRS_fix)
                        {
                            if (Custom_Switch.SW_GDC)
                            {
                                Custom_Config.GroundConflict[i, j] = 0;
                            }
                            if (Custom_Switch.SW_DSC)
                            {
                                Custom_Config.DestinationConflict[i, j] = 0;
                            }
                            if (Custom_Switch.SW_FSC)
                            {
                                Custom_Config.FlightSizeConflict[i, j] = 0;
                            }
                        }
                        else
                        {
                            if (Custom_Switch.SW_GDC)
                            {
                                Custom_Config.GroundConflict[i, j] = Custom_Config.TOC[i, j] * pValue * Custom_Config.GDC[i, j];
                            }
                            if (Custom_Switch.SW_DSC)
                            {
                                Custom_Config.DestinationConflict[i, j] = Custom_Config.TOC[i, j] * pValue * Custom_Config.DSC[i, j];
                            }
                            if (Custom_Switch.SW_FSC)
                            {
                                Custom_Config.FlightSizeConflict[i, j] = Custom_Config.TOC[i, j] * pValue * Custom_Config.FSC[i, j];
                            }
                        }
                        Custom_Config.TimeCarouselOverlap[i] += Custom_Config.TOC[i, j] * Custom_Config.COC[i, j];
                    }
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
