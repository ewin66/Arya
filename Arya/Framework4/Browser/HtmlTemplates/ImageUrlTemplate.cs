using Arya.UserControls;

namespace Arya.Framework4.Browser.HtmlTemplates
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Arya.Data;
    using Arya.HelperClasses;

    public class ImageUrlTemplate : Template
    {
        public override string Render(Change entityDataGridViewChange, Guid entityDataGridViewInstanceID, AssetCache assetCache, int currentColumnIndex)
        {
            AssetCache imageCache = assetCache;
            Change currentChange = entityDataGridViewChange;

            List<EntityData> entityDatas = currentChange.GetEntityDatas();
            string attributeName = entityDatas.Select(ed => ed.Attribute.AttributeName).FirstOrDefault() ??
                                   string.Empty;
          
            var currentSKuOrders = AryaTools.Instance.Forms.SkuOrders[entityDataGridViewInstanceID];

            var skusWithValues =
                entityDatas.Select(
                    ed => new { ed.Sku, ed.Value, Image = imageCache.GetAsset(ed.Sku), SkuIndex = currentSKuOrders.IndexOf(ed.Sku.ID) });

            var skusWithBlanks =
                currentChange.GetBlanks().Select(
                    sku => new { Sku = sku, Value = EntityDataGridView.EmptyValue, Image = imageCache.GetAsset(sku), SkuIndex = currentSKuOrders.IndexOf(sku.ID) });

            if (!entityDatas.Any())
            {
                skusWithBlanks =
                    skusWithBlanks.Union(
                        currentChange.GetSkus().Select(
                            sku => new { Sku = sku, Value = string.Empty, Image = imageCache.GetAsset(sku), SkuIndex = currentSKuOrders.IndexOf(sku.ID) }));
            }

            //group by image filename
            var files = skusWithValues.Union(skusWithBlanks).GroupBy(s => s.Image).OrderBy(p => p.Min(q => q.SkuIndex)).ToList();

            var html = new StringBuilder();

            html.Append(
                @"<html>
					<head>
					<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
					<style type=""text/css"">
						img.skuImage { max-width: 300px; max-height: 300px; margin-right: 10px; }
						div.inline { float:left;margin-right:20px;margin-bottom:10px; }
						.clearBoth { clear:both; }
						table { border-width: 2px; border-spacing: 3px; border-style: outset; border-color: gray; }
						td, th { border: 1px solid gray; }
						.topLine{
									width:600px;
									position:fixed;
									top:0;
									left:0;
									z-index:1;
									text-align:left;
									height: auto;
									border: 0 none;
									padding: 2px;
									background-color: #DDE5FF;
									margin-left: 8px;
									margin-top: 5px;
						}
						a img {
							text-decoration: none;
							border: 0 none;
						}
					</style>
					<script type=""text/javascript"" src='http://ajax.googleapis.com/ajax/libs/jquery/1.6.2/jquery.min.js'></script>"
                );

            html.AppendFormat(@"
					<link rel=""stylesheet"" type=""text/css"" href=""{0}\CSS\ImageAreaSelect\imgareaselect-animated.css"" />
					<link rel=""stylesheet"" type=""text/css"" href=""{0}\CSS\Colorbox\colorbox.css"" />
					<script type='text/javascript' src='http://toolsuite.empiriSense.com/Javascripts/ImageAreaSelect/jquery.imgareaselect.pack.js'></script>
					<script type='text/javascript' src='http://toolsuite.empiriSense.com/Javascripts/Colorbox/jquery.colorbox-min.js'></script>
                    <script type='text/javascript' src='http://toolsuite.empiriSense.com/Javascripts/Cookie/jquery.cookie.js'></script>
					", Environment.CurrentDirectory);

            html.Append(@"
							   <script type=""text/javascript""> 
								   var currentIASObjects;
								   var zoomEnabled=false;
								   var showAllSkuTables=false;
							   
								   $(document).ready(function () { 
										currentIASObjects = $('.skuImage').imgAreaSelect({ instance: true, handles: true, onSelectChange: preview }); 

										$('#clearAllIAS').click(function() {
											if( currentIASObjects == undefined || currentIASObjects.length == 0 ) return;
											$.each(currentIASObjects, function(index, value) { 
												value.cancelSelection();
											});
										});

                                        //bug in Windows server 2003, does not load the margin for the div
                                        $('#skuImageContainer').css('margin-top','61px');

                                        $('a.imageLink').click(function(e) {
											$.cookie('czimg',$(this).attr('id'));
                                            e.preventDefault();
                                        });

                                        $('.imageLink').ready(function() {
											var zoomed = $.cookie('enablezoom');
                                            if(zoomed != null && zoomed == '1')
                                            {
                                                $('#toggleZoom').click();
                                            }
                                            var czimg = $.cookie('czimg');
                                            if ( czimg != null && $('#'+czimg).length > 0) {
                                                $('#'+czimg).click();
                                            }
                                        });

										function skuTablesCheckAndToggle()
										{
											if($('table.skuTable:visible').length == 0)
											{
												$('#toggleSkuTables').html('Show All Skus');
											}
											else
											{
												$('#toggleSkuTables').html('Hide All Skus');
											}
										}

										$('#toggleSkuTables').click(function() {
											if(showAllSkuTables)
											{
												$('.skuTable').hide();
												showAllSkuTables=false;
												$(this).html('Show All Skus');
											}
											else
											{
												$('.skuTable').show();
												showAllSkuTables=true;
												$(this).html('Hide All Skus');
											}
										});

										$('#toggleZoom').click(function() {
											if(!zoomEnabled)
											{
												var zoomValue = $.cookie('zoomvalue');
												if(zoomValue != null) 
												{
													$('#zoomPercent').val(zoomValue);
													enableZoom(zoomValue,zoomValue);
												}
												else enableZoom('100%','100%');
												$(this).html('Disable Zoom');
											}
											else
											{
												disableZoom();
												$(this).html('Enable Zoom');
											}
										});

										$('#zoomPercent').change(function() { 
											disableZoom();
											p = $(this).val();
                                            $.cookie('zoomvalue',p);
											enableZoom(p,p);
										});

                                        $(document).bind('cbox_closed', function(){
                                            $.cookie('czimg',null);
                                        });

                                        $('#pinBrowser').click(function(){
                                            if ($(this).is(':checked'))
                                            {
                                                 if (window.external)
                                                 {
                                                    window.external.PinCurrentImages();
                                                 }
                                            }
                                            else
                                            {
                                                 if (window.external)
                                                 {
                                                    window.external.UnPinCurrentImages();
                                                 }
                                            }
                                        });
								   });

								   function enableZoom(maxWidth,maxHeight)
								   {
										if(maxWidth == '-' || maxHeight == '-')
											$(""a[rel='skuImageLink']"").colorbox({photo:true});
										else
											$(""a[rel='skuImageLink']"").colorbox({photo:true, maxWidth:maxWidth, maxHeight:maxHeight});
										$.colorbox.init();
										zoomEnabled = true;
										$('#zoomPercent').show();
                                        $.cookie('enablezoom','1');
								   }
								   
								   function disableZoom()
								   {
										$.colorbox.remove();
										zoomEnabled = false;
										$('#zoomPercent').hide();
                                        $.cookie('enablezoom',null);
								   }

								   function skuTablesCheckAndToggle()
								   {
										if($('table.skuTable:visible').length == 0)
										{
											$('#toggleSkuTables').html('Show All Skus');
											showAllSkuTables=false;
										}
										else
										{
											$('#toggleSkuTables').html('Hide All Skus');
											showAllSkuTables=true;
										}
								   }

								   function preview(img, selection) 
								   {
										if (!selection.width || !selection.height)
										return;
									
										$('#x1').val(selection.x1);
										$('#y1').val(selection.y1);
										$('#x2').val(selection.x2);
										$('#y2').val(selection.y2);
										$('#width').val(selection.width);
										$('#height').val(selection.height); 
									}

                                    function selectSku(skuID,columnIndex)
                                    {
                                        if (window.external)
                                        {
                                            window.external.SelectSku(skuID,columnIndex);
                                        }
                                    }
							  </script>
							");

            html.Append(@"</head><body>
								<div class='topLine'>
									<a href='javascript:void(0);' id='clearAllIAS'>Clear All</a> |
									<a href='javascript:void(0);' id='toggleSkuTables'>Show All Skus</a> | 
									<a href='javascript:void(0);' id='toggleZoom'>Enable Zoom</a>
									<select id='zoomPercent' style='display:none;'>
										<option value='100%'>50%</option>
										<option value='200%'>75%</option>
										<option value='-'>100%</option>
									</select> | 
                                    <input type='checkbox' id='pinBrowser'>Pin</input>
									<br/>
									<b>Width</b>
									<input type='text' value='-' id='width' style='width:30px;text-align:center;margin-right:15px;'>
									<b>Height</b>
									<input type='text' value='-' id='height' style='width:30px;text-align:center;margin-right:15px;'>
									<b>X<sub>1</sub></b>
									<input type='text' value='-' id='x1' style='width:30px;text-align:center;margin-right:15px;'>
									<b>Y<sub>1</sub></b>
									<input type='text' value='-' id='y1' style='width:30px;text-align:center;margin-right:15px;'>
									<b>X<sub>2</sub></b>
									<input type='text' value='-' id='x2' style='width:30px;text-align:center;margin-right:15px;'>
									<b>Y<sub>2</sub></b>
									<input type='text' value='-' id='y2' style='width:30px;text-align:center;margin-right:15px;'>
                                    
								</div>
								<div id='skuImageContainer' style='margin-top:60px;'>
							");
            var anyImagesToLoad = false;
            foreach (var valueGroup in files.Where(p => p.Key != null))
            {
                var tableId = Guid.NewGuid();
                html.AppendFormat(
                    @"<div class=""inline""><a id='{5}' rel='skuImageLink' class='imageLink' href='{0}'>
						<img class='skuImage' src = ""{0}"" /></a> <br />
						<a href=""javascript:void(0);"" onclick=""$('#{1}').toggle();skuTablesCheckAndToggle();"">{3} SKUs</a>
						<table id = {1} class='skuTable' style=""display:none;"">
						<tr><th>Item</th><th>{2}</th></tr>{4}",
                    valueGroup.Key.AssetLocation, tableId, attributeName, valueGroup.Count(), Environment.NewLine, valueGroup.Key.AssetName);

                //foreach (var val in valueGroup.OrderBy(v => v.Value))
                //    html.AppendFormat(
                //        "<tr><td>{0}</td><td>{1}</td></tr>{2}", val.Sku, val.Value, Environment.NewLine);

                foreach (var val in valueGroup.OrderBy(v => v.SkuIndex))
                    html.AppendFormat(
                        @"<tr><td><a href='javascript:void(0);' onclick=""selectSku('{3}','0');"">{0}</a></td><td><a href='javascript:void(0);' onclick=""selectSku('{3}','{4}');"">{1}</a></td></tr>{2}", val.Sku, val.Value, Environment.NewLine, val.Sku.ID, currentColumnIndex);

                html.Append("</table></div>" + Environment.NewLine);
                anyImagesToLoad = true;
            }

            if (!anyImagesToLoad)
                html.AppendFormat("<h2>No Image(s)</h2>");

            html.Append(@"<br class=""clearBoth"" /></div></body></html>");

            var fileName = AryaTools.Instance.ItemImagesTempFile;

            File.WriteAllText(fileName, html.ToString());

            //using (var stream = new FileStream(fileName, FileMode.Create))
            //using (TextWriter writer = new StreamWriter(stream))
            //{
            //    writer.Write(html.ToString());
            //}

            return fileName;
        }
    }
}
