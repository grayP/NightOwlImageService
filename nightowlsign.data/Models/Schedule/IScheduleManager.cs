using System;
using System.Collections.Generic;

namespace nightowlsign.data.Models.Schedule
{
    public interface IScheduleManager
    {
        List<data.ScheduleAndSign> Get(data.Schedule entity);
        data.Schedule Find(int scheduleId);
        bool Validate(data.Schedule entity);
        bool Update(data.Schedule entity);
        bool Insert(data.Schedule entity);
        bool Delete(data.Schedule entity);
    }
}