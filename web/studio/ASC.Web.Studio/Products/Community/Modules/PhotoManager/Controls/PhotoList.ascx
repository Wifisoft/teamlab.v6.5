<%@ Assembly Name="ASC.Web.Community.PhotoManager" %>
<%@ Assembly Name="ASC.PhotoManager" %>
<%@ Import Namespace="ASC.Web.Community.PhotoManager" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Control Language="C#" EnableViewState="true" AutoEventWireup="true" CodeBehind="PhotoList.ascx.cs"
    Inherits="ASC.Web.Community.PhotoManager.Controls.PhotoList" %>

<script type="text/javascript">
function loadPhoto(id, addParams)
{
    window.open("photodetails.aspx?photo=" + id + addParams, '_self');
}
</script>
<table cellpadding="0" cellspacing="0" border="0" width="100%">
<tr>
<td><img src="<%=WebImageSupplier.GetAbsoluteWebPath("left_panel.png", ASC.PhotoManager.PhotoConst.ModuleID)%>" /></td>
<td>
<div id="mainList" style=" text-align: center; background-color:#EFF0F2; padding:1px 0px;border-top:solid 1px #EBEBEB;border-bottom:solid 1px #CFCFCF">
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td>
                <asp:HiddenField ID="hdnStartPosiotion" runat="server" Value="0" />
                <a id="prevButton" ><img id="pht_prev_btn" <%=(LeftBtnEnabled ? "style='cursor: pointer;'" : "") %> src="<%=WebImageSupplier.GetAbsoluteWebPath((LeftBtnEnabled ? "prev_small.png" : "prev_small_inact.png"), ASC.PhotoManager.PhotoConst.ModuleID)%>" /></a>                
            </td>
        
            <td><div style="margin-left:10px;margin-right:10px;">
                <asp:Literal ID="ltrPhotoList" runat="server"></asp:Literal>
                </div>
            </td>
            <td>
                <a id="nextButton" ><img id="pht_next_btn" <%=(RightBtnEnabled ? "style='cursor: pointer;'" : "") %> src="<%=WebImageSupplier.GetAbsoluteWebPath((RightBtnEnabled ? "next_small.png" : "next_small_inact.png"), ASC.PhotoManager.PhotoConst.ModuleID)%>" /></a>
            </td>
        </tr>
    </table>
</div>

</td>
<td><img src="<%=WebImageSupplier.GetAbsoluteWebPath("right_panel.png", ASC.PhotoManager.PhotoConst.ModuleID)%>" /></td>
</td>
</tr>
</table>

<script type="text/javascript">

var pht_url_next_img        ='<%=WebImageSupplier.GetAbsoluteWebPath("next_small.png", ASC.PhotoManager.PhotoConst.ModuleID)%>';
var pht_url_next_img_inact  ='<%=WebImageSupplier.GetAbsoluteWebPath("next_small_inact.png", ASC.PhotoManager.PhotoConst.ModuleID)%>';
var pht_url_prev_img        ='<%=WebImageSupplier.GetAbsoluteWebPath("prev_small.png", ASC.PhotoManager.PhotoConst.ModuleID)%>';
var pht_url_prev_img_inact  ='<%=WebImageSupplier.GetAbsoluteWebPath("prev_small_inact.png", ASC.PhotoManager.PhotoConst.ModuleID)%>';


document.getElementById('prevButton').onclick = function()
{
    GetPreviousImage();
}
document.getElementById('nextButton').onclick = function()
{
    GetNextImage();
}

var position;
var visibleCount = <%=_imagesCount%>;
if(document.getElementById("startPosition") != null)
position =document.getElementById("startPosition").value*1;

function GetPreviousImage()
{
    var imageList = document.getElementById("imageList");
    
    if(position > 0)
    {
        imageList.childNodes[position-1].style.display = "";
        imageList.childNodes[position + visibleCount - 1].style.display = "none";        
        position--;
        
        if(position <= 0)
        {
            jq('#pht_prev_btn').attr('src', pht_url_prev_img_inact);
            jq('#pht_prev_btn').css('cursor', 'default');        
        }   
        
        jq('#pht_next_btn').attr('src', pht_url_next_img);
        jq('#pht_next_btn').css('cursor', 'pointer');   
    }
}

function GetNextImage()
{
    var imageList = document.getElementById("imageList");
    if(position < imageList.childNodes.length - visibleCount)
    {   
        position++;
        imageList.childNodes[position+visibleCount - 1].style.display = "";
        imageList.childNodes[position-1].style.display = "none";
        
        if(position >= imageList.childNodes.length - visibleCount)
        {
            jq('#pht_next_btn').attr('src', pht_url_next_img_inact);
            jq('#pht_next_btn').css('cursor', 'default');        
        }
        
        jq('#pht_prev_btn').attr('src', pht_url_prev_img);
        jq('#pht_prev_btn').css('cursor', 'pointer');       
    }
}
</script>

