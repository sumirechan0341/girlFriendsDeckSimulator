namespace GirlFriendDeckSimulator
open FSharp.Data
open SelectionBonus
open Girl
open System
open System.IO
open System.Text.RegularExpressions

module GirlFactory =
    
    type GirlInfo = JsonProvider<""" {"name": "上条るい", "club": "運動部(個人競技)", "grade": "2年生", "classRoom": "2年A組", "selectionBonus": ["Ｌｅｔ’ｓ 精神集中♪", "↑スレンダーズボルケーノ↑"], "birthDay": "5月2日"} """>
    
    let girlFactroyFromJson(girlInfoJson: string) = 
        let girlInfo = GirlInfo.Parse(girlInfoJson)
        {
            name = girlInfo.Name;
            club = ClubTypeConverter.fromString(girlInfo.Club);
            grade = GradeConverter.fromString(girlInfo.Grade);
            classRoom = if(String.IsNullOrEmpty(girlInfo.ClassRoom)) then None else Some(girlInfo.ClassRoom);
            selectionBonuses = Array.map(SelectionBonus) (girlInfo.SelectionBonus)
            birthDay = girlInfo.BirthDay
        }

    let getAllGirl = Seq.map(girlFactroyFromJson) (File.ReadAllText("..\..\GirlInfo.json") |> Regex("(?<=}),").Split)

    let getGirlByName(girlName: string) = 
        System.Console.WriteLine(girlName)
        Seq.find(fun girl -> (if(girlName.Contains("+")) then girlName.[0..girlName.Length-2] else girlName) |> (=) girl.name) (getAllGirl)
