namespace GirlFriendDeckSimulator
open EventType
open CardFactory
open System.Collections.ObjectModel
open Card
open GirlFactory
open Girl
open CardView
open System.Windows
open System.ComponentModel
open FSharp.Data
open System.IO
open PreciousScene
open PreciousSceneView
open PreciousSceneFactory
open SelectionBonus
open System.Text.RegularExpressions

type DeckElements = JsonProvider<""" 
    [{
        "eventName": "レイド",
        "mode": "攻援",
        "deckInfo": {
            "frontDeck": [{
                "card": 
                {
                    "girlName": "上条るい",
                    "eventName": "あの頃は…",
                    "rarity": "UR",
                    "attribute": "COOL",
                    "isExed": true,
                    "attack": 32744,
                    "defence": 27698,
                    "cost": 23,
                    "skillLevel": 15,
                    "skillType": "COOLの主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人の攻援ｽｰﾊﾟｰ特大UP"
                },
                "strap1": 27.6,
                "strap2": 27.6,
                "strap3": 27.6,
                "strap4": 27.6
            }],
            "backDeck": [
                {
                    "card": {
                        "girlName": "上条るい",
                        "eventName": "あの頃は…",
                        "rarity": "UR",
                        "attribute": "COOL",
                        "isExed": true,
                        "attack": 32744,
                        "defence": 27698,
                        "cost": 23,
                        "skillLevel": 15,
                        "skillType": "COOLの主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人の攻援ｽｰﾊﾟｰ特大UP"
                    }
                }
            ],
            "sceneList": [{
                "scene": 
                {
                    "sceneName":"[靴箱の邂逅]上条るい",
                    "level":4,
                    "effect":"COOLガールのコストが高いほど攻援UP",
                    "effectMaxTerms":"コスト23",
                    "effectMax":"最大4.5%"
                }
        
            }]
        }
    }]
""">
type DeckElement = JsonProvider<""" 
    {
        "eventName": "レイド",
        "mode": "攻援",
        "deckInfo": {
            "frontDeck": [{
                "card": 
                {
                    "girlName": "上条るい",
                    "eventName": "あの頃は…",
                    "rarity": "UR",
                    "attribute": "COOL",
                    "isExed": true,
                    "attack": 32744,
                    "defence": 27698,
                    "cost": 23,
                    "skillLevel": 15,
                    "skillType": "COOLの主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人の攻援ｽｰﾊﾟｰ特大UP"
                },
                "strap1": 27.6,
                "strap2": 27.6,
                "strap3": 27.6,
                "strap4": 27.6
            }],
            "backDeck": [
                {
                    "card": {
                        "girlName": "上条るい",
                        "eventName": "あの頃は…",
                        "rarity": "UR",
                        "attribute": "COOL",
                        "isExed": true,
                        "attack": 32744,
                        "defence": 27698,
                        "cost": 23,
                        "skillLevel": 15,
                        "skillType": "COOLの主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人の攻援ｽｰﾊﾟｰ特大UP"
                    }
                }
            ],
            "sceneList": [{
                "scene": 
                {
                    "sceneName":"[靴箱の邂逅]上条るい",
                    "level":4,
                    "effect":"COOLガールのコストが高いほど攻援UP",
                    "effectMaxTerms":"コスト23",
                    "effectMax":"最大4.5%"
                }
        
            }]
        }
    }
""">

