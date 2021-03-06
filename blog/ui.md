# DailyTick 开发实录 —— UI 设计

上次的文章中描述了 DailyTick 的设计理念。经过两周左右的设计和开发，现在 DailyTick 的主要 UI 已经完成了原型的设计和初步的实现。既然是原型，当然看起来就有点粗糙。

## 主 UI

![主UI图]()

主 UI 是使用一个 `TabbedView` 实现的。一个用来记录，一个用来统计。当然，最终的完成版应该至少有 3 个 Tab，因为还需要有一个“设置”的 Tab。现在因为我还没想到有什么需要设置的，暂时没有写。在“记录”这个 tab 里，有两个组件：一个 `ListView` 和一个 `NControl` 写成的圆形按钮。使用的时候，只需要点一下下面那个圆形按钮，就可以形成一条记录。当然，为了防止误点，我这里设置了一个限制，就是两次点击之间需要大于 1 分钟。这带来一个麻烦就是：你不能记录小于 1 分钟的事件。比如你想看看自己每天做时间记录的过程消耗了多长时间，这个暂时办不到。

上面这个列表，其实我还是有点担心的：如果这个软件可以被使用 10 年（应该不太可能，因为那个时候，应该会有比手机更好用的设备），按照我现在记录的情况来看，一天大概有 30 条左右的记录，一年是 365 * 30 = 10950 条，10 年就是 10 万条，如果每天记录得细点，可能就有几十万条，那会不会崩呢？虽然可以使用 Cell 的重用机制，不过也还是有点担心。当然，比起 `ListView` 的性能问题，我更担心的是内存够不够用。因为左边那个时间显示，并不是一个 Label。因为我的 UI 设计水平太差，所以我使用了 `ListView` 内置的 `ImageCell`，左边那个是一个图片。这个图片是在内存里绘制出来，再使用 `ImageSource.FromStream` 载入的。所以不知道会不会消耗太多的内存。这里可能是一个可以优化的点。

## 对事件/活动的修改

![ListView 操作]()

在记录完一个活动/事件之后，需要记录做了什么事情，也就是一个事件/活动的主题，还需要给一个事件添加标签，以便之后做统计。本来我想的是，使用 `ListView` 的 `Context Menu` 来实现的，但这东西有个问题，就是和 `TabbedView` 的左右滑动有冲突，所以怎么也不能显示 `Context Menu`。我只好放弃这个方案，使用了 `DisplayActionSheet`。结果，在 Android 上，就成了这个样子。不是很美观。当然，做为原型，也就先这样好了。

虽然这里写了5个操作，但我现在只实现了 1.5 个。这个 1 就是：

![编辑主题]()

好吧，我知道这个也有点丑（这也叫有点……）。功能很简单，就是一个文本的编辑，然后再更新回原来的列表里。另外的 0.5 个，是编辑 tag 的 page，这个只是完成了一个样子，但我感觉并不是很好用，可能在后面还会再改。

![Tag 编辑]()

按照我现在的设想，DailyTick 不会有其它时间记录软件那种编辑一个事件的开始时间、结束时间的功能，我提供的是把一个时间段进行拆分，或者把两个时间段合并。这样保证了一天的时间线是完整的，不存在“无记录时间”这个东西。因为按照我对这个软件的设计哲学，人是无法控制时间的，不管你怎么使用自己的时间，时间本身都会不受控制的流走。所以，我们能做的，只是记录自己在某一段时间里做了什么。这里可能有一个问题，需要在未来解决一下，就是在什么地方体现我一天的日程安排，也就是 TODO 要记录在哪儿的问题。

这里，时间段拆分是有一个 UI 的，我写了一个草稿，但没有放上来。对于合并操作，可能只有确认的提示，不会有单独的 UI。

## 统计 UI

统计页里，只有一个 `WebView`，当然是一个定制的 `WebView`，因为我需要在 C# 和 JS 之间传数据。这个 `HybridWebView` 的基础实现已经实现出来了，不过要怎么传递数据，还没有想法。按照现在的初步设想，应该会使用 highchart.js 和 jQuery 来完成统计页的实现（并没有什么复杂交互，不想上 MVVM 的东西了）。主要就是报表和饼图了。虽然我觉得饼图其实意义有限，直接一个 table 可以搞定很多工作，但如果这个工具被扩散到非理科出身的人那里（只是幻想一下），那么统计图就变成必不可少的东西了。

当然，这一切都还只是原型，如果你有什么对 UI 或者 UE 上的看法，欢迎在下面留言，一起讨论。也欢迎 star & fork 我的项目：https://github.com/holmescn/DailyTick
