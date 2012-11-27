#if DEBUG
namespace ASC.Web.Files.Tests
{
    using System.Collections.Generic;
    using ASC.Files.Core;
    using log4net;

    public static class DemoData
    {

        // Бухгалтерия
        // Инструкции
        // - обслуживание персонал
        // - отдел работы с клиентами
        // Кадры
        // - должностные записки, жалобы
        // - планирование отпусков, резюме
        // Маркетинг
        //- наружная реклама
        //- рекламные кампаниии
        // Приказы
        //- 2007
        //- 2008
        //- 2009

        private static readonly ILog _logger =
                LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<Folder> ListFolders = new List<Folder>
                                                 {
                                                   new  Folder
                                                   {
                                                        Title = "Бухгалтерия",
                                                        ParentFolderID = 0
                                                   },
                                                   new  Folder
                                                   {
                                                        Title = "Инструкции",
                                                        ParentFolderID = 0
                                                   },
                                                   new  Folder
                                                   {
                                                        Title = "обслуживание персонал",
                                                        ParentFolderID = 0
                                                   },
                                                   new  Folder
                                                   {
                                                        Title = "отдел работы с клиентами",
                                                        ParentFolderID = 0
                                                   },

                                                   new  Folder
                                                   {
                                                        Title = "Кадры",
                                                        ParentFolderID = 0
                                                   },
                                                   new  Folder
                                                   {
                                                        Title = "должностные записки, жалобы",
                                                        ParentFolderID = 0
                                                   },
                                                    new  Folder
                                                   {
                                                        Title = "планирование отпусков, резюме",
                                                        ParentFolderID = 0
                                                   },
                                                   new  Folder
                                                   {
                                                        Title = "Маркетинг",
                                                        ParentFolderID = 0
                                                   },
                                                   new  Folder
                                                   {
                                                        Title = "наружная реклама",
                                                        ParentFolderID = 0
                                                   },
                                                   new  Folder
                                                   {
                                                        Title = "рекламные кампании",
                                                        ParentFolderID = 0
                                                   },
                                                   new  Folder
                                                   {
                                                        Title = "Приказы",
                                                        ParentFolderID = 0
                                                   },
                                                   new  Folder
                                                   {
                                                        Title = "2007",
                                                        ParentFolderID = 0
                                                   },
                                                    new  Folder
                                                   {
                                                        Title = "2008",
                                                        ParentFolderID = 0
                                                   },
                                                   new  Folder
                                                   {
                                                        Title = "2009",
                                                        ParentFolderID = 0
                                                   },
                                                   new  Folder
                                                   {
                                                        Title = "Приказ № 146 О поощрении сотрудников ЗАО 'НКТ'",
                                                        ParentFolderID = 0
                                                   },
                                                   new  Folder
                                                   {
                                                        Title = "Исаншена",
                                                        ParentFolderID = 0
                                                   },
                                                   
                                                   new  Folder
                                                   {
                                                        Title = "Нагаева",
                                                        ParentFolderID = 0
                                                   }


                                                 };

        public static Folder SampleFolder = new Folder
                                                {
                                                    Title = "Новая папка",
                                                    ParentFolderID = 21

                                                };


        public static File SampleFile = new File
         {
             ContentLength = 123123,
             ContentType = "txt\acds",

             FolderID = 1,
             Title = "Program.avi",
             Version = 0,
             ID = 7
         };

    }
}
#endif