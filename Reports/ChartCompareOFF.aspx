<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainDoc.Master" AutoEventWireup="true" CodeBehind="ChartCompareOFF.aspx.cs" Inherits="PTT.GQMS.USL.Web.Reports.ChartCompareOFF" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
<meta http-equiv="X-UA-Compatitble" content="IE=edge,Chrome=1" />
<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.1/Chart.js"></script>


<script   type="text/javascript">

    $(document).ready(function () {
        var opt = {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                //yAxes: [{
                //    ticks: {
                //        reverse: true,
                //        min: -10,
                //        max: 100
                //    }
                //}]
            },
            elements: {
                line: {
                    tension: 0, // disables bezier curves
                    fill: false
                }
            },
            legend: {
                position: 'right',
                fullWidth: true
            }
        };

        $(function () {
            var Chart001Canvas = $("#001").get(0).getContext("2d");
            var Chart001Data = new Chart(Chart001Canvas,
                {
                    type: 'line',
                    data: {
                        labels: [<%=chartLabel%>],
                        datasets: [
                          <%=chartData%>
                        ]
                    }, options: opt
                });

        });


    });



</script>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box-header">
                  <h3 class="box-title"><b><asp:Label ID="lblPERIOD" runat="server" Text=""></asp:Label></b></h3>
                </div>

                <div class="box box-success box-solid">
                    <div class="box-header with-border"><h3 class="box-title"><b>
                        <asp:Label ID="lblCOMP" runat="server" Text=""></asp:Label></b></h3></div>
                    <div class="box-body">
                        <div class="chart"><canvas id = '001' style="height:400px"></canvas></div>
                    </div>
                </div>
        </div>
    </div>        
      

    </section> 



</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="FootScript" runat="server">
</asp:Content>
