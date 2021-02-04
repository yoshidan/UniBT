# UniBT

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![GitHub release](https://img.shields.io/github/release/yoshidan/UniBT.svg)](https://github.com/yoshidan/UniBT/releases)
[![Github all releases](https://img.shields.io/github/downloads/yoshidan/UniBT/total.svg)](https://gitHub.com/yoshidan/UniBT/releases/)

Unity向けの[Behavior Tree](https://en.wikipedia.org/wiki/Behavior_tree_(artificial_intelligence,_robotics_and_control)) デザイナーです。
 
## Supported version

* Unity 2019.4 or later.

## Features
* [GraphView](https://docs.unity3d.com/ScriptReference/Experimental.GraphView.GraphView.html)を利用してツリーを構築します。
* Unityアプリケーション実行中にノードの状態（成功、失敗、実行中）を可視化できます。
* 各種ビヘイビア（Action、Conditional、Composite）を追加して利用できます。
  
<img src="images/demo.jpg" />

## Get Started

1. [Unityパッケージ](https://github.com/yoshidan/UniBT/releases)をダウンロードしてインストールしてください。
2. 任意のGameObjectに `UniBT.BehaviorTree` コンポーネントを追加してください。  
   <img src="images/started1.jpg" width="240"/>
3. `Open Graph Editor` を押すと、GraphViewのウインドウが立ち上がり編集できるようになります。  
   <img src="images/started2.jpg" width="360"/>
4. ノードを追加してパラメータを設定します。
5. Saveボタンを設定がGameObjectに反映されます。ノードの設定に不備がある場合、エラーになり保存できません。（対象ノードが赤くなります）  
   <img src="images/started3.gif" width="480"/>  
6. Unityを実行すると、構築したツリーが起動します。各ノードの状態はGraphView上で閲覧可能です。  
   <img src="images/started4.jpg" width="480"/>
   
   * 赤色のノードは実行結果が「失敗」であることを示しています。
   * 緑色のノードは実行結果が「成功」であることを示しています。.
   * 黄色のノードは実行結果が「実行中」であることを示しています。.
7. BehaviorTreeを設定した対象のGameObjectはPrefabとして保存することが可能です。

## How It Works

* `UniBT.BehaviorTree`はMonoBehaviorです。Updateのタイミングでツリーを実行します。
* UpdateTypeを「Manual」に変更することで上記の自動実行を停止し、`UniBT.BehaviorTree.Tick()`メソッドを呼び出すことで任意のタイミングで実行することもできます。
* ツリー内のノードを示すクラスはMonoBehaviorではありません。ただのC#のSerializable classです。そのため、このツールでAddComponentされるのはUniBT.BehaviorTreeのみです。
  
### Core Behavior Node

| Name | Description |
| ------- | --- |
| Composite Node | 複数の子ノードを保持します。全てを実行したり一部だけを実行するなど子ノードの実行を制御します。 |
| Action Node | リーフノードです。プレイヤーを追いかけたり、攻撃したり、回復するなど、具体的なNPCの行動を定義します。 |
| Conditional Node | 子ノードの実行条件を定義します。ヘイトが一定以上超えたり、HPが少なくなったりするなど条件に応じて子ノードの実行を変えたい場合に使用します。 |

Conditional Nodeは次のパラメータを持ちます。

| Name | Description | 
|------|-------|
| dontReEvaluateOnRunning | trueにすると、前回子ノードが「実行中」を返した場合に、条件変化を無視して次のフレームも強制的に子ノードを実行します。 |

### Built In Composite Node

Composite Nodeを継承したいくつかの具体的な振る舞いを提供するノードを用意しています。

#### Sequence
* 一番上の子ノードを最高優先順位として、順番に子ノードを実行しています。
* 途中で子ノードが「失敗」した場合は、後続のノードを実行せずに直ちに親のノードに「失敗」を返却します。
* 途中で子ノードが「実行中」になった場合は、後続のノードを実行せずに直ちに親のノードに「実行中」を返却します。
* 全ての子ノードが「成功」した場合に親のノードに「成功」を返却します。

Sequenceは以下のパラメータを持ちます。

| Name | Description | 
|------|-------|
| abortOnConditionChanged | trueにすると、実行中のノードよりも優先度の高いノードが実行可能になると、実行中のノードを中止してそのノードを実行します。|

#### Selector
* 一番上の子ノードを最高優先順位として、順番に子ノードを実行しています。
* 途中で子ノードが「成功」した場合は、後続のノードを実行せずに直ちに親のノードに「成功」を返却します。
* 途中で子ノードが「実行中」になった場合は、後続のノードを実行せずに直ちに親のノードに「実行中」を返却します。
* 全ての子ノードが「失敗」した場合に親のノードに「失敗」を返却します。

Selectorは以下のパラメータを持ちます。

| Name | Description | 
|------|-------|
| abortOnConditionChanged | trueにすると、実行中のノードよりも優先度の高いノードが実行可能になると、実行中のノードを中止してそのノードを実行します。|

#### All
* 全ての子ノードを実行します。
* 一つ以上「実行中」の子ノードがあった場合に親のノードに「実行中」を返却します。
* そうでなく、一つ以上「失敗」の子ノードがあった場合に親のノードに「失敗」を返却します。
* そうでなければ、「成功」を返却します。

#### Random
* 子ノードは、実行毎に一様分布に基づく確率に従って一つ選択されます。
* 子ノードの結果が「実行中」だった場合、次の実行ではランダム選択せず、引き続きそのノードを実行します。

#### Rotator
* 子ノードを順番に更新します。シーケンサーとは異なり、すべての子ノードを1回の更新で実行するのではなく、1つの子ノードを1回の更新で実行します。
* たとえば、子ノードが3つある場合、最初の更新で最上位ノードが実行され、次の更新で2番目のノードが実行され、次の更新で3番目のノードが実行されます。
* 次の実行では、最上位ノードが再度実行されます。
* 子ノードが実行状態を返した場合は、後続の子ノードを実行せず終了し、次回の更新でもその子ノードが引き続き実行されます。

Rotatorは以下のパラメータを持ちます。

| Name | Description | 
|------|-------|
| resetOnAbort | trueの場合、先祖のConditional Nodeの実行条件が変化し、実行中の子ノードが中断された場合に、次の実行対象ノードを先頭に戻します。|

## Demo Scene
* [here](https://github.com/yoshidan/UniBT/tree/master/Assets/UniBT/Examples) is the example scene.

## Create Behaviors

### Create Action
* `UniBT.Action`を継承したC# Scriptを作成します。
* `OnUpdate`をオーバーライドして、実行結果に応じてステータス(Success/Running/Failure)を返却します。
* MonoBehavior#Awakeのタイミングで実行したい処理があれば、`Awake`をオーバーライドしてください。
* MonoBehavior#Startのタイミングで実行したい処理があれば、`Start`をオーバーライドしてください。
* 実行中状態からの中断時に初期化したい処理がある場合は `Abort`をオーバーライドしてください。
* `UniBT.BehaviortTree`がアタッチされたGameObjectを`gameObject`フィールドで取得できます。
* [SerializeField]フィールドとpublicフィールドは、Behavior Treeのエディタウインドウで設定することができます。

```c#
public class Wait : Action
{
    [SerializeField] 
    private float waitTime;

    private float elapsedTime = 0.0f;

    protected override Status OnUpdate()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime < waitTime)
        {
            return Status.Running;
        }

        elapsedTime = 0.0f;
        return Status.Success;
    }

    // abort when the parent conditional changed on previous status is running.
    public override void Abort()
    { 
        elapsedTime = 0.0f;
    }
}
```

### Create Conditional
* `UniBT.Conditional`を継承したC# Scriptを作成します。
* `IsUpdatabale`をオーバーライドして、実行条件を満たしたらtrueを返却し、そうでなければfalseを返却してください。
* MonoBehavior#Awakeのタイミングで実行したい処理があれば、`OnAwake`をオーバーライドしてください。
* MonoBehavior#Startのタイミングで実行したい処理があれば、`OnStart`をオーバーライドしてください。
* 実行中状態からの中断時に初期化したい処理がある場合は `Abort`をオーバーライドしてください。
* `UniBT.BehaviortTree`がアタッチされたGameObjectを`gameObject`フィールドで取得できます。
* [SerializeField]フィールドとpublicフィールドは、Behavior Treeのエディタウインドウで設定することができます。

```c#
public class IsHateGt: Conditional
{
    [SerializeField] 
    private int threshold;

    private Enemy enemy;

    protected override void OnAwake()
    {
        enemy = gameObject.GetComponent<Enemy>();
    }

    protected override bool IsUpdatable()
    {
        return enemy.Hate > threshold;
    }
}
```

* Conditional Node can be leaf node like Action Node.
<img src="images/conditional1.jpg" width="480"/>

* Conditional Node can be branch node.
<img src="images/conditional2.jpg" width="480"/>

### Create Composite
* `UniBT.Composite`を継承したC# Scriptを作成します。
* `OnUpdate`をオーバーライドして、実行結果に応じてステータス(Success/Running/Failure)を返却します。
* MonoBehavior#Awakeのタイミングで実行したい処理があれば、`Awake`をオーバーライドしてください。
* MonoBehavior#Startのタイミングで実行したい処理があれば、`Start`をオーバーライドしてください。
* 実行中状態からの中断時に初期化したい処理がある場合は `Abort`をオーバーライドしてください。
* `UniBT.BehaviortTree`がアタッチされたGameObjectを`gameObject`フィールドで取得できます。
* [SerializeField]フィールドとpublicフィールドは、Behavior Treeのエディタウインドウで設定することができます。

```c#
public class Random : Composite
{
    private NodeBehavior runningNode;

    protected override Status OnUpdate()
    {
        // proceed to update same node when the previous status is running
        if (runningNode != null)
        {
            return HandleStatus(runningNode.Update(), runningNode);
        }

        // update random children
        var result = UnityEngine.Random.Range(0, Children.Count);
        var target = Children[result];
        return HandleStatus(target.Update(), target);
    }

    private Status HandleStatus(Status status, NodeBehavior updated)
    {
        //save running node for next update.
        runningNode = status == Status.Running ? updated : null;
        return status;
    }

    // abort when the parent conditional changed on previous status is running.
    public override void Abort()
    {
        if (runningNode != null)
        {
            runningNode.Abort();
            runningNode = null;
        }
    }
}
```

