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
    let rec toString(skillAttrType: SkillAttributeType): string =
        match skillAttrType with
        | SkillAttributeType.Cool -> "COOL"
        | SkillAttributeType.Pop -> "POP"
        | SkillAttributeType.Sweet -> "SWEET"
        | SkillAttributeType.All -> "全タイプ"
        | SkillAttributeType.Combination(attr1, attr2) -> toString(attr1) + "&" + toString(attr2)

    let rec fromString(skillAttrTypeStr) =
        match skillAttrTypeStr with
        | "COOL" | "COOLタイプ" | "COOLﾀｲﾌﾟ" -> SkillAttributeType.Cool
        | "POP" | "POPタイプ" | "POPﾀｲﾌﾟ" -> SkillAttributeType.Pop
        | "SWEET" | "SWEETタイプ" | "SWEETﾀｲﾌﾟ" -> SkillAttributeType.Sweet
        | "全タイプ" | "全ﾀｲﾌﾟ" -> SkillAttributeType.All
        | "" -> SkillAttributeType.All // 全タイプのときタイプが明記されないことがある
        // | combiAttrStr -> SkillAttributeType.Combination(fromString(combiAttrStr.Split('&').[0]), fromString(combiAttrStr.Split('&').[1]))
        // 仲良し未実装