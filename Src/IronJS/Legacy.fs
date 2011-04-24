﻿namespace IronJS

// This file contains support classes needed in IronJS 
// that are not available on .NET versions before 4.0

#if NET2
open System.Collections
open System.Collections.Generic
#endif

#if CLR2
open System.Threading

type Func<'a, 'b, 'c, 'd, 'e, 'r> = delegate of 'a * 'b * 'c * 'd * 'e -> 'r
type Func<'a, 'b, 'c, 'd, 'e, 'f, 'r> = delegate of 'a * 'b * 'c * 'd * 'e * 'f -> 'r
#endif

#if NET2
type HashSet<'a when 'a : equality>() =
  let o = obj()
  let storage = new Dictionary<'a, obj>();

  member x.Add(item) = (x :> ICollection<'a>).Add(item)
  member x.Clear() = (x :> ICollection<'a>).Clear()
  member x.Contains(item) = (x :> ICollection<'a>).Contains(item)
  member x.CopyTo(a, b) = (x :> ICollection<'a>).CopyTo(a, b)
  member x.Count = (x :> ICollection<'a>).Count
  member x.IsReadOnly = (x :> ICollection<'a>).IsReadOnly
  member x.GetEnumerator() : IEnumerator<'a> = (x :> ICollection<'a>).GetEnumerator()

  interface ICollection<'a> with
    member x.Add(item) = storage.[item] <- o;
    member x.Clear() = storage.Clear();
    member x.Contains(item) = storage.ContainsKey(item);
    member x.CopyTo(_:'a array,_:int) : unit = failwith "Not implemented"
    member x.Count = storage.Count
    member x.IsReadOnly = false
    member x.Remove(item) = storage.Remove(item)
    member x.GetEnumerator() : IEnumerator<'a> = 
     (seq { for x in storage do yield x.Key }).GetEnumerator()

    member x.GetEnumerator() : IEnumerator = 
     (seq { for x in storage do yield x.Key }).GetEnumerator() :> IEnumerator

#endif

#if NET2
type Action = delegate of unit -> unit
//Already exists
//type Action<'a> = delegate of 'a -> unit
type Action<'a, 'b> = delegate of 'a * 'b -> unit
type Action<'a, 'b, 'c> = delegate of 'a * 'b * 'c -> unit
type Action<'a, 'b, 'c, 'd> = delegate of 'a * 'b * 'c * 'd -> unit

type Func<'r> = delegate of unit -> 'r
type Func<'a, 'r> = delegate of 'a -> 'r
type Func<'a, 'b, 'r> = delegate of 'a * 'b -> 'r
type Func<'a, 'b, 'c,  'r> = delegate of 'a * 'b * 'c -> 'r
type Func<'a, 'b, 'c, 'd, 'r> = delegate of 'a * 'b * 'c * 'd -> 'r
#endif

#if BIGINTEGER
type BigIntegerParser() =
  
  static member TryParse(s, f, i, bi:bigint byref) =
    try
      bi <- System.Numerics.BigInteger.Parse(s)
      true

    with
      | _ -> false
#endif

#if SILVERLIGHT
type SortedDictionary<'k, 'v>() =
  class

  end
#endif