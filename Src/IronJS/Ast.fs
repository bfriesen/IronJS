﻿module IronJS.Ast

//Import
open IronJS
open IronJS.Utils
open IronJS.CSharp.Parser

open Antlr.Runtime
open Antlr.Runtime.Tree

//Errors
let private EmptyScopeChain = "Empty scope-chain"
let private NoHandlerForType = new Printf.StringFormat<string -> int -> unit>("No generator function available for %s (%i)")
  
type Local = {
  UsedWith: string Set
  UsedAs: Types.JsTypes
  ForcedType: System.Type Option
  ClosedOver: bool
}

type Scope = {
  Locals: Map<string, Local>
  Closure: string list
}

type Number =
  | Double of double
  | Integer of int64

type BinaryOp =
  | Add = 0
  | Sub = 1
  | Div = 2
  | Mul = 3
  | Mod = 4

type UnaryOp =
  | Void = 0
  | Delete = 1

type Node =
  | Symbol of string
  | String of string
  | Number of Number
  | Block of Node list
  | Local of string
  | Closure of string
  | Global of string
  | If of Node * Node * Node
  | Function of string list * Scope * Node * Node * JitCache
  | Binary of BinaryOp * Node * Node
  | Unary of UnaryOp * Node
  | Var of Node
  | Assign of Node * Node
  | Return of Node
  | Invoke of Node * Node list
  | Null

//Type Aliases
type private Scopes = Scope list
type private Generator = CommonTree -> Scopes -> Node * Scopes
type private GeneratorMap = Map<int, CommonTree -> Scopes -> Generator -> Node * Scopes>

//Constants
let internal emptyScope = { 
  Locals = Map.empty;
  Closure = [];
}

let internal emptyLocal = {
  UsedWith = Set.empty;
  UsedAs = Types.JsTypes.None;
  ForcedType = None;
  ClosedOver = false;
}

let internal globalScope = 
  [emptyScope]

//Functions
let internal ct (o:obj) =
  o :?> CommonTree

//
let private child o n =
  (ct (ct o).Children.[n])

//
let private addLocal (s:Scopes) (n:string) =
  match s with
  | [] -> failwith EmptyScopeChain
  | x::[] -> s
  | x::xs -> {x with Locals = (Map.add n emptyLocal x.Locals)} :: xs

//
let private addLocals (s:Scopes) (ns:string list) =
  match s with 
  | [] -> failwith EmptyScopeChain
  | x::[] -> s
  | x::xs ->
    let mutable locals = x.Locals
    for n in ns do 
      locals <- Map.add n emptyLocal locals
    { x with Locals = locals } :: xs

//
let private getAst scope pr =
  scope := (snd pr)
  (fst pr)

//
let private getIdentifier (s:Scopes) (n:string) =
  match s with
  | [] -> failwith EmptyScopeChain
  | x::[] -> Global(n), s
  | x::xs ->
    if x.Locals.ContainsKey(n) then
      Local(n), s
    else
      let rec findLocal (s:Scopes) =
        match s with 
        | [] -> false, [] 
        | x::xs -> 
          if x.Locals.ContainsKey(n) then
            true, s
          else
            let found, lst = findLocal xs
            found, if found then { x with Closure = n :: x.Closure } :: lst else s

      let found, scopes = findLocal s
      (if found then Closure(n) else Global(n)), scopes

//
let private makeBlock ts s p =
  let scopes = ref s
  Block([for c in ts -> getAst scopes (p (ct c) !scopes)]), !scopes

//
let private cleanString = function
  | "" -> ""
  | s  -> if s.[0] = '"' then s.Trim('"') else s.Trim('\'')

//
let private exprType = function
  | Number(Integer(_)) -> Types.JsTypes.Integer
  | Number(Double(_)) -> Types.JsTypes.Double
  | String(_) -> Types.JsTypes.String
  | _ -> Types.JsTypes.Dynamic

//
let private addTypeData (s:Scopes) a b =
  match s with
  | [] -> failwith EmptyScopeChain
  | x::[] -> s
  | x::xs ->
    match a with
    | Local(a_name) ->
      let local = x.Locals.[a_name]

      let modified = 
        match b with
        | Local(b_name) -> { local with UsedWith = Set.add b_name local.UsedWith }
        | _ -> { local with UsedAs = (exprType b) ||| local.UsedAs }

      { x with Locals = Map.add a_name modified x.Locals } :: xs
    | _ -> s

//
let private forEachChild (func:CommonTree -> 'a) (tree:CommonTree) =
  match tree.Children with
  | null -> []
  | _    -> [for child in tree.Children -> func (ct child)]

//
let private genChildren gen (tree:CommonTree) (scopes:Scopes ref) =
  forEachChild (fun child -> getAst scopes (gen child !scopes)) tree

//Default Generators
let defaultGenerators = 
  Map.ofArray [|
    // NIL
    (0, fun (t:CommonTree) (s:Scopes) (p:Generator) -> 
      makeBlock (Utils.toList<CommonTree> t.Children) s p
    );

    (ES3Parser.Identifier, fun t s p -> 
      getIdentifier s t.Text
    );

    (ES3Parser.StringLiteral, fun t s p -> 
      String(cleanString t.Text), s
    );

    (ES3Parser.DecimalLiteral, fun t s p -> 
      Number(Integer(int64 t.Text)), s
    );

    (ES3Parser.BLOCK, fun t s p -> 
      makeBlock (Utils.toList<CommonTree> t.Children) s p
    );

    (ES3Parser.ASSIGN, fun t s p -> 
      let scopes = ref s

      let c0 = getAst scopes (p (child t 0) !scopes)
      let c1 = getAst scopes (p (child t 1) !scopes)

      scopes := addTypeData !scopes c0 c1
      scopes := addTypeData !scopes c1 c0

      Assign(c0, c1), !scopes
    );

    (ES3Parser.RETURN, fun t s p -> 
      let node, scopes = p (child t 0) s
      Return(node), scopes
    );

    (ES3Parser.VAR, fun t s p -> 
      let c0 = child t 0
      let id = if c0.Type = ES3Parser.ASSIGN then child c0 0 else c0
      p c0 (addLocal s id.Text)
    );

    (ES3Parser.FUNCTION, fun t s p -> 
      if t.ChildCount = 2 then
        let paramNames = "~closure" :: "this" :: forEachChild (fun c -> c.Text) (child t 0)
        let body, scopes = p (child t 1) (addLocals (emptyScope :: s) paramNames)
        Function(paramNames, scopes.Head, Null, body, new JitCache()), scopes.Tail
      else
        failwith "No support for named functions"
    );

    (ES3Parser.CALL, fun tree s gen -> 
      let scopes = ref s
      let target = getAst scopes (gen (child tree 0) !scopes)
      let args = genChildren gen (child tree 1) scopes
      Invoke(target, args), !scopes
    );
|]

let makeGenerator (handlers:GeneratorMap) =
  let rec gen (tree:CommonTree) (scopes:Scopes) = 
    if not (handlers.ContainsKey(tree.Type)) then
      failwithf NoHandlerForType ES3Parser.tokenNames.[tree.Type] tree.Type
    handlers.[tree.Type] tree scopes gen
  gen

let generator tree = 
  let generator = makeGenerator defaultGenerators
  let body, scopes = generator (ct tree) globalScope
  let scope = { 
    scopes.Head 
    with 
      Locals = scopes.Head.Locals
        .Add("~closure", emptyLocal)
        .Add("this", emptyLocal) 
  }

  Function(["~closure"; "this"], scope, Null, body, new JitCache())