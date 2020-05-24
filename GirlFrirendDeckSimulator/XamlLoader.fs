namespace GirlFriendDeckSimulator

open System.Windows.Controls
open System.Windows
open System
open System.Windows.Markup


type XamlLoader() =
  inherit UserControl()
 
  static let OnXamlPathChanged(d:DependencyObject) (e:DependencyPropertyChangedEventArgs) =
    let x = e.NewValue :?> string
    let control = d :?> XamlLoader
 
    let stream = Application.GetResourceStream(new Uri(x, UriKind.Relative)).Stream
    let children = XamlReader.Load(stream)
    control.AddChild(children)
 
  static let XamlPathProperty =
    DependencyProperty.Register("XamlPath", typeof<string>, typeof<XamlLoader>, new PropertyMetadata(new PropertyChangedCallback(OnXamlPathChanged)))
 
  member this.XamlPath
    with get() =
      this.GetValue(XamlPathProperty) :?> string
           
    and  set(x:string) =
      this.SetValue(XamlPathProperty, x)
 
  member this.AddChild child =
    base.AddChild(child)
