<%@ Page Title="Log - empiriSense" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="Arya.Portal.Log" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        #inputFile {
            width: 442px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="pnlArguments" runat="server" Visible="False">
        <asp:Label ID="lblArgumentsProcessTime" runat="server" Text="" ForeColor="White"></asp:Label>
        <h2>Arguments</h2>
        <asp:PlaceHolder ID="phArguments" runat="server"></asp:PlaceHolder>
    </asp:Panel>
    <asp:Panel ID="pnlFiles" runat="server" Visible="False">
        <asp:Label ID="lblFileListProcessTime" runat="server" Text="" ForeColor="White"></asp:Label>
        <h2>Files:</h2>
        <asp:LinkButton ID="BtnDownloadAllFiles" runat="server" OnClick="BtnDownloadAllFiles_Click">Download all files</asp:LinkButton>
        <asp:BulletedList ID="blFiles" runat="server" DisplayMode="LinkButton" OnClick="blFiles_Click" CssClass="LongList" />
    </asp:Panel>
    <asp:Panel ID="pnlLog" runat="server" Visible="False">
        <asp:Label ID="lblLogProcessTime" runat="server" Text="" ForeColor="White"></asp:Label>
        <h2>Log</h2>
        <asp:PlaceHolder ID="phSummaries" runat="server"></asp:PlaceHolder>
    </asp:Panel>
    <asp:Panel ID="pnlBadRequest" runat="server" Visible="False">
        <a href="Status.aspx">Click here</a> to go to the Job Status page.
    </asp:Panel>
</asp:Content>


