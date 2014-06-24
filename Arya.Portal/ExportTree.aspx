<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="ExportTree.aspx.cs" Inherits="Natalie.Portal.ExportTree" %>
<%@ Reference Control="~/UserControls/NatalieTreeView.ascx" %>
<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxHiddenField" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxFormLayout" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxSplitter" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>
<%@ Register Src="~/UserControls/NatalieTreeView.ascx" TagPrefix="uc" TagName="NatalieTreeView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>New Export
    </h2>
    <dx:ASPxHiddenField ID="userproject" ClientInstanceName="userproject" runat="server"></dx:ASPxHiddenField>
     <dx:ASPxLabel ID="ASPxLabel1" runat="server" Text="Select a Model" Width="150px" Height="25px" Font-Bold="true" />
    <dx:ASPxComboBox ID="ddlProject" runat="server" ValueType="System.String" Width="250px" Height="25px" DropDownStyle="DropDownList" OnValidation="ddlProject_Validation" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged" AutoPostBack="true">
        <ValidationSettings RequiredField-IsRequired="true" ErrorText="Please select a model"></ValidationSettings>
    </dx:ASPxComboBox>
    
    <dx:ASPxLabel ID="ASPxLabel2" runat="server" Text="Export Description" Width="150px" Height="25px" Font-Bold="true" />
    <dx:ASPxTextBox ID="txtDescription" runat="server" Width="250px" Height="25px" ReadOnly="false" OnValidation="txtDescription_Validation">
        <ValidationSettings RequiredField-IsRequired="true" ErrorText="Please enter a description"></ValidationSettings>
    </dx:ASPxTextBox>

<%--<dx:ASPxLabel ID="ASPxLabel3" runat="server" Text="Select an Export Type" Width="150px" Height="25px" Font-Bold="true" />
    <dx:ASPxComboBox ID="ddlExportType" runat="server" ValueType="System.String" Width="250px" Height="25px" DropDownStyle="DropDownList" OnValidation="ddlExportType_Validation">
     <ValidationSettings RequiredField-IsRequired="true" ErrorText="*Required"></ValidationSettings>
    </dx:ASPxComboBox>--%>

    <dx:ASPxLabel ID="ASPxLabel4" runat="server" Text="Select a Source Type" Width="150px" Height="25px" Font-Bold="true" />
    <dx:ASPxComboBox ID="ddlSourceType" runat="server" ValueType="System.String" Width="250px" Height="25px" DropDownStyle="DropDownList" OnValidation="ddlSourceType_Validation" OnSelectedIndexChanged="ddlSourceType_SelectedIndexChanged" AutoPostBack="true">
        <ValidationSettings RequiredField-IsRequired="true" ErrorText="*Required"></ValidationSettings>
    </dx:ASPxComboBox>

    <asp:LinkButton ID="lnk_addTax" runat="server" Text="Add taxonomy nodes..." Visible="false" OnClick="lnk_addTax_Click"/>
    <asp:LinkButton ID="lnk_skuList" runat="server" Text="Update list..." Visible="false" OnClick="lnk_skuList_Click"/>
    <br />

    <asp:Panel ID="panel1" runat="server"></asp:Panel>
</asp:Content>
