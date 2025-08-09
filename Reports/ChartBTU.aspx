<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainDoc.Master" AutoEventWireup="true" CodeBehind="ChartBTU.aspx.cs" Inherits="PTT.GQMS.USL.Web.Reports.ChartBTU" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
<meta http-equiv="X-UA-Compatitble" content="IE=edge,Chrome=1" />
<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.1/Chart.js"></script>


<script   type="text/javascript">

    $(document).ready(function () {
        var opt = {
            responsive: true,
            maintainAspectRatio: false,
            tooltips: { enabled: false },
            scales: {
                yAxes: [{
                    ticks: {
                       // min: 950,
                       // max: 1020,
                        stepSize: 2
                    },
                }],
                xAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            },
            elements: {
                line: {
                    tension: 0, // disables bezier curves
                    fill: false
                }
            },
            legend: {
                display: false,
            },
            animation: {
                duration: 1,
                onComplete: function () {
                    var chartInstance = this.chart,
                    ctx = chartInstance.ctx;
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'bottom';

                    Chart.helpers.each(this.data.datasets.forEach(function (dataset, i) {
                        var meta = chartInstance.controller.getDatasetMeta(i);
                        Chart.helpers.each(meta.data.forEach(function (bar, index) {
                            var centerPoint = bar.getCenterPoint();

                            if (dataset.data[index] != '0' && dataset.data[index] != '0.00') {
                                ctx.fillStyle = "#000000";
                                ctx.fillText(dataset.label, centerPoint.x, centerPoint.y+21);
                                ctx.fillText(dataset.data[index], centerPoint.x, centerPoint.y + 36);
                            }

                        }

                        ), this)
                    }), this);

                }

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
                        <asp:Label ID="lblCOMP" runat="server" Text="Average BTU (Btu/scf)"></asp:Label></b></h3></div>
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
