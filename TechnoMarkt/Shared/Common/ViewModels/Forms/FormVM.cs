using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.Common.ViewModels.Forms
{
    public enum FormAction
    {
        Add,
        Edit,
    }

    public abstract class FormVM
    {
        public FormAction Action { get; set; } = FormAction.Add;

        public bool IsEdit => Action == FormAction.Edit;
    }
}




