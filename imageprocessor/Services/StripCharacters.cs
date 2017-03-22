using System.Linq;

namespace ImageProcessor.Services
{
    public static class StripCharacters
    {
        internal static string Strip(string scheduleName)
        {
            var invalidChars = " *./\\$";
            var notNullName = scheduleName ?? "";
            string invalidCharsRemoved = new string(notNullName
              .Where(x => !invalidChars.Contains(x))
              .ToArray());
            return invalidCharsRemoved;
        }

    }
}
