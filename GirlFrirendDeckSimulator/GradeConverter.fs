namespace GirlFriendDeckSimulator
open Converter
open Grade
module GradeConverter = 
    
    let toString(grade: Grade): string =
        match grade with
        | Grade.First -> "1年生"
        | Grade.Second -> "2年生"
        | Grade.Third -> "3年生"
        | Grade.Other -> "その他"
        
    
    let fromString(gradeName: string): Grade = 
        match gradeName with
        | "1年生" -> Grade.First
        | "2年生" -> Grade.Second
        | "3年生" -> Grade.Third
        | _ -> Grade.Other
        
    
type GradeConverter() =
    inherit ConverterBase(GradeConverter.toString >> (fun s -> s :> obj) |> convert, GradeConverter.fromString >> (fun a -> a :> obj) |> convert)