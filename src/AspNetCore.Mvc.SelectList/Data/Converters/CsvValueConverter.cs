using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

namespace AspNetCore.Mvc.SelectList
{

    public static class CsvConverterExtensions
    {
        public static PropertyBuilder<T> HasCsvValueConversion<T>(this PropertyBuilder<T> propertyBuilder) where T : class
        {

            var type = propertyBuilder.Metadata.ClrType;
            var valueConverter = GetPropertyValueConverter(type);
            propertyBuilder.HasConversion(valueConverter);

            return propertyBuilder;
        }

        private static ValueConverter GetPropertyValueConverter(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = type.GetGenericArguments()[0];
                if (elementType.IsEnum)
                {
                    var converter = (ValueConverter)Activator.CreateInstance(typeof(EnumListToCsvValueConverter<>).MakeGenericType(elementType));
                    return converter;
                }
                else
                {
                    var converter = (ValueConverter)Activator.CreateInstance(typeof(ListToCsvValueConverter<>).MakeGenericType(elementType));
                    return converter;
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                var elementType = type.GetGenericArguments()[0];
                if (elementType.IsEnum)
                {
                    var converter = (ValueConverter)Activator.CreateInstance(typeof(EnumIListToCsvValueConverter<>).MakeGenericType(elementType));
                    return converter;
                }
                else
                {
                    var converter = (ValueConverter)Activator.CreateInstance(typeof(IListToCsvValueConverter<>).MakeGenericType(elementType));
                    return converter;
                }
            }
            else if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Collection<>))
            {
                var elementType = type.GetGenericArguments()[0];
                if (elementType.IsEnum)
                {
                    var converter = (ValueConverter)Activator.CreateInstance(typeof(EnumCollectionToCsvValueConverter<>).MakeGenericType(elementType));
                    return converter;
                }
                else
                {
                    var converter = (ValueConverter)Activator.CreateInstance(typeof(CollectionToCsvValueConverter<>).MakeGenericType(elementType));
                    return converter;
                }
            }
            else if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                var elementType = type.GetGenericArguments()[0];
                if (elementType.IsEnum)
                {
                    var converter = (ValueConverter)Activator.CreateInstance(typeof(EnumICollectionToCsvValueConverter<>).MakeGenericType(elementType));
                    return converter;
                }
                else
                {
                    var converter = (ValueConverter)Activator.CreateInstance(typeof(ICollectionToCsvValueConverter<>).MakeGenericType(elementType));
                    return converter;
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var elementType = type.GetGenericArguments()[0];
                if (elementType.IsEnum)
                {
                    var converter = (ValueConverter)Activator.CreateInstance(typeof(EnumIEnumerableToCsvValueConverter<>).MakeGenericType(elementType));
                    return converter;
                }
                else
                {
                    var converter = (ValueConverter)Activator.CreateInstance(typeof(IEnumerableToCsvValueConverter<>).MakeGenericType(elementType));
                    return converter;
                }
            }
            else if (type.IsArray)
            {
                var elementType = type.GetElementType();
                if (elementType.IsEnum)
                {
                    var converter = (ValueConverter)Activator.CreateInstance(typeof(EnumArrayToCsvValueConverter<>).MakeGenericType(elementType));
                    return converter;
                }
                else
                {
                    var converter = (ValueConverter)Activator.CreateInstance(typeof(ArrayToCsvValueConverter<>).MakeGenericType(elementType));
                    return converter;
                }
            }
            else
            {
                throw new Exception("Unsupported property type");
            }
        }

        public static void AddCsvValues(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach(var clrPropertyType in entityType.ClrType.GetProperties())
                {
                    if(clrPropertyType.GetCustomAttributes(false).OfType<CsvDbAttribute>().Any() && !clrPropertyType.GetCustomAttributes(false).OfType<NotMappedAttribute>().Any())
                    {
                        var property = modelBuilder.Entity(entityType.ClrType).Property(clrPropertyType.Name);
                        var converter = GetPropertyValueConverter(clrPropertyType.PropertyType);
                        property.HasConversion(converter);
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class CsvDbAttribute : Attribute
    { }

    public class ArrayToCsvValueConverter<T> : ValueConverter<T[], string>
    {
        public ArrayToCsvValueConverter()
        : base(Csv<T>(), List<T>())
        {
        }

        private static Expression<Func<U[], string>>
            Csv<U>()
        {
            return v => string.Join(",", v);
        }

        private static Expression<Func<string, U[]>>
            List<U>()
        {
            return x => x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => Convert.ChangeType(h.Trim(), typeof(U))).Cast<U>().ToArray();
        }
    }

    public class ListToCsvValueConverter<T> : ValueConverter<List<T>, string>
    {
        public ListToCsvValueConverter()
        : base(Csv<T>(), List<T>())
        {
        }

        private static Expression<Func<List<U>, string>>
            Csv<U>()
        {
            return v => string.Join(",", v);
        }

        private static Expression<Func<string, List<U>>>
            List<U>()
        {
            return x => new List<U>(x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => Convert.ChangeType(h.Trim(), typeof(U))).Cast<U>());
        }
    }

    public class IListToCsvValueConverter<T> : ValueConverter<IList<T>, string>
    {
        public IListToCsvValueConverter()
        : base(Csv<T>(), List<T>())
        {
        }

        private static Expression<Func<IList<U>, string>>
            Csv<U>()
        {
            return v => string.Join(",", v);
        }

        private static Expression<Func<string, IList<U>>>
            List<U>()
        {
            return x => new List<U>(x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => Convert.ChangeType(h.Trim(), typeof(U))).Cast<U>());
        }
    }

    public class CollectionToCsvValueConverter<T> : ValueConverter<Collection<T>, string>
    {
        public CollectionToCsvValueConverter()
        : base(Csv<T>(), Collection<T>())
        {
        }

        private static Expression<Func<Collection<U>, string>>
            Csv<U>()
        {
            return v => string.Join(",", v);
        }

        private static Expression<Func<string, Collection<U>>>
            Collection<U>()
        {
            return x => new Collection<U>(x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => Convert.ChangeType(h.Trim(), typeof(U))).Cast<U>().ToList());
        }
    }

    public class ICollectionToCsvValueConverter<T> : ValueConverter<ICollection<T>, string>
    {
        public ICollectionToCsvValueConverter()
        : base(Csv<T>(), Collection<T>())
        {
        }

        private static Expression<Func<ICollection<U>, string>>
            Csv<U>()
        {
            return v => string.Join(",", v);
        }

        private static Expression<Func<string, ICollection<U>>>
            Collection<U>()
        {
            return x => new Collection<U>(x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => Convert.ChangeType(h.Trim(), typeof(U))).Cast<U>().ToList());
        }
    }

    public class IEnumerableToCsvValueConverter<T> : ValueConverter<IEnumerable<T>, string>
    {
        public IEnumerableToCsvValueConverter()
        : base(Csv<T>(), Enumerable<T>())
        {
        }

        private static Expression<Func<IEnumerable<U>, string>>
            Csv<U>()
        {
            return v => string.Join(",", v);
        }

        private static Expression<Func<string, IEnumerable<U>>>
            Enumerable<U>()
        {
            return x => x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => Convert.ChangeType(h.Trim(), typeof(U))).Cast<U>();
        }
    }

    public class EnumArrayToCsvValueConverter<T> : ValueConverter<T[], string> where T : struct
    {
        public EnumArrayToCsvValueConverter()
        : base(Csv<T>(), List<T>())
        {

        }

        private static Expression<Func<TEnum[], string>> Csv<TEnum>() where TEnum : struct
        {
            var enumConverter = new EnumToStringConverter<TEnum>();
            return v => string.Join(",", v.Select(s => enumConverter.ConvertToProvider(s).ToString()));
        }

        private static Expression<Func<string, TEnum[]>> List<TEnum>() where TEnum : struct
        {
            var enumConverter = new EnumToStringConverter<TEnum>();
            return x => x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => (TEnum)enumConverter.ConvertFromProvider(h.Trim())).ToArray();

        }
    }

    public class EnumListToCsvValueConverter<T> : ValueConverter<List<T>, string> where T : struct
    {
        public EnumListToCsvValueConverter()
        : base(Csv<T>(), List<T>())
        {

        }

        private static Expression<Func<List<TEnum>, string>> Csv<TEnum>() where TEnum : struct
        {
            var enumConverter = new EnumToStringConverter<TEnum>();
            return v => string.Join(",", v.Select(s => enumConverter.ConvertToProvider(s).ToString()));
        }

        private static Expression<Func<string, List<TEnum>>> List<TEnum>() where TEnum : struct
        {
            var enumConverter = new EnumToStringConverter<TEnum>();
            return x => x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => (TEnum)enumConverter.ConvertFromProvider(h.Trim())).ToList();

        }
    }

    public class EnumIListToCsvValueConverter<T> : ValueConverter<IList<T>, string> where T : struct
    {
        public EnumIListToCsvValueConverter()
        : base(Csv<T>(), List<T>())
        {

        }

        private static Expression<Func<IList<TEnum>, string>> Csv<TEnum>() where TEnum : struct
        {
            var enumConverter = new EnumToStringConverter<TEnum>();
            return v => string.Join(",", v.Select(s => enumConverter.ConvertToProvider(s).ToString()));
        }

        private static Expression<Func<string, IList<TEnum>>> List<TEnum>() where TEnum : struct
        {
            var enumConverter = new EnumToStringConverter<TEnum>();
            return x => x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => (TEnum)enumConverter.ConvertFromProvider(h.Trim())).ToList();

        }
    }

    public class EnumCollectionToCsvValueConverter<T> : ValueConverter<Collection<T>, string> where T : struct
    {
        public  EnumCollectionToCsvValueConverter()
        : base(Csv<T>(), Collection<T>())
        {

        }

        private static Expression<Func<Collection<TEnum>, string>> Csv<TEnum>() where TEnum : struct
        {
            var enumConverter = new EnumToStringConverter<TEnum>();
            return v => string.Join(",", v.Select(s => enumConverter.ConvertToProvider(s).ToString()));
        }

        private static Expression<Func<string, Collection<TEnum>>> Collection<TEnum>() where TEnum : struct
        {
            var enumConverter = new EnumToStringConverter<TEnum>();
            return x => new Collection<TEnum>(x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => (TEnum)enumConverter.ConvertFromProvider(h.Trim())).ToList());

        }
    }

    public class EnumICollectionToCsvValueConverter<T> : ValueConverter<ICollection<T>, string> where T : struct
    {
        public EnumICollectionToCsvValueConverter()
        : base(Csv<T>(), Collection<T>())
        {

        }

        private static Expression<Func<ICollection<TEnum>, string>> Csv<TEnum>() where TEnum : struct
        {
            var enumConverter = new EnumToStringConverter<TEnum>();
            return v => string.Join(",", v.Select(s => enumConverter.ConvertToProvider(s).ToString()));
        }

        private static Expression<Func<string, ICollection<TEnum>>> Collection<TEnum>() where TEnum : struct
        {
            var enumConverter = new EnumToStringConverter<TEnum>();
            return x => x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => (TEnum)enumConverter.ConvertFromProvider(h.Trim())).ToList();

        }
    }

    public class EnumIEnumerableToCsvValueConverter<T> : ValueConverter<IEnumerable<T>, string> where T : struct
    {
        public EnumIEnumerableToCsvValueConverter()
        : base(Csv<T>(), Collection<T>())
        {

        }

        private static Expression<Func<IEnumerable<TEnum>, string>> Csv<TEnum>() where TEnum : struct
        {
            var enumConverter = new EnumToStringConverter<TEnum>();
            return v => string.Join(",", v.Select(s => enumConverter.ConvertToProvider(s).ToString()));
        }

        private static Expression<Func<string, IEnumerable<TEnum>>> Collection<TEnum>() where TEnum : struct
        {
            var enumConverter = new EnumToStringConverter<TEnum>();
            return x => x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => (TEnum)enumConverter.ConvertFromProvider(h.Trim()));

        }
    }
}
