using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using nightowlsign.data;
using nightowlsign.data.Models.Signs;
using nightowlsign.data.Models.StoreSignDto;


namespace nightowlsign.data.Models
{
    public class SendToSignManager
    {
        public SendToSignManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }
        //Properties
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public List<int?> Get(data.Schedule Entity)
        {
            int scheduleID = Entity.Id;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var query = (from c in db.ScheduleImages
                             where c.ScheduleID == scheduleID
                             select c.ImageID);
                return query.ToList();
            }

        }

        public List<ImageSelect> GetImagesForThisSchedule(int scheduleId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var query = (from s in db.Images
                             join si in db.ScheduleImages.Where(si => si.ScheduleID == scheduleId)
                             on s.Id equals si.ImageID
                             join ss in db.ScheduleSigns on si.ScheduleID equals ss.ScheduleID
                             join sign in db.Signs on ss.SignId equals sign.id
                             select new ImageSelect()
                             {
                                 ImageId = s.Id,
                                 Name = s.Caption,
                                 ThumbNail = s.ThumbNailSmall,
                                 ScheduleId = scheduleId,
                                 SignSize = ss.SignId ?? 0,
                                 ImageUrl = s.ImageURL
                             });
                return query.ToList();
            }
        }

        internal List<SignDto> GetSignsForSchedule(int scheduleId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var query = (from ss in db.ScheduleSigns.Where(ss => ss.ScheduleID == scheduleId)
                             join s in db.Signs on ss.SignId equals s.id
                             select new SignDto
                             {
                                 Id = ss.Id,
                                 Width = s.Width ?? 0,
                                 Height = s.Height ?? 0,
                                 Model = s.Model
                             });
                return query.ToList();
            }
        }

        internal List<StoreSignDTO> GetStoresWithThisSign(int scheduleId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var query = (from ss in db.ScheduleSigns.Where(ss => ss.ScheduleID == scheduleId)
                             join s in db.StoreSigns on ss.SignId equals s.SignId
                             join stores in db.Store on s.StoreId equals stores.id
                             select new StoreSignDTO()
                             {
                                 StoreId = s.StoreId ?? 0,
                                 StoreName = stores.Name,
                                 IPAddress = s.IPAddress,
                                 SubMask = s.SubMask,
                                 Port = s.Port
                             });
                return query.ToList();
            }
        }

        public SignParameters GetSignParameters(int SignId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var query = (from s in db.Signs.Where(s => s.id == SignId)
                             select new SignParameters()
                             {
                                 Width = s.Width ?? 0,
                                 Height = s.Height ?? 0
                             }).FirstOrDefault();
                return query;
            }
        }

        public data.Image Find(int Id)
        {
            data.Image ret = null;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                ret = db.Images.Find(Id);
            }
            return ret;
        }

        public void UpdateImageList(ImageSelect imageSelect, data.Schedule schedule)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                ScheduleImage imageSelected = new ScheduleImage
                {
                    ImageID = imageSelect.ImageId,
                    ScheduleID = schedule.Id,
                    Id = imageSelect.Id
                };
                if (imageSelect.Selected)
                {
                    db.Set<ScheduleImage>().AddOrUpdate(imageSelected);
                    db.SaveChanges();
                }
                else
                {
                    ScheduleImage scheduleImage =
                        db.ScheduleImages.Find(imageSelect.Id);
                    if (scheduleImage != null)
                    {
                        db.ScheduleImages.Attach(scheduleImage);
                        db.ScheduleImages.Remove(scheduleImage);
                        db.SaveChanges();
                    }

                }
            }
        }

        internal ScheduleImage GetValues(ImageSelect imageSelect)
        {
            ScheduleImage scheduleImage = null;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                scheduleImage =
                    db.ScheduleImages.FirstOrDefault(x => x.ImageID == imageSelect.ImageId && x.ScheduleID == imageSelect.ScheduleId);
            }
            return scheduleImage;
        }

        internal bool IsSelected(int ScheduleId, int ImageID)
        {
            bool ret = false;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                ret = db.ScheduleImages.Any(x => x.ScheduleID == ScheduleId && x.ImageID == ImageID);
            }
            return ret;
        }
    }
}
