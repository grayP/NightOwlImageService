using System;

namespace ImageProcessor.Services
{
    public static class RandomNumber
    {
        internal static int GenerateRandomNo()
        {
            const int min = 1000;
            const int max = 9999;
            var rdm = new Random();
            return rdm.Next(min, max);
        }
        }
}