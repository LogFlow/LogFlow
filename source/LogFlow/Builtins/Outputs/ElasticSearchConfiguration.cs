using System.Collections.Generic;

namespace LogFlow.Builtins.Outputs
{
    public class ElasticSearchConfiguration
    {
        public ElasticSearchConfiguration()
        {
            Host = "localhost";
            Port = 9200;
            IndexNameFormat = @"\l\o\g\f\l\o\w\-yyyyMM";
            ConnectionLimit = 5;
        }
        public string Type { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Ttl { get; set; }
        public int ConnectionLimit { get; set; }
        public string IndexNameFormat { get; set; }
    }
}
