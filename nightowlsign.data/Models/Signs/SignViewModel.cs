using nightowlsign.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightowlsign.data.Models.Signs
{
    public class SignViewModel : BaseModel.ViewModelBase
    {
        private readonly Inightowlsign_Entities _context;
        private readonly SignManager sm;
        public SignViewModel(Inightowlsign_Entities context) : base()
        {
            _context = context;
            sm = new SignManager(_context);

        }
        public List<Sign> Signs { get; set; }
        public Sign SearchEntity { get; set; }
        public Sign Entity { get; set; }

        protected override void Init()
        {
            Signs = new List<Sign>();
            SearchEntity= new Sign();
            Entity= new Sign();
            base.Init();
        }
        public override void HandleRequest()
        {
            base.HandleRequest();
        }
        protected override void ResetSearch()
        {
            SearchEntity = new Sign();
        }

        protected override void Get()
        {
            Signs = sm.Get(SearchEntity);
        }

        protected override void Edit()
        {
            Entity = sm.Find(Convert.ToInt32(EventArgument));
            base.Edit();
        }

        protected override void Add()
        {
            IsValid = true;
            Entity = new Sign
            {
                Model = "",
                InstallDate = DateTime.Now
            };
            base.Add();
        }
        protected override void Save()
        {
            if (Mode == "Add")
            {
                sm.Insert(Entity);
            }
            else
            {
                sm.Update(Entity);
            }
            ValidationErrors = sm.ValidationErrors;
            base.Save();
        }

        protected override void Delete()
        {
            Entity = sm.Find(Convert.ToInt32(EventArgument));
            sm.Delete(Entity);
            Get();
            base.Delete();
        }
    }
}
