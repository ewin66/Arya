<%@ Page Title="New Import - empiriSense" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Import.aspx.cs" Inherits="Arya.Portal.Import" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        New Import
    </h2>
    <asp:HiddenField ID="inputFileLocation" runat="server" />
    <asp:HiddenField ID="inputFilename" runat="server" />
    <asp:HiddenField ID="projectId" runat="server" />
    <asp:HiddenField ID="userId" runat="server" />
    <asp:HiddenField ID="userName" runat="server" />
    <asp:HiddenField ID="notificationEmail" runat="server" />
    <asp:HiddenField ID="folderGuid" runat="server" />
    <asp:HiddenField ID="importArgs" runat="server" />
    <asp:HiddenField ID="argsPath" runat="server" />

    <asp:Label ID="lblProjectID" runat="server" Text="Select a Project" CssClass="PropertyName">
    </asp:Label>
    <span class="PropertyValue">
        <asp:DropDownList ID="ddlProject" runat="server" Height="25px" Width="250px" DataTextField="ProjectName" DataValueField="ID" /></span>
    <asp:RequiredFieldValidator ID="rv_project" runat="server" ControlToValidate="ddlProject" Text="Please select a project" ErrorMessage="Please select a project" InitialValue="Select a Project" ForeColor="Red" />
    <br />

    <asp:Label ID="lblProjectDesc" runat="server" Text="Import Description" CssClass="PropertyName">
    </asp:Label>
    <span class="PropertyValue">
        <asp:TextBox ID="txtProjectDesc" runat="server" Height="16px" Width="242px" /></span>
    <asp:RequiredFieldValidator ID="rv_desc" runat="server" ControlToValidate="txtProjectDesc" Text="Please enter a description" ErrorMessage="Please enter a description" ForeColor="Red" />
    <br />

    <asp:Label ID="lblDelimiter" runat="server" Text="Field Delimiter" CssClass="PropertyName"></asp:Label>
    <span class="PropertyValue">
        <asp:DropDownList ID="ddlDelimiter" runat="server" Height="25px" Width="150px" OnSelectedIndexChanged="ddlDelimiter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
    </span>
    <br />

    <asp:Label ID="lblFileName" runat="server" Text="Input Filename" CssClass="PropertyName"></asp:Label>
    <span class="PropertyValue">
        <asp:FileUpload ID="inputFileUpload" runat="server" />
        <asp:Button ID="btnUpload" type="submit" Text="Upload" runat="server" OnClick="btnUpload_Click" Visible="true"></asp:Button>
    </span>
    <asp:Label ID="lblUploadResult" runat="server" Text=""></asp:Label>

    <asp:Panel ID="pnlImport" runat="server" GroupingText="Select from the list of Imports" Visible="false">
        <asp:Label ID="lblChkboxList" runat="server" />
        <asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="Please check at least one of the options" ForeColor="Red" OnServerValidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
        <asp:CheckBoxList ID="chkboxlist" runat="server" RepeatDirection="Vertical" OnSelectedIndexChanged="chkboxlist_SelectedIndexChanged" AutoPostBack="True"></asp:CheckBoxList>
    </asp:Panel>
   
      <asp:Label ID="lblGrid" runat="server" Visible="false"></asp:Label>
     <br />
       <asp:GridView ID="gv_Fields" runat="server" AutoGenerateColumns="False" Width="400px" OnRowDataBound="gv_Fields_RowDataBound">
        <Columns>
              <asp:BoundField DataField="MappingFields" HeaderText="Arya Field Mapping"/>
              <asp:TemplateField HeaderText="File Column Header">
                <ItemTemplate>
                    <asp:DropDownList ID="DropDownList1" runat="server" DataField="HeaderColumn" AppendDataBoundItems="true">
                        <asp:ListItem Value="0" Selected="True">_ignore_</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2"  EnableClientScript="true" runat="server" Font-Bold="true" Font-Italic="true" ForeColor="Red" ControlToValidate="DropDownList1" ErrorMessage="*Required" InitialValue="_ignore_"></asp:RequiredFieldValidator>
                </ItemTemplate>
              </asp:TemplateField>
         </Columns>
    </asp:GridView>
    
    <br />
    <asp:Label ID="lblImportType" Text="Import Type" runat="server" Font-Bold="true" Visible="false"></asp:Label>
    <br />
    <asp:CheckBoxList ID="chkBoxImportType" runat="server" RepeatDirection="Vertical" /><br />
    <asp:Button ID="btnSubmit" Text="Submit" runat="server" OnClick="btnSubmit_Click" Width="130px" Visible="false" />
</asp:Content>
