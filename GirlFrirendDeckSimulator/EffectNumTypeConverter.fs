namespace GirlFriendDeckSimulator
open EffectNumType
module EffectNumTypeConverter =
    let toString(effectNumType) =
    match effectNumType with
    | Percentage -> "%"
    | Value -> ""