using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASC.Files.Core.Data.ProviderDao;
using ASC.Files.Core.ThirdPartyDao.Sharpbox;
using ASC.Web.Files.Classes;

namespace ASC.Files.Core.ProviderDao
{
    internal class ProviderDaoBase
    {
        private static readonly List<IDaoSelector> Selectors = new List<IDaoSelector>();

        protected static IDaoSelector Default { get; private set; }

        static ProviderDaoBase()
        {
            //TODO: Think
            Default = new DbDaoSelector();

            //Fill in selectors
            Selectors.Add(Default); //Legacy DB dao
            Selectors.Add(new SharpBoxDaoSelector());
        }

        protected bool IsCrossDao(object id1, object id2)
        {
            if (id2 == null || id1 == null)
                return false;
            return !Equals(GetSelector(id1).GetIdCode(id1), GetSelector(id2).GetIdCode(id2));
        }

        protected IDaoSelector GetSelector(object id)
        {
            return Selectors.FirstOrDefault(selector => selector.IsMatch(id)) ?? Default;
        }

        protected IEnumerable<IDaoSelector> GetSelectors()
        {
            return Selectors;
        }

        //For working with function where no id is availible
        protected IFileDao TryGetFileDao()
        {
            foreach (var daoSelector in Selectors)
            {
                try
                {
                    return daoSelector.GetFileDao(null);
                }
                catch (Exception)
                {

                }
            }
            throw new InvalidOperationException("No DAO can't be instanced without ID");
        }

        //For working with function where no id is availible
        protected IFolderDao TryGetFolderDao()
        {
            foreach (var daoSelector in Selectors)
            {
                try
                {
                    return daoSelector.GetFolderDao(null);
                }
                catch (Exception)
                {

                }
            }
            throw new InvalidOperationException("No DAO can't be instanced without ID");
        }


        protected File PerformCrossDaoFileCopy(object fromFileId, object toFolderId, bool deleteSourceFile)
        {
            var fromSelector = GetSelector(fromFileId);
            var toSelector = GetSelector(toFolderId);
            //Get File from first dao
            var fromFileDao = fromSelector.GetFileDao(fromFileId);
            var toFileDao = toSelector.GetFileDao(toFolderId);

            var fromFile = fromFileDao.GetFile(fromSelector.ConvertId(fromFileId));

            fromFile.ID = fromSelector.ConvertId(fromFile.ID);
            using (var fromFileStream = fromFile.ConvertedType != null
                                            ? DocumentUtils.GetConvertedFile(fromFile).GetBuffered()
                                            : fromFileDao.GetFileStream(fromFile))
            {
                fromFile.ID = null; //Reset id, so it can be created by apropriate provider
                fromFile.FolderID = toSelector.ConvertId(toFolderId);
                var toFile = toFileDao.SaveFile(fromFile, fromFileStream);

                if (deleteSourceFile)
                {
                    //Delete source file if needed
                    fromFileDao.DeleteFileStream(fromSelector.ConvertId(fromFileId));
                    fromFileDao.DeleteFile(fromSelector.ConvertId(fromFileId));
                }
                return toFile;
            }
        }

        protected object PerformCrossDaoFolderCopy(object fromFolderId, object toRootFolderId, bool deleteSourceFolder)
        {
            //Things get more complicated
            var fromSelector = GetSelector(fromFolderId);
            var toSelector = GetSelector(toRootFolderId);

            var fromFolderDao = fromSelector.GetFolderDao(fromFolderId);
            //Create new folder in 'to' folder
            var toFolderDao = toSelector.GetFolderDao(toRootFolderId);
            //Ohh
            var fromFolder = fromFolderDao.GetFolder(fromSelector.ConvertId(fromFolderId));
            var toFolder = toFolderDao.GetFolder(fromFolder.Title, toSelector.ConvertId(toRootFolderId));
            if (toFolder == null)
            {
                //Create
                var createdId = toFolderDao.SaveFolder(new Folder
                                                           {
                                                               Title = fromFolder.Title,
                                                               ParentFolderID = toSelector.ConvertId(toRootFolderId)
                                                           }
                    );
                toFolder = toFolderDao.GetFolder(toSelector.ConvertId(createdId));
            }

            var foldersToCopy = fromFolderDao.GetFolders(fromSelector.ConvertId(fromFolderId));
            var filesToCopy = fromFolderDao.GetFiles(fromSelector.ConvertId(fromFolderId), false);
            //Copy files first
            foreach (var file in filesToCopy)
            {
                PerformCrossDaoFileCopy(file, toFolder.ID, deleteSourceFolder);
            }
            foreach (var folder in foldersToCopy)
            {
                PerformCrossDaoFolderCopy(folder.ID, toFolder.ID, deleteSourceFolder);
            }
            if (deleteSourceFolder)
            {
                fromFolderDao.DeleteFolder(fromSelector.ConvertId(fromFolderId));
            }
            return toFolder.ID;
        }
    }
}