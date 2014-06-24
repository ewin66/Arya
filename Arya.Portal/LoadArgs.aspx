<%@ Page EnableEventValidation="false" Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LoadArgs.aspx.cs" Inherits="Arya.Portal.LoadArgs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Existing Import
    </h2>
    <asp:HiddenField ID="inputFileLocation" runat="server" />
    <asp:HiddenField ID="inputFilename" runat="server" />
    <asp:HiddenField ID="projectId" runat="server" />
    <asp:HiddenField ID="userId" runat="server" />
    <asp:HiddenField ID="userName" runat="server" />
    <asp:HiddenField ID="notificationEmail" runat="server" />
    <asp:HiddenField ID="folderGuid" runat="server" />
    <asp:HiddenField ID="importArgs" runat="server" />
    <asp:HiddenField ID="des" runat="server" />
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:Label ID="lblProjectID" runat="server" Text="Select a Project" Height="25px" Width="150px"></asp:Label>
    <asp:DropDownList ID="ddlProject" runat="server" Height="25px" Width="250px" DataTextField="ProjectName" DataValueField="ID"></asp:DropDownList>
    <asp:RequiredFieldValidator ID="rv_project" runat="server" ControlToValidate="ddlProject" Text="Please select a project" ErrorMessage="Please select a project" InitialValue="Select a Project" ForeColor="Red"/>
    <br />
    <asp:Label ID="lblProjectDesc" runat="server" Text="Import Description" Height="25px" Width="150px"></asp:Label>
    <asp:TextBox ID="txtProjectDesc" runat ="server" Height="16px" Width="242px" ReadOnly="false" EnableViewState="true"></asp:TextBox>
     <asp:RequiredFieldValidator ID="rv_desc" runat="server" ControlToValidate="txtProjectDesc" Text="Please enter a description" ErrorMessage="Please enter a description" ForeColor="Red" />
    <br />

    <asp:Label ID="lblDelimiter" runat="server" Text="Field Delimiter" Height="25px" Width="153px"></asp:Label>
    <asp:DropDownList ID="ddlDelimiter" runat="server" Height="25px" Width="150px"></asp:DropDownList>

    <br />
    <asp:Label ID="lblFileName" runat="server" Text="Input Filename" Height="25px" Width="151px"></asp:Label>
    <asp:FileUpload ID="inputFileUpload" runat="server" Visible="true" />
     <asp:RequiredFieldValidator ID="rv_fileupload" runat="server" ControlToValidate="inputFileUpload" ErrorMessage="Select a file to upload" ForeColor="Red" /> 
    <asp:TextBox ID="txtinputfileName" runat="server" Visible ="false" />
    <asp:Button ID="btnUpload" type="submit" Text="Upload" runat="server" OnClick="btnUpload_Click" Visible="true"></asp:Button>
    <asp:Panel ID="frmConfirmation" Visible="False" runat="server">
    <asp:Label ID="lblUploadResult" runat="server"></asp:Label>
    </asp:Panel>
    <br />
    <asp:GridView ID="gv_Fields" runat="server" AutoGenerateColumns="False" Width="300px" >
        <Columns>
            <asp:BoundField DataField="HeaderColumn" HeaderText="File Column Header" />
            <asp:TemplateField HeaderText="Arya Field Mapping" >
                <ItemTemplate>
                    <asp:DropDownList ID="DropDownList1" runat="server" DataField="FieldMapping" DataDource='<%#("AllFields") %>' CommandArgument='<%# Container.DataItemIndex %>'>
                        <asp:ListItem Text="_ignore_" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
   <asp:Label ID="lblImportType" Text="Import Type" runat="server" Font-Bold="true"></asp:Label>
    <br />
    <asp:CheckBoxList ID="chkBoxImportType" runat="server" RepeatDirection="Vertical">
    </asp:CheckBoxList>
    <br />
    <asp:Button ID="btnSubmit" Text="Submit" runat="server" OnClick="btnSubmit_Click" Width="130px" />
  </asp:Content>
