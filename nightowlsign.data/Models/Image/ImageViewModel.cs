using ImageStorage;
using System;
using System.Collections.Generic;
using System.Web;
using System.Threading.Tasks;
using System.Linq;
using nightowlsign.data.Models.Signs;

namespace nightowlsign.data.Models.Image
{
    public class ImageViewModel : BaseModel.ViewModelBase
    {
        private readonly SignManager _signManager;
        public bool Selected { get; set; }

        public ImageViewModel() : base()
        {
            _signManager = new SignManager();
        }

        //Properties--------------
        public List<ImagesAndSign> Images { get; set; }
        public ImagesAndSign SearchEntity { get; set; }
        public data.Image Entity { get; set; }
        public HttpPostedFileBase File { get; set; }
        public bool LastImage { get; set; }
        public UploadedImage ImageToUpload { get; set; }

        public string CommandString { get; set; }
        public string Message { get; set; }
        public int? SearchSignId { get; set; }
        public IList<SelectListItem> SignList
        {
            get
            {
                using (nightowlsign_Entities db = new nightowlsign_Entities())
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

            ImageToUpload = new UploadedImage
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
            if (EventCommand.ToLower() == "insert")
            {
                var task = Task.Run(async () => { await Insert(); });
                task.Wait();
            }
            else
            {
                base.HandleRequest();
            }
        }

        protected override void ResetSearch()
        {
            SearchEntity = new ImagesAndSign();
        }

        protected override void Get()
        {
            ImageManager cmm = new ImageManager();
            //SearchEntity.Caption = SearchEntity.Caption;
            SearchEntity.SignSize = SearchSignId;
            Images = cmm.Get(SearchEntity);
        }

        protected override void Edit()
        {
            ImageManager imm = new ImageManager();
            Entity = imm.Find(Convert.ToInt32(EventArgument));

            ImageToUpload.Caption = Entity.Caption;
            ImageToUpload.Id = Entity.Id;
            ImageToUpload.Url = Entity.ImageURL;
            ImageToUpload.DateTaken = Entity.DateTaken ?? DateTime.Now;
            ImageToUpload.SignId = Entity.SignSize ?? 0;

            base.Edit();
        }

        protected override void Add()
        {
            IsValid = true;
            ImageToUpload = new UploadedImage
            {
                SignId = Entity.SignSize ?? SearchSignId ?? 1
            };
            base.Add();
        }

        protected override void Save()
        {
            ImageManager imm = new ImageManager();
            if (imm.Update(ImageToUpload))
            {
                Mode = "List";
                Message = "Image successfully updated";
            }
            ValidationErrors = imm.ValidationErrors;
            base.Save();
        }

        protected async Task<Boolean> Insert()
        {
            bool success = false;
            ImageManager imm = new ImageManager();
            if (Mode == "Add")
            {
                ImageService imageService = new ImageService();
                if (ImageToUpload.SignId > 0)
                {
                    SearchSignId = ImageToUpload.SignId;
                    var sign = _signManager.Find(ImageToUpload.SignId);
                    ImageToUpload.SignHeight = sign.Height ?? 96;
                    ImageToUpload.SignWidth = sign.Width ?? 244;
                    ImageToUpload = await imageService.CreateUploadedImage(File, ImageToUpload);
                    await imageService.AddImageToBlobStorageAsync(ImageToUpload);
                    success = await imm.Insert(File.FileName, ImageToUpload);
                }
                else
                {
                    Message = "Please select a Sign Size for the image(s)";
                }

                if (success && LastImage)
                {
                    Mode = "Add";
                    EventCommand = "add";
                    Message = "Image(s) successfully added";
                    base.HandleRequest();

                }
            }
            ValidationErrors = imm.ValidationErrors;
            return success;
        }

        protected override void Delete()
        {
            foreach (var image in Images)
            {
                if (image.Selected)
                {
                    DeleteTheImage(image);
                    //ToDo add in image delete
                }
            }
            Get();
        }

        private void DeleteTheImage(ImagesAndSign imageToDelete)
        {
            DeleteImage(imageToDelete.Id);
            DeleteFromScheduleImage(imageToDelete.Id);
            DeleteImageFromBlob(imageToDelete.Caption);
        }

        private void DeleteImageFromBlob(string imageName)
        {
            ImageService imageService= new ImageService();
            bool result= imageService.DeleteFile(imageName);
        }

        private void DeleteFromScheduleImage(int id)
        {
            ScheduleImageManager sim = new ScheduleImageManager();
            sim.RemoveImagesFromScheduleImage(id);
        }

        private void DeleteImage(int imageId)
        {
            ImageManager imm = new ImageManager();
            Entity = imm.Find(imageId);
            imm.Delete(Entity);
            base.Delete();
        }
    }
}
