using AspNetCore.Mvc.SelectList.Internal.RepositoryFileSystem.File;
using AspNetCore.Mvc.SelectList.Internal.RepositoryFileSystem.Folder;
using System;
using System.Threading;

namespace AspNetCore.Mvc.SelectList.Internal.RepositoryFileSystem
{
    internal interface IFileSystemGenericRepositoryFactory
    {
        IFileRepository CreateFileRepository(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false, string searchPattern = "*.*", params string[] extensions);
        IFileReadOnlyRepository CreateFileRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false, string searchPattern = "*.*", params string[] extensions);

        IFolderRepository CreateFolderRepository(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false, string searchPattern = "*", bool atLeastOneFile = true);
        IFolderReadOnlyRepository CreateFolderRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, Boolean includeSubDirectories = false, string searchPattern = "*", bool atLeastOneFile = true);
    }
}
