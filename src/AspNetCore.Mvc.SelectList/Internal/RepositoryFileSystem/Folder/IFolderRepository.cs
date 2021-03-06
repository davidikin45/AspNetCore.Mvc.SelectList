﻿namespace AspNetCore.Mvc.SelectList.Internal.RepositoryFileSystem.Folder
{
    internal interface IFolderRepository : IFolderReadOnlyRepository
    {
        void Create(string path);

        void Move(string sourcePath, string destinationPath);

        void Rename(string sourcePath, string newName);

        void Delete(string path);
    }
}
