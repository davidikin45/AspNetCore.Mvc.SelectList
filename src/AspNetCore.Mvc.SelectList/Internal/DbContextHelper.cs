using AspNetCore.Mvc.SelectList.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace AspNetCore.Mvc.SelectList.Internal
{
    internal static class DbContextHelper
    {
        public static Object SearchForEntityByValues(Type type, string propertyName, IEnumerable<object> values)
        {
            Type funcType = typeof(Func<,>).MakeGenericType(new[] { type, typeof(bool) });

            var item = Expression.Parameter(type, "entity");
            var prop = Expression.PropertyOrField(item, propertyName);

            var propType = type.GetProperty(propertyName).PropertyType;

            var genericType = typeof(List<>).MakeGenericType(propType);
            var idList = Activator.CreateInstance(genericType);

            var add_method = idList.GetType().GetMethod("Add");
            foreach (var value in values)
            {
                if(propType == typeof(Guid))
                {
                    if(value.GetType() == typeof(Guid))
                    {
                        add_method.Invoke(idList, new object[] { value });
                    }
                    else
                    {
                        add_method.Invoke(idList, new object[] { Guid.Parse(value.ToString()) });
                    }
                }
                else
                {
                    add_method.Invoke(idList, new object[] { (dynamic)Convert.ChangeType(value, propType) });
                }
            }

            var contains_method = idList.GetType().GetMethod("Contains");
            var value_expression = Expression.Constant(idList);
            var contains_expression = Expression.Call(value_expression, contains_method, prop);

            return typeof(LamdaHelper).GetMethod(nameof(LamdaHelper.Lambda)).MakeGenericMethod(funcType).Invoke(null, new object[] { contains_expression, new ParameterExpression[] { item } });
        }

        public static IOrderedQueryable<T> QueryableOrderBy<T>(this IQueryable<T> items, string property, bool ascending)
        {
            var MyObject = Expression.Parameter(typeof(T), "MyObject");
            var MyEnumeratedObject = Expression.Parameter(typeof(IQueryable<T>), "MyQueryableObject");
            var MyProperty = Expression.Property(MyObject, property);
            var MyLamda = Expression.Lambda(MyProperty, MyObject);
            var MyMethod = Expression.Call(typeof(Queryable), ascending ? "OrderBy" : "OrderByDescending", new[] { typeof(T), MyLamda.Body.Type }, MyEnumeratedObject, MyLamda);
            var MySortedLamda = Expression.Lambda<Func<IQueryable<T>, IOrderedQueryable<T>>>(MyMethod, MyEnumeratedObject).Compile();
            return MySortedLamda(items);
        }

    }
}
