using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nightowlsign.Services
{
    public static class RandomNumber
    {
        internal static int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        }
}