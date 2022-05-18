<p align="center">
<img src="_images/Meting4Net.png" alt="Meting4Net">
</p>
<h1 align="center">Meting4Net</h1>

> :cake: Wow, such a powerful music API framework for .Net

[![repo size](https://img.shields.io/github/repo-size/yiyungent/Meting4Net.svg?style=flat)]()
[![LICENSE](https://img.shields.io/github/license/yiyungent/Meting4Net.svg?style=flat)](https://mit-license.org/)
[![nuget](https://img.shields.io/nuget/v/Meting4Net.svg?style=flat)](https://www.nuget.org/packages/Meting4Net/)
[![downloads](https://img.shields.io/nuget/dt/Meting4Net.svg?style=flat)](https://www.nuget.org/packages/Meting4Net/)


## Introduction

Meting4Net: <a href="https://github.com/metowolf/Meting" target="_blank">Meting</a> for .Net, thanks to <a href="https://github.com/metowolf/Meting" target="_blank">Meting</a>.   

A powerful music API framework to accelerate your development
 + **Elegant** - Easy to use, a standardized format for all music platforms.
 + **Powerful** - Support various music platforms, including Tencent, NetEase, Xiami, KuGou, Baidu and more.
 + **Free** - Under MIT license, need I say more?
 
## Progress

- [x] 网易云音乐 Meting Open API 移植完成 v0.1.0
- [x] 腾讯QQ音乐 Meting Open API 移植完成 v0.2.0

## Requirement

Only need to match one.

- .NET Framework (>= 4.5) and Newtonsoft.Json (>= 12.0.1) installed.
- .NET Standard (>= 2.0) and Microsoft.CSharp (>= 4.5.0), and Newtonsoft.Json (>= 12.0.1) installed.

## Installation

Require this package, with [NuGet](https://www.nuget.org/packages/Meting4Net), in the root directory of your project, if you use Visual Studio, then click **Tools** -> **NuGet Package Manager** -> **Package Manager Console** , make sure "Default project" is the project you want to install, enter the command below to install.

```bash
PM> Install-Package Meting4Net
```

## Quick Start

```csharp
using Meting4Net.Core;
   ...
// Initialize to netease API
Meting api = new Meting("netease");
// Get data
string jsonStr = api.FormatMethod(true).Search("Soldier", new Meting4Net.Core.Models.Standard.Options
{
    page = 1,
    limit = 50
});

return Content(jsonStr, "application/json");
//[{"id":35847388,"name":"Hello","artist":["Adele"],"album":"Hello","pic_id":"1407374890649284","url_id":35847388,"lyric_id":35847388,"source":"netease"},{"id":33211676,"name":"Hello","artist":["OMFG"],"album":"Hello",...
```

## Environment

- Operating environment: .NET Framework (>= 4.5) or .NET Standard (>= 2.0)    
- Development environment: Visual Studio Community 2017

## Related Projects

 - [Meting](https://github.com/metowolf/Meting)
 
 