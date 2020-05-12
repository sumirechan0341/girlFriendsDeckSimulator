namespace GirlFriendDeckSimulator
open Card
open FSharp.Data
open System.IO
//open GirlFactory
//open AttributeTypeConverter
//open CardConverter
module CardFactroy =
    type CardElements = JsonProvider<""" {"girlName":"上条るい","eventName":"あの頃は…","rarity":"UR","attribute":"Cool","isExed":true,"attack":32744,"defence":27698,"cost":23,"skillLevel":15,"skillType":"COOLの主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人の攻援ｽｰﾊﾟｰ特大UP"} """>
    let cardFactoryFromJson(cardJson) = null
        //let cardElements = CardElements.Parse(cardJson)
        //{
        //    girl = getGirlByName(cardElements.GirlName);
        //    eventName = cardElements.EventName;
        //    attribute = AttributeTypeConverter.fromString(cardElements.Attribute);
        //    rarity = RarityConverter.fromString(cardElements.Rarity);
        //    cardType = CardTypeConverter.fromEventName(cardElements.EventName);
        //    isEXed = cardElements.IsExed;
        //    cost = cardElements.Cost;
        //}
    let cardList = Seq.map(fun line -> CardElements.Parse(line)) (File.ReadLines("CardList.json"))