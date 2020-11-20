using System;
using System.Collections.Generic;
using System.Data;

namespace T1_BHS_OAS.DBLib
{
    public class ALL_EXE_CONF : DBRecord
    {
        #region =====[Private] Readonly String =====

        private static readonly string APP_NAME = "App_Name";
        private static readonly string APP_PATH = "App_Path";
        private static readonly string RUN_TYPE = "Run_Type";
        private static readonly string EXECUTE_TYPE = "Execute_Type";
        private static readonly string EXECUTE_TIME = "Execute_Time";
        private static readonly string UPDATE_TIME = "Update_Time";
        private static readonly string CLEAR_DAY = "Clear_Day";
        private static readonly string CLEAR_TIME = "Clear_Time";
        private static readonly string SERVICE_TIMER = "Service_Timer";
        private static readonly string SELECTED_DATE = "Selected_Date";
        private static readonly string PROGRESS_RATE = "Progress_Rate";
        private static readonly string LAST_ITERATION = "Last_Iteration";
        private static readonly string EXCEL_FLAG = "Excel_Flag";
        private static readonly string POPULATION_SIZE = "PopulationSize";
        private static readonly string GENERATIONS = "Generations";
        private static readonly string STALL_GEN_LIMIT = "StallGenLimit";
        private static readonly string FITNESS_LIMIT = "FitnessLimit";
        private static readonly string CROSS_RATE = "CrossRate";
        private static readonly string MUTATION_RATE = "MutationRate";
        private static readonly string SELECT_FCN = "SelectFcn";
        private static readonly string CROSSOVER_FCN = "CrossoverFcn";
        private static readonly string MUTATION_FCN = "MutationFcn";
        private static readonly string ESTIMATED_OPEN_TIME = "EstimatedOpenTime";
        private static readonly string ESTINATED_CLOSE_TIME = "EstimatedCloseTime";
        private static readonly string PEAK_SMALL = "Peak_Small_Sorter";
        private static readonly string PEAK_LARGE = "Peak_Large_Sorter";
        private static readonly string PEAK_NORTH = "Peak_North_Sorter";
        private static readonly string PEAK_SOUTH = "Peak_South_Sorter";
        private static readonly string PEAK_NNB = "Peak_NNB_Sorter";
        private static readonly string PEAK_81to84 = "Peak_81to84_Sorter";
        private static readonly string PEAK_DISABLE = "Peak_Disable_Sorter";
        private static readonly string OFF_SMALL = "Off_Small_Sorter";
        private static readonly string OFF_LARGE = "Off_Large_Sorter";
        private static readonly string OFF_NORTH = "Off_North_Sorter";
        private static readonly string OFF_SOUTH = "Off_South_Sorter";
        private static readonly string OFF_NNB = "Off_NNB_Sorter";
        private static readonly string OFF_81to84 = "Off_81to84_Sorter";
        private static readonly string OFF_DISABLE = "Off_Disable_Sorter";
        private static readonly string SHARE = "Share_Sorter";
        private static readonly string LATER_ATTR = "LaterAttr";
        private static readonly string GROUND_ATTR = "GroundAttr";
        private static readonly string FLIGHT_ATTR = "FlightAttr";
        private static readonly string DESTINATION_ATTR = "DestinationAttr";
        private static readonly string COUNTER_ATTR = "CounterAttr";
        private static readonly string OTHER_ATTR = "OtherAttr";

        #endregion =====[Private] Readonly String =====

        #region =====[Public] Class=====

        /// <summary>
        /// 航班行李預排程式設定值欄位物件
        /// </summary>
        public class OASApp
        {
            public string App_Name { get { return APP_NAME; } }
            public string App_Path { get { return APP_PATH; } }
            public string Run_Type { get { return RUN_TYPE; } }
            public string Execute_Type { get { return EXECUTE_TYPE; } }
            public string Execute_Time { get { return EXECUTE_TIME; } }
            public string Update_Time { get { return UPDATE_TIME; } }
            public string Clear_Day { get { return CLEAR_DAY; } }
            public string Clear_Time { get { return CLEAR_TIME; } }
            public string Service_Timer { get { return SERVICE_TIMER; } }
            public string Selected_Date { get { return SELECTED_DATE; } }
            public string Excel_Flag { get { return EXCEL_FLAG; } }
            public string Progress_Rate { get { return PROGRESS_RATE; } }
            public string Last_Iteration { get { return LAST_ITERATION; } }
            public string LaterAttr { get { return LATER_ATTR; } }
            public string GroundAttr { get { return GROUND_ATTR; } }
            public string FlightAttr { get { return FLIGHT_ATTR; } }
            public string DestinationAttr { get { return DESTINATION_ATTR; } }
            public string CounterAttr { get { return COUNTER_ATTR; } }
            public string OtherAttr { get { return OTHER_ATTR; } }
        }

        /// <summary>
        /// 航班行李預排程式演算法設定值
        /// </summary>
        public class GAConfig
        {
            public string PopulationSize { get { return POPULATION_SIZE; } }
            public string Generations { get { return GENERATIONS; } }
            public string StallGenLimit { get { return STALL_GEN_LIMIT; } }
            public string FitnessLimit { get { return FITNESS_LIMIT; } }
            public string CrossRate { get { return CROSS_RATE; } }
            public string MutationRate { get { return MUTATION_RATE; } }
            public string SelectFcn { get { return SELECT_FCN; } }
            public string CrossoverFcn { get { return CROSSOVER_FCN; } }
            public string MutationFcn { get { return MUTATION_FCN; } }
            public string EstimatedOpenTime { get { return ESTIMATED_OPEN_TIME; } }
            public string EstimatedCloseTime { get { return ESTINATED_CLOSE_TIME; } }
        }

