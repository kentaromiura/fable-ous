
open Fable.Core
open Browser.Types
open Fable.Core.JsInterop
open Browser
open System
open System.Text.RegularExpressions

// For more information see https://aka.ms/fsharp-console-apps
type HyperTemplate = Object


[<Import("render", "uhtml")>]
let render(el:HTMLElement, tpl:HyperTemplate): unit = jsNative

[<Import("html", "uhtml")>]
let html(strs: string[], [<ParamArray>] args: obj[]): HyperTemplate = jsNative
//import { html, svg } from 'uhtml';
let getJsTemplate (s: FormattableString) =
    let str = s.Format
    let mutable prevIndex = 0
    let matches = Regex.Matches(str, @"\{\d+\}")
    Array.init (matches.Count + 1) (fun i ->
        if i < matches.Count then
            let m = matches.[i]
            let idx = prevIndex
            prevIndex <- m.Index + m.Length
            str.Substring(idx, m.Index - idx)
        else
            str.Substring(prevIndex)), s.GetArguments()


let onClicked _ =
    console.log(
        "clicked"
    )
let a body = 
    let strs, args = getJsTemplate $"""
    <div>
        
        <div onclick={onClicked}>{body}</div>
    </div>
    """
    html(strs, args)

let res _ = a "Hello"

//[<EntryPoint>]
let main _ = 
    render (document.body,res)
    //console.log res
    //0
    
match Browser.Dom.document.readyState <> "loading" with
    | true -> main()
    | false -> Browser.Dom.window.addEventListener ("DOMContentLoaded", main)
