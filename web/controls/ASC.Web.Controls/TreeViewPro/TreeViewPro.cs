using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Controls
{
    public enum RenderMode
    { 
        Static,
        Dynamic
    }

    [Themeable(true)]   
    [ToolboxData("<{0}:TreeViewPro runat=server></{0}:TreeViewPro>")]
    public class TreeViewPro : WebControl
    {
        public RenderMode RenderMode { get; set; }
        public List<TreeNode> Nodes { get; set; }

        public bool ShowExpandCollapse { get; set; }

        public string ExpandImageUrl { get; set; }

        public string ExpandImageToolTip{ get; set; }
        
        public string CollapseImageUrl {get; set;}

        public string CollapseImageToolTip{ get; set; }

        public string SelectNodeCCSClass { get; set; }

        public string NodeCCSClass { get; set; }

        public string BehaviorID { get; set; }

        private string _jsObjName;
 
        public TreeViewPro(): this(new List<TreeNode>(0))
        {   
            
        }

        public TreeViewPro(List<TreeNode> nodes)
        {
            this.Nodes = nodes;
            RenderMode = RenderMode.Static;
            ShowExpandCollapse = true;
        }

        private string GetRegistryStript(bool withTag)
        {
            _jsObjName = String.IsNullOrEmpty(this.BehaviorID) ? ("__treeViewPro" + this.UniqueID) : this.BehaviorID;
            StringBuilder script = new StringBuilder();
            if(withTag)
                script.Append("<script language='javascript' type='text/javascript'>");

            script.Append("var " + _jsObjName + " = new TreeViewPrototype('" + _jsObjName + "','" + this.ID + "','" + (String.IsNullOrEmpty(this.NodeCCSClass) ? "treeviewpro_node" : this.NodeCCSClass) + "','" + (String.IsNullOrEmpty(this.SelectNodeCCSClass) ? "treeviewpro_nodeSelect" : this.SelectNodeCCSClass) + "');");
            foreach (TreeNode node in this.Nodes)            
                script.Append(RegisterScriptForNode(node, ""));
            

            if(withTag)
                script.Append("</script>");

            return script.ToString();

        }

        private string RegisterScriptForNode(TreeNode node, string parentNodeID)
        {
            StringBuilder sb = new StringBuilder();
            bool isComponent = false;
            if (node.ChildNodes != null && node.ChildNodes.Count > 0)
                isComponent = true;

            string nodeID = Guid.NewGuid().ToString();
            node.ImageToolTip = nodeID;

            sb.Append(_jsObjName + ".RegistryNode(new TreeNodePro('" + nodeID + "','" + parentNodeID + "','" + (node.Value != null ? node.Value.ReplaceSingleQuote().HtmlEncode() : "") + "','" + node.Text.ReplaceSingleQuote().HtmlEncode() + "'," + (node.Selected ? "true" : "false") + "));\n");
            if (isComponent)
            {   
                foreach (TreeNode childNode in node.ChildNodes)
                {
                    sb.Append(RegisterScriptForNode(childNode, nodeID));
                }             
            }

            return sb.ToString();
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (Page == null)
                return;

            if (this.RenderMode == RenderMode.Static)
                Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), GetRegistryStript(false), true);

            if (ShowExpandCollapse && String.IsNullOrEmpty(ExpandImageUrl))
                this.ExpandImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.TreeViewPro.Images.plus.png");

            if (ShowExpandCollapse && String.IsNullOrEmpty(CollapseImageUrl))
                this.CollapseImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.TreeViewPro.Images.minus.png");
        }

        protected override void Render(HtmlTextWriter writer)
        {
            StringBuilder sb = new StringBuilder();
            if (this.RenderMode == RenderMode.Dynamic)
                sb.Append(GetRegistryStript(true));

            foreach (TreeNode node in this.Nodes) 
            {
                sb.Append(RenderNode(node,""));                
            }
            writer.Write(sb.ToString());
        }

        private string RenderNode(TreeNode node, string parentNodeID)
        {
            StringBuilder sb = new StringBuilder();           

            string nodeID = node.ImageToolTip;
            node.ImageToolTip = "";

            string nodeClass = String.IsNullOrEmpty(this.NodeCCSClass) ? "treeviewpro_node" : this.NodeCCSClass;
            string selectNodeClass = String.IsNullOrEmpty(this.SelectNodeCCSClass) ? "treeviewpro_nodeSelect" : this.SelectNodeCCSClass;

            bool isComponent = false;
            if (node.ChildNodes != null && node.ChildNodes.Count > 0)
                isComponent = true;

            sb.Append("<div id='node_" + nodeID+"'>");

            sb.Append("<div>");
            sb.Append("<table cellpadding=\"0\" cellspacing=\"0\"><tr valign=\"top\">");

            //+ -
            bool isExpanded = true;
            if (ShowExpandCollapse && isComponent)
            {
                if (node.Expanded != null && node.Expanded == true)
                    isExpanded = true;
                else
                    isExpanded = false;

                sb.Append("<td style='width:20px;'>");
                sb.Append("<div id='plus_" + nodeID + "' style='" + (isExpanded ? "" : "display:none;") + "'><a href=\"javascript:" + _jsObjName + ".ExpandCollapseNode('" + nodeID + "');\"><img style='border-width:0px;' title='" + this.CollapseImageToolTip + "' src='" + this.CollapseImageUrl + "'/></a></div>");
                sb.Append("<div id='minus_" + nodeID + "' style='" + (isExpanded ? "display:none;" : "") + "'><a href=\"javascript:" + _jsObjName + ".ExpandCollapseNode('" + nodeID + "');\"><img style='border-width:0px;' border='0' title='" + this.ExpandImageToolTip + "' src='" + this.ExpandImageUrl + "'/></a></div>");
                sb.Append("</td>");
            }
            else            
                sb.Append("<td style='width:20px;'>&nbsp;</td>");
            

            //checkbox
            if (node.ShowCheckBox != null && node.ShowCheckBox == true)
            {
                sb.Append("<td>");
                sb.Append("<input id=\"node_state_" + nodeID + "\" " + (node.Checked ? "checked='checked'" : "") + " onclick=\"javascript:" + _jsObjName + ".CheckNode('" + nodeID + "');\" type='checkbox'/>");
                sb.Append("</td>");
            }           

            if (!String.IsNullOrEmpty(node.ImageUrl))
            {
                sb.Append("<td valign='middle'>");
                sb.Append("<div " + (node.Target ?? "") + " onclick=\"javascript:" + _jsObjName + ".SelectNode('" + nodeID + "');\"><img alt='' src='" + node.ImageUrl + "'/></div>");
                sb.Append("</td>");
            }

            sb.Append("<td>");
            sb.Append("<div " + (node.Target ?? "") + " name='node_pro'  id=\""+ nodeID + "\" class=\"" + (node.Selected ? selectNodeClass : nodeClass) + "\" onclick=\"javascript:" + _jsObjName + ".SelectNode('" + nodeID + "');\" title=\"" + HttpUtility.HtmlEncode(node.ToolTip) + "\"  style=\"padding:3px;\"><input type='hidden' id=\"action_" + nodeID + "\" value=\"" + node.NavigateUrl + "\"/> " + HttpUtility.HtmlEncode(node.Text) + "</div>");
            sb.Append("</td>");

            sb.Append("</tr></table>");
            sb.Append("</div>");

            if (isComponent)
            {
                sb.Append("<div id='childs_" + nodeID + "' style='" + (isExpanded?"":"display:none;") + " padding-left:30px;'>");
                foreach (TreeNode childNode in node.ChildNodes)
                {
                    sb.Append(RenderNode(childNode,nodeID));                    
                }
                sb.Append("</div>");
            }

            sb.Append("</div>");
            return sb.ToString();
        }
    }
}
