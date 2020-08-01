namespace GirlFriendDeckSimulator
open AttributeType
open Club
open GirlFactory
open Girl
open System.ComponentModel
open Player
open FSharp.Data
open System.IO

type PlayerParameterViewModel() =
    let ev = Event<_, _>()
    let mutable playerName = "User1"
    let mutable coolColon = 0.0
    let mutable coolColonStr = "0.0"
    let mutable popColon = 0.0
    let mutable popColonStr = "0.0"
    let mutable sweetColon = 0.0
    let mutable sweetColonStr = "0.0"
    let mutable existWhiteboard = true
    let mutable existTelevision = true
    let mutable existLocker = true
    let mutable selectedClubType = ClubType.NoClub
    let mutable selectedAttributeType = AttributeType.Cool
    let mutable selectedClubRoleType = Role.Member
    let mutable attackCost = 20
    let mutable backDeckNum = 52

    member val ClubTypes = getAllClubTypes
    member val AttributeTypes = getAllAttributeTypes
    member val ClubRoleTypes = getAllClubRoleTypes

    member public this.PlayerName
        with get() = playerName
        and set(newPlayerName) = 
            playerName <- newPlayerName
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.CoolColon
        with get() = coolColon
        and set(newCoolColon) =
            coolColon <- newCoolColon
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.CoolColonStr
        with get() = coolColonStr
        and set(newCoolColonStr) =
            let result = coolColonStr |> float |> ref
            coolColonStr <- newCoolColonStr
            if System.Double.TryParse(newCoolColonStr, result)
                then 
                    if 0.0 <= result.Value && result.Value <= 25.0
                        then 
                            this.CoolColon <- result.Value 
                            ev.Trigger(this, PropertyChangedEventArgs(""))
                        else 
                            ev.Trigger(this, PropertyChangedEventArgs(""))
                else ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.PopColon
        with get() = popColon
        and set(newPopColon) =
            popColon <- newPopColon
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.PopColonStr
        with get() = popColonStr
        and set(newPopColonStr) =
            let result = popColonStr |> float |> ref
            popColonStr <- newPopColonStr
            if System.Double.TryParse(newPopColonStr, result)
                then 
                    if 0.0 <= result.Value && result.Value <= 25.0
                        then 
                            this.PopColon <- result.Value 
                            ev.Trigger(this, PropertyChangedEventArgs(""))
                        else 
                            ev.Trigger(this, PropertyChangedEventArgs(""))
                else ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SweetColon
        with get() = sweetColon
        and set(newSweetColon) =
            sweetColon <- newSweetColon
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SweetColonStr
        with get() = sweetColonStr
        and set(newSweetColonStr) =
            let result = sweetColonStr |> float |> ref
            sweetColonStr <- newSweetColonStr
            if System.Double.TryParse(newSweetColonStr, result)
                then 
                    if 0.0 <= result.Value && result.Value <= 25.0
                        then 
                            this.SweetColon <- result.Value 
                            ev.Trigger(this, PropertyChangedEventArgs(""))
                        else 
                            ev.Trigger(this, PropertyChangedEventArgs(""))
                else ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.ExistWhiteboard
        with get() = existWhiteboard
        and set(newExistWhiteboard) =
            existWhiteboard <- newExistWhiteboard
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.ExistTelevision
        with get() = existTelevision
        and set(newExistTelevision) =
            existTelevision <- newExistTelevision
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.ExistLocker
        with get() = existLocker
        and set(newExistLocker) =
            existLocker <- newExistLocker
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SelectedClubType
        with get() = selectedClubType
        and set(newSelectedClubType) =
            selectedClubType <- newSelectedClubType
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SelectedAttributeType
        with get() = selectedAttributeType
        and set(newSelectedAttributeType) =
            selectedAttributeType <- newSelectedAttributeType
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SelectedClubRoleType
        with get() = selectedClubRoleType
        and set(newSelectedClubRoleType) =
            selectedClubRoleType <- newSelectedClubRoleType
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.AttackCost
        with get() = attackCost
        and set(newAttackCost) =
            attackCost <- newAttackCost
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.BackDeckNum
        with get() = backDeckNum
        and set(newBackDeckNum) =
            backDeckNum <- newBackDeckNum
            ev.Trigger(this, PropertyChangedEventArgs(""))
    
    member this.playerFactory =
        {
            PlayerName = this.PlayerName;
            AttributeType = this.SelectedAttributeType;
            ClubType = this.SelectedClubType;
            AttackCost = this.AttackCost;
            CoolColon = this.CoolColon;
            PopColon = this.PopColon;
            SweetColon = this.SweetColon;
            ExistWhiteboard = this.ExistWhiteboard;
            ExistTelevision = this.ExistTelevision;
            ExistLocker = this.ExistLocker;
            AssignedRole = this.SelectedClubRoleType;
            BackDeckNum = this.BackDeckNum;
        }
    member this.save =
        let jsonData = JsonValue.Record[|
            "playerName", JsonValue.String this.PlayerName;
            "attributeType", JsonValue.String <| AttributeTypeConverter.toString(this.SelectedAttributeType);
            "clubType", JsonValue.String <| ClubTypeConverter.toString(this.SelectedClubType);
            "attackCost", JsonValue.Number <| (this.AttackCost |> decimal) ;
            "coolColon", JsonValue.Float this.CoolColon;
            "popColon", JsonValue.Float this.PopColon;
            "sweetColon", JsonValue.Float this.SweetColon;
            "existWhiteboard", JsonValue.Boolean this.ExistWhiteboard;
            "existTelevision", JsonValue.Boolean this.ExistTelevision;
            "existLocker", JsonValue.Boolean this.ExistLocker;
            "assignedRole", JsonValue.String <| ClubRoleTypeConverter.toString(this.SelectedClubRoleType);
            "backDeckNum", JsonValue.Number <| (this.BackDeckNum |> decimal)
        |]
        let writer = File.CreateText(".\PlayerParameter.json")
        jsonData.WriteTo(writer, JsonSaveOptions.None)
        writer.Close()

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = ev.Publish

