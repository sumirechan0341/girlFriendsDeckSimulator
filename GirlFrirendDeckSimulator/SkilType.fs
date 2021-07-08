namespace GirlFriendDeckSimulator
open SkillAttributeType
open Mode
open Card
open FSharp.Data
open System.IO

module SkillType =
    type Settings = JsonProvider<".\Setting.json">
    type SkillTarget = 
        Front 
        | Back of targetNum: int
        | FrontAndBack1 
        | MySelf 
        | SameGirl 
        | OpponentFront

    type Effect = 
        Ultra
        | ExtraSuper
        | Super 
        | ExtraLarge 
        | Large 
        | Middle 
        | Small
        | Random of min: Effect * max: Effect
    
    type SkillMode = Up | Down

    type SkillType = 
        { 
            attribute: SkillAttributeType
            target: SkillTarget
            effect: Effect
            mode: Mode
            skillMode: SkillMode
            skillEnchantLevel: int
        }

        member this.toBonusPercentage(card: Card) =
            // ボーナス計算するために設定ファイル読み込み
            let settings = Settings.Parse(File.ReadAllText(".\Setting.json"))
            match this.skillMode with
            | SkillMode.Down -> 0
            | SkillMode.Up ->
                match this.target with
                | SkillTarget.Front ->
                    if this.attribute.isAppliableOn(card.attribute) && not (this.attribute = SkillAttributeType.All) then 
                        1
                    else 0