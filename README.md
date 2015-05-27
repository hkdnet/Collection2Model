# Collection2Model

## 日本語版README

### 用途

GETパラメータを処理するのに疲れて作りました :moyai:  

#### 使い方
ざっくりこんな風に使えます。

```csharp
class HogeViewModel
{
  public int IntProp { get; set; }
  public bool BoolProp { get; set; }
  public String StrPropUpper { get; set; }
  // etc...
}

// sample url
// http://example.com?IntProp=1&StrPropUser=fuga&BoolProp=true
var ret = Mapper.MappingFromNameValueCollection<HogeViewModel>(Request.QueryString);
```

#### 限界

`Reflection`しつつ`Covnert.ChangeType`で殴っているのでConvertできない型には変換できません。  
~~いったん`List<String>`でプロパティ名を指定して変換対象外にしています。~~  
`Attribute`で指定するようになりました
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
