using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace T1_BHS_OAS.GA
{
    /// <summary>
    /// "Genetic" 基因演算法類別
    /// </summary>
    public class Genetic
    {
        #region =====[Private] Class=====

        /// <summary>
        /// "GA_Options" 基因演算法選項參數類別
        /// </summary>
        private class GA_Options
        {
            #region =====[Public] Property

            /// <summary>
            /// 基因組合個數(人口數)
            /// </summary>
            public int PopulationSize { get; set; }

            /// <summary>
            /// 總迭代次數(上限)
            /// </summary>
            public int Generations { get; set; }

            /// <summary>
            /// 最少迭代次數(下限)
            /// </summary>
            public int StallGenLimit { get; set; }

            /// <summary>
            /// 菁英個數
            /// </summary>
            public int EliteCount { get; set; }

            /// <summary>
            /// 適應值上限(遞迴平均誤差值)
            /// </summary>
            public double FitnessLimit { get; set; }

            /// <summary>
            /// 基因交換率
            /// </summary>
            public double CrossRate { get; set; }

            /// <summary>
            /// 基因突變率
            /// </summary>
            public double MutationRate { get; set; }

            /// <summary>
            /// 基因選擇函數
            /// </summary>
            public string SelectFcn { get; set; }

            /// <summary>
            /// 基因交換函數
            /// </summary>
            public string CrossoverFcn { get; set; }

            /// <summary>
            /// 基因突變函數
            /// </summary>
            public string MutationFcn { get; set; }

            /// <summary>
            /// 上次演算迭代次數
            /// </summary>
            public int LastIteration { get; set; }

            /// <summary>
            /// 自定義參數(資料集物件：航班行李之轉盤配置預排資料集)
            /// </summary>
            public object Custom { get; set; }

            /// <summary>
            /// 自定義限制條件開關物件
            /// </summary>
            public object SW_Custom { get; set; }

            /// <summary>
            /// 限制條件參數(函數之參數矩陣)
            /// </summary>
            public object ConstraintGA { get; set; }

            #endregion

            #region =====[Public] Constructor & Destructor

            public GA_Options()
            {
                PopulationSize = 50;
                Generations = 100;
                StallGenLimit = 50;
                EliteCount = (int)Math.Ceiling(0.05 * PopulationSize);
                FitnessLimit = 1E-06;
                CrossRate = 0.8;
                MutationRate = 0.01;
                SelectFcn = "selection-roulette";
                CrossoverFcn = "crossover-twopoint";
            }

            #endregion
        }

        #endregion

        #region =====[Public] Event=====

        /// <summary>
        /// 宣告族群基因組初始化事件
        /// </summary>
        public event GeneticEvent.IPopulationHandler PopulationInitializeEvent;

        /// <summary>
        /// 宣告族群基因組計算適應值事件
        /// </summary>
        public event GeneticEvent.FitnessCalcHandler FitnessCalcEvent;

        /// <summary>
        /// 宣告親代選擇事件
        /// </summary>
        public event GeneticEvent.SelectionHandler SelectPoolEvent;

        /// <summary>
        /// 宣告基因交換事件
        /// </summary>
        public event GeneticEvent.CrossoverHandler CrossoverEvent;

        /// <summary>
        /// 宣告基因突變事件
        /// </summary>
        public event GeneticEvent.MutationHandler MutationEvent;

        #endregion

        #region =====[Protected] Event Raise=====

        #region InitializePopulation(int nvars)//virtual void
        /// <summary>
        /// 族群基因組初始化處理
        /// </summary>
        /// <param name="nvars"></param>
        protected virtual void InitializePopulation(int nvars)
        {
            if (PopulationInitializeEvent != null)
            {
                if (PopulationInitializeEvent(nvars) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|族群基因組初始化發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("族群基因組初始化發生錯誤並停止演算！");
                    m_IsStop = true;
                }
            }
        }
        #endregion

        #region CalcFitness()//virtual void
        /// <summary>
        /// 族群基因組計算適應值處理
        /// </summary>
        protected virtual void CalcFitness()
        {
            if (FitnessCalcEvent != null)
            {
                if (FitnessCalcEvent(m_Population) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|計算適應值發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("計算適應值發生錯誤並停止運算！");
                    m_IsStop = true;
                }
            }
        }
        #endregion

        #region SelectPool()//virtual void
        /// <summary>
        /// 親代選擇與建立族群基因配對池處理
        /// </summary>
        protected virtual void SelectPool()
        {
            if (SelectPoolEvent != null)
            {
                if (SelectPoolEvent(m_Population, m_oFitness.FitnessSet) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|親代選擇與建立族群基因配對池發生錯誤！", DateTime.Now.ToString()));
                    TxtLog.Error("親代選擇與建立族群基因配對池發生錯誤並停止演算！");
                    m_IsStop = true;
                }
            }
        }
        #endregion

        #region CrossoverGene//virtual void
        /// <summary>
        /// 基因片段交換處理
        /// </summary>
        protected virtual void CrossoverGene()
        {
            if (CrossoverEvent != null)
            {
                if (CrossoverEvent(m_oSelection.Pool)!= 0)
                {
                    Console.WriteLine(string.Format("[{0}]|基因片段交換發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("基因片段交換發生錯誤並停止演算！");
                    m_IsStop = true;
                }
            }
        }
        #endregion

        #region MutationGene()//virtual void
        /// <summary>
        /// 基因突變處理
        /// </summary>
        protected virtual void MutationGene()
        {
            if (MutationEvent != null)
            {
                if (MutationEvent(m_oSelection.Pool) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|基因突變發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("基因突變發生錯誤並停止演算！");
                    m_IsStop = true;
                }
            }
        }
        #endregion

        #region =====[Private] Variable=====

        #endregion

        #endregion

        #region =====[Public] Default Event Body=====

        #region IPopulation_Custom(int nvars)//int
        /// <summary>
        /// 族群基因組初始化 - 自定義
        /// </summary>
        /// <param name="nvars">基因組中基因變數個數(number of design variables)</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 未建立資料集物件 DataPool</para>
        /// </returns>
        public virtual int IPopulation_Custom(int nvars)
        {
            m_Population = new List<List<double>>(m_oParam_GA.PopulationSize);

            try
            {
                DataPool oDataPool = (DataPool)m_oParam_GA.Custom;
                if (oDataPool == null)
                {
                    TxtLog.Error("CODE[-2]|未建立資料集物件 DataPool！");
                    return -2;
                }

                // 決定染色體(基因組合)
                for (int i = 0; i < m_oParam_GA.PopulationSize; i++)
                {
                    m_Population.Add(Chromosome(nvars));
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

        #region IPopulation_Fcn(int nvars)
        /// <summary>
        /// 族群基因組初始化 - 函數問題
        /// </summary>
        /// <param name="nvars">基因組中基因變數個數(number of design variables)</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        public int IPopulation_Fcn(int nvars)
        {
            try
            {
                // 決定染色體(基因組合)
                for (int i = 0; i < m_oParam_GA.PopulationSize; i++)
                {
                    m_Population.Add(Chromosome(nvars));
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

        #region =====[Private] Variable=====

        /// <summary>
        /// 基因演算法適應值、變數限制物件
        /// </summary>
        private Fitness m_oFitness = null;

        /// <summary>
        /// 基因演算法親代選擇物件
        /// </summary>
        private Selection m_oSelection = null;

        /// <summary>
        /// 基因演算法基因片段交換物件
        /// </summary>
        private Crossover m_oCrossover = null;

        /// <summary>
        /// 基因演算法基因突變物件
        /// </summary>
        private Mutation m_oMutation = null;

        /// <summary>
        /// 基因演算法選項參數物件物件
        /// </summary>
        private GA_Options m_oParam_GA = null;

        /// <summary>
        /// 停止基因演算法
        /// </summary>
        private bool m_IsStop = default(bool);

        /// <summary>
        /// 基因演算法執行緒
        /// </summary>
        private Thread m_GeneticThread = null;

        /// <summary>
        /// 族群基因組合(Row: 基因組合)
        /// </summary>
        private List<List<double>> m_Population = null;

        /// <summary>
        /// 同步資源鎖定
        /// </summary>
        private readonly static object m_oGeneticLock = new object();

        /// <summary>
        /// 隨機亂樹種子
        /// </summary>
        private static Random ms_rnd = new Random();

        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        #endregion

        #region =====[Public] Property

        /// <summary>
        /// 自定義適應函數選項字串
        /// </summary>
        public const string CustomFitnessFcn = "custom_fitnessfcn";

        /// <summary>
        /// 自定義突變函數選項字串
        /// </summary>
        public const string CustomMutationFcn = "custom_mutationfcn";

        /// <summary>
        /// 基因演算迭代次數
        /// </summary>
        public int Iteration { get; private set; }

        /// <summary>
        /// 演算進度
        /// </summary>
        public double ProgressRate { get; private set; }

        /// <summary>
        /// 適應值最小的最佳基因組合(最佳解)
        /// </summary>
        public List<double> BestChromosome { get; private set; }

        /// <summary>
        /// 最小的適應值(重疊時間)
        /// </summary>
        public double BestFitnessValue
        {
            get
            {
                return m_oFitness.FitnessSet_OT[m_oFitness.Best_Fitness_index];
            }
        }

        #endregion

        #region =====[Public] Constructor & Destructor=====

        /// <summary>
        /// Genetic類別建構子
        /// </summary>
        public Genetic()
        {
            try
            {
                m_oParam_GA = new GA_Options();
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        /// <summary>
        /// Genetic類別解構子
        /// </summary>
        ~Genetic()
        {
            try
            {
                m_oParam_GA = null;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        #endregion

        #region =====[Private] Thread=====

        /// <summary>
        /// 執行基因演算法執行緒
        /// </summary>
        /// <param name="nvars">基因組中基因變數個數(number of design variables)</param>
        private void RunGenetic(object nvars)
        {
            lock (m_oGeneticLock)
            {
                Iteration = 1;
                ProgressRate = 0;

                try
                {
                    // 持續或停止演化
                    while (!m_IsStop)
                    {
                        Console.Write("基因演算迭代次數: " + Iteration + " 次|");
                        // 演化初始先定義族群基因組合
                        if (Iteration == 1)
                        {
                            InitializePopulation((int)nvars);
                        }

                        // 計算族群基因組適應值
                        CalcFitness();

                        // 判斷是否停止演化
                        CheckStopCriteria();

                        // 計算預計停止迭代次數及目前演算迭代次數所佔比例(演算進度)
                        CalcProgressRate(m_oFitness.PolyParam);

                        // 選擇適應值最小的最佳基因組合(最佳解)
                        BestChromosome = m_Population[m_oFitness.Best_Fitness_index];

                        // 若未停止則繼續演化
                        if (!m_IsStop)
                        {
                            // 選擇欲交換基因之親代至族群基因配對池(Pool)
                            SelectPool();
                            m_Population = new List<List<double>>(m_oParam_GA.PopulationSize);  // 新的子代族群基因組合

                            // 挑選菁英至新的族群基因組合
                            SelectElite();                                                      // 子代族群基因組合(+Elite)

                            // 依交換函數交換基因片段、新增至新的族群基因組合
                            CrossoverGene();
                            m_Population.AddRange(m_oCrossover.Population_crossover);           // 子代族群基因組合(+Crossover)

                            // 依突變函數及機率突變基因並將族群空缺填滿、新增至新的族群基因組合
                            MutationGene();
                            m_Population.AddRange(m_oMutation.Population_mutation);             // 子代族群基因組合(+Mutation)
                        }

                        Iteration++;
                    }
                }
                catch (Exception ex)
                {
                    TxtLog.Error(ex);
                }
            }
        }

        #endregion

        #region =====[Private] Function=====

        #region Chromosome(int nvars)//virtual List<double>
        /// <summary>
        /// 隨機決定染色體(基因組合)
        /// </summary>
        /// <param name="nvars">基因組中基因變數個數(number of design variables)</param>
        /// <returns>基因組合List</returns>
        protected virtual List<double> Chromosome(int nvars)
        {
            List<double> listRtn = new List<double>(nvars);
            List<int> chromosome_index = Enumerable.Range(0, nvars).ToList();

            try
            {
                // 基因組合初始化
                for (int i = 0; i < nvars; i++)
                {
                    listRtn.Add(0);
                }

                for (int i = 0; i < nvars; i++)
                {
                    int gene_rnd = chromosome_index[ms_rnd.Next(0, chromosome_index.Count)];
                    chromosome_index.Remove(gene_rnd);
                    listRtn[gene_rnd] = Constraints.GetValueByConstraintInOrder(listRtn, gene_rnd);
                }
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }

            return listRtn;
        }
        #endregion

        #region CalcProgressRate(double[] poly_param)//int
        /// <summary>
        /// 計算演算進度(依誤差多項式擬合參數值推測距停止迭代次數所佔比例；%)
        /// </summary>
        /// <param name="poly_param">誤差多項式擬合參數值Array</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 演算迭代次數尚不足4次</para>
        /// <para>-3: 誤差多項式擬合參數值之多項式解僅有虛根</para>
        /// </returns>
        private int CalcProgressRate(double[] poly_param)
        {
            try
            {
                if (Iteration < 4)
                {
                    return -2;
                }

                double a = poly_param[0];
                double b = poly_param[1];
                double c = poly_param[2] - m_oParam_GA.FitnessLimit;

                // 檢查多項式解是否為實根
                if (Math.Pow(b, 2) - 4 * a * c < 0)
                {
                    Console.WriteLine(string.Format("[{0}]|誤差多項式擬合參數值之多項式解僅有虛根！", DateTime.Now.ToString("yyyyMMddHHmmss")));
                    TxtLog.Error("CODE[-3]|誤差多項式擬合參數值之多項式解僅有虛根！");
                    return -3;
                }

                // 計算預計停止演算迭代次數(小於StallGenLimit以此計)
                int x1 = (int)Math.Round((-b + Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a));
                int x2 = (int)Math.Round((-b - Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a));
                int e_iteration_stop = x1 > x2 ? x1 : x2;
                if (e_iteration_stop < m_oParam_GA.StallGenLimit)
                {
                    e_iteration_stop = m_oParam_GA.LastIteration;
                }

                // 演算進度
                ProgressRate = (double)Iteration / e_iteration_stop;

                if (ProgressRate >= 0.99)
                {
                    ProgressRate = 0.999;
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

        #region CheckStopCriteria()//virtual int
        /// <summary>
        /// 檢查基因演算法停止準則
        /// </summary>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        protected virtual int CheckStopCriteria()
        {
            try
            {
                int avg_wt_fitness_diff = m_oFitness.CalcAvgWeightedFitnessDiff(m_oParam_GA.PopulationSize);
                if (avg_wt_fitness_diff == 0)
                {
                    if (Iteration > m_oParam_GA.StallGenLimit)
                    {
                        if (m_oFitness.AvgWeightedFitnessDiff < m_oParam_GA.FitnessLimit)
                        {
                            // 若基因演算近二次最佳適應值遞迴平均差值低於上限則停止演化
                            m_IsStop = true;
                            Console.WriteLine(string.Format("[{0}]|基因演算近二次最佳適應值遞迴平均差值低於上限停止演化！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                            TxtLog.Info("基因演算近二次最佳適應值遞迴平均差值低於上限停止演化！");
                        }
                        else
                        {
                            // 未超過最佳適應值遞迴平均差值上限不停止(繼續演化)
                            m_IsStop = false;
                        }
                    }
                    else if (Iteration > m_oParam_GA.Generations)
                    {
                        // 若基因演算迭代次數超過總迭代次數(設定演化上限)則停止演化
                        m_IsStop = true;
                        Console.WriteLine(string.Format("[{0}]|基因演算迭代次數超過總迭代次數(設定演化上限)停止演化！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                        TxtLog.Info("基因演算迭代次數超過總迭代次數(設定演化上限)停止演化！");
                    }
                    else
                    {
                        // 其他情況則不停止(繼續演化)
                        m_IsStop = false;
                    }
                }
                else if (avg_wt_fitness_diff == -3)
                {
                    // 基因演算第一次迭代不停止(繼續演化)
                    m_IsStop = false;
                }
                else
                {
                    // 計算近二次遞迴最佳適應值差值之加權平均發生錯誤停止演化
                    m_IsStop = true;
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

        #region SelectElite()//int
        /// <summary>
        /// 從族群基因組合中選擇菁英至新的族群
        /// </summary>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        private int SelectElite()
        {
            try
            {
                for (int i = 0; i < m_oParam_GA.EliteCount; i++)
                {
                    m_Population.Add(BestChromosome);
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

        #region DelegateGeneticFcns(string fitnessfcn, int nvars)//int
        /// <summary>
        /// 依適應函數委派基因演算法中各項方法之適用類型
        /// </summary>
        /// <param name="fitnessfcn">適應函數(目標函數)</param>
        /// <param name="nvars">基因組中基因變數個數(number of design variables)</param>
        /// <returns><para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para></returns>
        private int DelegateGeneticFcns(string fitnessfcn, int nvars)
        {
            // 基因演算法各項方法物件實作
            m_oFitness = new Fitness(fitnessfcn, nvars);
            m_oSelection = new Selection();
            m_oCrossover = new Crossover(nvars, m_oParam_GA.PopulationSize, m_oParam_GA.CrossRate);
            m_oMutation = new Mutation(nvars, m_oParam_GA.PopulationSize, m_oParam_GA.EliteCount, m_oParam_GA.CrossRate, m_oParam_GA.MutationRate);

            try
            {
                // 委派適用類型(族群基因組初始化、族群基因組計算適應值)
                switch (fitnessfcn)
                {
                    case CustomFitnessFcn: 
                        PopulationInitializeEvent += new GeneticEvent.IPopulationHandler(IPopulation_Custom);
                        FitnessCalcEvent += new GeneticEvent.FitnessCalcHandler(m_oFitness.FitnessCalc_Custom);
                        break;
                    default: 
                        PopulationInitializeEvent += new GeneticEvent.IPopulationHandler(IPopulation_Fcn);
                        FitnessCalcEvent += new GeneticEvent.FitnessCalcHandler(m_oFitness.FitnessCalc_Fcn);
                        break;
                }

                // 委派適用類型(複製函數)
                switch (m_oParam_GA.SelectFcn)
                {
                    case "selection-roulette": 
                        SelectPoolEvent += new GeneticEvent.SelectionHandler(m_oSelection.SelectPool_Roulette);
                        break;
                    default: 
                        SelectPoolEvent += new GeneticEvent.SelectionHandler(m_oSelection.SelectPool_Roulette);
                        break;
                }

                // 委派適用類型(交換函數)
                switch (m_oParam_GA.CrossoverFcn)
                {
                    case "crossover-twopoint": 
                        CrossoverEvent += new GeneticEvent.CrossoverHandler(m_oCrossover.Crossover_TwoPoint);
                        break;
                    default: 
                        CrossoverEvent += new GeneticEvent.CrossoverHandler(m_oCrossover.Crossover_TwoPoint);
                        break;
                }

                // 委派適用類型(突變函數)
                switch (m_oParam_GA.MutationFcn)
                {
                    case CustomMutationFcn: 
                        MutationEvent += new GeneticEvent.MutationHandler(m_oMutation.Mutation_Custom);
                        break;
                    default:
                        MutationEvent += new GeneticEvent.MutationHandler(m_oMutation.Mutation_Fcn);
                        break;
                }

                return 0;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                PopulationInitializeEvent = null;
                SelectPoolEvent = null;
                CrossoverEvent = null;
                MutationEvent = null;
                return -1;
            }
        }
        #endregion

        #endregion

        #region =====[Public] Method=====

        #region Start(string fitnessfcn, int nvars)//void
        /// <summary>
        /// 執行基因演算法
        /// </summary>
        /// <param name="fitnessfcn">適應函數(目標函數)</param>
        /// <param name="nvars">基因組中基因變數個數(number of design variables)</param>
        public void Start(string fitnessfcn, int nvars)
        {
            try
            {
                // 委派適應函數、複製函數、交換函數、突變函數等適用類型
                DelegateGeneticFcns(fitnessfcn, nvars);

                // 基因演算法執行緒
                m_IsStop = false;
                m_GeneticThread = new Thread(new ParameterizedThreadStart(RunGenetic))
                {
                    IsBackground = true
                };
                m_GeneticThread.Start(nvars);

                Console.WriteLine(string.Format("[{0}]|基因演算法開始！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                TxtLog.Info("基因演算法開始！");
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }
        #endregion

        #region Stop()//void
        /// <summary>
        /// 停止基因演算法
        /// </summary>
        public void Stop()
        {
            try
            {
                m_IsStop = true;
                if (m_GeneticThread != null && m_GeneticThread.IsAlive)
                {
                    m_GeneticThread.Abort();
                    m_GeneticThread = null;
                }

                Console.WriteLine(string.Format("[{0}]|基因演算過程發生錯誤並停止演算！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                TxtLog.Info("基因演算過程發生錯誤並停止演算！");
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }
        #endregion

        #region CheckStatus()//bool
        /// <summary>
        /// 判斷基因演算執行緒狀態是否停止
        /// </summary>
        /// <returns>
        /// <para> true: 基因演算執行緒狀態停止</para>
        /// <para>false: 基因演算執行緒執行中</para>
        /// </returns>
        public bool CheckStatus()
        {
            if (m_GeneticThread.ThreadState == ThreadState.Stopped)
            {
                Console.WriteLine(string.Format("[{0}]|基因演算法結束！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                TxtLog.Info("基因演算法結束！");
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region SetGAOptions(Dictionary<string, object> options//int
        /// <summary>
        /// 設定基因演算法選項參數(此類別的演算法參數 + 演算法限制條件(計算適應值函數之參數矩陣 + 自定義限制條件所需之相關參數 + 自定義限制條件開關)) [自定義]
        /// </summary>
        /// <param name="options">基因演算選項參數字典</param>
        /// <param name="nvars">基因組中基因變數個數(number of design variables)</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 設定函數限制條件錯誤</para>
        /// <para>-3: 設定自定義限制條件開關錯誤</para>
        /// <para>-4: 設定自定義限制條件錯誤</para>
        /// </returns>
        public virtual int SetGAOptions(Dictionary<string, object> options, int nvars)
        {
            try
            {
                // 設定基因演算法之演算參數與其他選項參數
                List<System.Reflection.PropertyInfo> props = new List<System.Reflection.PropertyInfo>(m_oParam_GA.GetType().GetProperties());
                foreach (KeyValuePair<string, object> option in options)
                {
                    foreach (System.Reflection.PropertyInfo prop in props)
                    {
                        if (option.Key == prop.Name)
                        {
                            prop.SetValue(m_oParam_GA, option.Value);
                        }
                    }
                }

                // 設定函數限制條件
                if (Constraints.SetConstraints((Dictionary<string, Array>)m_oParam_GA.ConstraintGA, nvars) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|設定函數限制條件發生錯誤！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-2]|設定函數限制條件發生錯誤！");
                    return -2;
                }

                // 設定自定義限制條件與其開關
                if (Constraints.SetConstraints(m_oParam_GA.Custom, (Dictionary<string, bool>)m_oParam_GA.SW_Custom) != 0)
                {
                    Console.WriteLine(string.Format("[{0}]|設定自定義限制條件與其開關發生錯誤!", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    TxtLog.Error("CODE[-3]|設定自定義限制條件與其開關發生錯誤!");
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
