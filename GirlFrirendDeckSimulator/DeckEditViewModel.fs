namespace GirlFriendDeckSimulator
open EventType
open CardFactory
open System.Collections.ObjectModel
open Card
open GirlFactory
open Girl
open CardView
open CalcBonus
open System.Windows


type DeckEditViewModel() =
    member val EventType = Raid with get, set
    member val FrontDeck = [] with get, set
    member val BackDeck = [] with get, set
    member val SwitchGirlNum = 0 with get, set
    member val KiraGirlNum = 0 with get, set
    member val CardListView = Seq.map CardView (cardList)  //Seq.map(CardView) cardList with get, set
    
    
    
    