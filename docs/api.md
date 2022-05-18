## Search()关键词搜索歌曲

按关键词查询歌曲

```csharp
// 返回 json (多条满足关键词的歌曲)
string Search(string keyword, Options options = null)
```

#### 参数

| 参数 | 必须 | 描述 |
| :------: | :------: | :------: |
| keyword | √ | 搜索关键词 |
| options |   | 选项类，包括 limit,page,type 属性。limit,page,type 依次默认 30,1,1 |

#### 返回

| 属性 | 描述 |
| :------: | :------: |
|  |  |
|  |  |

## Song()单曲

根据歌曲ID获取单首歌曲

```csharp
string Song(string id)
```

#### 参数

| 参数 | 必须 | 描述 |
| :------: | :------: | :------: |
| id | √ | 歌曲ID |

#### 返回

| 属性 | 描述 |
| :------: | :------: |
|  |  |
|  |  |

## Album()专辑

根据专辑ID获取

```csharp
string Album(string id)
```

#### 参数

| 参数 | 必须 | 描述 |
| :------: | :------: | :------: |
| id | √ | 专辑ID |

#### 返回

| 属性 | 描述 |
| :------: | :------: |
|  |  |
|  |  |

## Artist()作家/歌手

根据作家ID获取

```csharp
string Artist(string id, int limit = 50)
```

#### 参数

| 参数 | 必须 | 描述 |
| :------: | :------: | :------: |
| id | √ | 作家ID |

#### 返回

| 属性 | 描述 |
| :------: | :------: |
|  |  |
|  |  |

## Playlist()歌单

根据歌单ID获取

```csharp
string Playlist(string id)
```

#### 参数

| 参数 | 必须 | 描述 |
| :------: | :------: | :------: |
| id | √ | 歌单ID |

#### 返回

| 属性 | 描述 |
| :------: | :------: |
|  |  |
|  |  |

## Url()音乐链接

根据歌曲ID获取音乐链接

```csharp
string Url(string id, int br = 320)
```

#### 参数

| 参数 | 必须 | 描述 |
| :------: | :------: | :------: |
| id | √ | 歌曲ID |
| br |   | 比特率, 默认 320 |

#### 返回

| 属性 | 描述 |
| :------: | :------: |
|  |  |
|  |  |

## Lyric()歌词

根据歌曲ID查歌词

```csharp
string Lyric(string id)
```

#### 参数

| 参数 | 必须 | 描述 |
| :------: | :------: | :------: |
| id | √ | 歌曲ID |

#### 返回

| 属性 | 描述 |
| :------: | :------: |
|  |  |
|  |  |

## Pic()封面图

歌曲图片(对指定歌曲编号，返回图片地址)

```csharp
/// <summary>
/// 歌曲图片(对指定歌曲编号，返回图片地址)
/// </summary>
/// <param name="id">eg.传递通过 api.Song("35847388") 获取到的 pic_id</param>
/// <param name="size"></param>
/// <returns></returns>
public string Pic(string id, int size = 300)
```

#### 参数

| 参数 | 必须 | 描述 |
| :------: | :------: | :------: |
| id | √ | 图片ID（pic_id） |
| size |   | 图片尺寸,默认300*300 |

#### 返回

| 属性 | 描述 |
| :------: | :------: |
| url | 歌曲封面图片链接地址 |
|  |  |

## FormatMethod()统一格式

是否格式化(统一返回的 json 歌曲信息格式)

```csharp
/// <summary>
/// 是否格式化
/// </summary>
/// <param name="value">默认格式化 true</param>
/// <returns></returns>
Meting FormatMethod(bool value = true)
```

> Note: 一旦调用此方法，不传参时为格式化，而若不调用此方法，也不通过设置属性Format=true, 那么默认 为 false,不格式化

#### 参数

| 参数 | 必须 | 描述 |
| :------: | :------: | :------: |
| value |   | 是否格式化 |

#### 返回

| 属性 | 描述 |
| :------: | :------: |
|  |  |
|  |  |








