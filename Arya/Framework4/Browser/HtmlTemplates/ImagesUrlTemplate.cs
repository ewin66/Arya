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

    public class ImagesUrlTemplate : Template
    {
        public override string Render(Change entityDataGridViewChange, Guid entityDataGridViewInstanceID, AssetCache assetCache ,int currentColumnIndex)
        {
            List<EntityData> entityDatas = entityDataGridViewChange.GetEntityDatas();
            string attributeName = entityDatas.Select(ed => ed.Attribute.AttributeName).FirstOrDefault() ??
                                   string.Empty;

            AssetCache imageCache = assetCache;
            Change currentChange = entityDataGridViewChange;

            var currentSKuOrders = AryaTools.Instance.Forms.SkuOrders[entityDataGridViewInstanceID];

            var skusWithValues =
               entityDatas.Select(
                   ed => new { ed.Sku, ed.Value, Images = imageCache.GetAssets(ed.Sku), SkuIndex = currentSKuOrders.IndexOf(ed.Sku.ID) });

            var skusWithBlanks =
                currentChange.GetBlanks().Select(
                    sku => new { Sku = sku, Value = EntityDataGridView.EmptyValue, Images = imageCache.GetAssets(sku), SkuIndex = currentSKuOrders.IndexOf(sku.ID) });

            if (!entityDatas.Any())
            {
                skusWithBlanks =
                    skusWithBlanks.Union(
                        currentChange.GetSkus().Select(
                            sku => new { Sku = sku, Value = string.Empty, Images = imageCache.GetAssets(sku), SkuIndex = currentSKuOrders.IndexOf(sku.ID) }));
            }

            var allSkus = skusWithValues.Union(skusWithBlanks).OrderBy(sku => sku.SkuIndex).ToList();

            var html = new StringBuilder();

            html.Append(
                @"<html>
					<head>
					<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
					<style type=""text/css"">
						img.skuImage { max-width: 300px; max-height: 300px; margin-right: 10px; }
						div.inline { float:left;margin-right:20px;margin-bottom:10px; }
						.clearBoth { clear:both; }
						
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

                        table
						{
							border: 0;
							padding: 0;
							margin: 0 0 20px 0;
							border-collapse: collapse;
						}
						th
						{
							padding: 5px; /* NOTE: th padding must be set explicitly in order to support IE */
							text-align: left;
							line-height: 2em;
							color: #FFF;
							background-color: #555;
						}

                        td, th { border: 1px solid gray; }

						body.fixed_header
						{
							padding: 60px 20px;
						}

						body.fixed_header .header
						{
							position: fixed;
							top: 0;
							background: #080808;
							color: #FFC234;
							width: 100%;
							z-index: 1000;
							margin: 0 -20px;
							padding: 0 20px;
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
                    <script type='text/javascript' src='http://toolsuite.empiriSense.com/Javascripts/StickyTableHeaders/jquery.stickytableheaders.js'></script>
					", Environment.CurrentDirectory);

            html.Append(@"
							   <script type=""text/javascript""> 
								   var currentIASObjects;
								   var zoomEnabled=false;
							   
								   $(document).ready(function () { 
										currentIASObjects = $('.skuImage').imgAreaSelect({ instance: true, handles: true, onSelectChange: preview }); 

										$('#clearAllIAS').click(function() {
											if( currentIASObjects == undefined || currentIASObjects.length == 0 ) return;
											$.each(currentIASObjects, function(index, value) { 
												value.cancelSelection();
											});
										});

                                        $(function(){
                                            $('table').stickyTableHeaders({ fixedOffset : 50 });
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
            var isAttributeSelected = !string.IsNullOrEmpty(attributeName);

            var tableHeaders =
                AryaTools.Instance.InstanceData.CurrentProject.BrowserUrls.OrderBy(p => p.Order).Select(p => p.Type).ToList();

            //create the table header
            html.AppendFormat("<table><thead><tr><th>ItemID</th>");

            if (isAttributeSelected)
                html.AppendFormat("<th>{0}</th>", attributeName);

            html.Append(tableHeaders.Select(th => string.Format("<th>{0}</th>", th)).Aggregate((a, b) => a + b));

            html.Append("</tr></thead><tbody>");

            foreach (var sku in allSkus)
            {
                html.Append("<tr>");
                html.AppendFormat(@"<td><a href='javascript:void(0);' onclick=""selectSku('{0}','0');"">{1}</a></td>", sku.Sku.ID, sku.Sku.ItemID);
                if (isAttributeSelected)
                    html.AppendFormat(@"<td style='text-align: center;'><a href='javascript:void(0);' onclick=""selectSku('{1}','{2}');"">{0}</a></td>", sku.Value, sku.Sku.ID, currentColumnIndex);

                foreach (var tableHeader in tableHeaders)
                {
                    var header = tableHeader;
                    var imageAsset = sku.Images.SingleOrDefault(p => p.AssetType == header);
                    if (imageAsset == null)
                    {
                        html.Append("<td></td>");
                    }
                    else
                    {
                        html.AppendFormat(@"<td><a id='{1}' rel='skuImageLink' class='imageLink' href='{0}'>
						                        <img class='skuImage' src = ""{0}"" /></a></td>", imageAsset.AssetLocation, imageAsset.AssetName);
                        anyImagesToLoad = true;
                    }
                }

                html.Append("</tr>");
            }

            if (!anyImagesToLoad)
                html.AppendFormat("<tr><td colspan={0}><h2>No Image(s)</td></h2>", tableHeaders.Count);

            html.Append(@"</tbody></table><br class=""clearBoth"" /></div></body></html>");

            var fileName = AryaTools.Instance.ItemImagesTempFile;

            File.WriteAllText(fileName,html.ToString());

            //using (var stream = new FileStream(fileName, FileMode.Create))
            //using (TextWriter writer = new StreamWriter(stream))
            //{
            //    writer.Write(html.ToString());
            //}

            return fileName;
        }
    }
}
