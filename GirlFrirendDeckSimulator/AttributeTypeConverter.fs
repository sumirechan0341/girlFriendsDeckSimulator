namespace GirlFriendDeckSimulator
open AttributeType
open Converter
module AttributeTypeConverter =
    let toString(attrType: AttributeType): string =
        match attrType with
        | AttributeType.Cool -> "Cool"
        | AttributeType.Pop -> "Pop"
        | AttributeType.Sweet -> "Sweet"

    let fromString(attrTypeName: string): AttributeType = 
        match attrTypeName with
        | "Cool" -> AttributeType.Cool
        | "Pop" -> AttributeType.Pop
        | "Sweet" -> AttributeType.Sweet
        | "COOLタイプ" -> AttributeType.Cool
        | "POPタイプ" -> AttributeType.Pop
        | "SWEETタイプ" -> AttributeType.Sweet

type AttributeTypeConverter() =
    inherit ConverterBase(AttributeTypeConverter.toString >> (fun s -> s :> obj) |> convert, AttributeTypeConverter.fromString >> (fun a -> a :> obj) |> convert)


