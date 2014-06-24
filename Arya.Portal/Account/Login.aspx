<%@ Page Title="Log In" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="AryaPortal.Account.Login" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <!-- Simple OpenID Selector -->
    <link type="text/css" rel="stylesheet" href="../Styles/openid.css" />
    <script type="text/javascript" src="../js/jquery-1.2.6.min.js"></script>
    <script type="text/javascript" src="../js/openid-jquery.js"></script>
    <script type="text/javascript" src="../js/openid-en.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            openid.init('openid_identifier');
        });
    </script>
    <!-- /Simple OpenID Selector -->
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Log In
    </h2>
    <p>
        Redirecting to Google...</p>
    <div id="openid_choice">
        <div id="openid_btns">
        </div>
    </div>
    <div id="openid_input_area">
        <input id="openid_identifier" name="openid_identifier" type="text" value="http://" />
        <input id="openid_submit" type="submit" value="Sign-In" />
    </div>
    <br />
    <asp:Label ID="loginFailedLabel" runat="server" EnableViewState="False" Text="Login failed"
        Visible="False" />
    <asp:Label ID="loginCanceledLabel" runat="server" EnableViewState="False" Text="Login canceled"
        Visible="False" />
    <noscript>
        <p>
            Please enable scripts to login into your Google Account.</p>
    </noscript>
</asp:Content>
