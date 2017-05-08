using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using nightowlsign.data.Models.Signs;
using nightowlsign.data.Models.StoreSignDto;

namespace nightowlsign.data.Models.SendToSign
{
    public class SendToSignManager : ISendToSignManager
    {
        private readonly Inightowlsign_Entities _context;
        public SendToSignManager(Inightowlsign_Entities context)
        {
            _context = context;
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }
        //Properties
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public List<int?> Get(data.Schedule Entity)
        {
            int scheduleID = Entity.Id;
            var query = (from c in _context.ScheduleImages
                         where c.ScheduleID == scheduleID
                         select c.ImageID);
            return query.ToList();
        }

        public List<ImageSelect> GetImagesForThisSchedule(int scheduleId)
        {
            var query = (from s in _context.Images
                         join si in _context.ScheduleImages.Where(si => si.ScheduleID == scheduleId)
                         on s.Id equals si.ImageID
                         select new ImageSelect()
                         {
                             ImageId = s.Id,
                             Name = s.Caption,
                             ThumbNail = s.ThumbNailSmall,
                             ScheduleId = scheduleId,
                             ImageUrl = s.ImageURL
                         });
            return query.ToList();
        }

        public List<SignDto> GetSignsForSchedule(int scheduleId)
        {
            var query = (from ss in _context.ScheduleStores.Where(ss => ss.ScheduleID == scheduleId)
                         join s in _context.Store on ss.StoreId equals s.id
                         join sn in _context.Signs on s.SignId equals sn.id
                         select new SignDto
                         {
                             Id = sn.id,
                             Width = sn.Width ?? 0,
                             Height = sn.Height ?? 0,
                             Model = sn.Model
                         });
            return query.ToList();
        }

        public List<StoreSignDTO> GetStoresWithThisSign(int scheduleId)
        {
            var query = (from ss in _context.ScheduleStores.Where(ss => ss.ScheduleID == scheduleId)
                         join stores in _context.Store on ss.StoreId equals stores.id
                         select new StoreSignDTO()
                         {
                             StoreId = stores.id,
                             StoreName = stores.Name,
                             IPAddress = stores.IpAddress,
                             SubMask = stores.SubMask,
                             Port = stores.Port
                         });
            return query.ToList();
        }

        public SignParameters GetSignParameters(int signId)
        {
            var query = (from s in _context.Signs.Where(s => s.id == signId)
                         select new SignParameters()
                         {
                             Width = s.Width ?? 0,
                             Height = s.Height ?? 0
                         }).FirstOrDefault();
            return query;
        }

        public data.Image Find(int Id)
        {
            return _context.Images.Find(Id);
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
                        _context.ScheduleImages.Find(imageSelect.Id);
                    if (scheduleImage != null)
                    {
                        db.ScheduleImages.Attach(scheduleImage);
                        db.ScheduleImages.Remove(scheduleImage);
                        db.SaveChanges();
                    }
                }
            }
        }

        public ScheduleImage GetValues(ImageSelect imageSelect)
        {
            ScheduleImage scheduleImage = null;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                scheduleImage =
                    db.ScheduleImages.FirstOrDefault(x => x.ImageID == imageSelect.ImageId && x.ScheduleID == imageSelect.ScheduleId);
            }
            return scheduleImage;
        }

        public bool IsSelected(int ScheduleId, int ImageID)
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
