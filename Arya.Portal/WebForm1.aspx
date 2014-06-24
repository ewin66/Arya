<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Natalie.Portal.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <asp:HiddenField ID="modelName" runat="server" />
    <asp:HiddenField ID="desc" runat="server" />
    <asp:HiddenField ID="fileName" runat="server" />
    <asp:GridView ID="gv_status" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" >
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:BoundField DataField="Model" HeaderText="Model" SortExpression="Model" />
            <asp:HyperLinkField DataTextField="Description" DataNavigateUrlFormatString="~/Import.aspx?Model={0}&Description={1}&FileName={2}" SortExpression="Description" DataNavigateUrlFields ="Model, Description, FileName" HeaderText="Description"/>
            <asp:BoundField DataField="SubmittedBy" HeaderText="SubmittedBy" SortExpression="SubmittedBy" />
            <asp:BoundField DataField="SubmittedOn" HeaderText="SubmittedOn" SortExpression="SubmittedOn" />
            <asp:BoundField DataField="FileName" HeaderText="FileName" SortExpression="FileName" />
            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
            <asp:TemplateField ShowHeader="True" HeaderText="Download">
             <ItemTemplate>
             <asp:ImageButton runat="server" ID="imgDownload" ImageUrl="~\images\downloadfolder.jpg" Width ="40px" Height ="40px" OnClick="imgDownload_Click" />
             </ItemTemplate>
           </asp:TemplateField>
             <asp:TemplateField ShowHeader="True" HeaderText="Log">
             <ItemTemplate>
             <asp:ImageButton runat="server" ID="imgLog" ImageUrl="~\images\viewlog.jpg" Width ="40px" Height ="40px" OnClick="imgLog_Click" />
             </ItemTemplate>
           </asp:TemplateField>
        </Columns>
        <FooterStyle BackColor="#CCCC99" />
        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
        <RowStyle BackColor="#F7F7DE" />
        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#FBFBF2" />
        <SortedAscendingHeaderStyle BackColor="#848384" />
        <SortedDescendingCellStyle BackColor="#EAEAD3" />
        <SortedDescendingHeaderStyle BackColor="#575357" />
    </asp:GridView>
    <br />
    
    </asp:Content>

