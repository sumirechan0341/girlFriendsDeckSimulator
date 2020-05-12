namespace GirlFriendDeckSimulator
open FSharp.Data
open SelectionBonus
open Girl
open System
open System.IO

module GirlFactory =
    
    type GirlInfo = JsonProvider<""" {"name": "上条るい", "club": "運動部(個人競技)", "grade": "2年生", "classRoom": "2年A組", "selectionBonus": ["Ｌｅｔ’ｓ 精神集中♪", "↑スレンダーズボルケーノ↑"], "birthDay": "5月2日"} """>
    let girlFactroyFromJson(girlInfoJson) = 
        let girlInfo = GirlInfo.Parse(girlInfoJson)
        {
            name = girlInfo.Name;
            club = ClubTypeConverter.fromString(girlInfo.Club);
            grade = GradeConverter.fromString(girlInfo.Grade);
            classRoom = if(String.IsNullOrEmpty(girlInfo.ClassRoom)) then None else Some(girlInfo.ClassRoom);
            selectionBonuses = Array.map(SelectionBonus) (girlInfo.SelectionBonus)
            birthDay = girlInfo.BirthDay
        }
    let getAllGirl = Seq.map(fun json -> girlFactroyFromJson(json)) (File.ReadAllText("GirlInfo.json").Split('}'))

    let getGirlByName(girlName: string) = Seq.find(fun girl -> girl.name = girlName) (getAllGirl)
