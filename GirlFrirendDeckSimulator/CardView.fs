namespace GirlFriendDeckSimulator
open Card
open System.Data
open System
open PetitGirl
open EventType
open System.ComponentModel
open Player

module CardView =
    type CardView(c: Card) =
        let ev = Event<_, _>()
        let mutable attack = c.attack
        let mutable defence = c.defence
        let mutable isFavoriteCard = false
        let mutable isDatingCard = false
        let mutable isTouched = false
        let correctedVals = {|CorrectedAttack = 0; CorrectedDefence = 0;|}
        let mutable straps = [||]
        let mutable index = ""
        let mutable isTriggeredSkillBonus = false
        
        member this.Straps
            with get() = straps
            and set(newStraps) =
                straps <- newStraps
                ev.Trigger(this, PropertyChangedEventArgs(""))

        member val Card = c
        member val CardName = 
            (match c.eventName with
            | None -> c.girl.name
            | Some(evName) -> "[" + evName + "]" + c.girl.name) with get, set
        member this.Attack 
            with get() = attack
            and set(newAttack) = 
                attack <- newAttack
                ev.Trigger(this, PropertyChangedEventArgs(""))
        member this.Defence
            with get() = defence
            and set(newDefence) =
                defence <- newDefence
                ev.Trigger(this, PropertyChangedEventArgs(""))
        member this.IsFavoriteCard
            with get() = isFavoriteCard
            and set(newFavoriteCard) =
                isFavoriteCard <- newFavoriteCard
                ev.Trigger(this, PropertyChangedEventArgs(""))
        member this.IsDatingCard
            with get() = isDatingCard
            and set(newDatingCard) =
                isDatingCard <- newDatingCard
                ev.Trigger(this, PropertyChangedEventArgs(""))
        member this.IsTouched
            with get() = isTouched
            and set(newTouched) =
                isTouched <- newTouched
                ev.Trigger(this, PropertyChangedEventArgs(""))
        member this.Index
            with get() = index
            and set(newIndex) =
                index <- newIndex
                ev.Trigger(this, PropertyChangedEventArgs(""))

        member val AppliedAttackSkillBonus: ResizeArray<float> = [||] |> ResizeArray
        member val AppliedDefenceSkillBonus: ResizeArray<float> = [||] |> ResizeArray
        member val Attribute = c.attribute with get, set
        member val CorrectedAttack = correctedVals.CorrectedAttack with get, set
        member val CorrectedDefence = correctedVals.CorrectedDefence with get, set
        member val Rarity = CardConverter.RarityConverter.toString(c.rarity)
        member val SkillType = 
            (match c.skillType with 
            | None -> ""
            | Some(skillType) -> SkillTypeConverter.toString(skillType)) with get, set
        member val SkillLevel = c.skillLevel with get, set

        member this.IsTriggeredSkillBonus
            with get() = isTriggeredSkillBonus
            and set(newIsTriggeredSkillBonus) =
                isTriggeredSkillBonus <- newIsTriggeredSkillBonus
                ev.Trigger(this, PropertyChangedEventArgs(""))
        
        interface INotifyPropertyChanged with
            [<CLIEvent>]
            member this.PropertyChanged = ev.Publish

        interface IEquatable<CardView> with
            member this.Equals(other) =
                this.CardName = other.CardName &&
                this.Attack = other.Attack &&
                this.Defence = other.Defence &&
                this.Attribute = other.Attribute &&
                this.SkillType = other.SkillType &&
                this.SkillLevel = other.SkillLevel &&
                this.IsFavoriteCard = other.IsFavoriteCard &&
                this.IsDatingCard = other.IsDatingCard

    type CardViewWithStrap(c: Card, strap1, strap2, strap3, strap4) as this =
        inherit CardView(c)
        let ev = Event<_, _>()
        let mutable strap1 = strap1
        let mutable strap2 = strap2
        let mutable strap3 = strap3
        let mutable strap4 = strap4
        let mutable strap1Str = strap1.ToString()
        let mutable strap2Str = strap2.ToString()
        let mutable strap3Str = strap3.ToString()
        let mutable strap4Str = strap4.ToString()
        do
            this.Straps <- [|strap1; strap2; strap3; strap4|]

        member this.removeStraps = 
            this.Straps <- [||]
            this
        member this.Strap1 
            with get() = strap1
            and set(newStrap) =
                strap1 <- newStrap
                this.Straps.[0] <- strap1
                ev.Trigger(this, PropertyChangedEventArgs(""))
        member this.Strap1Str
            with get() = strap1Str
            and set(newStrap1Str) =
                let result = newStrap1Str |> float |> ref
                strap1Str <- newStrap1Str
                if System.Double.TryParse(newStrap1Str, result)
                    then 
                        if 0.0 <= result.Value && result.Value <= 27.6
                            then 
                                this.Strap1 <- result.Value 
                                ev.Trigger(this, PropertyChangedEventArgs(""))
                            else 
                                ev.Trigger(this, PropertyChangedEventArgs(""))
                    else ev.Trigger(this, PropertyChangedEventArgs(""))
        member this.Strap2
            with get() = strap2
            and set(newStrap) =
                strap2 <- newStrap
                this.Straps.[1] <- strap2
                ev.Trigger(this, PropertyChangedEventArgs(""))
        member this.Strap2Str
            with get() = strap2Str
            and set(newStrap2Str) =
                let result = newStrap2Str |> float |> ref
                strap2Str <- newStrap2Str
                if System.Double.TryParse(newStrap2Str, result)
                    then 
                        if 0.0 <= result.Value && result.Value <= 27.6
                            then 
                                this.Strap2 <- result.Value 
                                ev.Trigger(this, PropertyChangedEventArgs(""))
                            else 
                                ev.Trigger(this, PropertyChangedEventArgs(""))
                    else ev.Trigger(this, PropertyChangedEventArgs(""))
        member this.Strap3 
            with get() = strap3
            and set(newStrap) =
                strap3 <- newStrap
                this.Straps.[2] <- strap3
                ev.Trigger(this, PropertyChangedEventArgs(""))
        member this.Strap3Str
            with get() = strap3Str
            and set(newStrap3Str) =
                let result = newStrap3Str |> float |> ref
                strap3Str <- newStrap3Str
                if System.Double.TryParse(newStrap3Str, result)
                    then 
                        if 0.0 <= result.Value && result.Value <= 27.6
                            then 
                                this.Strap3 <- result.Value 
                                ev.Trigger(this, PropertyChangedEventArgs(""))
                            else 
                                ev.Trigger(this, PropertyChangedEventArgs(""))
                    else ev.Trigger(this, PropertyChangedEventArgs(""))
        member this.Strap4 
            with get() = strap4
            and set(newStrap) =
                strap4 <- newStrap
                this.Straps.[3] <- strap4
                ev.Trigger(this, PropertyChangedEventArgs(""))
        member this.Strap4Str
            with get() = strap4Str
            and set(newStrap4Str) =
                let result = newStrap4Str |> float |> ref
                strap4Str <- newStrap4Str
                if System.Double.TryParse(newStrap4Str, result)
                    then 
                        if 0.0 <= result.Value && result.Value <= 27.6
                            then 
                                this.Strap4 <- result.Value 
                                ev.Trigger(this, PropertyChangedEventArgs(""))
                            else 
                                ev.Trigger(this, PropertyChangedEventArgs(""))
                    else ev.Trigger(this, PropertyChangedEventArgs(""))
        new(c) = CardViewWithStrap(c, 0.0, 0.0, 0.0, 0.0)
        interface INotifyPropertyChanged with
            [<CLIEvent>]
            member this.PropertyChanged = ev.Publish