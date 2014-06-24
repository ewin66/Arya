<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="Arya.Portal.Report" EnableEventValidation="false"%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">
        function AutoExpand(txtimportSelected) {
            txtimportSelected.style.height = "1px";
            txtimportSelected.style.height = (25 + txtimportSelected.scrollHeight) + "px";

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="projectId" runat="server" />
    <asp:HiddenField ID="desc" runat="server" />
    <asp:HiddenField ID="fileName" runat="server" />
    <asp:Label ID="lblProject" runat="server" Text="Select a Project" Height="16px" Width="150px"></asp:Label>
    <asp:TextBox ID="txtProject" runat="server" Height="16px" Width="300px" DataTextField="ProjectName" DataValueField="ID" Text='<%# Server.UrlDecode((Request.QueryString["ProjectID"])) %>' ReadOnly="true"></asp:TextBox>
    <br />
    <asp:Label ID="lblProjectDesc" runat="server" Text="Project Description" Height="16px" Width="150px"></asp:Label>
    <asp:TextBox ID="txtProjectDesc" runat ="server" Height="16px" Width="300px" Text='<%# Server.UrlDecode((Request.QueryString["Description"])) %>' ReadOnly="true"></asp:TextBox>
    <br />
    <asp:Label ID="lblDelimiter" runat="server" Text="Field Delimiter" Height="16px" Width="150px"></asp:Label>
    <asp:TextBox ID="txtDelimiter" runat="server" Height="16px" Width="300px" ReadOnly="true"></asp:TextBox>
      <br />
    <asp:Label ID="lblFileName" runat="server" Text="Input Filename" Height="16px" Width="150px"></asp:Label>
    <asp:TextBox ID="txtinputfileName" runat="server"  Height="16px" Width="300px" ReadOnly="true" />
    <br />

    <asp:Label ID="lblSubmittedBy" runat="server" Text="Submitted By" Height="16px" Width="150px"></asp:Label>
    <asp:TextBox ID="txtSubmittedBy" runat="server"  Height="16px" Width="300px" ReadOnly="true" />
    <br />

    <asp:Label ID="lblSubmittedOn" runat="server" Text="Submitted On" Height="16px" Width="150px"></asp:Label>
    <asp:TextBox ID="txtSubmittedOn" runat="server"  Height="16px" Width="300px" ReadOnly="true" />
    <br />
     <asp:Label ID ="lblID" runat="server" Text='<%# Eval("ID")%>' Visible="false"></asp:Label>
    <br />
     <asp:Label ID="lblLog" runat="server" Text="Log File" Height="16px" Width="150px"></asp:Label>
     <asp:PlaceHolder ID="phLog" runat="server"></asp:PlaceHolder>
    <br />
    <br />
    
    <asp:GridView ID="gv_mapping" runat="server" ReadOnly="true" Height="16px" Width="460px">
    </asp:GridView>
    <br />
    <asp:Label ID="lblImportType" Text="Selected Import Types" runat="server" Height="16px" Width="150px" ></asp:Label>
    <asp:TextBox ID="txtimportSelected" runat="server" ReadOnly="true" TextMode="MultiLine" style="overflow:hidden"  Width="300px" onkeypress="AutoExpand(this)" Rows="2"></asp:TextBox>
   
</asp:Content>
