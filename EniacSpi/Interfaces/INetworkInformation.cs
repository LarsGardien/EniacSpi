﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EniacSpi.Interfaces
{
    public interface INetworkInformation
    {
        string SSID { get; set; }
        string MAC { get; set; }
        string Security { get; set; }
        int Signal { get; set; }
        
        bool IsCracking { get; set; }
        int CrackProgressStatus { get; set; }
        int CrackProgressEnd { get; set; }
        string Password { get; set; }
    }
}
