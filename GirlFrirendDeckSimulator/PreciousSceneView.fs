namespace GirlFriendDeckSimulator
open PreciousScene
open SkillAttributeTypeConverter

module PreciousSceneView = 
    type PreciousSceneView(preciousScene: PreciousScene) =
        member val PreciousScene = preciousScene
        member val SceneName = preciousScene.sceneName
        member val Level = preciousScene.level
        member val SceneEffect = SceneEffectConverter.toString(preciousScene.sceneEffect)
        member val EffectMax = preciousScene.effectMaxNum.ToString("F1") + EffectNumTypeConverter.toString(preciousScene.effectNumType)
        member val TargetAttribute = SkillAttributeTypeConverter.toString(preciousScene.sceneEffect.sceneTargetAttribute)