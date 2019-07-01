using AspNetCore.Mvc.SelectList.Helpers;
using AspNetCore.Mvc.SelectList.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListDbAttribute : SelectListAttribute
    {
        public SelectListDbAttribute(Type dbContextType, Type modelType, string dataTextFieldExpression = "Id")
        {
            DbContextType = dbContextType;
            ModelType = modelType;
            DataTextFieldExpression = dataTextFieldExpression;
        }

        public Type DbContextType { get; set; }
        public Type ModelType { get; set; }

        public string DataTextFormat { get; set; } = "";
        public string DataTextFieldExpression { get; set; } = "Id";
        public string DataValueField { get; set; } = "Id";

        public string OrderByProperty { get; set; } = "Id";
        public string OrderByType { get; set; } = "desc";

        protected override async Task<IEnumerable<SelectListItem>> GetSelectListItemsAsync(SelectListContext context)
        {
            DbContext db = (DbContext)context.HttpContext.RequestServices.GetService(DbContextType);

            if (db == null)
                throw new Exception("Database not found");

            var pi = ModelType.GetProperty(OrderByProperty);

            Type iQueryableType = typeof(IQueryable<>).MakeGenericType(new[] { ModelType });

            IQueryable query = (IQueryable)db.GetType().GetMethod("Set").MakeGenericMethod(ModelType).Invoke(db, null);

            if (context.SelectedOnly)
            {
                var whereClause = DbContextHelper.SearchForEntityByValues(ModelType, DataValueField, context.SelectedValues.Cast<Object>());
                query = (IQueryable)typeof(LamdaHelper).GetMethod(nameof(LamdaHelper.Where)).MakeGenericMethod(ModelType).Invoke(null, new object[] { query, whereClause });
            }
            else
            {
                if (!(context.ModelExplorer.Metadata is DefaultModelMetadata defaultModelMetadata))
                    throw new Exception("Expected DefaultModelMetadata");

                var whereClauseAttributes = defaultModelMetadata.Attributes.PropertyAttributes.OfType<DbContextSelectListWhereAttribute>().ToList();

                foreach (var where in whereClauseAttributes)
                {
                    var whereClause = LamdaHelper.SearchForEntityByProperty(ModelType, where.PropertyName, where.Values);
                    query = (IQueryable)typeof(LamdaHelper).GetMethod(nameof(LamdaHelper.Where)).MakeGenericMethod(ModelType).Invoke(null, new object[] { query, whereClause });
                }

                if (!string.IsNullOrWhiteSpace(OrderByProperty))
                {
                    if (OrderByType == "asc")
                    {
                        query = (IQueryable)typeof(DbContextHelper).GetMethod(nameof(DbContextHelper.QueryableOrderBy)).MakeGenericMethod(ModelType).Invoke(null, new object[] { query, OrderByProperty, true });
                    }
                    else
                    {
                        query = (IQueryable)typeof(DbContextHelper).GetMethod(nameof(DbContextHelper.QueryableOrderBy)).MakeGenericMethod(ModelType).Invoke(null, new object[] { query, OrderByProperty, false });
                    }
                }
            }

            dynamic dynamicQuery = query;
            IEnumerable results = (IEnumerable)(await EntityFrameworkQueryableExtensions.ToListAsync(dynamicQuery, CancellationToken.None));

            var items = new List<SelectListItem>();
            foreach (var item in results)
            {
                var selectListItem = new SelectListItem()
                {
                    Text = context.Display(item, DataTextFieldExpression),
                    Value = item.GetPropValue(DataValueField) != null ? item.GetPropValue(DataValueField).ToString() : ""
                };

                items.Add(selectListItem);
            }

            return items;
        }
    }

    public class DbContextSelectListWhereAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public IEnumerable<Object> Values { get; set; }
    }
}
