<%@ Page Title="Status - empiriSense" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Status.aspx.cs" Inherits="Arya.Portal.Status" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Job Status
    </h2>
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:LinkButton ID="lnkClearAllFilters" runat="server" OnClick="lnkClearAllFilters_Click">Clear All Filters</asp:LinkButton>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
            <asp:GridView ID="GvStatus" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" GridLines="None" AllowSorting="True" OnSorting="GvStatus_Sorting">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <EmptyDataTemplate>There are no jobs in your history (that match your filter criteria).</EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" Visible="False" />
                    <asp:BoundField DataField="ProjectID" HeaderText="ProjectID" SortExpression="ProjectID" Visible="False" />
                    <asp:TemplateField HeaderText="Project" SortExpression="ProjectName">
                        <HeaderTemplate>
                            Project<br />
                            <asp:DropDownList ID="DdlProjects" runat="server" AutoPostBack="True" AppendDataBoundItems="False" OnSelectedIndexChanged="UpdateGridView" DataTextField="Text" DataValueField="Value">
                            </asp:DropDownList>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="LblProject" runat="server" Text='<%# Bind("ProjectName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Type" SortExpression="JobType">
                        <HeaderTemplate>
                            Type<br />
                            <asp:DropDownList ID="DdlJobType" runat="server" AutoPostBack="True" AppendDataBoundItems="False" OnSelectedIndexChanged="UpdateGridView" DataTextField="Text" DataValueField="Value">
                            </asp:DropDownList>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="LblJobType" runat="server" Text='<%# Bind("Type") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:HyperLinkField DataTextField="Description" DataNavigateUrlFormatString="~/Log.aspx?ID={0}#MainContent_pnlArguments" SortExpression="Description" DataNavigateUrlFields="ID" HeaderText="Description" />
                    <asp:TemplateField HeaderText="Submitted By" SortExpression="FullName">
                        <HeaderTemplate>
                            Submitted By<br />
                            <asp:DropDownList ID="DdlSubmittedBy" runat="server" AutoPostBack="True" AppendDataBoundItems="False" OnSelectedIndexChanged="UpdateGridView" DataTextField="Text" DataValueField="Value">
                            </asp:DropDownList>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="LblSubmittedBy" runat="server" Text='<%# Bind("FullName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SubmittedOn" HeaderText="SubmittedOn" SortExpression="SubmittedOn" />
                    <asp:TemplateField HeaderText="Status" SortExpression="Status">
                        <HeaderTemplate>
                            Status<br />
                            <asp:DropDownList ID="DdlStatus" runat="server" AutoPostBack="True" AppendDataBoundItems="False" OnSelectedIndexChanged="UpdateGridView" DataTextField="Text" DataValueField="Value">
                            </asp:DropDownList>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:HyperLink ID="LnkLog" runat="server" Text='<%# Bind("FriendlyStatus") %>' NavigateUrl='<%# Eval("ID", "Log.aspx?ID={0}#MainContent_pnlLog") %>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnAbortJob" runat="server" Text="Cancel Job" CommandName="CancelJob" CommandArgument='<%# Eval("ID") %>' OnCommand="CancelCurrentJob" Visible='<%# ((String)Eval("Status"))=="New" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#13263D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#13263D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#363636" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#363636" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
