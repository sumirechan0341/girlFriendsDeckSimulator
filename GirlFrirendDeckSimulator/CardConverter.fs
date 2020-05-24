namespace GirlFriendDeckSimulator
open Card
open System.Text.RegularExpressions
module CardConverter =
    module RarityConverter =
        let fromString(rarityStr: string) =
            match rarityStr with
            | "N" -> Card.Rarity.N
            | "HN" -> Card.Rarity.HN
            | "R" -> Card.Rarity.R
            | "HR" -> Card.Rarity.HR
            | "SR" -> Card.Rarity.SR
            | "SSR" -> Card.Rarity.SSR
            | "UR" -> Card.Rarity.UR

        let toString(rarity: Rarity) =
            match rarity with
            | Card.Rarity.N -> "N"
            | Card.Rarity.HN -> "HN"
            | Card.Rarity.R -> "R"
            | Card.Rarity.HR -> "HR"
            | Card.Rarity.SR -> "SR"
            | Card.Rarity.SSR -> "SSR"
            | Card.Rarity.UR -> "UR"

    module CardTypeConverter =

        let (|ParseRegex|_|) regex str =
            let m = Regex(regex).Match(str)
            if m.Success
            then Some (m.Value)
            else None

        let fromEventName(eventName: string) =
            match eventName with
            | ParseRegex "ｷﾗｶﾞｰﾙ" _
                 -> CardType.Kira
            | ParseRegex "ｽｲｯﾁｶﾞｰﾙ" _
                 -> CardType.Switch
            | ParseRegex "ﾐﾗｰｶﾞｰﾙ." _
                 -> CardType.Mirror
            | ParseRegex "ﾊﾞｰｽﾃﾞｰｶﾞｰﾙ|誕生日\d+" _
                -> CardType.Birthday
            | ParseRegex "仲良し|×ｱﾆﾒ仲良し" _
                -> CardType.Friends
            | _ -> CardType.Common

    let toString(card: Card) =
        card.girl.name + " " + card.attack.ToString()

type CardConverter() =
    inherit ConverterBase(CardConverter.toString >> (fun s -> s :> obj) |> convert, nullFunction)