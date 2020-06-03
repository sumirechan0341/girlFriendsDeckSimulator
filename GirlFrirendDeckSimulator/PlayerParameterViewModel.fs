namespace GirlFriendDeckSimulator
open AttributeType
open Club
open GirlFactory
open Girl
open System.ComponentModel


type PlayerParameterViewModel() =
    let ev = Event<_, _>()
    let mutable playerName = "User1"
    let mutable coolColon = 0.0
    let mutable popColon = 0.0
    let mutable sweetColon = 0.0
    let mutable existWhiteboard = true
    let mutable existTelevision = true
    let mutable existLocker = true
    let mutable selectedClubType = ClubType.NoClub
    let mutable selectedAttributeType = AttributeType.Cool
    let mutable selectedClubRoleType = Role.Member
    let mutable attackCost = 20
    let mutable backDeckNum = 52
    let mutable birthdaySetttingGirl = "指定なし"

    member val ClubTypes = getAllClubTypes
    member val AttributeTypes = getAllAttributeTypes
    member val ClubRoleTypes = getAllClubRoleTypes
    member val GirlList = girlFactroyFromJson |> Seq.filter(fun g -> g.birthday.IsSome) |> Seq.map(fun g -> g.name) |> Seq.append (seq{"指定なし"}) with get, set

    member this.PlayerName
        with get() = playerName
        and set(newPlayerName) = 
            playerName <- newPlayerName
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.CoolColon
        with get() = coolColon
        and set(newCoolColon) =
            coolColon <- newCoolColon
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.PopColon
        with get() = popColon
        and set(newPopColon) =
            popColon <- newPopColon
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.SweetColon
        with get() = sweetColon
        and set(newSweetColon) =
            sweetColon <- newSweetColon
            ev.Trigger(this, PropertyChangedEventArgs(""))
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
    member this.BirhtdaySettingGirl
        with get() = birthdaySetttingGirl
        and set(newBirthdaySettingGirl) =
            birthdaySetttingGirl <- newBirthdaySettingGirl
            ev.Trigger(this, PropertyChangedEventArgs(""))
    

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = ev.Publish