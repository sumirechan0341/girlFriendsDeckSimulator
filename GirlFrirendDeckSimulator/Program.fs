open System
open System.Windows
open System.Windows.Controls
open System.Windows.Markup
open FsXaml
open System.ComponentModel
open System.Diagnostics

 
 type PlayerParameterWindow = XAML<"PlayerParameterWindow.xaml">
 
[<STAThread>]
[<EntryPoint>]
let main argv =
    //initializeでフォーム生成
    let window = PlayerParameterWindow()
    //loadでセーブデータがあれば読み込み
    //なければ初期状態から
    
    //window.DataContext <- vm
    let application = Application()
    
    application.Run(window) 

