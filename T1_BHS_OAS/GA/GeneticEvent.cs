using System;
using System.Collections.Generic;

namespace T1_BHS_OAS.GA
{
    /// <summary>
    /// "GeneticEvent" 基因演算事件類別
    /// </summary>
    public class GeneticEvent
    {
        #region "GeneticEventArgs" 類別(GA事件參數)
        public class GeneticEventArgs : EventArgs
        {
            #region Properties



            #endregion

            #region Constructor & Destructor



            #endregion
        }
        #endregion

        #region GeneticEvent委派處理

        /// <summary>
        /// GeneticEvent 通用委派處理
        /// </summary>
        /// <param name="sender">GA相關觸發事件傳送者</param>
        /// <param name="e">GA事件參數</param>
        public delegate void GeneticEventHandler(object sender, GeneticEventArgs e);

        /// <summary>
        /// 族群基因組初始化 PopulationInitializeEvent 委派處理
        /// </summary>
        /// <param name="nvars"></param>
        /// <returns></returns>
        public delegate int IPopulationHandler(int nvars);

        /// <summary>
        /// 計算適應職 FitnessCalcEvent 委派處理
        /// </summary>
        /// <param name="population">族群基因組合</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        public delegate int FitnessCalcHandler(List<List<double>> population);

        /// <summary>
        /// 親代選擇 SelectPoolEvent 委派處理
        /// </summary>
        /// <param name="population">原族群基因組合</param>
        /// <param name="fitness_set">族群基因組合適應值集</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        public delegate int SelectionHandler(List<List<double>> population, List<double> fitness_set);

        /// <summary>
        /// 交換基因片段 CrossoverEvent 委派處理
        /// </summary>
        /// <param name="pool">族群基因配對池(Pool)</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        public delegate int CrossoverHandler(List<List<double>> pool);

        /// <summary>
        /// 基因突變 MutationEvent 委派處理
        /// </summary>
        /// <param name="pool">族群基因配對池(Pool)</param>
        /// <returns>
        /// <para> 0: 成功</para>
        /// <para>-1: 例外錯誤</para>
        /// </returns>
        public delegate int MutationHandler(List<List<double>> pool);

        #endregion
    }
}
