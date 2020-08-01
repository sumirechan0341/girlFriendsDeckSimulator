namespace GirlFriendDeckSimulator
open System.ComponentModel
open FSharp.Data
open System.IO
open GirlFactory

type SpecialBonusEditViewModel() =
    let ev = Event<_, _>()
    let mutable raidNormal = 0
    let mutable raidSuperRare = 0
    let mutable raidCoolTrio = 0
    let mutable raidPopTrio = 0
    let mutable raidSweetTrio = 0
    let mutable raidCoolMega = 0
    let mutable raidPopMega = 0
    let mutable raidSweetMega = 0

    let mutable huntersNormal = 0
    let mutable huntersSuperRare = 0
    let mutable huntersNocturnalRare = 0

    let mutable memorialStorySpecialGirl1 = "指定なし"
    let mutable memorialStorySpecialGirl2 = "指定なし"

    member this.RaidNormal
        with get() = raidNormal
        and set(newRaidNormal) =
            raidNormal <- newRaidNormal
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()
    member this.RaidSuperRare
        with get() = raidSuperRare
        and set(newRaidSuperRare) =
            raidSuperRare <- newRaidSuperRare
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()
    member this.RaidCoolTrio
        with get() = raidCoolTrio
        and set(newRaidCoolTrio) =
            raidCoolTrio <- newRaidCoolTrio
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()
    member this.RaidPopTrio
        with get() = raidPopTrio
        and set(newRaidPopTrio) =
            raidPopTrio <- newRaidPopTrio
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()
    member this.RaidSweetTrio
        with get() = raidSweetTrio
        and set(newRaidSweetTrio) =
            raidSweetTrio <- newRaidSweetTrio
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()
    member this.RaidCoolMega
        with get() = raidCoolMega
        and set(newRaidCoolMega) =
            raidCoolMega <- newRaidCoolMega
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()
    member this.RaidPopMega
        with get() = raidPopMega
        and set(newRaidPopMega) =
            raidPopMega <- newRaidPopMega
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()
    member this.RaidSweetMega
        with get() = raidSweetMega
        and set(newRaidSweetMega) =
            raidSweetMega <- newRaidSweetMega
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()
    member this.HuntersNormal
        with get() = huntersNormal
        and set(newHuntersNormal) =
            huntersNormal <- newHuntersNormal
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()
    member this.HuntersSuperRare
        with get() = huntersSuperRare
        and set(newHuntersSuperRare) =
            huntersSuperRare <- newHuntersSuperRare
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()
    member this.HuntersNocturnalRare
        with get() = huntersNocturnalRare
        and set(newHuntersNocturnalRare) =
            huntersNocturnalRare <- newHuntersNocturnalRare
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()
    member val GirlList = girlFactroyFromJson |> Seq.filter(fun g -> g.birthday.IsSome) |> Seq.map(fun g -> g.name) |> Seq.append (seq{"指定なし"}) with get, set
    member this.MemorialStorySpecialGirl1
        with get() = memorialStorySpecialGirl1
        and set(newMemorialStorySpecialGirl1) =
            memorialStorySpecialGirl1 <- newMemorialStorySpecialGirl1
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()
    member this.MemorialStorySpecialGirl2
        with get() = memorialStorySpecialGirl2
        and set(newMemorialStorySpecialGirl2) =
            memorialStorySpecialGirl2 <- newMemorialStorySpecialGirl2
            ev.Trigger(this, PropertyChangedEventArgs(""))
            this.save()

    member this.save() =
        let jsonData = 
            JsonValue.Record[|
                "raidNormal", JsonValue.Number <| (this.RaidNormal |> decimal);
                "raidSuperRare", JsonValue.Number <| (this.RaidSuperRare |> decimal);
                "raidCoolTrio", JsonValue.Number <| (this.RaidCoolTrio |> decimal);
                "raidPopTrio", JsonValue.Number <| (this.RaidPopTrio |> decimal);
                "raidSweetTrio", JsonValue.Number <| (this.RaidSweetTrio |> decimal);
                "raidCoolMega", JsonValue.Number <| (this.RaidCoolMega |> decimal);
                "raidPopMega", JsonValue.Number <| (this.RaidPopMega |> decimal);
                "raidSweetMega", JsonValue.Number <| (this.RaidSweetMega |> decimal);
                "huntersNormal", JsonValue.Number <| (this.HuntersNormal |> decimal);
                "huntersSuperRare", JsonValue.Number <| (this.HuntersSuperRare |> decimal);
                "huntersNocturnalRare", JsonValue.Number <| (this.HuntersNocturnalRare |> decimal);
            |]
        let writer = File.CreateText(".\SpecialBonus.json")
        jsonData.WriteTo(writer, JsonSaveOptions.None)
        writer.Close()
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = ev.Publish
module SpecialBonusEditViewModel =
    type SpecialBonusElements = JsonProvider<""" 
        {
          "raidNormal": 0,
          "raidSuperRare": 0,
          "raidCoolTrio": 0,
          "raidPopTrio": 0,
          "raidSweetTrio": 0,
          "raidCoolMega": 2338500,
          "raidPopMega": 0,
          "raidSweetMega": 0,
          "huntersNormal": 0,
          "huntersSuperRare": 0,
          "huntersNocturnalRare": 0
        }
        """>
    let loadSpecialBonusEditViewModel =
        let newSpecialBonusEditViewModel = SpecialBonusEditViewModel()
        if File.Exists(".\SpecialBonus.json")
        then
            let specialBonusElements = SpecialBonusElements.Parse(File.ReadAllText(".\SpecialBonus.json"))
            newSpecialBonusEditViewModel.RaidNormal <- specialBonusElements.RaidNormal
            newSpecialBonusEditViewModel.RaidSuperRare <- specialBonusElements.RaidSuperRare
            newSpecialBonusEditViewModel.RaidCoolTrio <- specialBonusElements.RaidCoolTrio
            newSpecialBonusEditViewModel.RaidPopTrio <- specialBonusElements.RaidPopTrio
            newSpecialBonusEditViewModel.RaidSweetTrio <- specialBonusElements.RaidSweetTrio
            newSpecialBonusEditViewModel.RaidCoolMega <- specialBonusElements.RaidCoolMega
            newSpecialBonusEditViewModel.RaidPopMega <- specialBonusElements.RaidPopMega
            newSpecialBonusEditViewModel.RaidSweetMega <- specialBonusElements.RaidSweetMega
            newSpecialBonusEditViewModel.HuntersNormal <- specialBonusElements.HuntersNormal
            newSpecialBonusEditViewModel.HuntersSuperRare <- specialBonusElements.HuntersSuperRare
            newSpecialBonusEditViewModel.HuntersNocturnalRare <- specialBonusElements.HuntersNocturnalRare
            Some(newSpecialBonusEditViewModel)
        else 
            None
            
        

