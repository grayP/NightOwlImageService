using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightowlsign.data.Models.ScreenBrightness
{
    public class ScreenBrightnessManager
    {
        public int currentBrightness { get; set; }
        private nightowlsign_Entities _context;
        public ScreenBrightnessManager()
        {
            _context = new nightowlsign_Entities();
        }

        private int GetBrightness(int storeId, int MinutesPastMidnight)
        {
            return _context.SignBrightnesses.Where(s => s.StoreId == storeId && s.FromPeriod <= MinutesPastMidnight && s.ToPeriod >= MinutesPastMidnight).Select(s => s.BrightnessLevel).FirstOrDefault() ?? 31;
        }

        public bool SignBrightnessNeedsToBeSet(int storeId, int minutesPastMidnight)
        {
            var CurrentStore = _context.StoreAndSigns.First(s => s.id == storeId);
            currentBrightness = GetBrightness(storeId, minutesPastMidnight);
            return currentBrightness == CurrentStore.Brightness;
        }

    }
}
