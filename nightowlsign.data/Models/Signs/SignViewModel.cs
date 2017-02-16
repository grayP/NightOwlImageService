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

        public SignViewModel() : base()
        {

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
            SignManager sm = new SignManager();
            Signs = sm.Get(SearchEntity);
        }

        protected override void Edit()
        {
            SignManager sm = new SignManager();
            Entity = sm.Find(Convert.ToInt32(EventArgument));
            base.Edit();
        }

        protected override void Add()
        {
            IsValid = true;
            Entity = new Sign();
            Entity.Model = "";
            Entity.InstallDate = DateTime.Now;
            base.Add();
        }
        protected override void Save()
        {
            SignManager sm = new SignManager();
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
            SignManager sm= new SignManager();
            Entity = sm.Find(Convert.ToInt32(EventArgument));
            sm.Delete(Entity);
            Get();
            base.Delete();
        }
    }
}
