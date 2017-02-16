using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.Services
{
   public static class StripCharacters
    {
        internal static string Strip(string scheduleName)
        {
            var invalidChars = " *./\\$";
            string invalidCharsRemoved = new string(scheduleName
              .Where(x => !invalidChars.Contains(x))
              .ToArray());
            return invalidCharsRemoved;
        }

    }
}
