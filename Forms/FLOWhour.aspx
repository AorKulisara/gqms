<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainDoc.Master" AutoEventWireup="true" CodeBehind="FLOWhour.aspx.cs" Inherits="PTT.GQMS.USL.Web.Forms.FLOWhour" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<input type="hidden" id="ServerAction" name="ServerAction" />
<asp:HiddenField ID="hidFLOW_NAME" runat="server" />
<asp:HiddenField ID="hidDATE" runat="server" />

    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">
                <div class='box box-success'>
                    <div class="box-header">
                       <h3 class="box-title"><i class="fa fa-tachometer"></i> FLOW hours
                           <asp:Label ID="lblNAME" runat="server" Text=""></asp:Label>
                       </h3>
                    </div>
                    <div class="box-body">

<!------------------------------------------>
     <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="RDATE" GridLines="Both"  
         OnRowDataBound="gvResult_RowDataBound"  ShowFooter="True">
                            <HeaderStyle CssClass="Table-head-gray" HorizontalAlign="Center" />
                            <FooterStyle CssClass="ItemFooter_green" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" />
                            <PagerStyle CssClass="pagination-ys cell-borderW1" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>

                                <asp:BoundField HeaderText="TIME" DataField="RDATE" DataFormatString="{0:dd/MM/yyyy HH:mm}" >
                                    <HeaderStyle CssClass="cell-center" Width="130px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="130px"/>
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                </asp:BoundField>
                                
                                <asp:BoundField HeaderText="Flow" DataField="VALUE"  DataFormatString="{0:#,##0.000}" >
                                    <HeaderStyle CssClass="Table-head-success cell-center" Width="140px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="140px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                              
                                 

                            </Columns>
                        </asp:GridView>

<!------------------------------------------>
                    </div>



                    
                </div>



            </div>
            <!-- /.col -->
        </div>
        <!-- /.row -->

    </section>
    <!-- /.content -->


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootScript" runat="server">

          <!--#include file="../Includes/JSfooter.html" -->

</asp:Content>
