<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="WebForm" runat="server">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr><td>
                <asp:TextBox runat="server" ID="txtCommand" Width="100%" Height="200px" TextMode="MultiLine" />
            </td></tr>
            <tr><td>
                <asp:Button runat="server" ID="btExecute" Width="100px" Text="OK" />
            </td></tr>
            <tr><td>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr><td><%= TxtResult %></td></tr>
                  </table>
            </td></tr>
        </table>
    </form>
</body>
</html>
