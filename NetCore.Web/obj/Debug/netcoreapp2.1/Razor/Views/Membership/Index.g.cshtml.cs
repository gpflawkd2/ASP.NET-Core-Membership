#pragma checksum "D:\GitHub\ASP.NET-Core-Membership\NetCore.Web\Views\Membership\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "754e5d0ca3786eb4625add7e9b4a723ea0af0826"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Membership_Index), @"mvc.1.0.view", @"/Views/Membership/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Membership/Index.cshtml", typeof(AspNetCore.Views_Membership_Index))]
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
#line 1 "D:\GitHub\ASP.NET-Core-Membership\NetCore.Web\Views\_ViewImports.cshtml"
using NetCore.Web;

#line default
#line hidden
#line 2 "D:\GitHub\ASP.NET-Core-Membership\NetCore.Web\Views\_ViewImports.cshtml"
using NetCore.Web.Models;

#line default
#line hidden
#line 3 "D:\GitHub\ASP.NET-Core-Membership\NetCore.Web\Views\_ViewImports.cshtml"
using NetCore.Data.ViewModels;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"754e5d0ca3786eb4625add7e9b4a723ea0af0826", @"/Views/Membership/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"59ea6c56e5655097cd1a119e9f07c967221b761d", @"/Views/_ViewImports.cshtml")]
    public class Views_Membership_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "D:\GitHub\ASP.NET-Core-Membership\NetCore.Web\Views\Membership\Index.cshtml"
  
    ViewData["Title"] = "멤버십";
    Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
            BeginContext(88, 6, true);
            WriteLiteral("\r\n<h2>");
            EndContext();
            BeginContext(95, 17, false);
#line 7 "D:\GitHub\ASP.NET-Core-Membership\NetCore.Web\Views\Membership\Index.cshtml"
Write(ViewData["Title"]);

#line default
#line hidden
            EndContext();
            BeginContext(112, 32, true);
            WriteLiteral("</h2>\r\n\r\n<div class=\"text-info\">");
            EndContext();
            BeginContext(145, 19, false);
#line 9 "D:\GitHub\ASP.NET-Core-Membership\NetCore.Web\Views\Membership\Index.cshtml"
                  Write(TempData["Message"]);

#line default
#line hidden
            EndContext();
            BeginContext(164, 6, true);
            WriteLiteral("</div>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591