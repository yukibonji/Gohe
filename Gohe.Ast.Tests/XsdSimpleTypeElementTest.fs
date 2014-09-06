﻿module Test.``Xsd SimpleType Element Test``

open NUnit.Framework
open FsUnit

open System.Xml
open System.Xml.Schema
open XdefUtility
open Xsd
open XsdUtility

let primitiveTypeTestCases : obj [][] = [|
  [|Xdef.Bool; "boolean"|]
  [|Xdef.String; "string"|]
  [|Xdef.Int; "integer"|]
  [|Xdef.Float; "float"|]
  [|Xdef.Decimal; "decimal"|]
|]

let fixedTypeTestCases : obj [][] = [|
  [|Xdef.FixedBool(true); "true"|]
  [|Xdef.FixedString("hello"); "hello"|]
  [|Xdef.FixedInt(100); "100"|]
  [|Xdef.FixedFloat(100.001); "100.001"|]
|]

[<TestCaseSource("primitiveTypeTestCases")>]
let ``PrimitiveTypeの要素をXsd化できる`` inputType expected = 
  let input = elm "Root" required None inputType
  
  Xsd.fromNode input |> asElm |> name |> should equal "Root"
  Xsd.fromNode input |> asElm |> typeNameOf |> should equal (XmlQualifiedName(expected, "http://www.w3.org/2001/XMLSchema"))

[<TestCaseSource("fixedTypeTestCases")>]
let ``PrimitiveType(Fixed)の要素をXsd化できる`` inputType expected = 
  let input = elm "Root" required None inputType
  
  Xsd.fromNode input |> asElm |> name |> should equal "Root"
  Xsd.fromNode input |> asElm |> fixedValue |> should equal expected

[<Test>]
let ``PrimitiveType(EnumeratedString)の要素をXsd化できる`` () = 
  let input = elm "Root" required None (Xdef.EnumeratedString ["A"; "B"])
  
  Xsd.fromNode input |> asElm |> typeOf |> enumString  |> should equal ["A"; "B"]

[<Test>]
let ``PrimitiveType(FixedLengthString)の要素をXsd化できる`` () = 
  let input = elm "Root" required None (Xdef.FixedLengthString 100)
  
  Xsd.fromNode input |> asElm |> typeOf |> fixedLengthString  |> should equal 100


let occursTestFactors = [
  required, 1, Some 1
  optional, 0, Some 1
  many , 0, None 
  requiredMany, 1, None
  specific 0 100, 0, Some 100
]

let occursTestCases : obj [][] = [|
  for (occursInput, minOccursExpected, maxOccursExpected) in occursTestFactors do
    yield [| occursInput; minOccursExpected; maxOccursExpected |]
|]

[<TestCaseSource("occursTestCases")>]
let ``PrimitiveTypeの要素(出現回数指定)をXsd化できる`` occursInput (minOccursExpected : int) (maxOccursExpected : int option) = 
  let input = elm "Root" occursInput None Xdef.String
  
  Xsd.fromNode input |> asElm |> minOccurs |> should equal minOccursExpected
  Xsd.fromNode input |> asElm |> maxOccurs |> should equal maxOccursExpected