        /// <summary>
        /// 預排程式轉盤設定值記錄欄位物件
        /// </summary>
        public class PreArrange
        {
            public string Peak_Small { get { return PEAK_SMALL; } }
            public string Peak_Large { get { return PEAK_LARGE; } }
            public string Peak_North { get { return PEAK_NORTH; } }
            public string Peak_South { get { return PEAK_SOUTH; } }
            public string Peak_NNB { get { return PEAK_NNB; } }
            public string Peak_81to84 { get { return PEAK_81to84; } }
            public string Peak_Disable { get { return PEAK_DISABLE; } }
            public string Off_Small { get { return OFF_SMALL; } }
            public string Off_Large { get { return OFF_LARGE; } }
            public string Off_North { get { return OFF_NORTH; } }
            public string Off_South { get { return OFF_SOUTH; } }
            public string Off_NNB { get { return OFF_NNB; } }
            public string Off_81to84 { get { return OFF_81to84; } }
            public string Off_Disable { get { return OFF_DISABLE; } }
            public string Share { get { return SHARE; } }
        }

        /// <summary>
        /// 資料庫表格欄位物件
        /// </summary>
        public class Row : AbstractRow
        {
            public string CONFIG_ID { get; set; }
            public string CONF_KEY { get; set; }
            public string CONF_VALUE { get; set; }
        }

        #endregion

        #region =====[Public] Object=====

        public static OASApp oOASApp { get { return new OASApp(); } }
        public static GAConfig oGAConfig { get { return new GAConfig(); } }
        public static PreArrange oPreArrange { get { return new PreArrange(); } }

        #endregion =====[Public] Object=====

        #region =====[Private] Parameter=====

        /// <summary>
        /// log記錄檔物件
        /// </summary>
        private static readonly ClassLibrary.FX.Utility.Log TxtLog = new ClassLibrary.FX.Utility.Log();

        #endregion

        #region =====[Public] Property=====



        #endregion

        #region =====Constructor & Destructor=====

        /// <summary>
        /// ALL_EXE_CONF類別建構子
        /// </summary>
        /// <param name="pConn">資料庫連接物件</param>
        public ALL_EXE_CONF(IDbConnection pConn) : base(pConn)
        {
            try
            {
                m_sTableName = Properties.Settings.Default.TableName_ALL_EXE_CONF;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ALL_EXE_CONF類別解構子
        /// </summary>
        ~ALL_EXE_CONF()
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region =====[Protected] Function (Extended)=====

        /// <summary>
        /// 擷取一筆資料表當前的目標列資料，使用前請將AbstractRow重作新的物件個體為Row
        /// </summary>
        /// <param name="pRs">擷取資料使用之執行個體命令物件</param>
        /// <returns>資料庫表格之列資料欄位物件</returns>
        protected override object FetchRecord(IDataReader pRs)
        {
            Row pRow = new Row();

            try
            {
                List<System.Reflection.PropertyInfo> props = new List<System.Reflection.PropertyInfo>(pRow.GetType().GetProperties());
                for (int i = 0; i < pRs.FieldCount; i++)
                {
                    string readerName = pRs.GetName(i);
                    foreach (System.Reflection.PropertyInfo prop in props)
                    {
                        if (readerName == prop.Name)
                        {
                            if (prop.PropertyType == typeof(string))
                            {
                                prop.SetValue(pRow, GetValueOrDefault<string>(pRs, i));
                            }
                            else
                            {
                                prop.SetValue(pRow, GetValueOrDefault<object>(pRs, i));
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pRow;
        }

        #endregion

        #region =====[Public] Method (Extended)=====

        /// <summary>
        /// 依SQL指令字串取得對應的資料列
        /// </summary>
        /// <param name="pSQLStr">SQL指令字串(SELECT)</param>
        /// <returns>
        /// <para> n: 讀取的資料列筆數</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 非"SELECT"之SQL指令字串</para>
        /// </returns>
        public int SelectBySQL(string pSQLStr)
        {
            if (!pSQLStr.Contains("SELECT"))
            {
                TxtLog.Error(string.Format("SQL string: [{0}] 非'SELECT'之SQL指令字串！", pSQLStr));
                return -2;
            }
            m_sSQLStr = pSQLStr;

            try
            {
                return QuerybySQL(m_sSQLStr);
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }

        /// <summary>
        /// 依CODE_ID取得對應的SQL指令字串資料列
        /// </summary>
        /// <param name="pSQLStr">SQL指令字串(UPDATE)</param>
        /// <returns>
        /// <para> n: 變更成功的資料列筆數</para>
        /// <para>-1: 例外錯誤</para>
        /// <para>-2: 非"UPDATE"之SQL指令字串</para>
        /// </returns>
        public int UpdateBySQL(string pSQLStr)
        {
            if (!pSQLStr.Contains("UPDATE"))
            {
                TxtLog.Error(string.Format("SQL string: [{0}] 非'UPDATE'之SQL指令字串！", pSQLStr));
                return -2;
            }
            m_sSQLStr = pSQLStr;

            try
            {
                return ExecuteBySQL(m_sSQLStr);
            }
            catch (Exception ex)
            {
                TxtLog.Error(ex);
                return -1;
            }
        }

        #endregion
    }
}
