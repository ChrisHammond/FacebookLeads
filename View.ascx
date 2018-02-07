<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Christoc.Modules.FacebookLeads.View" %>
<asp:Label ID="lblModuleInfo" runat="server"></asp:Label>

<div id="NoAppInfo" runat="server">
    <asp:Label ID="lblNoAppInfo" resourcekey="lblNoAppInfo" runat="server" />
</div>

<div id="AppInfo" runat="Server">
    <div id="fb-root"></div>
    <script>

        // Load the SDK Asynchronously
        (function (d) {
            var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
            if (d.getElementById(id)) { return; }
            js = d.createElement('script'); js.id = id; js.async = true;
            js.src = "//connect.facebook.net/en_US/all.js";
            ref.parentNode.insertBefore(js, ref);
        }(document));

        window.fbAsyncInit = function () {
            FB.init({
                appId: '<%=AppId%>',//TODO: if you don't have the APP ID configured this won't work.
                status: true, // check login status
                cookie: true, // enable cookies to allow the server to access the session
                xfbml: true  // parse XFBML
            });
            FB.Event.subscribe('auth.logout', function (response) {
                return false;
            });
        };

        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) { return; }
            js = d.createElement(s); js.id = id;
            js.src = "https://connect.facebook.net/en_US/sdk.js";
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));

        function subscribeApp(page_id, page_access_token) {
            
            $("#hfAuthToken").val(page_access_token);
            //alert(hfAuthToken.value);

            console.log('Subscribing page to app! ' + page_id);
            FB.api(
                '/' + page_id + '/subscribed_apps',
                'post',
                { access_token: page_access_token },
                function (response) {

                    console.log('Successfully subscribed page', response);
                });
        }

        // Only works after `FB.init` is called
        function myFacebookLogin() {

            FB.login(function (response) {

                console.log('Successfully logged in', response);
                FB.api('/me/accounts', function (response) {
                    console.log('Successfully retrieved pages', response);

                    var pages = response.data;
                    var ul = document.getElementById('list');
                    for (var i = 0, len = pages.length; i < len; i++) {
                        var page = pages[i];
                        var li = document.createElement('li');
                        var a = document.createElement('a');
                        a.href = "#";
                        a.onclick = subscribeApp.bind(this, page.id, page.access_token);
                        a.innerHTML = page.name;
                        li.appendChild(a);
                        ul.appendChild(li);
                    }
                });
            }, { scope: 'manage_pages' });
        }
    </script>
    <asp:Label ID="lblStep1" resourcekey="lblStep1" runat="server" />

    <p>
        <fb:login-button length="long" autologoutlink="false" onlogin="myFacebookLogin();"></fb:login-button>

        <asp:HiddenField ID="hfAuthToken" runat="server" ClientIDMode="Static" />
    </p>
    <ul id="list"></ul>
    <p>
        <asp:Label ID="lblStep2" resourcekey="lblStep2" runat="server" />
    </p>
    <asp:LinkButton ID="lbAuth" runat="server" Text="Click to Update" OnClick="lbAuth_Click" CssClass="btn primary" />

</div>

<div id="AlreadySetup" runat="server" >

    <asp:Label ID="lblAlreadySetup" runat="server" resourcekey="lblAlreadySetup" />

</div>
