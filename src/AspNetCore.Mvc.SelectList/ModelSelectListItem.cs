using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetCore.Mvc.SelectList
{
    public class ModelSelectListItem<TModel> : SelectListItem
    {
        public ModelSelectListItem(ModelSelectListItem item)
        {
            this.Disabled = item.Disabled;
            this.Group = item.Group;
            this.Selected = item.Selected;
            this.Text = item.Text;
            this.Value = item.Value;

            this.Model = item.Model;
            this.Html = (IHtmlHelper<TModel>)item.Html;
        }

        public TModel Model { get; set; }
        public IHtmlHelper<TModel> Html { get; set; }
    }

    public class ModelSelectListItem : SelectListItem
    {
        public ModelSelectListItem<TModel> Cast<TModel>()
        {
            return new ModelSelectListItem<TModel>(this);
        }

        public dynamic Model {get; set;}

        public IHtmlHelper Html { get; set; }

        public ModelSelectListItem()
        {

        }
        public ModelSelectListItem(object model)
            : base() {
            Model = model;
        }
        public ModelSelectListItem(object model, string text, string value) : base(text, value)
        {
            Model = model;
        }
        public ModelSelectListItem(object model, string text, string value, bool selected) : base(text, value, selected)
        {
            Model = model;
        }
        public ModelSelectListItem(object model, string text, string value, bool selected, bool disabled) 
            : base(text, value, selected, disabled)
        {
            Model = model;
        }
    }
}
