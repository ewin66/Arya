<%@ Page Title="Profile - empiriSense" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Profile.aspx.cs" Inherits="AryaPortal.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        #profile
        {
            font-size: 1em;
        }
        #profile .label
        {
            float: left;
            width: 150px;
            margin: 0 10px 5px 0;
            text-align: right;
            font-weight: bold;
            clear: left;
        }
        
        #profile .label2
        {
            width: 230px;
            text-align: right;
            font-weight: bold;
            margin: 0 10px 5px 10px;
        }
    </style>
    <script type="text/javascript">
        $('#lblSsoIdValue').ready(function () {
            if ('DoLogin' in window.external) {
                window.external.DoLogin($('#lblSsoIdValue').text(), $('#lblFullNameValue').text(), $('#lblEmailValue').text());
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        User Profile
    </h2>
    <div>
        <fieldset>
            <legend>Account Information</legend>
            <div id="profile">
                <p>
                    <label id="lblEmailAtt" class="label">
                        Email:
                    </label>
                    <label id="lblEmailValue" class="label2">
                        <%= Email %></label></p>
                <p>
                    <label id="lblFullNameAtt" class="label">
                        Full Name:
                    </label>
                    <label id="lblFullNameValue" class="label2">
                        <%= FullName %></label></p>
                <p>
                    <label id="lblSsoIdAtt" class="label">
                        ID:
                    </label>
                    <label id="lblSsoIdValue" class="label2">
                        <%= SsoId %></label></p>
            </div>
        </fieldset>
    </div>
</asp:Content>
