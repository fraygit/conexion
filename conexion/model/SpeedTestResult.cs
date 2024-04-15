using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace conexion.model
{
    public class SpeedTestResult
    {
        public Ping Ping { get; set; }

        public Download Download { get; set; }
        public Result Result { get; set; }

        public Upload Upload { get; set; }

    }

    public class Download
    {
        public int Bandwidth { get; set; }
    }

    public class Upload
    {
        public int Bandwidth { get; set; }
    }
    public class Result
    {
        public string Url { get; set; }
    }
    public class Ping {
        public double Jitter { get; set; }
        public double Latency { get; set; }

    }
}