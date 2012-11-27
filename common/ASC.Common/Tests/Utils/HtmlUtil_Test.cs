#if DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ASC.Common.Tests.Utils
{
    using NUnit.Framework;
    using ASC.Common.Utils;


    [TestFixture]
    public class HtmlUtil_Test
    {
        [Test]
        public void GetTextBr()
        {
            string html = "Hello";
            Assert.AreEqual("Hello", HtmlUtil.GetText(html));

            html = "Hello    anton";
            Assert.AreEqual("Hello    anton", HtmlUtil.GetText(html));

            html = "Hello<\\ br>anton";
            //Assert.AreEqual("Hello\n\ranton", HtmlUtil.GetText(html));
        }

        public void Hard()
        {
            string html = @"<a href=""http://mediaserver:8080/Products/Community/Modules/Blogs/ViewBlog.aspx?blogID=94fae49d-2faa-46d3-bf34-655afbc6f7f4""><font size=""+1"">Неадекватное коммерческое предложение</font></a>
<div class=""moz-text-html"" lang=""x-unicode""><hr />
По работе много &quot;листаю&quot; спама, но пришел спам который меня заинтересовал:<br />
<blockquote> План действий, способствующих достижению успеха и богатства Аудиокнига mp3 &quot;ДУМАЙ И БОГАТЕЙ&quot;<br />
<br />
&quot;Думай и богатей&quot; - эта книга получила статус непревзойденного классического учебника по достижению богатства. В каждой главе автор упоминает о секрете добывания денег, пользуясь которым тысячи людей приобрели, преумножили и продолжают ...</blockquote>... <br />
<hr />
опубликовано <a href=""http://mediaserver:8080/Products/Community/Modules/Blogs/UserPage.aspx?userid=731fa2f6-0283-41ab-b4a6-b014cc29f358"">Хурлапов Павел</a> 20 авг 2009 15:53<br />
<a href=""http://mediaserver:8080/Products/Community/Modules/Blogs/ViewBlog.aspx?blogID=94fae49d-2faa-46d3-bf34-655afbc6f7f4#comments"">прокомментировать</a></div>";

            System.Diagnostics.Trace.Write(HtmlUtil.GetText(html));
        }

        [Test]
        public void FromFile()
        {
            var html = File.ReadAllText("tests/utils/html_test.html");//Include file!
            //var text = HtmlUtil.GetText(html);

            //var advancedFormating = HtmlUtil.GetText(html, true);
            var advancedFormating2 = HtmlUtil.GetText(html,40);
            Assert.LessOrEqual(advancedFormating2.Length,40);

            var advancedFormating3 = HtmlUtil.GetText(html, 40, "...", true);
            Assert.LessOrEqual(advancedFormating3.Length,40);
            Assert.That(advancedFormating3.EndsWith("..."));

            var empty = HtmlUtil.GetText(string.Empty);
            Assert.IsEmpty(empty);

            var invalid = HtmlUtil.GetText("This is not html <div>");
            Assert.AreEqual(invalid, "This is not html");

            var xss = HtmlUtil.GetText("<script>alert(1);</script> <style>html{color:#444}</style>This is not html <div on click='javascript:alert(1);'>");
            Assert.AreEqual(xss, "This is not html");

            //var litleText = HtmlUtil.GetText("12345678901234567890", 20, "...",true);

            var test1 = HtmlUtil.GetText(null);
            Assert.IsEmpty(test1);

            var test2 = HtmlUtil.GetText("text with \r\n line breaks",20);
            Assert.LessOrEqual(test2.Length, 20);

            var test3 = HtmlUtil.GetText("long \r\n text \r\n with \r\n text with \r\n line breaks", 20);
            Assert.LessOrEqual(test3.Length, 20);

            var test4 = HtmlUtil.GetText("русский текст блеать!", 20);
            Assert.LessOrEqual(test3.Length, 20);
            Assert.That(test4.StartsWith("русский текст"));

        }
    }
}
#endif