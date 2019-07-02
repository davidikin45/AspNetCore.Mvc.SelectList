using AspNetCore.Mvc.SelectList.Internal.RepositoryFileSystem.File;
using AspNetCore.Mvc.SelectList.Internal.RepositoryFileSystem.Folder;
using System.Threading;

namespace AspNetCore.Mvc.SelectList.Internal.RepositoryFileSystem
{
    internal class FileSystemGenericRepositoryFactory : IFileSystemGenericRepositoryFactory
    {
        public FileSystemGenericRepositoryFactory()
            :base()
        {

        }

        public IFileRepository CreateFileRepository(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false, string searchPattern = "*.*", params string[] extensions)
        {
            return new FileRepository(physicalPath, includeSubDirectories, searchPattern, cancellationToken, extensions);
        }

        public IFileReadOnlyRepository CreateFileRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false, string searchPattern = "*.*", params string[] extensions)
        {
            return new FileReadOnlyRepository(physicalPath, includeSubDirectories, searchPattern, cancellationToken, extensions);
        }

        public IFolderRepository CreateFolderRepository(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false, string searchPattern = "*", bool atLeastOneFile = true)
        {
            return new FolderRepository(physicalPath, includeSubDirectories, searchPattern, cancellationToken);
        }

        public IFolderReadOnlyRepository CreateFolderRepositoryReadOnly(CancellationToken cancellationToken, string physicalPath, bool includeSubDirectories = false, string searchPattern = "*", bool atLeastOneFile = true)
        {
            return new FolderReadOnlyRepository(physicalPath, includeSubDirectories, searchPattern, cancellationToken, atLeastOneFile);
        }
    }
}
