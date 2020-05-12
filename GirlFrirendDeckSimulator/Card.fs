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
        | BirthDay

    type Card = {
        girl: Girl
        eventName: string
        attribute: AttributeType
        rarity: Rarity
        cardType: CardType
        isEXed: bool
        cost: int
        skillType: SkillType
        skillLevel: int
    }

