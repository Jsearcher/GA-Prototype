using System;
using System.Collections.Generic;
using System.Linq;

namespace T1_BHS_OAS.GA
{
    /// <summary>
    /// "Fitness" 基因演算法適應值類別
    /// </summary>
    public class Fitness
    {
        #region =====[Public] Event=====



        #endregion

        #region =====[Protected] event Raise=====



        #endregion

        #region =====[Public] Default Event Body

        /// <summary>
        /// 計算適應值 - 函數問題
        /// </summary>
        /// <param name="population">族群基因組合</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        public int FitnessCalc_Fcn(List<List<double>> population)
        {
            int ns = population.Count;
            this.FitnessSet = new List<double>(ns);

            try
            {
                for (int i = 0; i < ns; i++)
                {
                    this.CalcFitnessValue_Fcn(population[i]);
                }

                return 0;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }

        #region FitnessCalc_Custom(List<List<double>>//int
        /// <summary>
        /// 計算適應值 - 自定義
        /// </summary>
        /// <param name="population">族群基因組合</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 計算各基因組合(chromosome)之適應值發生錯誤</para>
        /// </returns>
        public virtual int FitnessCalc_Custom(List<List<double>> population)
        {
            int ns = population.Count;
            FitnessSet = new List<double>(ns);
            FitnessSet_OT = new List<double>(ns);
            m_FitnessSet_Penalty = new List<double>(ns);

            try
            {
                for (int i = 0; i < ns; i++)
                {
                    // 各基因組合(chromosome)之適應值
                    if (CalcFitnesValue_Custom(population[i]) != 0)
                    {
                        Console.WriteLine(string.Format("[{0}]|計算個基因組合(chromosome)之適應值發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                        TxtLog.Error("CODE[-2]|計算各基因組合(chromosome)之適應值發生錯誤！");
                        return -2;
                    }

                    FitnessSet.Add(m_iFitnessValue);
                    FitnessSet_OT.Add(m_iFitnessOT);
                    m_FitnessSet_Penalty.Add(m_iFitnessPenalty);
                }

                int best_index = Best_Fitness_index;
                Console.WriteLine(string.Format("最佳適應值-重疊時間: [{0}], 最佳適應值-懲罰值: [{1}]", FitnessSet_OT[best_index], m_FitnessSet_Penalty[best_index]));
                TxtLog.Info(string.Format("最佳適應值-重疊時間: [{0}], 最佳適應值-懲罰值: [{1}]", FitnessSet_OT[best_index], m_FitnessSet_Penalty[best_index]));

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

        #region =====[Private] Variable=====

        /// <summary>
        /// 基因組中基因變數個數(number of design variables)
        /// </summary>
        private readonly int nvars = default(int);

        /// <summary>
        /// 適應函數(目標函數)
        /// </summary>
        private readonly string m_sFitnessFcn = string.Empty;

        /// <summary>
        /// 四則運算程序列表
        /// </summary>
        private List<string> m_OperationList = null;

        /// <summary>
        /// 各基因組合(chromosome)之適應值
        /// </summary>
        private int m_iFitnessValue = default(int);

        /// <summary>
        /// 各基因組合(chromosome)之重疊時間
        /// </summary>
        private int m_iFitnessOT = default(int);

        /// <summary>
        /// 各基因組合(chromosome)之懲罰值
        /// </summary>
        private int m_iFitnessPenalty = default(int);

        /// <summary>
        /// 族群基因組合(population)之懲罰值集
        /// </summary>
        private List<double> m_FitnessSet_Penalty = null;

        /// <summary>
        /// 最近二次遞迴之最佳基因組合(chromosome)適應值
        /// </summary>
        private List<double> m_Best_FitnessValue_List = null;

        /// <summary>
        /// 最近二次遞迴最佳適應值差值之加權變化List
        /// </summary>
        private List<double> m_WeightedFitnessDiff = null;

        /// <summary>
        /// 最近二次最佳適應值差值記錄List(多項式f(x)值)
        /// </summary>
        private List<double> m_FitnessDiff_List = null;

        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        #endregion

        #region =====[Public] Property=====

        /// <summary>
        /// 族群基因組合(population)之適應值集
        /// </summary>
        public List<double> FitnessSet { get; private set; }

        /// <summary>
        /// 族群基因組合(population)之重疊時間集
        /// </summary>
        public List<double> FitnessSet_OT = null;

        /// <summary>
        /// 近二次遞迴最佳適應值誤差之加權平均
        /// </summary>
        public double AvgWeightedFitnessDiff { get { return m_WeightedFitnessDiff.Average(); } }

        /// <summary>
        /// 誤差多項式擬合參數值Array
        /// </summary>
        public double[] PolyParam { get; private set; }

        /// <summary>
        /// 最佳基因組合(chromosome)適應值集序號(-1 表示沒有FitnessSet)
        /// </summary>
        public int Best_Fitness_index
        {
            get
            {
                if (FitnessSet != null)
                {
                    if (FitnessSet.Count > 0)
                    {
                        return FitnessSet.IndexOf(FitnessSet.Min());
                    }
                }

                return -1;
            }
        }

        #endregion

        #region =====[Public] Constructor & Destructor

        /// <summary>
        /// Fitness類別建構子
        /// </summary>
        /// <param name="fitnessfcn">適應函數(目標函數)</param>
        /// <param name="nlength">基因變數個數</param>
        public Fitness(string fitnessfcn, int nlength)
        {
            try
            {
                nvars = nlength;
                m_sFitnessFcn = fitnessfcn;
                ParsingFitnessFcn();
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        /// <summary>
        /// Fitness類別解構子
        /// </summary>
        ~Fitness()
        {
            try
            {
                FitnessSet = null;
                FitnessSet_OT = null;
                m_FitnessSet_Penalty = null;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        #endregion

        #region =====[Private] Function=====

        /// <summary>
        /// 依四則運算程序計算適應值
        /// </summary>
        /// <param name="variables">適應函數的多變數值</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        private int CalcFitnessValue_Fcn(List<double> variables)
        {
            try
            {
                for (int i = 0; i < this.m_OperationList.Count; i++)
                {
                    // do something...
                }

                this.FitnessSet.Add(this.m_iFitnessValue);
                return 0;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }

        #region CalcFitnessValue_Custom(List<double> chromosome)//int
        /// <summary>
        /// <para>計算適應值(自定義): </para>
        /// <para>各基因組合(chromosome)之</para>
        /// <para>轉盤共用總重疊時間 [必要] + </para>
        /// <para>地勤相異(GDC)的轉盤共用總重疊時間(懲罰值) [非必要] + </para>
        /// <para>目的地太相似(DSC)的轉盤共用總重疊時間(懲罰值) [非必要] + </para>
        /// <para>航班大小配置問題(FSC)的轉盤共用總重疊時間(懲罰值) [非必要] + </para>
        /// <para>依超過尖峰離峰(RC)上限值個數中最多的轉盤共用總重疊時間(懲罰值) [非必要]</para>
        /// </summary>
        /// <param name="chromosome">目前基因組合變數</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 設定COC發生錯誤</para>
        /// </returns>
        private int CalcFitnesValue_Custom(List<double> chromosome)
        {
            int fitness_OT = 0, fitness_GDC = 0, fitness_DSC = 0, fitness_FSC = 0, fitness_RC = 0;

            try
            {
                // 更新目前航班間轉盤重疊共用之配置(COC; Carousel Overlap Configuration)
                if (Constraints.SetCOC(chromosome) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|更新目前航班間轉盤重疊共用之配置COC發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-2]|更新目前航班間轉盤重疊共用之配置COC發生錯誤！");
                    return -2;
                }

                // 計算轉盤共用總重疊時間 + 懲罰值
                for (int i = 0; i < nvars - 1; i++)
                {
                    for (int j = i + 1; j < nvars; j++)
                    {
                        // 計算轉盤共用總重疊時間
                        fitness_OT += Constraints.Custom_Config.OTC[i, j] * Constraints.Custom_Config.COC[i, j];
                        // 計算地勤相異(GDC)的轉盤共用總重疊時間(懲罰值) [非必要]
                        if (Constraints.Custom_Switch.SW_GDC)
                        {
                            fitness_GDC += Constraints.Custom_Config.OTC[i, j] * Constraints.Custom_Config.GroundConflict[i, j];
                        }
                        // 計算目的地太相似(DSC)的轉盤共用總重疊時間(懲罰值) [非必要]
                        if (Constraints.Custom_Switch.SW_DSC)
                        {
                            fitness_DSC += Constraints.Custom_Config.OTC[i, j] * Constraints.Custom_Config.DestinationConflict[i, j];
                        }
                        // 計算航班大小配置問題(FSC)的轉盤共用總重疊時間(懲罰值) [非必要]
                        if (Constraints.Custom_Switch.SW_FSC)
                        {
                            fitness_FSC += Constraints.Custom_Config.OTC[i, j] * Constraints.Custom_Config.FlightSizeConflict[i, j];
                        }
                    }
                }

                // 計算依超過尖峰離峰上限值個數中最多的轉盤共用總重疊時間(懲罰值) [非必要]
                if (Constraints.Custom_Switch.SW_RC)
                {
                    for (int i = 0; i < nvars; i++)
                    {
                        List<int> OTList = new List<int>();                                                                 // 與對象目標(chromosome[i])共用轉盤之重疊時間List
                        int current_carousel_index = Array.IndexOf(Constraints.Carousel_Prop.CarouselNo, chromosome[i]);    // 對象目標(chromosome[i])目前轉盤序號
                        int rush = 0;                                                                                       // 與目標轉盤共用(包含自身)之尖峰時段航班數量
                        int normal = 0;                                                                                     // 與目標轉盤共用(包含自身)之離峰時段航班數量

                        // 對象目標(chromosome[i])自身需判斷為尖峰或離峰
                        if (Constraints.Custom_Config.RC[i] == 1)
                        {
                            rush++;
                        }
                        else
                        {
                            normal++;
                        }

                        // 取出並計算與對象目標(chromosome[i])轉盤共用之重疊的時間
                        for (int j = 0; j < nvars; j++)
                        {
                            int r = i > j ? j : i;
                            int c = i > j ? i : j;
                            int ot = Constraints.Custom_Config.OTC[r, c] * Constraints.Custom_Config.COC[r, c]; // 重疊的時間
                            if (ot > 0)
                            {
                                OTList.Add(ot);
                                if (Constraints.Custom_Config.RC[j] == 1)
                                {
                                    rush++;
                                }
                                else
                                {
                                    normal++;
                                }
                            }
                        }

                        // 依尖峰數量與離峰數量比例決定參考上限值
                        int UpLimit = rush > normal ? Constraints.Carousel_Prop.UpLimitRush[current_carousel_index] : Constraints.Carousel_Prop.UpLimitNormal[current_carousel_index];

                        // 比對共用重疊數是否超過參考上限值
                        if (rush + normal > UpLimit)
                        {
                            OTList = OTList.OrderByDescending(x => x).ToList(); // 降冪排列重疊的時間List
                            for (int j = 0; j < rush + normal - UpLimit; j++)
                            {
                                fitness_RC += OTList[j];    // 取出超過個數的重疊時間
                            }
                        }
                    }
                }
                
                m_iFitnessOT = fitness_OT;
                m_iFitnessPenalty = fitness_GDC + fitness_DSC + fitness_FSC + fitness_RC;
                m_iFitnessValue = m_iFitnessOT + m_iFitnessPenalty;
                return 0;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }
        #endregion

        /// <summary>
        /// 剖析適應函數字串並轉換為簡單四則運算之程序
        /// </summary>
        /// <returns>
        /// <para> 1: 適應函數為自定義</para>
        /// <para> 0: 適應函數為多項式</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 適應函數字串為null</para>
        /// </returns>
        private int ParsingFitnessFcn()
        {
            m_OperationList = new List<string>();

            try
            {
                // 未設定適應函數(目標函數)字串
                if (m_sFitnessFcn == null)
                {
                    Console.WriteLine(string.Format("[{0}]|適應函數(目標函數)字串未設定！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-2]|適應函數(目標函數)字串未設定！");
                    return -2;
                }

                // 自定義的適應函數(目標函數)字串
                if (m_sFitnessFcn == Genetic.CustomFitnessFcn)
                {
                    Console.WriteLine(string.Format("[{0}]|使用自定義的適應函數(目標函數)！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Info("CODE[1]|使用自定義的適應函數(目標函數)！");
                }

                // 解析多項式函數字串為簡單四則運算之程序
                // do something...

                return 0;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }

        #region MatrixMultiply(double[,] A, double[,] B//double[,]
        /// <summary>
        /// 計算兩矩陣相乘
        /// </summary>
        /// <param name="A">被乘矩陣A(乘號前面的矩陣)</param>
        /// <param name="B">乘矩陣B(乘號後面的矩陣)</param>
        /// <returns></returns>
        private double[,] MatrixMultiply(double[,] A, double[,] B)
        {
            int rs = A.GetLength(0);
            int ks = A.GetLength(1);
            int cs = B.GetLength(1);
            double[,] MatrixRtn = new double[rs, cs];

            try
            {
                for (int i = 0; i < rs; i++)
                {
                    for (int j = 0; j < cs; j++)
                    {
                        for (int k = 0; k < ks; k++)
                        {
                            MatrixRtn[i, j] += A[i, k] * B[k, j];
                        }
                    }
                }

                return MatrixRtn;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return null;
            }
        }
        #endregion

        #region MatrixInverse_3(double[,] matrix)//double[,]
        /// <summary>
        /// 計算3*3矩陣之反矩陣
        /// </summary>
        /// <param name="matrix">3*3矩陣</param>
        /// <returns>3*3反矩陣</returns>
        private double[,] MatrixInverse_3(double[,] matrix)
        {
            double[,] Matrix_inv = new double[3, 3];

            try
            {
                // 計算目標矩陣行列式值
                double N = (matrix[0, 0] * matrix[1, 1] * matrix[2, 2] + matrix[0, 1] * matrix[1, 2] * matrix[2, 0] + matrix[0, 2] * matrix[1, 0] + matrix[2, 1]) - (matrix[0, 2] * matrix[1, 1] * matrix[2, 0] + matrix[0, 1] * matrix[1, 0] * matrix[2, 2] + matrix[0, 0] * matrix[1, 2] * matrix[2, 1]);

                // 計算反矩陣各元素所求之行列式值
                double E00 = matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1];
                double E01 = matrix[0, 1] * matrix[2, 2] - matrix[0, 2] * matrix[2, 1] * (-1);
                double E02 = matrix[0, 1] * matrix[1, 2] - matrix[0, 2] * matrix[1, 1];
                double E10 = matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0] * (-1);
                double E11 = matrix[0, 0] * matrix[2, 2] - matrix[0, 2] * matrix[2, 0];
                double E12 = matrix[0, 0] * matrix[1, 2] - matrix[0, 2] * matrix[1, 0] * (-1);
                double E20 = matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0];
                double E21 = matrix[0, 0] * matrix[2, 1] - matrix[0, 1] * matrix[2, 0] * (-1);
                double E22 = matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

                Matrix_inv[0, 0] = E00 / N;
                Matrix_inv[0, 1] = E01 / N;
                Matrix_inv[0, 2] = E02 / N;
                Matrix_inv[1, 0] = E10 / N;
                Matrix_inv[1, 1] = E11 / N;
                Matrix_inv[1, 2] = E12 / N;
                Matrix_inv[2, 0] = E20 / N;
                Matrix_inv[2, 1] = E21 / N;
                Matrix_inv[2, 2] = E22 / N;

                return Matrix_inv;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return null;
            }
        }
        #endregion

        #endregion

        #region =====[Public] Method=====

        #region CalcAvgWeightedFitnessDiff()//int
        /// <summary>
        /// 計算近二次遞迴最佳適應值差值之加權平均
        /// </summary>
        /// <param name="npopulations">族群基因組合(population)數</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: FitnessSet未計算</para>
        /// <para>-3: 基因演算第一次迭代</para>
        /// <para>-4: m_Best_FitnessValue_List使用RemoveAt錯誤</para>
        /// </returns>
        public int CalcAvgWeightedFitnessDiff(int npopulations)
        {
            try
            {
                if (FitnessSet == null || FitnessSet.Count != npopulations)
                {
                    Console.WriteLine(string.Format("[{0}]|族群基因組合(population)適應值集未計算！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-2]|族群基因組合(population)適應值集未計算！");
                    return -2;
                }

                // 計算近二次遞迴之最佳基因組合
                if (m_Best_FitnessValue_List == null)
                {
                    m_Best_FitnessValue_List = new List<double>(2);
                }
                if (m_Best_FitnessValue_List.Count == 2)
                {
                    m_Best_FitnessValue_List.RemoveAt(0);
                }
                m_Best_FitnessValue_List.Add(FitnessSet[Best_Fitness_index]);
                if (m_Best_FitnessValue_List.Count < 2)
                {
                    Console.WriteLine(string.Format("[{0}]|基因演算第一次迭代...！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Info("CODE[-3]|基因演算第一次迭代...");
                    return -3;
                }
                else if (m_Best_FitnessValue_List.Count > 2)
                {
                    Console.WriteLine(string.Format("[{0}]|最近二次遞迴之最佳基因組合適應值移除(RemoveAt)發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-4]|最近二次遞迴之最佳基因組合適應值移除(RemoveAt)發生錯誤！");
                    return -4;
                }

                // 計算最近二次最佳適應值差值並記錄至紀錄List(作為誤差多項式f(x)值; x值為index)
                double fitness_diff = (double)Math.Abs(m_Best_FitnessValue_List[1] - m_Best_FitnessValue_List[0]) / m_Best_FitnessValue_List[0];
                if (m_FitnessDiff_List == null)
                {
                    m_FitnessDiff_List = new List<double>();
                }
                m_FitnessDiff_List.Add(fitness_diff);

                // 計算誤差多項式 f(x) = ax^2 + bx + c 的參數值Array
                if (m_FitnessDiff_List.Count >= 3)
                {
                    double[,] X = new double[m_FitnessDiff_List.Count, 3];
                    double[,] XT = new double[3, m_FitnessDiff_List.Count];
                    double[] Y = m_FitnessDiff_List.ToArray();
                    double[,] XT_X = new double[3, 3];

                    for (int i = 0; i < m_FitnessDiff_List.Count; i++)
                    {
                        X[i, 0] = Math.Pow(i + 1, 2);
                        X[i, 1] = i + 1;
                        X[i, 2] = 1;
                        XT[0, i] = Math.Pow(i + 1, 2);
                        XT[1, i] = i + 1;
                        XT[2, i] = 1;
                    }

                    // 計算 b_head = inv(XT*X)*XT*Y
                    XT_X = MatrixMultiply(XT, X);
                    double[,] XT_X_inv = MatrixInverse_3(XT_X);
                    double[,] temp_matrix = MatrixMultiply(XT_X_inv, XT);
                    PolyParam = new double[3];
                    for (int i = 0; i < PolyParam.Length; i++)
                    {
                        for (int j = 0; j < m_FitnessDiff_List.Count; j++)
                        {
                            PolyParam[i] += temp_matrix[i, j] * Y[j];
                        }
                    }
                }

                // 計算近二次最佳適應值插值之加權變化List
                if (m_WeightedFitnessDiff == null)
                {
                    m_WeightedFitnessDiff = new List<double>();
                }
                for (int i = 0; i < m_WeightedFitnessDiff.Count; i++)
                {
                    m_WeightedFitnessDiff[i] *= 0.5;    // 最佳適應值差值隨遞迴次數而影響減半
                }
                m_WeightedFitnessDiff.Add(fitness_diff);

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
