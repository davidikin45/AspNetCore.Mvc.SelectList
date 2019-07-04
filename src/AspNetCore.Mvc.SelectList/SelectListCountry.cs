using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListCountryAttribute : SelectListAttribute
    {
        public string DataTextFieldExpression { get; set; } = nameof(RegionInfo.EnglishName);
        public string DataValueFieldExpression { get; set; } = nameof(RegionInfo.TwoLetterISORegionName);

        protected override Task<IEnumerable<SelectListItem>> GetSelectListItemsAsync(SelectListContext context)
        {
            Dictionary<string, RegionInfo> countryList = new Dictionary<string, RegionInfo>();
            CultureInfo[] cInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo CInfo in cInfoList)
            {
                RegionInfo R = new RegionInfo(CInfo.LCID);
                if (!(countryList.ContainsKey(R.EnglishName)))
                {
                    countryList.Add(R.EnglishName, R);
                }
            }

            return Task.FromResult(new ModelSelectList(context.Html, countryList.Keys.OrderBy(k => k).Select(k => countryList[k]), DataValueFieldExpression, DataTextFieldExpression).AsEnumerable<SelectListItem>());
        }
    }
}
