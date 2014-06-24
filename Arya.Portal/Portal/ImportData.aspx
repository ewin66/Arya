<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ImportData.aspx.cs" Inherits="Arya.Portal.Portal.ImportData" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnInputFile" />
        </Triggers>
        <ContentTemplate>
            <h2>
                Import Data
            </h2>
            <asp:Panel ID="pnlFormat" runat="server">
                <div class="inputLabel">
                    Format
                </div>
                <div class="inputValue">
                    <asp:DropDownList ID="ddFileFormat" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddFileFormat_SelectedIndexChanged">
                        <asp:ListItem Selected="true">Vertical File Format: ItemId - Attribute - Value - UoM</asp:ListItem>
                        <asp:ListItem>Horizontal File Format: ItemId in first column, Attributes as Column Headers</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="sampleImage">
                    Sample:<br />
                    <asp:Image ID="imgSampleImage" runat="server" Width="500px" />
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlFile" runat="server">
                <div class="inputLabel">
                    File
                </div>
                <div class="inputValue">
                    <asp:FileUpload ID="fuInputFile" runat="server" />
                    &nbsp;
                    <asp:Button ID="btnInputFile" runat="server" Text="Upload" OnClick="btnInputFile_Click" />
                    &nbsp;
                    <asp:Label ID="lblInputFile" runat="server" Text=""></asp:Label>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlTables" runat="server">
                <div class="inputLabel">
                    Tables
                </div>
                <div class="inputValue">
                    <asp:ListBox ID="lbTables" runat="server" AutoPostBack="True" OnSelectedIndexChanged="lbTables_SelectedIndexChanged">
                    </asp:ListBox>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlSampleData" runat="server">
                <div class="inputLabel">
                    Sample Data
                </div>
                <div class="inputValue">
                    <asp:GridView ID="gvDataSample" runat="server">
                    </asp:GridView>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlMapping" runat="server">
                <div class="inputLabel">
                    Mapping
                </div>
                <div class="inputValue">
                    <asp:GridView ID="gvMapping" runat="server" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField HeaderText="Source column" DataField="ColumnName" />
                            <asp:TemplateField HeaderText="Maps to">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddMapsTo" runat="server">
                                        <asp:ListItem></asp:ListItem>
                                        <asp:ListItem>Item Id</asp:ListItem>
                                        <asp:ListItem>Taxonomy</asp:ListItem>
                                        <asp:ListItem>Attribute</asp:ListItem>
                                        <asp:ListItem>Value</asp:ListItem>
                                        <asp:ListItem>Unit of Measure</asp:ListItem>
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlDoImport" runat="server">
                <div class="inputLabel">
                </div>
                <div class="inputValue">
                    <asp:Button ID="btnDoImport" runat="server" Text="Import Data" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
