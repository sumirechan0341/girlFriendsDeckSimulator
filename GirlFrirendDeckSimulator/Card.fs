namespace GirlFriendDeckSimulator
open Girl
open AttributeType
open SkillType
open FSharp.Data
module Card =
    type Rarity = N | HN | R | HR | SR | SSR | UR

    type Rarity with
        member this.getId = 
            match this with
            | N -> 1
            | HN -> 2
            | R -> 3
            | HR -> 4
            | SR -> 5
            | SSR -> 6
            | UR -> 7
    
    type CardType = 
        Common 
        | Switch 
        | Kira 
        | Mirror
        | Friends
        | Birthday

    type Card = {
        girl: Girl
        eventName: option<string>
        attribute: AttributeType
        rarity: Rarity
        attack: int
        defence: int
        cardType: CardType
        isEXed: bool
        cost: int
        skillType: option<SkillType>
        skillLevel: int
        cardJson: JsonValue
    }

