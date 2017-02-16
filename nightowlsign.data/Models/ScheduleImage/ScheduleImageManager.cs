using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using nightowlsign.data;


namespace nightowlsign.data.Models
{
    public class ScheduleImageManager
    {
        public ScheduleImageManager()
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

        public List<ImageSelect> GetAllImages(int signId, int scheduleId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                //var query = (from s in db.Images
                //             join ss in db.ScheduleSigns.Where(ss => ss.ScheduleID == scheduleId)
                //             on s.SignSize equals ss.SignId
                //             select new ImageSelect() { ImageId = s.Id, Name = s.Caption, ThumbNail = s.ThumbNailSmall });
                var query = (from s in db.Images.Where(s=>s.SignSize==signId)
                             select new ImageSelect()
                             {
                                 ImageId = s.Id,
                                 Name = s.Caption,
                                 ThumbNail = s.ThumbNailLarge,
                                 SignId = signId,
                                 SignSize = s.SignSize ??0
                             });
                return query.ToList();
            }
        }


        public data.Image Find(int id)
        {
            data.Image ret = null;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                ret = db.Images.FirstOrDefault(e=>e.Id==id);
            }
            return ret;
        }

        public void RemoveImagesFromScheduleImage(int imageId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
             var ret= db.ScheduleImages.Where(e => e.ImageID == imageId);
                db.ScheduleImages.RemoveRange(ret);
                db.SaveChanges();
            }      
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
