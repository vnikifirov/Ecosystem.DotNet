<%@ Page Title="Advertising agency | About" Language="C#" MasterPageFile="~/SourcePage.Master" AutoEventWireup="true" CodeBehind="AboutPage.aspx.cs" Inherits="AdvertisingAgency.Pages.AboutPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        header .link2 a {
            color: #e8491d;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section id="main">
        <div class="container">
            <article id="main-col">
                <h1 class="page-title">About Us</h1>
                <p>
                    We are a NY based strategic media planning and buying agency specializing in outdoor advertising, digital,
                    social media and inbound marketing.
                </p>
                <p>
                    First founded in 2007 by partners Ronnie Ram and Nicholas Simard, Inspiria quickly grew to be
                    a trusted advisor to agencies and clients throughout the United States. Our goal is to provide
                    customized solutions and outsourced marketing and advertising services. 
                </p>
                <p>
                    We work with industries ranging from healthcare and education to fintech and non profits
                    with annual revenues ranging from $1mm -$50mm. Each of our clients receive dedicated
                    staff and monthly reporting.
                </p>
            </article>

            <aside id="sidebar">
                <div class="dark">
                    <h3>What We Do</h3>
                    <p> Our Mission is to provide exceptional visibility and business driven results for our clients through strategic
                        media placements and inbound marketing. We are accountable to our clients and operate with full transparency
                        into our process. Our client relationships are viewed as partnerships and we work with successful business
                        professionals in varying industries. </p>
                </div>
            </aside>
        </div>
    </section>
</asp:Content>
