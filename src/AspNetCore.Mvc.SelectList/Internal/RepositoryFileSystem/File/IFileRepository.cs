using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mvc.SelectList.Internal.RepositoryFileSystem.File
{
    internal interface IFileRepository : IFileReadOnlyRepository
    {
        void Save(string path, byte[] bytes);

        void Save(string path, string text);

        void Move(string sourcePath, string destinationPath);

        void Rename(string sourcePath, string newName);

        void Delete(string path);
    }
}
