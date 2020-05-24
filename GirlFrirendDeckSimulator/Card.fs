namespace GirlFriendDeckSimulator
open Girl
open AttributeType
open SkillType
module Card =
    type Rarity = N | HN | R | HR | SR | SSR | UR
    
    type CardType = 
        Common 
        | Switch 
        | Kira 
        | Mirror
        | Friends
        | Birthday

    type Card = {
        girl: Girl
        eventName: string
        attribute: AttributeType
        rarity: Rarity
        attack: int
        defence: int
        cardType: CardType
        isEXed: bool
        cost: int
        skillType: option<SkillType>
        skillLevel: int
    }

