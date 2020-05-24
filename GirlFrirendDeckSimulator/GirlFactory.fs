namespace GirlFriendDeckSimulator
open FSharp.Data
open SelectionBonus
open Girl
open System
open System.IO
open System.Text.RegularExpressions

module GirlFactory =
    
    type GirlInfo = JsonProvider<"./GirlInfo.json">
    
    let girlFactroyFromJson: seq<Girl> = 
        let girlInfo = GirlInfo.Parse(File.ReadAllText("..\..\GirlInfo.json"))
        seq {
            for girl in girlInfo do 
            {   name = girl.Name
                club = ClubTypeConverter.fromString(girl.Club);
                grade = GradeConverter.fromString(girl.Grade);
                classRoom = girl.ClassRoom;
                selectionBonuses = Array.map(SelectionBonus) (girl.SelectionBonus);
                birthday = girl.Birthday;
            }
        }
 

    let getGirlByName(girlName: string) = 
        Seq.find(fun girl -> (if(girlName.Contains("+")) then girlName.[0..girlName.Length-2] else girlName) |> (=) girl.name) girlFactroyFromJson
