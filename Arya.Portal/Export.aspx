<%@ Page Title="New Export - empiriSense" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Export.aspx.cs" Inherits="Arya.Portal.Export" %>

<%@ Register TagPrefix="owe" Namespace="Arya.Portal.SupportingClasses" Assembly="AryaPortal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function setSourceItemsLabel(sourceType) {
            document.getElementById("SourceTaxonomies").style.display = (sourceType == "Taxonomy") ? "inline-block" : "none";
            document.getElementById("SourceSkus").style.display = (sourceType == "Taxonomy") ? "none" : "inline-block";
        }

        function css(selector, property, value) {
            for (var i = 0; i < document.styleSheets.length; i++) { //Loop through all styles
                try { //Try add rule
                    document.styleSheets[i].insertRule(selector + ' {' + property + ':' + value + '}', document.styleSheets[i].cssRules.length);
                } catch (err) { try { document.styleSheets[i].addRule(selector, property + ':' + value); } catch (err) { } }//IE
            }
        }

        function updateHelpText(e) {
            if (e.text == "Show Help Text") {
                css('.PropertyDefinition', 'display', 'inline');
                css('.PropertyName', 'text-align', 'left');
                e.innerHTML = "Hide Help Text";
            } else {
                css('.PropertyDefinition', 'display', 'none');
                css('.PropertyName', 'text-align', 'right');
                e.innerHTML = "Show Help Text";
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>New Export
    </h2>
    <asp:HiddenField ID="UserId" runat="server" />
    <asp:Label CssClass="PropertyName" ID="lblModel" runat="server" Text="Model" />
    <asp:DropDownList CssClass="PropertyValue" ID="ddlModel" runat="server" DataTextField="Text" DataValueField="Value" AutoPostBack="true" OnSelectedIndexChanged="ddlModel_SelectedIndexChanged" />
    <asp:RequiredFieldValidator ID="rv_model" runat="server" ControlToValidate="ddlModel" Text="*Required" ErrorMessage="*Required" InitialValue="---Select a Model---" ForeColor="Red" />
    <br />
    <asp:Label CssClass="PropertyName" ID="lblExportType" runat="server" Text="Export Type" Visible="false" />
    <asp:DropDownList CssClass="PropertyValue" ID="ddlExportType" runat="server" OnSelectedIndexChanged="ddlExportType_SelectedIndexChanged" Visible="false" DataTextField="Text" DataValueField="Value" AutoPostBack="true" />
    <asp:RequiredFieldValidator ID="rv_exportType" runat="server" ControlToValidate="ddlExportType" Text="*Required" ErrorMessage="*Required" InitialValue="---Select an Export Type---" ForeColor="Red" />
    <br />
    <asp:Label ID="LblMessage" runat="server" ForeColor="Maroon"></asp:Label>
    <asp:Panel ID="pnlStandardExportParameters" Visible="False" runat="server">
        <span class="PropertyName">
            <a style="font-weight: normal" onclick="updateHelpText(this); return false;" href="#">Show Help Text</a></span>
        <br />
        <span class="PropertyName">Select Source Type
            <span class="PropertyDefinition">
                <br />
                The method used to specify which SKUs to export.
                <br />
                If TAXONOMY, the SKUs are selected by listing the taxonomy nodes in which they are classified.
                <br />
                If SKULIST, the SKUs are selected by listing the Item IDs.
            </span>
        </span>
        <asp:DropDownList CssClass="PropertyValue" ID="ddlSourceType" runat="server" />
        <br />
        <span id="SourceTaxonomies" class="PropertyName">Taxonomy Paths 
            <span class="PropertyDefinition">
                <br />
                The list of taxonomy paths (in L1>L2>L3 format) to export. The export assumes all children of the listed nodes are part of the export.
                Multiple nodes can be specified, one taxonomy path per line.
            </span>
        </span>
        <span id="SourceSkus" class="PropertyName" style="display: none">SKU Item IDs
            <span class="PropertyDefinition">
                <br />
                The list of Item IDs to export. Multiple items can be specified, one Item ID per line.
            </span>
        </span>
        <asp:TextBox CssClass="PropertyValue" ID="txtSourceData" runat="server" TextMode="MultiLine" Height="120px" Rows="5"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rv_sourceData" runat="server" ControlToValidate="txtSourceData" Text="*Required" ErrorMessage="*Required" ForeColor="Red" />
        <owe:ObjectWebEditor ID="exportArgsEditor" runat="server" ViewStateMode="Enabled" HideReadOnly="True">
        </owe:ObjectWebEditor>
    </asp:Panel>
    <asp:Panel ID="pnlCustomExportParameters" Visible="False" runat="server">
        <asp:Label ID="lblDescription" runat="server" Text=""></asp:Label><br />
        <asp:Label CssClass="PropertyName" ID="Label1" runat="server" Text="Export File Format" Height="25px" Width="150px"></asp:Label>
        <asp:DropDownList ID="ddExportType" runat="server">
            <asp:ListItem Selected="True">Text</asp:ListItem>
            <asp:ListItem>Excel</asp:ListItem>
        </asp:DropDownList><br />
        <asp:Label CssClass="PropertyName" ID="Label3" runat="server" Text="Task Description" Height="25px" Width="150px"></asp:Label>
        <asp:TextBox runat="server" CssClass="PropertyValue" ID="txtExportDescription"></asp:TextBox><br />
        <asp:Label CssClass="PropertyName" ID="Label4" runat="server" Text="Notification Email Addresses" Height="25px" Width="150px"></asp:Label>
        <asp:TextBox runat="server" CssClass="PropertyValue" ID="txtEmailAddresses"></asp:TextBox><br />
        <asp:Label CssClass="PropertyName" ID="Label2" runat="server" Text="Delimiter" Height="25px" Width="150px"></asp:Label>
        <asp:DropDownList ID="ddDelimiter" runat="server">
            <asp:ListItem Selected="True" Value="\t" Text="Tab (\t)"></asp:ListItem>
            <asp:ListItem Value="," Text="Comma (,)"></asp:ListItem>
            <asp:ListItem Value=";" Text="Semi-colon (;)"></asp:ListItem>
            <asp:ListItem Value="|" Text="Pipe (|)"></asp:ListItem>
        </asp:DropDownList>
        (for text files only)<br />
        <asp:CheckBox ID="chkEmptyFiles" runat="server" Text="Export Empty Files" Height=" 25px" />
        &nbsp; &nbsp; &nbsp; 
        <asp:CheckBox ID="chkQueriesOnly" runat="server" Text="Generate Queries Only (do not execute them)" Visible="False" Height=" 25px" />
        <br />
        <br />
        <asp:Label ID="lblGlobals" runat="server" Text="Global Parameters:" Font-Bold="True" ForeColor="DarkGray"></asp:Label><br />
        <asp:GridView ID="GrdGlobalParameters" runat="server" AutoGenerateColumns="False" CellPadding="4" BackColor="White"
            BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" GridLines="Horizontal" OnRowDataBound="Parameters_RowDataBound">
            <EmptyDataTemplate>
                No global parameters found.
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Required" HeaderText="Required" ReadOnly="True" />
                <asp:BoundField DataField="Name" HeaderText="Name" ReadOnly="True" />
                <asp:BoundField DataField="Description" HeaderText="Description" ReadOnly="True" />
                <asp:TemplateField HeaderText="Value">
                    <ItemTemplate>
                        <asp:TextBox ID="TxtValue" Text='<%# Bind("Value") %>' Width="300px" runat="server"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="White" ForeColor="#333333" />
            <HeaderStyle BackColor="#336666" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#336666" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="White" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F7F7F7" />
            <SortedAscendingHeaderStyle BackColor="#487575" />
            <SortedDescendingCellStyle BackColor="#E5E5E5" />
            <SortedDescendingHeaderStyle BackColor="#275353" />
        </asp:GridView>
        <br />
        <asp:GridView ID="RptQueries" runat="server" AutoGenerateColumns="False" OnRowDataBound="RptQueries_RowDataBound"
            BorderStyle="Double" BackColor="White" BorderColor="#336666" BorderWidth="3px" CellPadding="4" GridLines="Horizontal">
            <Columns>
                <asp:TemplateField HeaderText="Reports">
                    <HeaderTemplate>
                        <asp:LinkButton ForeColor="Gray" ID="btnClearAll" runat="server" OnClick="btnClearAll_Click">Clear</asp:LinkButton>
                        |
                        <asp:LinkButton ForeColor="Gray" ID="btnSelectAll" runat="server" OnClick="btnSelectAll_Click">Select All</asp:LinkButton>
                        | Queries/Reports:
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="ChkExecute" runat="server" /><br />
                        <asp:Label ID="LblDescription" runat="server" Text="Label"></asp:Label><br />
                        <asp:GridView ID="GrdParameters" runat="server" AutoGenerateColumns="False" CellPadding="4" BackColor="White" BorderColor="#336666"
                            BorderStyle="Double" BorderWidth="3px" GridLines="Horizontal" OnRowDataBound="Parameters_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="Required" HeaderText="Required" ReadOnly="True" />
                                <asp:BoundField DataField="Name" HeaderText="Name" ReadOnly="True" />
                                <asp:BoundField DataField="Description" HeaderText="Description" ReadOnly="True" />
                                <asp:TemplateField HeaderText="Value">
                                    <ItemTemplate>
                                        <asp:TextBox ID="TxtValue" Text='<%#Bind("Value") %>' Width="300px" runat="server"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <FooterStyle BackColor="White" ForeColor="#333333" />
                            <HeaderStyle BackColor="#336666" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#336666" ForeColor="White" HorizontalAlign="Center" />
                            <RowStyle BackColor="White" ForeColor="#333333" />
                            <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
                            <SortedAscendingCellStyle BackColor="#F7F7F7" />
                            <SortedAscendingHeaderStyle BackColor="#487575" />
                            <SortedDescendingCellStyle BackColor="#E5E5E5" />
                            <SortedDescendingHeaderStyle BackColor="#275353" />
                        </asp:GridView>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="White" ForeColor="#333333" />
            <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="DarkGray" HorizontalAlign="Left" />
            <PagerStyle BackColor="#336666" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="White" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F7F7F7" />
            <SortedAscendingHeaderStyle BackColor="#487575" />
            <SortedDescendingCellStyle BackColor="#E5E5E5" />
            <SortedDescendingHeaderStyle BackColor="#275353" />
        </asp:GridView>
    </asp:Panel>
    <br />
    <span class="PropertyName"></span>
    <span class="PropertyValue">
        <asp:Button ID="BtnSubmit" runat="server" Text="Submit" OnClick="BtnSubmit_Click" Visible="False" /></span>
</asp:Content>
