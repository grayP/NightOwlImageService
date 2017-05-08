using System.Collections.Generic;
using nightowlsign.data.Models.Signs;
using nightowlsign.data.Models.StoreSignDto;

namespace nightowlsign.data.Models.SendToSign
{
    public interface ISendToSignManager
    {
        List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        List<int?> Get(data.Schedule Entity);
        List<ImageSelect> GetImagesForThisSchedule(int scheduleId);
        List<SignDto> GetSignsForSchedule(int scheduleId);
        List<StoreSignDTO> GetStoresWithThisSign(int scheduleId);
        SignParameters GetSignParameters(int SignId);
        data.Image Find(int Id);
        void UpdateImageList(ImageSelect imageSelect, data.Schedule schedule);
        ScheduleImage GetValues(ImageSelect imageSelect);
        bool IsSelected(int ScheduleId, int ImageID);
    }
}