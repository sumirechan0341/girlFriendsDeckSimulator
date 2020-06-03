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
open System.ComponentModel


type DeckEditViewModel() =
    let ev = Event<_, _>()
    let mutable frontDeck: ResizeArray<CardView> = [] |> ResizeArray
    let mutable cardListView: ResizeArray<CardView> = Seq.map CardView (cardList) |> ResizeArray
    let mutable eventType = Raid
    let mutable backDeck = []
    let mutable switchGirlNum = 0
    let mutable kiraGirlNum = 0
    member this.EventType
        with get() = eventType
        and set(newEventType) =
            eventType <- newEventType
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.FrontDeck 
        with get() = frontDeck 
        and set(newFrontDeck) =
            frontDeck <- newFrontDeck
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.BackDeck 
        with get() = backDeck
        and set(newBackDeck) =
            backDeck <- newBackDeck
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SwitchGirlNum
        with get() = switchGirlNum
        and set(newSwitchGirlNum) =
            switchGirlNum <- newSwitchGirlNum
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.KiraGirlNum
        with get() = kiraGirlNum
        and set(newKiraGirlNum) =
            kiraGirlNum <- newKiraGirlNum
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.CardListView
        with get() = cardListView
        and set(newCardListView) =
            cardListView <- newCardListView
            ev.Trigger(this, PropertyChangedEventArgs(""))
    
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = ev.Publish