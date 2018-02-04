<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Christoc.Modules.FacebookLeads.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
	<fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblAppId" runat="server" /> 
 
            <asp:TextBox ID="txtAppId" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lblAccessToken" runat="server" />
            <asp:TextBox ID="txtAccessToken" runat="server" />
        </div>
    </fieldset>

