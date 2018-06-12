<%@ Page Title="Advertising agency | Services" Language="C#" MasterPageFile="~/SourcePage.Master" AutoEventWireup="true" CodeBehind="ServicesPage.aspx.cs" Inherits="AdvertisingAgency.Pages.ServicesPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        header .link3 a {
            color: #e8491d;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section id="main">
        <div class="container">
            <article id="main-col">
                <h1 class="page-title">Services</h1>
                <ul id="services">
                    <li>
                        <h3>Bus advertising</h3>
                        <p>Bus is capable of delivering a range of marketing communication objectives.
                           So whether you’re driving brand awareness, promoting an offer, launching a new product or looking for a direct response, or indeed all of these things, Bus advertising can communicate your message to a large and valuable audience, quickly and affordably.</p>
                        <p>Pricing: $1,000 - $3,000</p>
                    </li>
                    <li>
                        <h3>Graffiti Advertising</h3>
                        <p>Graffiti Advertising cuts through the clutter of me too billboard advertising with
                           a bespoke piece of stunning art that's also cool</p>
                        <p>Pricing: $2,500 - $4,000</p>
                    </li>
                    <li>
                        <h3>Billboard advertising</h3>
                        <p>If you have never considered buying ad space on a billboard, you could be missing out on some significant benefits for your business. Billboards have been around for many years and have been proven to be a very effective way to get your business’s
                           message in front of lots of people. They are also known to be effective at driving sales or other actions by your potential customers.</p>
                        <p>Pricing: $3,000 - $5,200</p>
                    </li>
                </ul>
            </article>

            <aside id="sidebar">
                <div class="dark">
                    <form class="contact-us">
                        <h3>How Can We Help You?</h3>
                        <div>
                            <label>Name</label><br>
                            <input type="text" placeholder="Name">
                        </div>
                        <div>
                            <label>Email</label><br>
                            <input type="text" placeholder="Email Address">
                        </div>
                        <div>
                            <label>Message</label><br>
                            <textarea placeholder="Message"></textarea>
                        </div>
                        <button class="button_1">Send</button>
                    </form>
                </div>
            </aside>
        </div>
    </section>
</asp:Content>
