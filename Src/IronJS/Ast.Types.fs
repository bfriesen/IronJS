﻿namespace IronJS.Ast

open IronJS
open IronJS.Utils
open Antlr.Runtime.Tree
open System.Diagnostics

type JsTypes = 
  | Nothing   = 0
  | Double    = 1
  | Integer   = 2 // Not used
  | String    = 4
  | Object    = 8
  | Dynamic   = 16

type ClosureAccess =
  | Nothing
  | Read
  | Write

[<DebuggerDisplay("{DebugView}")>]
type Local = {
  ClosureAccess: ClosureAccess
  ParamIndex: int
  UsedAs: JsTypes
  UsedWith: string Set
  UsedWithClosure: string Set
  InitUndefined: bool
  Expr: EtParam
} with
  member x.IsClosedOver = not (x.ClosureAccess = ClosureAccess.Nothing)
  member x.IsParameter  = x.ParamIndex > -1
  member x.DebugView = (sprintf 
    @"access:%A/index:%i/undefined:%b/as:%A/with:%A, %A" 
    x.ClosureAccess x.ParamIndex x.InitUndefined x.UsedAs x.UsedWith x.UsedWithClosure)
  static member New = {
    ClosureAccess = ClosureAccess.Nothing
    ParamIndex = -1
    UsedAs = JsTypes.Nothing
    UsedWith = Set.empty
    UsedWithClosure = Set.empty
    InitUndefined = false
    Expr = null
  }
  
[<DebuggerDisplay("{DebugView}")>]
type Closure = {
  Index: int
  IsLocalInParent: bool
} with
  member x.DebugView = sprintf "index:%i/local:%b" x.Index x.IsLocalInParent
  static member New = {
    Index = -1
    IsLocalInParent = false
  }

type LocalMap = Map<string, Local>
type ClosureMap = Map<string, Closure>

type Scope = {
  Locals: LocalMap
  Closure: ClosureMap
  Arguments: bool
  ScopeLevel: int
} with
  static member New = { 
    Locals = Map.empty
    Closure = Map.empty
    Arguments = false
    ScopeLevel = 0
  }
  static member Global = Scope.New

type ParserState = { 
  ScopeLevel: int
  ScopeChain: Scope list
} with
  member x.InDynamicScope = x.ScopeLevel > 0
  static member New = {
    ScopeLevel = 0
    ScopeChain = []
  }

type Node =
  //Error
  | Error of string

  //Constants
  | String of string
  | Number of double
  | Pass
  | Null
  | Undefined

  //Identifiers
  | Local of string
  | Closure of string
  | Global of string
  | Dynamic of string
  | Property of Node * string

  //Magic
  | Arguments
  | This

  //
  | DynamicScope of Node * Node
  | Function of Scope * Node
  
  //
  | Block of Node list
  | Invoke of Node * Node list
  | Assign of Node * Node
  | Return of Node
  | Object of Map<string, Node> option