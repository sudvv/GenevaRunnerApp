namespace GenevaRunnerApp
{
    using System.Collections.Generic;

    public class Quries
    {
        #region " Property declaration "

        public string Name { get; set; }
        public string Query { get; set; }
        public string FilterCondition { get; set; }

        public string Summarize { get; set; }

        public int Dimensions { get; set; }

        public string ExpectedResult { get; set; }

        public string ExpectedCondition { get; set; }

        public string Result { get; set; }

        public string Category { get; set; }

        public string Prefix { get; set; }

        public Dictionary<string, double> PerfResult { get; set; }

        #endregion
    }

    public class AllQuery
    {
        #region " Property declaration "
        public string Ctegory { get; set; }
        public List<Quries> Quries { get; set; }

        #endregion
    }

    public class JsonRoot
    {
        #region " Property declaration "
        public List<AllQuery> AllQuries { get; set; }

        #endregion
    }
}
