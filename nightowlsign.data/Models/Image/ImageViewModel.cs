using nightowlsign.data.Models.ScheduleImage;
using nightowlsign.data.Models.Signs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nightowlsign.data.Models.Image
{
    public class ImageViewModel : BaseModel.ViewModelBase
    {
        private readonly ImageManager _imageManager;
        private readonly ScheduleImageManager _scheduleImageManager;


        public bool Selected { get; set; }

        public ImageViewModel() : base()
        {
            _imageManager  = new ImageManager();
            _scheduleImageManager = new ScheduleImageManager();
        }

        //Properties--------------
        public List<ImagesAndSign> Images { get; set; }
        public ImagesAndSign SearchEntity { get; set; }
        public data.Image Entity { get; set; }
        public HttpPostedFileBase File { get; set; }
        public bool LastImage { get; set; }
 
        public string CommandString { get; set; }
        public string Message { get; set; }
        public int? SearchSignId { get; set; }
        public IList<SelectListItem> SignList
        {
            get
            {
                using (var db = new nightowlsign_Entities())
                {
                    var selectList = new List<SelectListItem>()
                    {
                        new SelectListItem
                        {
                            Id = 0,
                            Model = "Show All"
                        }
                    };
                    selectList.AddRange(from item in
                                      db.Signs.OrderBy(x => x.Model)
                                        select new SelectListItem()
                                        {
                                            SignId = item.id,
                                            Model = item.Model
                                        });
                    return selectList;
                }
            }
        }


        //---------------------------------------------------------------
        protected override void Init()
        {
            Images = new List<ImagesAndSign>();
            SearchEntity = new ImagesAndSign();
            Entity = new data.Image
            {
                DateTaken = DateTime.Now
            };

            LastImage = false;
            base.Init();
        }

        public override void HandleRequest()
        {
            if (CommandString?.ToLower() == "delete")
            {
                EventCommand = "delete";
            }
           switch (EventCommand.ToLower())
            {
                case "edit":
                case "save":
                    CommandString = "save";
                    break;

                case "add":
                    CommandString = "insert";
                    break;
                default:
                    CommandString = "";
                    break;
            }
                base.HandleRequest();
         }

        protected override void ResetSearch()
        {
            SearchEntity = new ImagesAndSign();
        }

        protected override void Get()
        {
            SearchEntity.SignSize = SearchSignId;
            Images = _imageManager.Get(SearchEntity);
        }

 protected override void Delete()
        {
            foreach (var image in Images)
            {
                if (image.Selected)
                {
                    DeleteTheImage(image);
                }
            }
            Get();
        }

        private void DeleteTheImage(ImagesAndSign imageToDelete)
        {
            DeleteImage(imageToDelete.Id);
            DeleteFromScheduleImage(imageToDelete.Id);
        }

  
        private void DeleteFromScheduleImage(int id)
        {
            _scheduleImageManager.RemoveImagesFromScheduleImage(id);
        }

        private void DeleteImage(int imageId)
        {
            var imagetoDelete = _imageManager.Find(imageId);
            _imageManager.Delete(imagetoDelete);
            base.Delete();
        }
    }
}
