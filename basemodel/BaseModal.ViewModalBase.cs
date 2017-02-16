using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseModel
{
    public class ViewModelBase
    {
        public ViewModelBase()
        {
            Init();
        }
        public string Mode { get; set; }
        public string EventCommand { get; set; }
        public string EventArgument { get; set; }
        public bool IsDetailVisible { get; set; }
        public bool IsSearchVisible { get; set; }
        public bool IsListAreaVisible { get; set; }
        public bool IsValid { get; set; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        //----------------------------------------------------------------------------------------
        protected virtual void Init()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
            EventCommand = "list";
            EventArgument = String.Empty;
            ListMode();
        }

        //-----------------------------------------------------------------------------
        public virtual void HandleRequest()
        {
            switch (EventCommand.ToLower())
            {
                case "list":
                case "search":
                    ListMode();
                    Get();
                    break;
                case "resetsearch":
                    ResetSearch();
                    Get();
                    break;
                case "add":
                    Add();
                    Get();
                    break;
                case "cancel":
                    ListMode();
                    Get();
                    break;
                case "save":
                    Save();
                    if (IsValid)
                    {
                        Get();
                    }
                    break;
                case "edit":
                    IsValid = true;
                    Edit();
                    break;

                case "delete":
                    Delete();
                    break;
                default:
                    break;
            }
        }


        //--------------------------------------------------------------------------------------

        protected virtual void ListMode()
        {
            IsValid = true;

            IsListAreaVisible = true;
            IsSearchVisible = true;
            IsDetailVisible = false;
            Mode = "list";

        }

        protected virtual void AddMode()
        {
            IsListAreaVisible = false;
            IsSearchVisible = false;
            IsDetailVisible = true;
            Mode = "Add";
        }

        protected virtual void EditMode()
        {
            IsListAreaVisible = false;
            IsSearchVisible = false;
            IsDetailVisible = true;
            Mode = "Edit";
        }





        protected virtual void ResetSearch()
        {
            ListMode();

        }


        protected virtual void Get()
        {

        }
        protected virtual void Edit()
        {
            EditMode();
        }
        protected virtual void Add()
        {

            AddMode();

        }

        protected virtual void Delete()
        {
            ListMode();
        }


        protected virtual void Save()
        {
            if (ValidationErrors.Count > 0)
            {
                IsValid = false;
            }

            if (!IsValid)
            {
                if (Mode == "Add")
                {
                    AddMode();
                }
                else
                {
                    EditMode();
                }
            }
        }

    }
}

