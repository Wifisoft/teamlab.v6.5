using System;

namespace ASC.Web.Core.Helpers
{
    public class GrammaticalHelper
    {
        public static string ChooseNumeralCase(int number, string nominative, string genitiveSingular, string genitivePlural)
        {
            if (
                String.Compare(
                    System.Threading.Thread.CurrentThread.CurrentUICulture.ThreeLetterISOLanguageName,
                    "rus", true) == 0)
            {
                int[] formsTable = { 2, 0, 1, 1, 1, 2, 2, 2, 2, 2 };

                number = Math.Abs(number);
                int res = formsTable[((((number % 100) / 10) != 1) ? 1 : 0) * (number % 10)];
                switch (res)
                {
                    case 0:
                        return nominative;
                    case 1:
                        return genitiveSingular;
                    default:
                        return genitivePlural;
                }
            }
            else
                return number == 1 ? nominative : genitivePlural;
        }
    }
}
