<%@ page language="c#" %>
<script runat="server">
void Page_Load(Object source, EventArgs e)
{
    String action = Request.QueryString["action"];
    if ( !String.IsNullOrEmpty(action) && action.Equals("logout") )
    {
        System.Web.Security.FormsAuthentication.SignOut();

        String returnUrl = Request.QueryString["returnUrl"];
        if ( !String.IsNullOrEmpty(returnUrl) )
        {
            Response.Redirect(returnUrl);
        }
    }
}
</script>
<html>
  <head>
    <title>Login to my blog</title>
  </head>
  <body>
    <form runat="server">
      <asp:LoginStatus runat="server" />

      <br /> 

      <asp:Login runat="server" />
        
    </form>
  </body>
<html>
