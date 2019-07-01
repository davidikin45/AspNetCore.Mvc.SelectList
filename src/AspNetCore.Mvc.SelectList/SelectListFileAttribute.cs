using AspNetCore.Mvc.SelectList.Helpers;
using AspNetCore.Mvc.SelectList.Internal;
using AspNetCore.Mvc.SelectList.Internal.RepositoryFileSystem;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListFileAttribute : SelectListAttribute
    {

        public SelectListFileAttribute(string path, string dataTextFieldExpression = nameof(FileInfo.Name))
        {
            Path = path;
            DataTextFieldExpression = dataTextFieldExpression;
        }

        private static IFileSystemGenericRepositoryFactory _fileSystemGenericRepositoryFactory = new FileSystemGenericRepositoryFactory();
        public string Path { get; set; }

        public string DataTextFieldExpression { get; set; } = nameof(FileInfo.Name);
        public string OrderByProperty { get; set; } = nameof(FileInfo.LastWriteTime);
        public string OrderByType { get; set; } = "desc";

        public bool IncludeSubDirectories { get; set; } = true;
        public string SearchPattern { get; set; } = "*";
        public bool PhysicalPathAsValue { get; set; }

        protected async override Task<IEnumerable<SelectListItem>> GetSelectListItemsAsync(SelectListContext context)
        {
            var dataValueField = nameof(FileInfo.FullName);

            var hostingEnvironment = context.HttpContext.RequestServices.GetRequiredService<IHostingEnvironment>();
            var mappedPath = hostingEnvironment.MapWwwPath(Path);

            var repository = _fileSystemGenericRepositoryFactory.CreateFileRepositoryReadOnly(default(CancellationToken), mappedPath, IncludeSubDirectories, SearchPattern);
            var data = await repository.GetAllAsync(LamdaHelper.GetOrderByFunc<FileInfo>(OrderByProperty, OrderByType), null, null);

            var results = new List<SelectListItem>();
            foreach (var item in data)
            {
                results.Add(new SelectListItem()
                {
                    Text = context.Display(item, DataTextFieldExpression),
                    Value = item.GetPropValue(dataValueField) != null ? (PhysicalPathAsValue ? item.GetPropValue(dataValueField).ToString() : item.GetPropValue(dataValueField).ToString().Replace(mappedPath, "")) : ""
                });
            }

            return results;
        }
    }
}