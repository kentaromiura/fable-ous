#!/usr/bin/env -S dotnet fsi
// Original from https://gist.github.com/DanThiffault/f8833eac6a08ea2c9595
// Slightly modified to work and handle UTF-8.

// Hint: To add mimetypes just change the guessMimeType function.
// For a better server I suggest `npx serve`

open System
open System.Net
open System.Text
open System.IO

 
// Modified from http://sergeytihon.wordpress.com/2013/05/18/three-easy-ways-to-create-simple-web-server-with-f/
// download this file to your root directory and name as ws.fsx
// run with `fsi --load:ws.fsx`
// visit http://localhost:8080
 
let siteRoot = @"."
let defaultFile = "index.html"
let host = "http://localhost:8080/"
 
let listener (handler:(HttpListenerRequest->HttpListenerResponse->Async<unit>)) =
    let hl = new HttpListener()
    hl.Prefixes.Add host
    hl.Start()
    let task = Async.FromBeginEnd(hl.BeginGetContext, hl.EndGetContext)
    async {
        while true do
            let! context = task
            Async.Start(handler context.Request context.Response)
    } |> Async.Start
 
let getFileNameWithDefault (req:HttpListenerRequest) = 
    let relPath = Uri(host).MakeRelativeUri(req.Url).OriginalString
    if (String.IsNullOrEmpty(relPath))
    then Path.Combine(siteRoot, defaultFile)
    else Path.Combine(siteRoot, relPath)

let guessMimeType (fileName: string) =
    let ext = Path.GetExtension(fileName)
    
    match ext with
    | ".html" -> "text/html"
    | ".js" -> "text/javascript"
    | _ -> "text/plain"

listener (fun req resp ->
    async {
        let fileName = getFileNameWithDefault req
        if (File.Exists fileName)
            then
                let output = File.ReadAllText(fileName)
                let txt = Encoding.UTF8.GetBytes(output)
                resp.ContentType <-  guessMimeType(fileName) //System.Web.MimeMapping.GetMimeMapping(fileName)         
                resp.OutputStream.Write(txt, 0, txt.Length)
                resp.OutputStream.Close()
            else
                resp.StatusCode <- 404
                let output ="File not found"
                let txt = Encoding.UTF8.GetBytes(output)
                resp.ContentType <- "text/plain"
                resp.OutputStream.Write(txt, 0, txt.Length)
                resp.OutputStream.Close()
    });
Console.WriteLine("served page on http://localhost:8080 press Enter to exit")
Console.ReadKey() |> ignore
