
function toggleNewsControllist(element) 
{
    var divid = "textNewsDiv"+element.id.substr(2);
    
    if(document.getElementById(divid).className != "newsText") 
    {
        document.getElementById(divid).className = "newsText"; 
    } 
    else 
    {
        document.getElementById(divid).className = "newsFullText"; 
    }
}

function callbackRemove(result)
{
    if (result.value.rs1!="0")
    {
        var itemDiv = "viewItem";
        if (itemDiv!=null)
        {
            var divDel = document.getElementById(itemDiv);
            divDel.className = 'errorBox errorText';
            divDel.innerHTML = result.value.rs2;
        }
    }
}

function callbackRemoveFromTable(result)
{
    if (result.value.rs1!="0")
    {
        var itemDiv = "item_"+result.value.rs1;
        if (itemDiv!=null)
        {
            var trDel = document.getElementById(itemDiv);
            trDel.className = 'errorBox errorText';
            var firstTD = 0;
            for(var i = 0; i < trDel.childNodes.length; i++)
            {
                if(trDel.childNodes[i].tagName == 'TD')
                {
                    if (firstTD == 0)
                    {
                        trDel.childNodes[i].innerHTML = "<td>"+result.value.rs2+"</td>"
                        firstTD = 1;
                    }
                    else
                    {
                        trDel.childNodes[i].innerHTML = "<td></td>"
                    }
                        
                }
            }
            
        }
    }
}

function ShowMore()
{
    jq('#divMore').show();
    //document.getElementById('divMore').style.display = 'block';
}

function HideMore()
{
    jq('#divMore').hide();
    //document.getElementById('divMore').style = 'none';
}

function NewsBlockButtons()
{
    jq('#panel_buttons').hide();
    jq('#action_loader').show();
}
function NewsUnBlockButtons() {
    jq('#panel_buttons').show();
    jq('#action_loader').hide();
}
function CheckData() {
    var question = jq("input[name$='poll_question']");
    if (jq(question).val()=="") {
        ShowRequiredError(question);
        NewsUnBlockButtons();
        return;
    }
    else {
        __doPostBack('', '');
    }
}

function CheckDataNews() {
    var newsName = jq("input[id$='_feedName']");
    if (jq(newsName).val() == "") {
        ShowRequiredError(newsName);
        NewsUnBlockButtons();
        return;
    }
    else {
        __doPostBack('', '');
    }
}