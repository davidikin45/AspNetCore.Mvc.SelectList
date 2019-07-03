using AspNetCore.Mvc.SelectList.Helpers;
using AspNetCore.Mvc.SelectList.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public SelectListDbAttribute(Type dbContextType, Type modelType, string rawSql, object[] parameters, string dataTextFieldExpression = "Id")
        {
            DbContextType = dbContextType;
            ModelType = modelType;
            RawSql = rawSql;
            RawSqlParameters = parameters;
            DataTextFieldExpression = dataTextFieldExpression;
        }

        public Type DbContextType { get; set; }
        public Type ModelType { get; set; }

        public string DataTextFormat { get; set; } = "";
        public string DataTextFieldExpression { get; set; } = "Id";
        public string DataValueField { get; set; } = "Id";

        public string OrderByProperty { get; set; } = "Id";
        public string OrderByType { get; set; } = "desc";

        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 1000;

        public string RawSql { get; set; } = "";
        public object[] RawSqlParameters { get; set; }

        public bool EnableChangeTracking { get; set; } = false;

        private static MethodInfo _dbContextSetMethod = typeof(DbContext).GetMethod("Set");
        private static MethodInfo _dbContextWhereClauseMethod = typeof(LamdaHelper).GetMethod(nameof(LamdaHelper.Where));
        private static MethodInfo _dbContextOrderByMethod = typeof(DbContextHelper).GetMethod(nameof(DbContextHelper.QueryableOrderBy));

        protected override async Task<IEnumerable<SelectListItem>> GetSelectListItemsAsync(SelectListContext context)
        {
            //Get DbContext from DI container
            DbContext db = (DbContext)context.HttpContext.RequestServices.GetService(DbContextType);
              
            if (db == null)
                throw new Exception("Database not found");

            //Get DbSet as IQueryable
            IQueryable query = (IQueryable)_dbContextSetMethod.MakeGenericMethod(ModelType).Invoke(db, null);

            if (!string.IsNullOrEmpty(RawSql))
            {
                query = (IQueryable)RelationalQueryableExtensions.FromSql((dynamic)query, RawSql, RawSqlParameters);
            }

            if (!EnableChangeTracking)
            {
                query = (IQueryable)EntityFrameworkQueryableExtensions.AsNoTracking((dynamic)query);
            }

            if (context.SelectedOnly)
            {
                //Select by Id
                var whereClause = DbContextHelper.SearchForEntityByValues(ModelType, DataValueField, context.CurrentValues);
                query = (IQueryable)_dbContextWhereClauseMethod.MakeGenericMethod(ModelType).Invoke(null, new object[] { query, whereClause });
            }
            else
            {
                if (context.ModelExplorer.Metadata is DefaultModelMetadata defaultModelMetadata)
                {
                    //Loop over where clauses

                    IEnumerable<SelectListDbWhereEqualsAttribute> whereClauseAttributes = null;
                    if(defaultModelMetadata.MetadataKind == ModelMetadataKind.Property)
                    {
                        whereClauseAttributes = defaultModelMetadata.Attributes.PropertyAttributes.OfType<SelectListDbWhereEqualsAttribute>().Where(a => a.SelectListId == this.SelectListId);
                    }
                    else if (defaultModelMetadata.MetadataKind == ModelMetadataKind.Type)
                    {
                        whereClauseAttributes = defaultModelMetadata.Attributes.TypeAttributes.OfType<SelectListDbWhereEqualsAttribute>().Where(a => a.SelectListId == this.SelectListId);
                    }

                    if(whereClauseAttributes != null)
                    {
                        foreach (var where in whereClauseAttributes)
                        {
                            var whereClause = LamdaHelper.SearchForEntityByProperty(ModelType, where.PropertyName, where.Values);
                            query = (IQueryable)_dbContextWhereClauseMethod.MakeGenericMethod(ModelType).Invoke(null, new object[] { query, whereClause });
                        }
                    }
                }
            }

            //Order By
            if (!string.IsNullOrWhiteSpace(OrderByProperty))
            {
                if (OrderByType == "asc")
                {
                    query = (IQueryable)_dbContextOrderByMethod.MakeGenericMethod(ModelType).Invoke(null, new object[] { query, OrderByProperty, true });
                }
                else
                {
                    query = (IQueryable)_dbContextOrderByMethod.MakeGenericMethod(ModelType).Invoke(null, new object[] { query, OrderByProperty, false });
                }
            }

            //Skip
            query = (IQueryable)Queryable.Skip((dynamic)query, Skip);

            //Take
            query = (IQueryable)Queryable.Take((dynamic)query, Take);

            //Get Results
            IEnumerable results = (IEnumerable)(await EntityFrameworkQueryableExtensions.ToListAsync((dynamic)query, CancellationToken.None));

            return new ModelMultiSelectList(context.Html, results, DataValueField, DataTextFieldExpression);
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class SelectListDbWhereEqualsAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public object[] Values { get; set; }

        public string SelectListId { get; set; }

        public SelectListDbWhereEqualsAttribute(string propertyName, params object[] values)
        {
            PropertyName = propertyName;
            Values = values;
        }
    }
}
