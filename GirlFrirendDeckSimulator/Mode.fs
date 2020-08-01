namespace GirlFriendDeckSimulator

module Mode =
    type Mode = Attack | Defence | AttackAndDefence
    
    type Mode with
        member this.IsAppliable(other: Mode) =
            match this with
            | Attack -> 
                match other with
                | Attack -> true
                | otherwise -> false
            | Defence ->
                match other with
                | Defence -> true
                | otherwise -> false
            | AttackAndDefence -> true