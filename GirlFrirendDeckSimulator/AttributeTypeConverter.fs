namespace GirlFriendDeckSimulator
open AttributeType
open SkillAttributeType
open Converter
module AttributeTypeConverter =
    let toString(attrType: AttributeType): string =
        match attrType with
        | AttributeType.Cool -> "COOL"
        | AttributeType.Pop -> "POP"
        | AttributeType.Sweet -> "SWEET"

    let fromString(attrTypeName: string): AttributeType = 
        match attrTypeName with
        | "COOL" -> AttributeType.Cool
        | "POP" -> AttributeType.Pop
        | "SWEET" -> AttributeType.Sweet
        | "COOLタイプ" -> AttributeType.Cool
        | "POPタイプ" -> AttributeType.Pop
        | "SWEETタイプ" -> AttributeType.Sweet

type AttributeTypeConverter() =
    inherit ConverterBase(AttributeTypeConverter.toString >> (fun s -> s :> obj) |> convert, AttributeTypeConverter.fromString >> (fun a -> a :> obj) |> convert)

module SkillAttributeTypeConverter =
    let toString(skillAttrType: SkillAttributeType): string =
        match skillAttrType with
        | SkillAttributeType.Cool -> "COOL"
        | SkillAttributeType.Pop -> "POP"
        | SkillAttributeType.Sweet -> "SWEET"
        | SkillAttributeType.All -> "全タイプ"

    let fromString(skillAttrTypeStr) =
        match skillAttrTypeStr with
        | "COOL" -> SkillAttributeType.Cool
        | "POP" -> SkillAttributeType.Pop
        | "SWEET" -> SkillAttributeType.Sweet
        | "COOLタイプ" -> SkillAttributeType.Cool
        | "POPタイプ" -> SkillAttributeType.Pop
        | "SWEETタイプ" -> SkillAttributeType.Sweet
        | "全タイプ" -> SkillAttributeType.All



