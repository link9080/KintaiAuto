#pragma checksum "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c8305571ea0345ceb91412c924382b289838befd"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index), @"mvc.1.0.view", @"/Views/Home/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\work\Kintai_Auto\KintaiAuto\Views\_ViewImports.cshtml"
using KintaiAuto;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\work\Kintai_Auto\KintaiAuto\Views\_ViewImports.cshtml"
using KintaiAuto.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c8305571ea0345ceb91412c924382b289838befd", @"/Views/Home/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"5a193b965e803833a2f0220f38c3aee97cf000b5", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Home_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<KintaiAuto.ViewModel.KintaiView>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
  
    ViewData["Title"] = "勤怠経費連動";


#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div class=\"text-center\">\r\n    <h1 class=\"display-4\">勤怠経費連動</h1>\r\n");
            WriteLiteral("    <div class=\"col-12\">\r\n");
#nullable restore
#line 11 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
         if (ViewData["ErrorMessage"] != null)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div class=\"alert alert-warning alert-dismissible fade show\" role=\"alert\">\r\n                ");
#nullable restore
#line 14 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
            Write(ViewData["ErrorMessage"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">\r\n                    <span aria-hidden=\"true\">&times;</span>\r\n                </button>\r\n            </div>\r\n");
#nullable restore
#line 19 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
        }
        else
        {
            

#line default
#line hidden
#nullable disable
#nullable restore
#line 22 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
             if (ViewData["Message"] != null)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <div class=\"alert alert-success alert-dismissible fade show\" role=\"alert\">\r\n                    ");
#nullable restore
#line 25 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                Write(ViewData["Message"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">\r\n                        <span aria-hidden=\"true\">&times;</span>\r\n                    </button>\r\n                </div>\r\n");
#nullable restore
#line 30 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div>\r\n");
#nullable restore
#line 32 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                 using (Html.BeginForm("serch", "Home"))
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <button type=\"submit\" class=\"btn btn-primary\">再取得</button>\r\n");
#nullable restore
#line 35 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("            </div>\r\n");
#nullable restore
#line 38 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
             using (Html.BeginForm("Update", "Home"))
            {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                <table class=""table table-sm table-responsive-sm table-striped"">
                    <thead>
                        <tr>
                            <th></th>
                            <th>日付</th>
                            <th>開始</th>
                            <th>終了</th>
                            <th>休憩開始</th>
                            <th>休憩終了</th>
                            <th>パターン</th>
                        </tr>
                    </thead>
                    <tbody>
");
#nullable restore
#line 53 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                         for (var i = 0; i < Model.Kintais.Count(); i++)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <tr>\r\n                                <td>\r\n                                    ");
#nullable restore
#line 57 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                               Write(Html.CheckBoxFor(model => model.Kintais[i].inputflg, new { @class = "chk" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </td>\r\n                                <td>\r\n                                    ");
#nullable restore
#line 60 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                                Write(Model.Kintais[i].Date.ToString("d"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    ");
#nullable restore
#line 61 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                               Write(Html.HiddenFor(model => model.Kintais[i].Date));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    ");
#nullable restore
#line 62 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                               Write(Html.HiddenFor(model => model.Kintais[i].Rakutrue));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </td>\r\n                                <td>\r\n                                    ");
#nullable restore
#line 65 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                               Write(Html.EditorFor(model => model.Kintais[i].StrTime,"",new { htmlAttributes =   new { @class = "str"} }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    ");
#nullable restore
#line 66 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                               Write(Html.HiddenFor(model => model.Kintais[i].strID, new { }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </td>\r\n                                <td>\r\n                                    ");
#nullable restore
#line 69 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                               Write(Html.EditorFor(model => model.Kintais[i].EndTime, "", new { htmlAttributes = new { @class = "str" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    ");
#nullable restore
#line 70 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                               Write(Html.HiddenFor(model => model.Kintais[i].endID, new { }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </td>\r\n                                <td>\r\n                                    ");
#nullable restore
#line 73 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                               Write(Html.EditorFor(model => model.Kintais[i].KyuStrTime, new { }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </td>\r\n                                <td>\r\n                                    ");
#nullable restore
#line 76 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                               Write(Html.EditorFor(model => model.Kintais[i].KyuEndTime, new { }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </td>\r\n                                <td>\r\n");
#nullable restore
#line 79 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                                     if (Model.Kintais[i].Rakutrue == false)
                                    {
                                        

#line default
#line hidden
#nullable disable
#nullable restore
#line 81 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                                   Write(Html.DropDownListFor(model => model.Kintais[i].RakuPtn, null, "", new { @class = "form-control", @style = "width:160px; padding: 6px 6px;", disabled = "true" }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 81 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                                                                                                                                                                                                         
                                    }
                                    else
                                    {
                                        

#line default
#line hidden
#nullable disable
#nullable restore
#line 85 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                                   Write(Html.DropDownListFor(model => model.Kintais[i].RakuPtn, null, "", new { @class = "form-control", @style = "width:160px; padding: 6px 6px;" }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 85 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                                                                                                                                                                                      
                                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                </td>\r\n                            </tr>\r\n");
#nullable restore
#line 89 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                    </tbody>\r\n                </table>\r\n                <div style=\"text-align:right;\">\r\n                    <button type=\"submit\" class=\"btn btn-success\">反映</button>\r\n                </div>\r\n");
#nullable restore
#line 95 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 95 "D:\work\Kintai_Auto\KintaiAuto\Views\Home\Index.cshtml"
             
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </div>\r\n</div>\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script>
        (function ($) {

            $("".str"").on(""change"", function () {
                var tr = $(this).closest(""tr"");
                var chk = $(tr).find("".chk"");

                if (!$(chk).prop(""checked"")) {
                    $(chk).click();
                } else {
                    if ($(this).val() == """") {
                        $(chk).click();
                    }
                }
            })
            
        })(jQuery);
    </script>
    ");
            }
            );
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<KintaiAuto.ViewModel.KintaiView> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
