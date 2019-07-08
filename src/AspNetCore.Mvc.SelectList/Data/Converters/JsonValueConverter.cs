using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AspNetCore.Mvc.SelectList
{
    public static class JsonConverterExtensions
    {
        public static PropertyBuilder<T> HasJsonValueConversion<T>(this PropertyBuilder<T> propertyBuilder, JsonSerializerSettings settings = null) where T : class
        {
            settings = settings ?? new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>() { new Newtonsoft.Json.Converters.StringEnumConverter() },
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };

            propertyBuilder
            .HasConversion(new JsonValueConverter<T>(settings))
            .Metadata.SetValueComparer(new JsonValueComparer<T>());

            return propertyBuilder;
        }

        public static void AddJsonValues(this ModelBuilder modelBuilder, JsonSerializerSettings settings = null)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            settings = settings ?? new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>() { new Newtonsoft.Json.Converters.StringEnumConverter() },
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var clrPropertyType in entityType.ClrType.GetProperties())
                {
                    if (clrPropertyType.GetCustomAttributes(false).OfType<JsonDbAttribute>().Any() && !clrPropertyType.GetCustomAttributes(false).OfType<NotMappedAttribute>().Any())
                    {
                        var property = modelBuilder.Entity(entityType.ClrType).Property(clrPropertyType.Name).Metadata;
             
                        var modelType = property.PropertyInfo.PropertyType;
                        var converterType = typeof(JsonValueConverter<>).MakeGenericType(modelType);
                        var converter = (ValueConverter)Activator.CreateInstance(converterType, new object[] { settings, null });
                        property.SetValueConverter(converter);
                        var valueComparer = typeof(JsonValueComparer<>).MakeGenericType(modelType);
                        property.SetValueComparer((ValueComparer)Activator.CreateInstance(valueComparer, new object[0]));
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class JsonDbAttribute : Attribute
    { }

    public class JsonValueConverter<T> : ValueConverter<T, string> where T : class
    {
        public JsonValueConverter(JsonSerializerSettings settings = null, ConverterMappingHints hints = default) :
          base(v => JsonHelper.Serialize(v, settings), v => JsonHelper.Deserialize<T>(v, settings), hints)
        {

        }
    }

    internal class JsonValueComparer<T> : ValueComparer<T>
    {

        private static string Json(T instance)
        {
            return JsonConvert.SerializeObject(instance);
        }

        private static T DoGetSnapshot(T instance)
        {

            if (instance is ICloneable cloneable)
                return (T)cloneable.Clone();

            var result = (T)JsonConvert.DeserializeObject(Json(instance), typeof(T));
            return result;

        }

        private static int DoGetHashCode(T instance)
        {

            if (instance is IEquatable<T>)
                return instance.GetHashCode();

            return Json(instance).GetHashCode();

        }

        private static bool DoEquals(T left, T right)
        {

            if (left is IEquatable<T> equatable)
                return equatable.Equals(right);

            var result = Json(left).Equals(Json(right));
            return result;

        }

        public JsonValueComparer() : base(
          (t1, t2) => DoEquals(t1, t2),
          t => DoGetHashCode(t),
          t => DoGetSnapshot(t))
        {
        }

    }

    internal static class JsonHelper
    {
        public static T Deserialize<T>(string json, JsonSerializerSettings settings = null) where T : class
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static string Serialize<T>(T obj, JsonSerializerSettings settings = null) where T : class
        {
            return obj == null ? null : JsonConvert.SerializeObject(obj, settings);
        }
    }
}
