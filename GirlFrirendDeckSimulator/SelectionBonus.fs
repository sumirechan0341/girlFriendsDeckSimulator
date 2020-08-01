namespace GirlFriendDeckSimulator
open Mode
open System
module SelectionBonus =
    type SelectionBonus(selectionBonusName: string) =
        interface IComparable with
            member this.CompareTo(other: obj) =
                this.getSelectionBonusId.CompareTo((other:?>SelectionBonus).getSelectionBonusId)
                
        member val SelectionBonusName = selectionBonusName
        
        member _.getSelectionBonusId =
            match selectionBonusName with
            | "生活指導" -> 0
            | "★☆部長バースト☆★" -> 1
            | "！ゴールドフィンガーズ！" -> 2
            | "おかえりby帰宅部応援団" -> 3
            | "Ｌｅｔ’ｓ 精神集中♪" -> 4
            | "応援五重奏" -> 5
            | "☆ミラクルパフォーマンス★" -> 6
            | "マニアックトルネード" -> 7
            | "スポーティーザッパー" -> 8
            | "ファイターチアーズ！" -> 9
            | "委員会ペンタゴン" -> 10
            | "☆コスメティックパワー☆" -> 11
            | "ザ・コレクターズパワー" -> 12
            | "クリエイティブ魂" -> 13
            | "ロンリーガールズレクイエム" -> 14
            | "歴史より愛をこめて" -> 15
            | "算術スクレイパー" -> 16
            | "シーフードランデブー" -> 17
            | "♪ＮＯＴスイーツ女子パワー♪" -> 18
            | "↑ＮＯＴ肉食女子パワー↑" -> 19
            | "セクシーガールズ★キッス" -> 20
            | "↑スレンダーズボルケーノ↑" -> 21
            | "グラマラスソウル" -> 22
            | "クールビューティーフラッシュ" -> 23
            | "ミクロズ☆チアフル" -> 24
            | "センター★サポート" -> 25
            | "ふわふわらっぴんぐ" -> 26
            | "ライク★アニマル★チアーズ" -> 27
            | "未知への探究心" -> 28
            | "シャイニング★スプラッシュ" -> 29
            | "JUMP↑JUMP↑JUMP" -> 30
            | "にゅーろん★くりぃむそふと" -> 31
            | "Other☆School☆Girls" -> 32
            | "Precious★Friend" -> 33
            | _ -> failwith "サポートされていないセンバツボーナスが含まれています。"

        member _.getSelectionBonusMode =
            match selectionBonusName with
            | "生活指導" -> AttackAndDefence
            | "★☆部長バースト☆★" -> AttackAndDefence
            | "！ゴールドフィンガーズ！" -> Defence
            | "おかえりby帰宅部応援団" -> Defence
            | "Ｌｅｔ’ｓ 精神集中♪" -> Attack
            | "応援五重奏" -> Defence
            | "☆ミラクルパフォーマンス★" -> Attack
            | "マニアックトルネード" -> Defence
            | "スポーティーザッパー" -> Attack
            | "ファイターチアーズ！" -> Attack
            | "委員会ペンタゴン" -> AttackAndDefence
            | "☆コスメティックパワー☆" -> AttackAndDefence
            | "ザ・コレクターズパワー" -> Defence
            | "クリエイティブ魂" -> Defence
            | "ロンリーガールズレクイエム" -> Attack
            | "歴史より愛をこめて" -> Attack
            | "算術スクレイパー" -> Attack
            | "シーフードランデブー" -> Attack
            | "♪ＮＯＴスイーツ女子パワー♪" -> Defence
            | "↑ＮＯＴ肉食女子パワー↑" -> Defence
            | "セクシーガールズ★キッス" -> AttackAndDefence
            | "↑スレンダーズボルケーノ↑" -> Defence
            | "グラマラスソウル" -> Attack
            | "クールビューティーフラッシュ" -> Attack
            | "ミクロズ☆チアフル" -> Defence
            | "センター★サポート" -> Attack
            | "ふわふわらっぴんぐ" -> Attack
            | "ライク★アニマル★チアーズ" -> Attack
            | "未知への探究心" -> AttackAndDefence
            | "シャイニング★スプラッシュ" -> AttackAndDefence
            | "JUMP↑JUMP↑JUMP" -> AttackAndDefence
            | "にゅーろん★くりぃむそふと" -> Attack
            | "Other☆School☆Girls" -> AttackAndDefence
            | "Precious★Friend" -> AttackAndDefence
            | _ -> failwith "サポートされていないセンバツボーナスが含まれています。"

    let getAllSelectionBonus =
        Array.map (SelectionBonus) [|
            "生活指導";
            "★☆部長バースト☆★";
            "！ゴールドフィンガーズ！";
            "おかえりby帰宅部応援団";
            "Ｌｅｔ’ｓ 精神集中♪";
            "応援五重奏";
            "☆ミラクルパフォーマンス★";
            "マニアックトルネード";
            "スポーティーザッパー";
            "ファイターチアーズ！";
            "委員会ペンタゴン";
            "☆コスメティックパワー☆";
            "ザ・コレクターズパワー";
            "クリエイティブ魂";
            "ロンリーガールズレクイエム";
            "歴史より愛をこめて";
            "算術スクレイパー";
            "シーフードランデブー";
            "♪ＮＯＴスイーツ女子パワー♪";
            "↑ＮＯＴ肉食女子パワー↑";
            "セクシーガールズ★キッス";
            "↑スレンダーズボルケーノ↑";
            "グラマラスソウル";
            "クールビューティーフラッシュ";
            "ミクロズ☆チアフル";
            "センター★サポート";
            "ふわふわらっぴんぐ";
            "ライク★アニマル★チアーズ";
            "未知への探究心";
            "シャイニング★スプラッシュ";
            "JUMP↑JUMP↑JUMP";
            "にゅーろん★くりぃむそふと";
            "Other☆School☆Girls";
            "Precious★Friend";
        |]