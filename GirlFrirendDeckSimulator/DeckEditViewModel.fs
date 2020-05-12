namespace GirlFriendDeckSimulator
open EventType

type DeckEditViewModel() =
    member val eventType = Raid with get, set
    member val frontDeck = [] with get, set
    member val backDeck = [] with get, set
    member val cardList = [] with get, set
    member val switchGirlNum = 0 with get, set
    member val KiraGirlNum = 0 with get, set