namespace GirlFriendDeckSimulator
open Card
open FSharp.Data
open System.IO
open GirlFactory
open CardConverter
open System
module CardFactory =
    type CardElements = JsonProvider<""" {"girlName":"上条るい","eventName":"あの頃は…","rarity":"UR","attribute":"COOL","isExed":true,"attack":32744,"defence":27698,"cost":23,"skillLevel":15,"skillType":"COOLの主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人の攻援ｽｰﾊﾟｰ特大UP"} """>
    let cardFactoryFromJson(cardJson) = 
        let cardElements = CardElements.Parse(cardJson)
        {
            girl = getGirlByName(cardElements.GirlName);
            eventName = 
                match cardElements.EventName with
                | "" -> None
                | _ -> Some(cardElements.EventName) 
            attribute = AttributeTypeConverter.fromString(cardElements.Attribute);
            rarity = RarityConverter.fromString(cardElements.Rarity);
            attack = cardElements.Attack;
            defence = cardElements.Defence;
            cardType = CardTypeConverter.fromEventName(cardElements.EventName);
            isEXed = cardElements.IsExed;
            cost = cardElements.Cost;
            skillType =  SkillTypeConverter.fromString(cardElements.SkillType);
            skillLevel = cardElements.SkillLevel;
            cardJson = cardElements.JsonValue
        }

    let cardList = 
        Seq.map(cardFactoryFromJson) <| 
        if File.Exists(".\CardList.txt")
        then 
            Seq.filter (String.IsNullOrWhiteSpace >> not) (File.ReadLines(".\CardList.txt"))
        else Seq.empty