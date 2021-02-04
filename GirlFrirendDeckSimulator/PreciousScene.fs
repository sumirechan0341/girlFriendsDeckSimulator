namespace GirlFriendDeckSimulator
open Mode
open Card
open EffectNumType
open SkillAttributeType
open FSharp.Data

module PreciousScene =
    type SceneEffectType = CostType of int | ExedGirlNum of int | SkillLevel of int | Rarity of Card.Rarity | SpecificGirl of int | Uniform | WholeExedGirlNum of int

    type SceneTargetType = FrontDeck | BackDeck | All

    type SceneEffect = {
        sceneEffectType: SceneEffectType
        sceneTarget: SceneTargetType
        sceneTargetAttribute: SkillAttributeType
        mode: Mode
    }

    type PreciousScene = {
        sceneName: string
        level: int
        sceneEffect: SceneEffect
        effectNumType: EffectNumType
        effectMaxNum: float
        sceneJson: JsonValue
    }
