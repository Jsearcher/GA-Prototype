using System;
using System.Collections.Generic;
using System.Linq;

namespace T1_BHS_OAS.GA
{
    /// <summary>
    /// "Crossover" 基因片段交換類別
    /// </summary>
    public class Crossover
    {
        #region =====[Public] Event=====



        #endregion

        #region =====[Protected] Event Raise=====



        #endregion

        #region =====[Public] Default Event Body=====

        #region Crossover_TwoPoint(List<List<double>> Pool)//int
        /// <summary>
        /// 交換函數 - Two Point
        /// </summary>
        /// <param name="Pool">族群基因配對池(Pool)</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 族群基因配對池(Pool)未建立</para>
        /// </returns>
        public virtual int Crossover_TwoPoint(List<List<double>> Pool)
        {
            if (Pool == null)
            {
                Console.WriteLine(string.Format("[{0}]|未從親代選擇建立族群基因配對池(Pool)！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                TxtLog.Error("CODE[-2]|位從親代選擇建立族群基因配對池(Pool)！");
            }

            //經交叉替換基因片段產生之基因組合(chromosome)數
            int cross_length = (int)Math.Round(m_iPopulationSize * m_lfCrossRate);
            Population_crossover = new List<List<double>>(cross_length);
            List<int> pool_index = Enumerable.Range(0, Pool.Count).ToList();

            try
            {
                for (int i = 0; i < cross_length; i++)
                {
                    // 從配對池中選擇兩個不同的基因組合(chromosome)做基因片段交換(選取目標交換後留下)
                    int pool_index_1 = pool_index[ms_rnd.Next(0, pool_index.Count)];
                    pool_index.Remove(pool_index_1);
                    int pool_index_2 = pool_index[ms_rnd.Next(0, pool_index.Count)];

                    // 從目標基因組合(chromosome)中選擇兩個不同的基因位置作為片段分割點(頭尾基因位置不算)
                    List<int> chromosome_index = Enumerable.Range(0, nvars).ToList();
                    int chromosome_index_1 = chromosome_index[ms_rnd.Next(1, chromosome_index.Count - 1)];
                    chromosome_index.Remove(chromosome_index_1);
                    int chromosome_index_2 = chromosome_index[ms_rnd.Next(1, chromosome_index.Count - 1)];

                    // 調整片段分割點1之位置較小(前)；片段分割點2之位置較大(後)
                    if (chromosome_index_1 > chromosome_index_2)
                    {
                        int temp = -chromosome_index_1;
                        chromosome_index_1 = chromosome_index_2;
                        chromosome_index_2 = temp;
                    }

                    // 將配對池位置pool_index_1的基因組合(chromosome)作為目標；而將配對池位置pool_index_2的基因組合(chromosome)作為替代交換用
                    // 將所選基因組合(chromosome)依片段分割點chromosome_index_1及chromosome_index_2作交換並新增目標至子代族群基因組合
                    List<double> target_chromosome = new List<double>(nvars);
                    for (int j = 0; j < nvars; j++)
                    {
                        // 建立Pool中第pool_index_1的基因組合
                        target_chromosome.Add(Pool[pool_index_1][j]);
                    }
                    for (int j = chromosome_index_1 + 1; j <= chromosome_index_2; j++)
                    {
                        // 若未指定特定值則正常交換基因片段
                        target_chromosome[j] = Constraints.Custom_Config.FlightCarouselNo[j] == 0 ? Pool[pool_index_2][j] : Constraints.Custom_Config.FlightCarouselNo[j];
                    }

                    Population_crossover.Add(target_chromosome);
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
        /// 基因組中基因變數個數(number of design variables)
        /// </summary>
        private int nvars = default(int);

        /// <summary>
        /// 基因組合個數(人口數)
        /// </summary>
        private int m_iPopulationSize = default(int);

        /// <summary>
        /// 基因交換率
        /// </summary>
        private double m_lfCrossRate = default(double);

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
        /// 基因交換後之族群基因組合(共有 PopulationSize * Rate 組)
        /// </summary>
        public List<List<double>> Population_crossover { get; private set; }

        #endregion

        #region =====[Public] Constructor & Destructor=====

        /// <summary>
        /// Crossover類別建構子
        /// </summary>
        /// <param name="nlength">基因組中基因變數個數(number of designed variables)</param>
        /// <param name="population_size">基因組合個數(人口數)</param>
        /// <param name="cross_rate">基因交換率</param>
        public Crossover(int nlength, int population_size, double cross_rate)
        {
            try
            {
                nvars = nlength;
                m_iPopulationSize = population_size;
                m_lfCrossRate = cross_rate;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        /// <summary>
        /// Crossover類別解構子
        /// </summary>
        ~Crossover()
        {
            try
            {
                Population_crossover = null;
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        #endregion
    }
}
