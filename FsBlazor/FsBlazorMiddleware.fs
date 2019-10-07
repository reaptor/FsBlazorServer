﻿namespace FsBlazor

open System.IO
open System.Text.Encodings.Web
open Microsoft.AspNetCore.Mvc.Rendering
open Microsoft.AspNetCore.Mvc.ViewFeatures
open Microsoft.AspNetCore.Http
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.AspNetCore.Builder
open FSharp.Control.Tasks.V2.ContextInsensitive
open FsBlazor.Page

type FsBlazorMiddleware<'a when 'a :> Page>(next : RequestDelegate) =
    member this.InvokeAsync(context : HttpContext, html : IHtmlHelper) : Task =
        let vca = html :?> IViewContextAware
        let vc = ViewContext()
        vc.HttpContext <- context
        vca.Contextualize(vc)
        task {
            let! htmlContent = html.RenderComponentAsync<'a>(RenderMode.Static)
            context.Response.ContentType <- "text/html"
            let sw = new StreamWriter(context.Response.Body)
            htmlContent.WriteTo(sw , HtmlEncoder.Default)
            do! sw.FlushAsync()
        } :> Task

[<Extension>]
type FsBlazorMiddlewareExtensions =
    [<Extension>]
    static member UseFsBlazor<'a when 'a :> Page>(builder : IApplicationBuilder) =
        builder.UseMiddleware<FsBlazorMiddleware<'a>>()

