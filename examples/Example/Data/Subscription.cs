using System.ComponentModel.DataAnnotations;

namespace Example.Data
{
    public class Subscription
    {
        public string Id { get; set; }

        public int Order { get; set; }

        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Cost { get; set; }
    }
}
