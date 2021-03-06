﻿module FsPad //  ignore-cat

let private header =
    """
<!DOCTYPE HTML>
<html>
    <head>
    <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
	<meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="Generator" content="LINQ to XML, baby!" />
    <style type='text/css'>
body {
	margin: 0.3em 0.3em 0.4em 0.4em;
	font-family: Verdana;
	font-size: 80%;
	background: white
}

p, pre {
	margin:0;
	padding:0;
	font-family: Verdana;
}

table {
	border-collapse: collapse;
	border: 2px solid #17b;
	margin: 0.3em 0.2em;
}

table.limit {
	border-bottom-color: #c31;
}

table.expandable {
	border-bottom-style: dashed;
}

table.error {
	border-bottom-width: 4px;
}

table td.some
{
   border: 2px solid LightGray;
}      
      
table td.none
{
   background-image: linear-gradient(to bottom right,  transparent calc(50% - 1px), lightgray, transparent calc(50% + 1px)); 
}

td, th {
	vertical-align: top;
	border: 1px solid #aaa;
	padding: 0.1em 0.2em;
	margin: 0;
}

th {
	text-align: left;
	background-color: #ddd;
	border: 1px solid #777;
	font-family: tahoma;
	font-size:90%;
	font-weight: bold;
}

th.member {
	padding: 0.1em 0.2em 0.1em 0.2em;
}

td.typeheader {
	font-family: tahoma;
	font-size: 100%;
	font-weight: bold;
	background-color: #17b;
	color: white;
	padding: 0 0.2em 0.15em 0.1em;
}

td.n { text-align: right }

a:link.typeheader, a:visited.typeheader, a:link.extenser, a:visited.extenser, a:link.fixedextenser, a:visited.fixedextenser {
	font-family: tahoma;
	font-size: 90%;
	font-weight: bold;
	text-decoration: none;
	background-color: #17b;
	color: white;
	float:left;
}

a:link.extenser, a:visited.extenser, a:link.fixedextenser, a:visited.fixedextenser {
	float:right; 
	padding-left:2pt;
	margin-left:4pt
}

span.typeglyph, span.typeglyphx {
	padding: 0 0.2em 0 0;
	margin: 0;
}

span.extenser, span.extenserx, span.fixedextenser {	
	margin-top:1.2pt;
}

span.typeglyph, span.extenser, span.fixedextenser {
	font-family: webdings;
}

span.fixedextenser {
	display:none;
	position:fixed;
	right:6px;
}

td.typeheader:hover .fixedextenser {
	display:block
}

span.typeglyphx, span.extenserx {
	font-family: arial;
	font-weight: bold;
	margin: 2px;
}

table.group {
	border: none;
	margin: 0;
}

td.group {
	border: none;
	padding: 0 0.1em;
}

div.spacer { margin: 0.6em 0; }

table.headingpresenter {
	border: none;
	border-left: 3px dotted #1a5;
	margin: 1em 0em 1.2em 0.15em;
}

th.headingpresenter {
	font-family: Arial;
	border: none;
	padding: 0 0 0.2em 0.5em;
	background-color: white;
	color: green;
	font-size: 110%;        
}

td.headingpresenter {
	border: none;
	padding: 0 0 0 0.6em;
}

td.summary { 
	background-color: #def;
	color: #024;
	font-family: Tahoma;
	padding: 0 0.1em 0.1em 0.1em;
}

td.columntotal {
	font-family: Tahoma;
	background-color: #eee;
	font-weight: bold;
	color: #17b;
	font-size:90%;
	text-align:right;
}

span.graphbar {
	background: #17b;
	color: #17b;
	margin-left: -2px;
	margin-right: -2px;
}

a:link.graphcolumn, a:visited.graphcolumn {
	color: #17b;
	text-decoration: none;
	font-weight: bold;
	font-family: Arial;
	font-size: 110%;
	letter-spacing: -0.2em;	
	margin-left: 0.3em;
	margin-right: 0.1em;
}

a:link.collection, a:visited.collection { color:green }

a:link.reference, a:visited.reference { color:blue }

i { color: green }

em { color:red; }

span.highlight { background: #ff8 }

code { font-family: Consolas }

code.xml b { color:blue; font-weight:normal }
code.xml i { color:maroon; font-weight:normal; font-style:normal }
code.xml em { color:red; font-weight:normal; font-style:normal }
    </style>

    <script language='JavaScript' type='text/javascript'>


	    function toggle(id)
        {
        table = document.getElementById(id);
        if (table == null) return false;
        updown = document.getElementById(id + 'ud');
        if (updown == null) return false;
        if (updown.innerHTML == '5' || updown.innerHTML == '6') {
            expand = updown.innerHTML == '6';
            updown.innerHTML = expand ? '5' : '6';
        } else {
            expand = updown.innerHTML == '˅';
            updown.innerHTML = expand ? '˄' : '˅';
        }
        table.style.borderBottomStyle = expand ? 'solid' : 'dashed';
        elements = table.rows;
        if (elements.length == 0 || elements.length == 1) return false;
        for (i = 1; i != elements.length; i++)
            if (elements[i].id.substring(0,3) != 'sum')
            elements[i].style.display = expand ? 'table-row' : 'none';
        return false;
        }
    
    </script>
    </head>
<body><div class="spacer">
"""

