using System;
using System.Collections.Generic;
using System.Linq;

namespace T1_BHS_OAS.GA
{
    /// <summary>
    /// "Mutation" 基因突變類別
    /// </summary>
    public class Mutation
    {
        #region =====[Public] Event=====



        #endregion

        #region =====[Protected] Event Raise=====



        #endregion

        #region =====[Public] Default Event Body=====

        #region Mutation_Custom(List<List<double>> Pool)//int
        /// <summary>
        /// 基因突變 - 自定義
        /// </summary>
        /// <param name="Pool">族群基因配對池(Pool)</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 族群基因配對池(Pool)未建立</para>
        /// </returns>
        public virtual int Mutation_Custom(List<List<double>> Pool)
        {
            if (Pool == null)
            {
                Console.WriteLine(string.Format("[{0}]|未從親代選擇建立族群基因配對池(Pool)！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                TxtLog.Error("CODE[-2]|未從親代選擇建立族群基因配對池(Pool)！");
                return -2;
            }

            // 剩餘所需基因突變產生之基因組合(chromosome)數
            int mutate_length = m_iPopulationSize - m_iEliteCount - (int)Math.Round(m_iPopulationSize * m_lfCrossRate);
            Population_mutation = new List<List<double>>(mutate_length); ;
            List<int> pool_index = Enumerable.Range(0, Pool.Count).ToList();

            try
            {
                for (int i = 0; i < mutate_length; i++)
                {
                    // 從配對池中選擇一個基因組合(chromosome)作基因突變
                    int pool_index_1 = pool_index[ms_rnd.Next(0, pool_index.Count)];
                    List<double> target_chromosome = new List<double>(nvars);
                    List<double> temp_chromosome = new List<double>(nvars);
                    for (int j = 0; j < nvars; j++)
                    {
                        target_chromosome.Add(Pool[pool_index_1][j]);
                        temp_chromosome.Add(Pool[pool_index_1][j]);
                    }

                    // 對目標基因組合(chromosome)中每個基因依基因突變率判斷是否發生突變，並新增目標至子代族群基因組合
                    for (int j = 0; j < nvars; j++)
                    {
                        double mutate_rate = ms_rnd.NextDouble();
                        if (mutate_rate <= m_lfMutationRate)
                        {
                            temp_chromosome[j] = 0;
                            target_chromosome[j] = Constraints.GetValueByConstraintInOrder(temp_chromosome, j);
                        }
                    }

                    Population_mutation.Add(target_chromosome);
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

        /// <summary>
        /// 基因突變 - 函數問題
        /// </summary>
        /// <param name="Pool">族群基因配對池(Pool)</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        public int Mutation_Fcn(List<List<double>> Pool)
        {
            try
            {
                // do something...

                return 0;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }

        #endregion

        #region =====[Private] Variable=====

        /// <summary>
        /// 基因組中基因變數個數(number of design variables)
        /// </summary>
        private readonly int nvars = default(int);

        /// <summary>
        /// 基因組合個數(人口數)
        /// </summary>
        private readonly int m_iPopulationSize = default(int);

        /// <summary>
        /// 菁英個數
        /// </summary>
        private readonly int m_iEliteCount = default(int);

        /// <summary>
        /// 基因交換率
        /// </summary>
        private readonly double m_lfCrossRate = default(double);

        /// <summary>
        /// 基因突變率
        /// </summary>
        private readonly double m_lfMutationRate = default(double);

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
        /// 基因突變後之族群基因組合(人口數剩餘之組數)
        /// </summary>
        public List<List<double>> Population_mutation { get; private set; }

        #endregion

        #region =====[Public] Constructor & Destructor

        /// <summary>
        /// Mutation類別建構子
        /// </summary>
        /// <param name="nlength">基因組中基因變數個數(number of design variables)</param>
        /// <param name="population_size">基因組合個數(人口數)</param>
        /// <param name="elite_count">菁英個數</param>
        /// <param name="cross_rate">基因交換率</param>
        /// <param name="mutation_rate">基因突變率</param>
        public Mutation(int nlength, int population_size, int elite_count, double cross_rate, double mutation_rate)
        {
            try
            {
                nvars = nlength;
                m_iPopulationSize = population_size;
                m_iEliteCount = elite_count;
                m_lfCrossRate = cross_rate;
                m_lfMutationRate = mutation_rate;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        /// <summary>
        /// Mutation類別解構子
        /// </summary>
        ~Mutation()
        {
            try
            {
                Population_mutation = null;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        #endregion
    }
}