type SelectionBonusInfoView(selectionBonus: SelectionBonus, currentSelectionBonusLevel: int, maxSelectionBonusLevel: int) =
    let ev = Event<_, _>()
    
    let mutable selectionBonusInfo: string = selectionBonus.SelectionBonusName + ": Lv." + currentSelectionBonusLevel.ToString() + " / Lv." +  maxSelectionBonusLevel.ToString()
    let mutable isIntoDeck: bool = false
    let mutable isOutOfDeck: bool = false
    let mutable currentSelectionBonusLevel: int = currentSelectionBonusLevel
    let mutable maxSelectionBonusLevel: int = maxSelectionBonusLevel
    let mutable afterEditSelectionBonusLevel: int = currentSelectionBonusLevel
    
    member val SelectionBonus = selectionBonus
    member this.SelectionBonusInfo
        with get() = selectionBonusInfo
        and set(newSelectionBonusInfo) =
            selectionBonusInfo <- newSelectionBonusInfo
            ev.Trigger(this, PropertyChangedEventArgs(""))

    member this.IsIntoDeck
        with get() = isIntoDeck
        and set(newIsIntoDeck) =
            isIntoDeck <- newIsIntoDeck
            ev.Trigger(this, PropertyChangedEventArgs(""))

    member this.IsOutOfDeck
        with get() = isOutOfDeck
        and set(newIsOutOfDeck) =
            isOutOfDeck <- newIsOutOfDeck
            ev.Trigger(this, PropertyChangedEventArgs(""))

    member this.CurrentSelectionBonusLevel
        with get() = currentSelectionBonusLevel
        and set(newSelectionBonusLevel) =
            currentSelectionBonusLevel <- newSelectionBonusLevel
            ev.Trigger(this, PropertyChangedEventArgs(""))

    member this.MaxSelectionBonusLevel
        with get() = maxSelectionBonusLevel
        and set(newMaxSelectionBonusLevel) =
            maxSelectionBonusLevel <- newMaxSelectionBonusLevel
            ev.Trigger(this, PropertyChangedEventArgs(""))

    member this.AfterEditSelectionBonusLevel
        with get() = afterEditSelectionBonusLevel
        and set(newAfterSelectionBonusLevel) =
            afterEditSelectionBonusLevel <- newAfterSelectionBonusLevel
            ev.Trigger(this, PropertyChangedEventArgs(""))

    member this.ConvertToEditSelectionBonusInfo() =
        this.SelectionBonusInfo <- 
            this.SelectionBonus.SelectionBonusName 
            + ": Lv." + this.CurrentSelectionBonusLevel.ToString() 
            + " → Lv." + this.AfterEditSelectionBonusLevel.ToString() 
            + " / Lv." +  this.MaxSelectionBonusLevel.ToString()

    member this.ConvertToOrdinarySelectionBonusInfo() =
        this.SelectionBonusInfo <- 
            this.SelectionBonus.SelectionBonusName 
            + ": Lv." + this.CurrentSelectionBonusLevel.ToString() 
            + " / Lv." +  this.MaxSelectionBonusLevel.ToString()

    
    new() = SelectionBonusInfoView(SelectionBonus(""), 0, 5)
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = ev.Publish

