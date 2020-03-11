<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default2.aspx.cs" Inherits="dayaxe.com.Default2" %>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
    <meta charset="UTF-8">
    <%--<meta http-equiv="refresh" content="1;url=http://test.dayaxe.com">--%>
    <script type="text/javascript">
        //window.location.href = "http://test.dayaxe.com"
    </script>
    <title>Dayaxe</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Repeater runat="server" ID="DistanceRpt">
            <HeaderTemplate>
                <table>
                    <tr>
                        <td>
                            BookingId
                        </td>
                        <td>
                            Email
                        </td>
                        <td>
                            User zipcode
                        </td>
                        <td>
                            Hotel zipcode
                        </td>
                        <td>
                            Distance (miles)
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="row">
                    <td>
                        <%#Eval("BookingId") %>
                    </td>
                    <td>
                        <%#Eval("EmailAddress") %>
                    </td>
                    <td>
                        <%#Eval("UserZipcode") %>
                    </td>
                    <td>
                        <%#Eval("HotelZipcode") %>
                    </td>
                    <td>
                        <%#Eval("Distance") %>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </form>
</body>
</html>