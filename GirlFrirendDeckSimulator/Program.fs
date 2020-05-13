open System
open System.Windows
open System.Windows.Controls
open System.Windows.Markup
open FsXaml
open System.ComponentModel
open System.Diagnostics

 
 type DeckEditWindow = XAML<"DeckEditWindow.xaml">
 
[<STAThread>]
[<EntryPoint>]
let main argv =
    //initializeでフォーム生成
    let window = DeckEditWindow()
    //loadでセーブデータがあれば読み込み
    //なければ初期状態から
    
    //window.DataContext <- vm
    let application = Application()
    
    application.Run(window) 

