<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LoggerWeb._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="From:"></asp:Label><asp:TextBox ID="TextBox1"
            runat="server"></asp:TextBox><br />
        <asp:Label ID="Label2" runat="server" Text="To:"></asp:Label>
            <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
    </div>
    <div>
        <asp:Image ID="Image1" runat="server" ImageUrl="GraphHandler.ashx" />
    </div>
    </form>
</body>
</html>
