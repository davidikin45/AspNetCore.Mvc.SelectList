namespace AspNetCore.Mvc.SelectList
{
    public class SelectListOptionAttribute : SelectListOptionsAttribute
    {
        public SelectListOptionAttribute(string text)
            :this(text, "true")
        {

        }

        public SelectListOptionAttribute(string text, string value)
            :base(new object[] { text }, new object[] { value })
        {

        }  
    }
}
