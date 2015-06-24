# Collection2Model

##Version
1.0.0

## 日本語版README

### 用途

GETとかPOSTのパラメータを処理するのに疲れて作りました :moyai:  

#### 使い方
ざっくりこんな風に使えます。

```csharp
class HogeViewModel
{
  public int IntProp { get; set; }
  public bool BoolProp { get; set; }
  public String StrProp { get; set; }
  // etc...
}

// sample GET request
// http://example.com?IntProp=1&StrProp=fuga&BoolProp=true
var ret = Mapper.MappingFromNameValueCollection<HogeViewModel>(Request.QueryString);

// or POST request
var ret = Mapper.MappingFromNameValueCollection<HogeViewModel>(Request.Form);
```

#### 限界

* Convertできない型には変換できません。  
  - `Reflection`しつつ`Covnert.ChangeType`で殴っているので。
  - ~~`List<String>`でプロパティ名を指定して変換対象外にできます~~  ← できなくなりました。
  - 無視したいプロパティは`IgnorePropertyAttribute`で修飾するようになりました。
* `System.ComponentModel.DataAnnotations.ValidationAttribute`にしたがってバリデーションできます。
  - `StringLength`
  - `Range`
  - `Required`
* 評価順序は以下に従います
  - `Required`
  - `Convert.ChangeType`のFormatチェック
  - `Required`以外の`ValidationAttribute`

```csharp
class HogeViewModel
{
  [IgnoreProperty]
  public int Ignored { get; set; }
}

// http://example.com?Ignored=1
var ret = Mapper.MappingFromNameValueCollection<HogeViewModel>(Request.QueryString);
Console.WriteLine(ret.Ignored); // => 0 default(int)
```
