namespace GirlFriendDeckSimulator
open EventType
open CardFactory

type DeckEditViewModel() =
    member val EventType = Raid with get, set
    member val FrontDeck = [] with get, set
    member val BackDeck = [] with get, set
    member val CardList = cardList with get, set
    member val SwitchGirlNum = 0 with get, set
    member val KiraGirlNum = 0 with get, set