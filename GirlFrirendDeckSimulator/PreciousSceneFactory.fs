namespace GirlFriendDeckSimulator
open System
open FSharp.Data
open System.IO
open PreciousScene
open System.Text.RegularExpressions

module PreciousSceneFactory =
    type PreciousSceneElements = JsonProvider<""" {"sceneName":"[靴箱の邂逅]上条るい","level":3,"effect":"COOLガールのコストが高いほど攻援UP","effectMaxTerms":"コスト23","effectMax":"最大4.0%"} """>

    let preciousSceneFactoryFromJson(preciousSceneJson) = 
        let preciousSceneElements = PreciousSceneElements.Parse(preciousSceneJson)

        {
            sceneName = preciousSceneElements.SceneName;
            level = preciousSceneElements.Level;
            sceneEffect = SceneEffectConverter.fromString(preciousSceneElements.Effect, preciousSceneElements.EffectMaxTerms);
            effectNumType = if preciousSceneElements.EffectMax.[preciousSceneElements.EffectMax.Length-1] = '%' then EffectNumType.Percentage else EffectNumType.Value
            effectMaxNum = Regex.Match(preciousSceneElements.EffectMax, "(?:最大)?(\d+(?:\.\d+)?)(?:%)?").Groups.[1].Value |> float
            sceneJson = preciousSceneElements.JsonValue
        }

    let preciousSceneList = 
        Seq.map(preciousSceneFactoryFromJson) <| 
        if File.Exists(".\PreciousSceneList.txt")
        then
            Seq.filter (String.IsNullOrWhiteSpace >> not) (File.ReadLines(".\PreciousSceneList.txt"))
        else Seq.empty