type DeckEditViewModel(frontDeck: CardViewWithStrap[], backDeck: CardView[], selectedPreciousScene: PreciousSceneView[], eventType: EventType) =
    let ev = Event<_, _>()
    let mutable frontDeck: ResizeArray<CardViewWithStrap> = frontDeck |> ResizeArray
    let mutable backDeck: ResizeArray<CardView> = backDeck |> ResizeArray
    let mutable cardListView: ResizeArray<CardView> =
        let cardViewList = cardList |> Seq.map CardView |> ResizeArray
        for frontCardView in frontDeck do
            cardViewList.Remove(frontCardView) |> ignore
        for backCardView in backDeck do
            cardViewList.Remove(backCardView) |> ignore
        cardViewList
    let mutable selectedPreciousSceneList: ResizeArray<PreciousSceneView> = selectedPreciousScene |> ResizeArray // 後で保存機構作る

    let mutable preciousSceneListView: ResizeArray<PreciousSceneView> =
        let sceneViewList = Seq.map PreciousSceneView preciousSceneList |> ResizeArray
        for scene in selectedPreciousSceneList do
            sceneViewList.Remove(scene) |> ignore
        sceneViewList

    let mutable filteredGirl = [||] |> ResizeArray
    let mutable selectedFilterAttribute = "指定なし"
    let mutable selectedFilterCardType = "指定なし"
    let mutable selectedFilterGirl = "指定なし"
    let mutable selectedFilterSelectionBonus = "指定なし"
    let mutable selectedFilterRarity = "指定なし"
    let mutable selectedEventType = eventType
    let mutable selectedMode = Mode.Attack
    let mutable birthdaySetttingGirl = "指定なし"
    let mutable totalAttack = 0
    let mutable totalDefence = 0
    let mutable estimatedDamage: int64 = 0L
    let mutable damageExpectation: float = 0.0
    let mutable selectionBonusInfoViews: ResizeArray<SelectionBonusInfoView> = [||] |> ResizeArray
    let mutable saveDeckList: Map<(string * string), JsonValue> = 
        if File.Exists(".\DeckEdit.json")
        then  
            let jsonText = File.ReadAllText(".\DeckEdit.json")
            let deckEditJson = DeckElements.Parse(jsonText)
            let result = 
                [for deck in deckEditJson -> 
                    ((deck.EventName, deck.Mode), deck.JsonValue)
                ]
            Map.ofList(result)
        else 
            Map.empty
        
    do
        0 |> ignore
        //match saveDeckList.TryFind((EventTypeConverter.toString(selectedEventType), ModeConverter.toString(selectedMode))) with
        //| Some(jsonValue) ->
        //    let deckJson = DeckElement.Parse(jsonValue.ToString())
        //    let newFrontDeck = 
        //        Array.map (fun (girlWithStrap: DeckElement.FrontDeck) -> 
        //            CardViewWithStrap(girlWithStrap.Card.JsonValue.ToString() |> cardFactoryFromJson, girlWithStrap.Strap1 |> float, girlWithStrap.Strap2 |> float, girlWithStrap.Strap3 |> float, girlWithStrap.Strap4 |> float)
        //        ) (deckJson.DeckInfo.FrontDeck)
        //    let newBackDeck =
        //        Array.map (fun (girl: DeckElement.BackDeck) -> 
        //            CardView(girl.Card.JsonValue.ToString() |> cardFactoryFromJson)
        //        ) (deckJson.DeckInfo.BackDeck)
        //    let newSceneList =
        //        Array.map (fun (scene: DeckElement.SceneList) -> PreciousSceneView(scene.Scene.JsonValue.ToString() |> preciousSceneFactoryFromJson)) deckJson.DeckInfo.SceneList
        //    frontDeck <- newFrontDeck |> ResizeArray
        //    backDeck <- newBackDeck |> ResizeArray
        //    selectedPreciousSceneList <- newSceneList |> ResizeArray
        //    for frontCardView in frontDeck do
        //        frontCardView.Straps <- [||]
        //        cardListView.Remove(frontCardView) |> ignore
        //    for backCardView in backDeck do
        //        cardListView.Remove(backCardView) |> ignore
        //    for scene in selectedPreciousSceneList do
        //        preciousSceneListView.Remove(scene) |> ignore
        //| None -> 
        //    0 |> ignore
           
        //if selectedMode = Mode.Attack
        //    then 
        //        backDeck.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
        //        cardListView.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
        //        preciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
        //    else 
        //        backDeck.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
        //        cardListView.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
        //        preciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))


    member val Events = 
        [|
            EventType.Raid; 
            EventType.CoolTrio; 
            EventType.PopTrio; 
            EventType.SweetTrio; 
            EventType.CoolMega; 
            EventType.PopMega; 
            EventType.SweetMega; 
            EventType.Hunters;
            EventType.Charisma;
            EventType.MemorialStory;
            EventType.Battle
        |] with get, set
    member val Modes = [|Mode.Attack; Mode.Defence|] with get, set
    member val GirlList = girlFactroyFromJson |> Seq.filter(fun g -> g.birthday.IsSome) |> Seq.map(fun g -> g.name) |> Seq.append (seq{"指定なし"}) with get, set
    member val AllAttributes = [|"指定なし"; "COOL"; "POP"; "SWEET"|]
    member val AllCardTypes = [|"指定なし"; "キラ"; "スイッチ"; "ミラー"; "バースデー"; "仲良し"|]
    member val AllSelectionBonus = 
        (Array.filter (fun (sb: SelectionBonus) -> (sb.SelectionBonusName = "Precious★Friend" || sb.SelectionBonusName = "シャイニング★スプラッシュ") |> not) <| getAllSelectionBonus)
        |> Array.map (fun (sb: SelectionBonus) -> sb.SelectionBonusName)
        |> Array.append [|"指定なし"|]
    member val AllRarities = [|"指定なし"; "UR"; "SSR"; "SR"; "HR"; "R"; "HN"; "N"|]

    member this.SelectedFilterAttribute
        with get() = selectedFilterAttribute
        and set(newSelectedFilterAttribute) =
            selectedFilterAttribute <- newSelectedFilterAttribute
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SelectedFilterCardType
        with get() = selectedFilterCardType
        and set(newSelectedFilterCardType) =
        selectedFilterCardType <- newSelectedFilterCardType
        ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SelectedFilterGirl
        with get() = selectedFilterGirl
        and set(newSelectedFilterGirl) =
            selectedFilterGirl <- newSelectedFilterGirl
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SelectedFilterSelectionBonus
        with get() = selectedFilterSelectionBonus
        and set(newSelectedFilterSelectionBonus) =
            selectedFilterSelectionBonus <- newSelectedFilterSelectionBonus
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SelectedFilterRarity
        with get() = selectedFilterRarity
        and set(newSelectedFilterRarity) =
            selectedFilterRarity <- newSelectedFilterRarity
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SelectedEventType
        with get() = selectedEventType
        and set(newEventType) =
            selectedEventType <- newEventType
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
    member this.CardListView
        with get() = cardListView
        and set(newCardListView) =
            cardListView <- newCardListView
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SelectedMode
        with get() = selectedMode
        and set(newSelectedMode) =
            selectedMode <- newSelectedMode
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.BirthdaySettingGirl
        with get() = birthdaySetttingGirl
        and set(newBirthdaySettingGirl) =
            birthdaySetttingGirl <- newBirthdaySettingGirl
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.PreciousSceneListView
        with get() = preciousSceneListView
        and set(newPreciousSceneListView) =
            preciousSceneListView <- newPreciousSceneListView
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SelectedPreciousSceneList
        with get() = selectedPreciousSceneList
        and set(newSelectedPreciousSceneList) =
            selectedPreciousSceneList <- newSelectedPreciousSceneList
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.TotalAttack
        with get() = totalAttack
        and set(newTotalAttack) =
            totalAttack <- newTotalAttack
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.TotalDefence
        with get() = totalDefence
        and set(newTotalDefence) =
            totalDefence <- newTotalDefence
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.EstimatedDamage
        with get() = estimatedDamage
        and set(newEstimatedDamage) =
            estimatedDamage <- newEstimatedDamage
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.DamageExpectation
        with get() = damageExpectation
        and set(newDamageExpectation) =
            damageExpectation <- newDamageExpectation
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SelectionBonusInfoViews
        with get() = selectionBonusInfoViews
        and set(newSelectionBonusInfoViews) =
            selectionBonusInfoViews <- newSelectionBonusInfoViews
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SaveDeckList
        with get() = saveDeckList
        and set(newSaveDeckList) =
            saveDeckList <- newSaveDeckList
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.FilteredGirl
        with get() = filteredGirl
        and set(newFilteredGirl) =
            filteredGirl <- newFilteredGirl
            ev.Trigger(this, PropertyChangedEventArgs(""))

    member this.save() =
        let jsonData = 
            JsonValue.Record [|
                "eventName", JsonValue.String <| EventTypeConverter.toString(this.SelectedEventType);
                "mode", JsonValue.String <| ModeConverter.toString(this.SelectedMode);
                "deckInfo", JsonValue.Record
                    [|
                        "frontDeck", JsonValue.Array <| 
                            Array.map(
                                fun (c: CardViewWithStrap) -> 
                                    JsonValue.Record [|
                                        "card", c.Card.cardJson;
                                        "strap1", JsonValue.Float c.Strap1;
                                        "strap2", JsonValue.Float c.Strap2;
                                        "strap3", JsonValue.Float c.Strap3;
                                        "strap4", JsonValue.Float c.Strap4
                                    |]) 
                                (frontDeck.ToArray());
                        "backDeck", JsonValue.Array <| 
                            Array.map(
                                fun (c: CardView) ->
                                    JsonValue.Record [|
                                        "card", c.Card.cardJson;
                                    |]) 
                                (backDeck.ToArray());
                        "sceneList", JsonValue.Array <|
                            Array.map(
                                fun (s: PreciousSceneView) ->
                                    JsonValue.Record [|
                                        "scene", s.PreciousScene.sceneJson;
                                    |]) 
                                (selectedPreciousSceneList.ToArray());
                    |]
            |]
        this.SaveDeckList <- this.SaveDeckList.Add((EventTypeConverter.toString(this.SelectedEventType), ModeConverter.toString(this.SelectedMode)), jsonData)

    member this.write() =
        let writer = File.CreateText(".\DeckEdit.json")
        let deckEditJson = JsonValue.Array <| [|for pair in this.SaveDeckList -> pair.Value|]
        deckEditJson.WriteTo(writer, JsonSaveOptions.None)
        writer.Close()

    member this.loadDeck() =
        match this.SaveDeckList.TryFind((EventTypeConverter.toString(this.SelectedEventType), ModeConverter.toString(this.SelectedMode))) with
        | Some(jsonValue) ->
            let deckJson = DeckElement.Parse(jsonValue.ToString())
            let frontDeck = 
                Array.map (fun (girlWithStrap: DeckElement.FrontDeck) -> 
                    CardViewWithStrap(girlWithStrap.Card.JsonValue.ToString() |> cardFactoryFromJson, girlWithStrap.Strap1 |> float, girlWithStrap.Strap2 |> float, girlWithStrap.Strap3 |> float, girlWithStrap.Strap4 |> float)
                ) (deckJson.DeckInfo.FrontDeck)
            let backDeck =
                Array.map (fun (girl: DeckElement.BackDeck) -> 
                    CardView(girl.Card.JsonValue.ToString() |> cardFactoryFromJson)
                ) (deckJson.DeckInfo.BackDeck)
            let sceneList =
                Array.map (fun (scene: DeckElement.SceneList) -> PreciousSceneView(scene.Scene.JsonValue.ToString() |> preciousSceneFactoryFromJson)) deckJson.DeckInfo.SceneList
            
            // 元々入っていたアイテムを全部消去
            for card in this.FrontDeck do
                card.Straps <- [||]
                this.CardListView.Add(card)
            this.FrontDeck.RemoveAll(fun _ -> true) |> ignore
            for card in this.BackDeck do
                this.CardListView.Add(card)
            this.BackDeck.RemoveAll(fun _ -> true) |> ignore
            for scene in this.SelectedPreciousSceneList do
                this.PreciousSceneListView.Add(scene)
            this.SelectedPreciousSceneList.RemoveAll(fun _ -> true) |> ignore

            // jsonから復元したアイテムを追加
            this.FrontDeck.AddRange(frontDeck)
            this.BackDeck.AddRange(backDeck)
            this.SelectedPreciousSceneList.AddRange(sceneList)
            
            // 所持リストから追加したアイテムを消去
            for card in frontDeck do
                this.CardListView.Remove(card) |> ignore
            for card in backDeck do
                this.CardListView.Remove(card) |> ignore
            for scene in sceneList do
                this.PreciousSceneListView.Remove(scene) |> ignore
        | None -> 
            for card in this.FrontDeck do
                card.Straps <- [||]
                this.CardListView.Add(card)
            this.FrontDeck.RemoveAll(fun _ -> true) |> ignore
            for card in this.BackDeck do
                this.CardListView.Add(card)
            this.BackDeck.RemoveAll(fun _ -> true) |> ignore
            for scene in this.SelectedPreciousSceneList do
                this.PreciousSceneListView.Add(scene)
            this.SelectedPreciousSceneList.RemoveAll(fun _ -> true) |> ignore
        // フィルタ適用
        this.applyFilter()

    member this.resetFilter() =
        this.CardListView.AddRange(this.FilteredGirl)
        this.FilteredGirl.RemoveAll(fun _ -> true) |> ignore

    member this.applyFilter() =
        match this.SelectedFilterAttribute with
        | "指定なし" -> 0 |> ignore
        | attributeTypeStr ->
            let targetAttribute = AttributeTypeConverter.fromString(attributeTypeStr)
            this.FilteredGirl.AddRange(this.CardListView.FindAll(fun card -> (card.Card.attribute = targetAttribute) |> not))
            this.CardListView.RemoveAll(fun card -> (card.Card.attribute = targetAttribute) |> not) |> ignore
        match this.SelectedFilterCardType with
        | "指定なし" -> 0 |> ignore
        | cardTypeStr ->
            this.FilteredGirl.AddRange(this.CardListView.FindAll(fun card -> (card.Card.cardType = CardConverter.CardTypeConverter.fromString(cardTypeStr)) |> not))
            this.CardListView.RemoveAll(fun card -> (card.Card.cardType = CardConverter.CardTypeConverter.fromString(cardTypeStr)) |> not) |> ignore
        match this.SelectedFilterGirl with
        | "指定なし" -> 0 |> ignore
        | girlName ->
            this.FilteredGirl.AddRange(this.CardListView.FindAll(fun card -> (card.Card.girl.name = girlName) |> not))
            this.CardListView.RemoveAll(fun card -> (card.Card.girl.name = girlName) |> not) |> ignore
        match this.SelectedFilterRarity with
        | "指定なし" -> 0 |> ignore
        | rarityStr ->
            let targetRarity = CardConverter.RarityConverter.fromString(rarityStr)
            this.FilteredGirl.AddRange(this.CardListView.FindAll(fun card -> (card.Card.rarity = targetRarity) |> not))
            this.CardListView.RemoveAll(fun card -> (card.Card.rarity = targetRarity) |> not) |> ignore
        match this.SelectedFilterSelectionBonus with
        | "指定なし" -> 0 |> ignore
        | selectionBonusName ->
            this.FilteredGirl.AddRange(this.CardListView.FindAll(fun card -> (Array.contains (SelectionBonus(selectionBonusName)) card.Card.girl.selectionBonuses) |> not))
            this.CardListView.RemoveAll(fun card -> (Array.contains (SelectionBonus(selectionBonusName)) card.Card.girl.selectionBonuses) |> not) |> ignore

    member this.getSelectionBonusLevelMap =
        let mutable selectionBonusLevelMap = Map.empty
        for cardView in frontDeck do
            if cardView.Card.cardType = CardType.Kira
            then 
                match selectionBonusLevelMap.TryFind(SelectionBonus("シャイニング★スプラッシュ")) with
                | Some(level) -> selectionBonusLevelMap <- selectionBonusLevelMap.Add(SelectionBonus("シャイニング★スプラッシュ"), level + 1)
                | None -> selectionBonusLevelMap <- selectionBonusLevelMap.Add(SelectionBonus("シャイニング★スプラッシュ"), 1)
            else
                0 |> ignore

            for selectionBonus in cardView.Card.girl.selectionBonuses do
                match selectionBonusLevelMap.TryFind(selectionBonus) with
                | Some(level) -> selectionBonusLevelMap <- selectionBonusLevelMap.Add(selectionBonus, level + 1)
                | None -> selectionBonusLevelMap <- selectionBonusLevelMap.Add(selectionBonus, 1)

        for cardView in backDeck do
            if cardView.Card.cardType = CardType.Kira
            then 
                match selectionBonusLevelMap.TryFind(SelectionBonus("シャイニング★スプラッシュ")) with
                | Some(level) -> selectionBonusLevelMap <- selectionBonusLevelMap.Add(SelectionBonus("シャイニング★スプラッシュ"), level + 1)
                | None -> selectionBonusLevelMap <- selectionBonusLevelMap.Add(SelectionBonus("シャイニング★スプラッシュ"), 1)
            else
                0 |> ignore

            for selectionBonus in cardView.Card.girl.selectionBonuses do
                match selectionBonusLevelMap.TryFind(selectionBonus) with
                | Some(level) -> selectionBonusLevelMap <- selectionBonusLevelMap.Add(selectionBonus, level + 1)
                | None -> selectionBonusLevelMap <- selectionBonusLevelMap.Add(selectionBonus, 1)
        // levelでソート
        selectionBonusLevelMap <- selectionBonusLevelMap |> Map.toArray |> Array.sortBy(fun (k, v) -> v) |> Map.ofArray
        selectionBonusLevelMap
    
    new() = DeckEditViewModel([||], [||], [||], EventType.Raid)  
    new(ev: EventType) = DeckEditViewModel([||], [||], [||], ev)
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = ev.Publish

