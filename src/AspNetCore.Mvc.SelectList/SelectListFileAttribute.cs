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

        public SelectListFileAttribute(string path, string dataTextFieldExpression = nameof(FileInfo.FullName))
        {
            Path = path;
            DataTextFieldExpression = dataTextFieldExpression;
        }

        private static IFileSystemGenericRepositoryFactory _fileSystemGenericRepositoryFactory = new FileSystemGenericRepositoryFactory();
        public string Path { get; set; }

        public string DataTextFieldExpression { get; set; } = nameof(FileInfo.FullName);
        public string OrderByProperty { get; set; } = nameof(FileInfo.LastWriteTime);
        public string OrderByType { get; set; } = "desc";

        public bool IncludeSubDirectories { get; set; } = true;
        public string SearchPattern { get; set; } = "*";

        public bool RemoveSearchPathFromText { get; set; } = true;
        public bool RootRelativeValue { get; set; } = true;

        protected async override Task<IEnumerable<SelectListItem>> GetSelectListItemsAsync(SelectListContext context)
        {
            var dataValueField = nameof(FileInfo.FullName);

            if (!Path.EndsWith(@"\"))
            {
                Path = Path + @"\";
            }

            var hostingEnvironment = context.HttpContext.RequestServices.GetRequiredService<IHostingEnvironment>();
            var mappedWwwPath = hostingEnvironment.MapWwwPath(Path);
            var mappedContentPath = hostingEnvironment.MapContentPath(Path);

            var searchPath = Path;
            var root = "";
            var webFolder = false;
            if (mappedWwwPath != mappedContentPath)
            {
                searchPath = mappedContentPath;
                root = hostingEnvironment.ContentRootPath + @"\";
                if (Directory.Exists(mappedWwwPath))
                {
                    webFolder = true;
                    searchPath = mappedWwwPath;
                    root = hostingEnvironment.WebRootPath + @"\";
                }
            }

            var repository = _fileSystemGenericRepositoryFactory.CreateFileRepositoryReadOnly(default(CancellationToken), searchPath, IncludeSubDirectories, SearchPattern);
            var data = await repository.GetAllAsync(LamdaHelper.GetOrderByFunc<FileInfo>(OrderByProperty, OrderByType), null, null);

            var results = new List<SelectListItem>();
            foreach (var item in data)
            {
                IHtmlHelper html = context.CreateHtmlHelper((dynamic)item);

                results.Add(new ModelSelectListItem()
                {
                    Model = item,
                    Html = html,
                    Text = RemoveSearchPathFromText ? context.Eval(html, item, DataTextFieldExpression).Replace(searchPath, "") : context.Eval(html, item, DataTextFieldExpression),
                    Value = RootRelativeValue ? webFolder ? context.Eval(html, item, dataValueField).Replace(root, "").Replace(@"\", @"/") : context.Eval(html, item, dataValueField).Replace(root, "") : context.Eval(html, item, dataValueField),
                });
            }

            return results;
        }
    }
}