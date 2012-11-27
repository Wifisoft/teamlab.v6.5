using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.UI;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ASC.Web.Controls")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Ascensio System SIA")]
[assembly: AssemblyProduct("ASC.Web.Controls")]
[assembly: AssemblyCopyright("Ascensio System SIA 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

//bbcodearea
[assembly: WebResourceAttribute("ASC.Web.Controls.BBCodeTextArea.js.bbcodeeditor.js","text/javascript")]
[assembly: WebResourceAttribute("ASC.Web.Controls.BBCodeTextArea.css.style.css", "text/css")]
[assembly: WebResourceAttribute("ASC.Web.Controls.BBCodeTextArea.Images.text_bold.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.BBCodeTextArea.Images.text_italic.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.BBCodeTextArea.Images.text_strike.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.BBCodeTextArea.Images.text_underline.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.BBCodeTextArea.Images.code.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.BBCodeTextArea.Images.mail.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.BBCodeTextArea.Images.image.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.BBCodeTextArea.Images.world_link.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.BBCodeTextArea.Images.comments.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.BBCodeTextArea.Images.smile.png", "image/png")]


//pollform
[assembly: WebResourceAttribute("ASC.Web.Controls.PollForm.js.pollform.js", "text/javascript")]
[assembly: WebResourceAttribute("ASC.Web.Controls.PollForm.css.style.css", "text/css")]

//codeHighlighter
[assembly: WebResourceAttribute("ASC.Web.Controls.CodeHighlighter.js.highlight.pack.js", "text/javascript")]
[assembly: WebResourceAttribute("ASC.Web.Controls.CodeHighlighter.css.ascetic.css", "text/css")]
[assembly: WebResourceAttribute("ASC.Web.Controls.CodeHighlighter.css.dark.css", "text/css")]
[assembly: WebResourceAttribute("ASC.Web.Controls.CodeHighlighter.css.default.css", "text/css")]
[assembly: WebResourceAttribute("ASC.Web.Controls.CodeHighlighter.css.far.css", "text/css")]
[assembly: WebResourceAttribute("ASC.Web.Controls.CodeHighlighter.css.idea.css", "text/css")]
[assembly: WebResourceAttribute("ASC.Web.Controls.CodeHighlighter.css.magula.css", "text/css")]
[assembly: WebResourceAttribute("ASC.Web.Controls.CodeHighlighter.css.sunburst.css", "text/css")]
[assembly: WebResourceAttribute("ASC.Web.Controls.CodeHighlighter.css.vs.css", "text/css")]
[assembly: WebResourceAttribute("ASC.Web.Controls.CodeHighlighter.css.zenburn.css", "text/css")]

//treeviewpro
[assembly: WebResourceAttribute("ASC.Web.Controls.TreeViewPro.css.style.css", "text/css")]
[assembly: WebResourceAttribute("ASC.Web.Controls.TreeViewPro.js.treeviewprototype.js", "text/javascript")]
[assembly: WebResourceAttribute("ASC.Web.Controls.TreeViewPro.Images.plus.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.TreeViewPro.Images.minus.png", "image/png")]

//Container
[assembly: WebResource("ASC.Web.Controls.Container.Css.ContainerDefault.css", "text/css", PerformSubstitution = true)]

//Container Css Images
[assembly: WebResource("ASC.Web.Controls.Container.Css.Images.cancelButton.png", "image/png")]

//Container
[assembly: WebResource("ASC.Web.Controls.Tabs.Css.TabsDefault.css", "text/css")]

//Main NameSpace
[assembly: TagPrefix("ASC.Web.Controls", "ascwc")]

//Comments
[assembly: WebResourceAttribute("ASC.Web.Controls.Comments.Images.comment_delete.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.Comments.Images.comment_edit.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.Comments.Images.comment_response.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.Comments.Images.new.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.Comments.Images.comments_small.png", "image/png")]
[assembly: WebResourceAttribute("ASC.Web.Controls.Comments.js.comments.js", "text/javascript")]
[assembly: WebResourceAttribute("ASC.Web.Controls.Comments.js.onReady.js", "text/javascript")]

//fileuploader
[assembly: WebResourceAttribute("ASC.Web.Controls.FileUploader.js.fileuploader.js", "text/javascript")]
[assembly: WebResourceAttribute("ASC.Web.Controls.FileUploader.js.fileHtml5Uploader.js", "text/javascript")]
[assembly: WebResourceAttribute("ASC.Web.Controls.FileUploader.js.ajaxupload.js", "text/javascript")]
[assembly: WebResourceAttribute("ASC.Web.Controls.FileUploader.js.swfupload.swf", "application/x-shockwave-flash", PerformSubstitution=true)]
[assembly: WebResourceAttribute("ASC.Web.Controls.FileUploader.js.swfupload.js", "text/javascript")]
[assembly: WebResourceAttribute("ASC.Web.Controls.FileUploader.img.trash.png", "image/png")]


//ViewSwitcher
[assembly: WebResourceAttribute("ASC.Web.Controls.ViewSwitcher.js.viewswitcher.js", "text/javascript")]
[assembly: WebResourceAttribute("ASC.Web.Controls.ViewSwitcher.css.viewswitcher.css", "text/css")]

//ActionButton
[assembly: WebResourceAttribute("ASC.Web.Controls.ActionButton.js.actionbutton.js", "text/javascript")]
[assembly: WebResourceAttribute("ASC.Web.Controls.ActionButton.images.ajax_progress_loader.gif", "image/gif")]

//AdvancedUserSelector
[assembly: WebResource("ASC.Web.Controls.AdvancedUserSelector.images.sort_down_black.png", "image/png")]
[assembly: WebResource("ASC.Web.Controls.AdvancedUserSelector.images.search.png", "image/png")]
[assembly: WebResource("ASC.Web.Controls.AdvancedUserSelector.images.people_icon.png", "image/png")]
[assembly: WebResource("ASC.Web.Controls.AdvancedUserSelector.images.cross_grey.png", "image/png")]
[assembly: WebResource("ASC.Web.Controls.AdvancedUserSelector.images.collapse_down_dark.png", "image/png")]
[assembly: WebResource("ASC.Web.Controls.AdvancedUserSelector.js.AdvUserSelectorScript.js", "text/javascript")]
[assembly: WebResource("ASC.Web.Controls.AdvancedUserSelector.css.default.css", "text/css")]

//PageNavigator
[assembly: WebResource("ASC.Web.Controls.PageNavigator.js.pagenavigator.js", "text/javascript")]

//JsHtmlDecoder
[assembly: WebResource("ASC.Web.Controls.JsHtmlDecoder.js.decoder.js", "text/javascript")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("e8cb7dc7-ba62-499d-ade2-42c5f05df0dd")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