//module DeckEditViewModel =
    
    //let loadDeckEditViewModel(ev: EventType) = 
    //    match Map.tryFind (EventTypeConverter.toString(ev)) this.SaveDeckList with
    //    | Some(a) -> None
    //    | None -> None
        //if File.Exists(".\DeckEdit.json")
        //    then  
        //        let deckElements = DeckElements.Parse(File.ReadAllText(".\DeckEdit.json"))
        //        Option.map (fun (json: DeckElements.Root) ->
        //            let deckInfo = json.DeckInfo
        //            let frontDeck = 
        //                Array.map (
        //                    fun (girlWithStrap: DeckElements.FrontDeck) -> 
        //                        CardViewWithStrap(girlWithStrap.Card.JsonValue.ToString() |> cardFactoryFromJson, girlWithStrap.Strap1 |> float, girlWithStrap.Strap2 |> float, girlWithStrap.Strap3 |> float, girlWithStrap.Strap4 |> float)
        //                ) (deckInfo.FrontDeck)
        //            let backDeck =
        //                Array.map (
        //                    fun (girl: DeckElements.BackDeck) -> 
        //                        CardView(girl.Card.JsonValue.ToString() |> cardFactoryFromJson)
        //                ) (deckInfo.BackDeck)
        //            let sceneList =
        //                Array.map (fun (scene: DeckElements.SceneList) -> PreciousSceneView(scene.Scene.JsonValue.ToString() |> preciousSceneFactoryFromJson)) deckInfo.SceneList
        //            DeckEditViewModel(frontDeck, backDeck, sceneList, ev)
        //        ) (Array.tryFind (fun (json: DeckElements.Root) -> json.EventName = EventTypeConverter.toString(ev)) <| (deckElements |> ResizeArray).ToArray())
        //    else None
