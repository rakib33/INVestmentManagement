<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="InvestmentManagement.ReportViewer" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" AsyncRendering="true" Font-Names="Verdana" Font-Size="8pt" Height="794px" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="1350px"  SizeToReportContent = "true">
         
             

        </rsweb:ReportViewer> 
   


    </div>
    </form>
</body>
</html>--%>



<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="InvestmentManagement.ReportViewer" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" AsyncRendering="true" Font-Names="Verdana" Font-Size="8pt" Height="794px" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="1350px"  SizeToReportContent = "true">
         
             

        </rsweb:ReportViewer>
   


   
   


    </div>
    </form>
</body>
</html>