let footer = """</div></body>
</html>"""


////////////////////////////////////////////////////
// END Linqpad style
////////////////////////////////////////////////////


open System
open TypeShape  // ignore-cat

type PrettyPrint =
    | List  of list<PrettyPrint>
    | Optn  of option<PrettyPrint>
    | Table of list<Field>
    | Value of string * string
    | MaxRecurse
and Field = {name : string; value : PrettyPrint}

type PrettyPrinter<'T> = int -> 'T -> PrettyPrint

// Generic value to PrettyPrint

let rec mkPrinter<'T> () : PrettyPrinter<'T> =
    let ctx = new RecTypeManager()
    mkPrinterCached<'T> ctx

and private mkPrinterCached<'T> (ctx : RecTypeManager) : PrettyPrinter<'T> =
    match ctx.TryFind<PrettyPrinter<'T>> () with
    | Some p -> p
    | None ->
        let _ = ctx.CreateUninitialized<PrettyPrinter<'T>>(fun c t -> c.Value t)
        let p = mkPrinterAux<'T> ctx
        ctx.Complete p

and private mkPrinterAux<'T> (ctx : RecTypeManager) : PrettyPrinter<'T> =
    let wrap(p : PrettyPrinter<'a>) = unbox<PrettyPrinter<'T>> p
    let wrapNested(p : PrettyPrinter<'a>) = unbox<PrettyPrinter<'T>>(fun level x -> if level <= 0 then MaxRecurse else p level x)

    let mkFieldPrinter (field : IShapeMember<'DeclaringType>) =
        field.Accept {
            new IMemberVisitor<'DeclaringType, string * (PrettyPrinter<'DeclaringType>)> with
                member __.Visit(field : ShapeMember<'DeclaringType, 'Field>) =
                    let fp = mkPrinterCached<'Field> ctx
                    field.Label, (fun l x -> x |> field.Project |> fp l)
        }

    match shapeof<'T> with
    | Shape.Unit     -> wrap (fun _ _ -> Value ("Unit", "()"))
    | Shape.Bool     -> wrap (fun _ v -> Value ("Boolean", sprintf "%b" v))
    | Shape.Byte     -> wrap (fun _ (v:byte) -> Value ("Byte", sprintf "%duy" v))
    | Shape.Int32    -> wrap (fun _ v -> Value ("Int"  , string<int> v))
    | Shape.Int64    -> wrap (fun _ v -> Value ("Int64", string<int64> v))
    | Shape.Double   -> wrap (fun _ v -> Value ("Float", string<float> v))
    | Shape.String   -> wrap (fun _ v -> Value ("String", v))
    | Shape.DateTime -> wrap (fun _ (b:DateTime) -> Value ("DateTime", sprintf "(%i, %i, %i, %i, %i, %i, %i)" b.Year b.Month b.Day b.Hour b.Minute b.Second b.Millisecond))

    | Shape.FSharpOption s ->
        s.Accept { new IFSharpOptionVisitor<PrettyPrinter<'T>> with
                    member __.Visit<'a> () = 
                        let tp = mkPrinterCached<'a> ctx 
                        wrapNested (fun level x -> x |> Option.map (tp (level - 1)) |> Optn) }

    | Shape.FSharpList s ->
        s.Accept { new IFSharpListVisitor<PrettyPrinter<'T>> with
                    member __.Visit<'a> () = 
                        let tp = mkPrinterCached<'a> ctx 
                        wrapNested (fun level x -> x |> List.map (tp (level - 1)) |> List) }

    | Shape.Array s when s.Rank = 1 ->
        s.Accept { new IArrayVisitor<PrettyPrinter<'T>> with 
                    member __.Visit<'a> _  = 
                        let tp = mkPrinterCached<'a> ctx 
                        wrapNested (fun level x -> x |> Array.map (tp (level - 1)) |> Array.toList |> List) }

    | Shape.FSharpSet s ->
        s.Accept { new IFSharpSetVisitor<PrettyPrinter<'T>> with 
                    member __.Visit<'a when 'a : comparison> () = 
                        let tp = mkPrinterCached<'a> ctx 
                        wrapNested (fun level (s : Set<'a>) -> s |> Seq.map (tp (level - 1)) |> Seq.toList |> List) }

    | Shape.Tuple (:? ShapeTuple<'T> as shape) ->
        let elemPrinters = shape.Elements |> Array.map mkFieldPrinter
        wrapNested(fun level (t:'T) -> 
            elemPrinters |> Seq.map (fun (n, ep) -> {name = n.Replace("Item", "#"); value = ep (level - 1) t}) |> Seq.toList |> Table)

    | Shape.FSharpRecord (:? ShapeFSharpRecord<'T> as shape) ->
        let fieldPrinters = shape.Fields |> Array.map mkFieldPrinter
        wrapNested(fun level (r:'T) -> 
            fieldPrinters |> Seq.map (fun (name, ep) -> {name = name; value = ep (level - 1) r} ) |> Seq.toList |> Table)


    //| Shape.FSharpUnion (:? ShapeFSharpUnion<'T> as shape) -> TODO

    | Shape.Poco (:? ShapePoco<'T> as shape) ->
        let propPrinters = shape.Properties |> Array.map mkFieldPrinter
        wrapNested(
            fun level (r:'T) ->
                propPrinters
                |> Seq.map (fun (name, ep) ->
                    let value = ep (level - 1) r
                    {name = name; value = value}  ) |> Seq.toList |> Table
        )

    | _ -> failwithf "unsupported type '%O'" typeof<'T>



//---------------------------------------

let pprint (level) (x:'t) = let p = mkPrinter<'t>() in p level x

//---------------------------------------

// HTML Generation

let htmlEncode s = System.Net.WebUtility.HtmlEncode(s)


let render x =
    let rec render forceTabular td x =
        match x with        
        | MaxRecurse -> "..."
        | Value (ty, vl) -> 
            if not td then htmlEncode vl
            else  "<td>" + htmlEncode vl + "</td>"
            
        | Optn lst ->
            if not td then
                match lst with
                | Some x -> render false false x
                | None   -> render false false (Value ("", "-"))
            else
                match lst with
                | Some x -> "<td style=some>" + render false false (Optn lst) + "</td>"
                | None   -> "<td class=none>" + render false false (Optn lst) + "</td>"

        | Table fields ->
            
            seq {
                
                if forceTabular then

                    for f in fields do
                        yield render false true f.value
                else
                    if td then yield "<td>"
                    yield "<table>"                            
                    for f in fields do
                        yield "<tr>"
                        yield "<th>" + htmlEncode f.name + "</th>"
                        yield render false true f.value
                        yield "</tr>"
                    yield "</table>" 
                    if td then yield "</td>"
                } |> String.concat "  "
        | List lst -> 
            let fields =
                lst |> List.fold (fun (s:Set<string>) x -> 
                        match x with
                        | Table x ->
                            let c = List.map (fun {name = n} -> n) x
                            set c + s
                        | _ -> s
                ) Set.empty

            let body =
                    seq {
                        yield "<table>"
                        yield "<tr>"
                        for j in fields do
                            yield "<th>" + htmlEncode j + "</th>"
                        yield "</tr>"

                        for e in lst do
                            yield "<tr>"
                            yield render true true e
                            yield "</tr>"
                        yield "</table>" 
                        } |> String.concat "  "
            if not td then body
            else "<td>" + body + "</td>"

    header + render false false x + footer

type Printer() = 
    static member Print (value, maxLevel) = value |> pprint maxLevel |> render
    static member Print (value) = value |> pprint 2 |> render