<%@ Page Language="C#" AutoEventWireup="true"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">
        protected string Origin
        {
            get { return (string) Session["origin"]; }
            set { Session["origin"] = value; }
        }

        protected string Token
        {
            get { return Request["auth_token"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["origin"]))
            {
                Origin = Request["origin"];
            }
            if (!string.IsNullOrEmpty(Request["go"]))
            {
                Response.Redirect(Request["go"]);
            }
        }    
</script>
    
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function sendAuth() {
            window.parent.postMessage('<%=Token%>','<%=Origin %>');
        }
    </script>
</head>
<body onload="<%=string.IsNullOrEmpty(Token)?"":"javascript:sendAuth()" %>">
    <h1>All your base are belong to us!</h1>
</body>
</html>
