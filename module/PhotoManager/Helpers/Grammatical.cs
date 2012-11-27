using System;
using ASC.PhotoManager.Resources;
using ASC.Web.Core.Helpers;

namespace ASC.PhotoManager.Helpers
{
    public class Grammatical
    {
        public static string ViewsCount(string format,int number)
        {
            return String.Format(
                    format,
                    number,
                    GrammaticalHelper.ChooseNumeralCase(
                            number,
                            PhotoManagerResource.ViewTitle.ToLower(),
                            PhotoManagerResource.ViewTitleR1.ToLower(),
                            PhotoManagerResource.ViewTitleRm.ToLower()
                        )
                );
        }
        public static string ViewsCount(int number)
        {
            return ViewsCount("{0} {1}", number);
        }

        public static string CommentsCount(string format, int number)
        {
            return String.Format(
                    format,
                    number,
                    GrammaticalHelper.ChooseNumeralCase(
                            number,
                            PhotoManagerResource.CommentTitle.ToLower(),
                            PhotoManagerResource.CommentTitleR1.ToLower(),
                            PhotoManagerResource.CommentTitleRm.ToLower()
                        )
                );
        }
        public static string CommentsCount(int number)
        {
            return CommentsCount("{0} {1}", number);
        }

        public static string PhotosCount(string format, int number)
        {
            return String.Format(
                    format,
                    number,
                    GrammaticalHelper.ChooseNumeralCase(
                            number,
                            PhotoManagerResource.PhotoTitle.ToLower(),
                            PhotoManagerResource.PhotoTitleR1.ToLower(),
                            PhotoManagerResource.PhotoTitleRm.ToLower()
                        )
                );
        }
        public static string PhotosCount(int number)
        {
            return PhotosCount("{0} {1}", number);
        }

        public static string AlbumsCount(string format, int number)
        {
            return String.Format(
                    format,
                    number,
                    GrammaticalHelper.ChooseNumeralCase(
                            number,
                            PhotoManagerResource.AlbumTitle.ToLower(),
                            PhotoManagerResource.AlbumTitleR1.ToLower(),
                            PhotoManagerResource.AlbumTitleRm.ToLower()
                        )
                );
        }
        public static string AlbumsCount(int number)
        {
            return AlbumsCount("{0} {1}", number);
        }
    }
}
