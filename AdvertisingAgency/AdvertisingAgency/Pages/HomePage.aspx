<%@ Page Title="Advertising agency | Welcome" Language="C#" MasterPageFile="~/SourcePage.Master" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="AdvertisingAgency.Pages.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        header .link1 a {
            color: #e8491d;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <section id="showcase">
        <div class="container">
            <h1>You'll look better with us</h1>
            <p>
                Whether you want to create a buzz, reach a mass audience or direct locals
               to your front door, find out how outdoor media can help you achieve your goal.
            </p>
        </div>
    </section>

    <section id="newsletter">
        <div class="container">
            <h1>Check out our most recent Blog posts</h1>
        </div>
    </section>

    <section id="boxes">
        <div class="container">
            <div class="box">
                <asp:Image runat="server" ImageUrl="~/Images/live-stream-post.jpg" />
                <h3>Live streaming video</h3>
                <p>
                    Live stream video is the superpower of the video marketing world.
                   It offers the potential of a memorable marketing video while multiplying
                   engagement with its interactive nature. Viewers can respond to the video
                   in real-time, creating a conversation between customer and company.
                   Unlike other forms of media that are strategized, planned, and produced, live stream…
                </p>
                <div class="post-data">april 24, 2017</div>
            </div>

            <div class="box">
                <asp:Image runat="server" ImageUrl="~/Images/mobile-phone-post.jpg" />
                <h3>Accelerated Mobile Pages</h3>
                <p>
                    It’s been a year since Google introduced the Accelerated Mobile Pages (AMP)
                   project — a collaborative initiative that substantially decreases the loading
                   time for mobile web pages. The project focuses on minimizing the code for static
                   content (content that doesn’t change based on user behavior) to make pages load almost
                   instantaneously. A regular mobile page takes…
                </p>
                <div class="post-data">marh 29, 2017</div>
            </div>

            <div class="box">
                <asp:Image runat="server" ImageUrl="~/Images/marketing-strategy-post.jpg" />
                <h3>Using market segmentation</h3>
                <p>
                    The internet creates useful technologies for personalizing your marketing content.
                   This is market segmentation. The medium allows you to address your audience by name,
                   give them specific recommendations based on their interests, or send them discounts
                   that they’re more likely to respond to. These technologies allow you to utilize market
                   segmentation and develop strategies to…
                </p>
                <div class="post-data">april 12. 2017</div>
            </div>
        </div>
    </section>
</asp:Content>