module PlayerParameterViewModel =
    type PlayerParameterElements = JsonProvider<""" 
    {
    "playerName": "macaroon",
    "attributeType": "COOL",
    "clubType": "帰宅部",
    "attackCost": 2732,
    "coolColon": 6.2,
    "popColon": 2.2,
    "sweetColon": 1.2,
    "existWhiteboard": true,
    "existTelevision": true,
    "existLocker": true,
    "assignedRole": "攻キャプテン",
    "backDeckNum": 52
    } """>
    let loadPlayerParameterViewModel =
        if File.Exists(".\PlayerParameter.json")
            then
                let playerParameterElements = PlayerParameterElements.Parse(File.ReadAllText(".\PlayerParameter.json"))
                let playerParameterViewModel = PlayerParameterViewModel()
                playerParameterViewModel.PlayerName <- playerParameterElements.PlayerName
                playerParameterViewModel.SelectedAttributeType <- AttributeTypeConverter.fromString(playerParameterElements.AttributeType)
                playerParameterViewModel.SelectedClubType <- ClubTypeConverter.fromString(playerParameterElements.ClubType)
                playerParameterViewModel.AttackCost <- playerParameterElements.AttackCost
                playerParameterViewModel.CoolColonStr <- playerParameterElements.CoolColon.ToString()
                playerParameterViewModel.PopColonStr <- playerParameterElements.PopColon.ToString()
                playerParameterViewModel.SweetColonStr <- playerParameterElements.SweetColon.ToString()
                playerParameterViewModel.ExistWhiteboard <- playerParameterElements.ExistWhiteboard
                playerParameterViewModel.ExistTelevision <- playerParameterElements.ExistTelevision
                playerParameterViewModel.ExistLocker <- playerParameterElements.ExistLocker
                playerParameterViewModel.SelectedClubRoleType <- ClubRoleTypeConverter.fromString(playerParameterElements.AssignedRole)
                playerParameterViewModel.BackDeckNum <- playerParameterElements.BackDeckNum
                playerParameterViewModel |> Some
            else None
                
        