using System;
using System.Collections.Generic;
using System.Linq;

namespace T1_BHS_OAS.GA
{
    /// <summary>
    /// "Selection" 基因演算法親代選擇類別
    /// </summary>
    public class Selection
    {
        #region =====[Public] Event=====



        #endregion

        #region =====[Protected] Event Raise=====



        #endregion

        #region =====[Public] Default Event Body=====

        #region SelectPool_Roulette(List<List<double>> population, List<double> fitness_set)//int
        /// <summary>
        /// 親代選擇 - Roulette輪盤法
        /// </summary>
        /// <param name="population">原族群基因組合</param>
        /// <param name="fitness_set">族群基因組合適應值集</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        public int SelectPool_Roulette(List<List<double>> population, List<double> fitness_set)
        {
            int ns = population.Count;  // 族群數
            Pool = new List<List<double>>(ns);    // 重製族群基因配對池(Pool)
            List<double> fitness_inv = new List<double>(ns);
            List<double> fitness_accprob = new List<double>(ns);    // 作為輪盤用
            double fitness_acc = 0, rnd_num;

            try
            {
                // 計算各適應值倒數的累積機率密度分佈(0 - 1)
                for (int i = 0; i < ns; i ++)
                {
                    fitness_inv.Add(1 / fitness_set[i]);
                }
                double fitness_sum = fitness_inv.Sum();
                for (int i = 0; i < ns; i++)
                {
                    fitness_acc += fitness_inv[i] / fitness_sum;
                    fitness_accprob.Add(fitness_acc);
                }

                // 隨機選擇輪盤位置以挑選親代基因組合至配對池
                for (int i = 0; i < ns; i++)
                {
                    rnd_num = ms_rnd.NextDouble();
                    for (int j = 0; j < fitness_accprob.Count; j++)
                    {
                        if (fitness_accprob[j] > rnd_num)
                        {
                            Pool.Add(population[j]);
                            break;
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

        #endregion

        #region =====[Private] Variable=====

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
        /// 族群基因配對池(Row: 基因組合)
        /// </summary>
        public List<List<double>> Pool { get; private set; }

        #endregion

        #region =====[Public] Constructor & Destructor=====

        /// <summary>
        /// Selection類別建構子
        /// </summary>
        public Selection()
        {
            try
            {

            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        /// <summary>
        /// Selection類別解構子
        /// </summary>
        ~Selection()
        {
            try
            {

            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
            }
        }

        #endregion
    }
}
