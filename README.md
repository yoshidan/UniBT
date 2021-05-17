# UniBT

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![GitHub release](https://img.shields.io/github/release/yoshidan/UniBT.svg)](https://github.com/yoshidan/UniBT/releases)
[![Github all releases](https://img.shields.io/github/downloads/yoshidan/UniBT/total.svg)](https://gitHub.com/yoshidan/UniBT/releases/)

Free [Behavior Tree](https://en.wikipedia.org/wiki/Behavior_tree_(artificial_intelligence,_robotics_and_control)) designer for Unity.
 
## Motivation
I know some designers like [Behavior Designer](https://assetstore.unity.com/packages/tools/visual-scripting/behavior-designer-behavior-trees-for-everyone-15277) or [Arbor3](https://assetstore.unity.com/packages/tools/visual-scripting/arbor-3-fsm-bt-graph-editor-112239).  
They have many features but not for free.
I will provide easily customizable and free designer.

## Supported version

* Unity 2019.4 or later.

## Features
* Supports constructing behavior tree by [GraphView](https://docs.unity3d.com/ScriptReference/Experimental.GraphView.GraphView.html).
* Supports visualizing active node in runtime.
* Easily add original behaviors(Action,Conditional,Composite).
  
<img src="images/demo.jpg" />

## Get Started

1. [Download](https://github.com/yoshidan/UniBT/releases) and install unity package.   
  The plugin can also be imported with Package Manager, by adding the following entry in your `Packages/manifest.json`:
```json
{
  "dependencies": {
    ...
    "com.github.yoshidan.unibt": "https://github.com/yoshidan/UniBT.git?path=Assets/UniBT/Scripts"
  }
}
```


2. Add `UniBT.BehaviorTree` component for any GameObject.  
   <img src="images/started1.jpg" width="240"/>
3. `Open Graph Editor` button opens GraphView for Behavior Tree.  
   <img src="images/started2.jpg" width="360"/>
4. Add behaviors and set parameters.  
5. Finally press save button on tool bar of the editor window. (If invalid node found the color of the node become red.)  
   <img src="images/started3.gif" width="480"/>  
6. Run the unity application. you can see node status in the editor window.  
   <img src="images/started4.jpg" width="480"/>
   
   * The red node means that last `Update` returned Status.Failure`.
   * The green node means that last `Update` returned `Status.Success`.
   * The yellow node means that last `Update` returned `Status.Running`.
7. you can save the GameObject with `UniBT.BehaviorTree` as prefab.

## How It Works

* `UnitBT.BehaviorTree` updates child nodes in `Update` timing when the UpdateType is `UpdateType.Auto`.
* If you want to update at any time, change UpdateType to `UpdateType.Manual` and call `BehaviorTree.Tick()`;
* Only `UniBT.BehaviorTree` is the `MonoBehavior`. Each node is just a C# Serializable class.
  
### Core Behavior Node

| Name | Description |
| ------- | --- |
| Composite Node | It has one or more child nodes and controls which child node to update. |
| Action Node | This is the leaf node. It execute action such as follow player, attack, escape or others you define. |
| Conditional Node | It has one child node and check the condition whether child is updatable. when having no child, Conditional Node is the leaf node like Action Node. |

Conditional Node has following parameter.

| Name | Description | 
|------|-------|
| dontReEvaluateOnRunning | true: don't re evaluate the condition if the previous status is `Status.Running`. |

### Built In Composite Node

I have prepared several built in Composite Node.

#### Sequence
* Updates the child nodes in order from the top. 
* Returns failure immediately if the child node returns failure. 
* Returns running immediately and calls the child at the next update timing if the child node returns running.
* Returns success if all child nodes return success.  

Sequence has following parameter.

| Name | Description | 
|------|-------|
| abortOnConditionChanged | true: Aborts the running node when a node with a higher priority than the running node becomes infeasible. Specifically, the execution result of `Conditional.CanUpdate`, which is a descendant of a node with a higher priority than the running node, is used.|

#### Selector
* Updates the child nodes in order from the top. 
* Returns success immediately if the child node returns success. 
* Returns running immediately and calls the child at the next update timing if the child node returns running.
* Returns failure if all child nodes return failure.  

Selector has following parameter.

| Name | Description | 
|------|-------|
| abortOnConditionChanged | true: Aborts the running node when a node with a higher priority than the running node becomes executable. Specifically, the execution result of `Conditional.CanUpdate`, which is a descendant of a node with a higher priority than the running node, is used.|

#### All
* Updates all child nodes.  
* Returns running if any child node returns running.
* Returns failure if any child node returns failure.
* Otherwise, returns success.

#### Random
* The child nodes are elected and executed according to the probability based on the uniform distribution.  
* Select one for each update. However, if the running status is returned during the last update, the node will continue to run.

#### Rotator
* Updates the child nodes in order. Unlike Sequencer, one child node is executed by one update instead of executing all child nodes by one update.  
* For example, if there are three child nodes, the first Update will execute the top node, the next Update will execute the second node, and the next Update will execute the third node.  
* The next run will run the top node again.  
* If a child node returns a running state, it exits without executing subsequent child nodes, and the child node continues to run on the next update.

Rotator has following parameter.

| Name | Description | 
|------|-------|
| resetOnAbort | It is a flag whether to return the next execution target node from the top when the execution condition of the ancestor Conditional Node changes and the running node is interrupted.|

## Demo Scene
* [here](https://github.com/yoshidan/UniBT/tree/master/Assets/UniBT/Examples) is the example scene.

## Create Behaviors

### Create Action
* Create C# Script and extends `UniBT.Action`
* Override `OnUpdate` and return status(Success/Running/Failure).
* Override `Awake` called by `UniBT.BehaviorTree.Awake` if needed.
* Override `Start` called by `UniBT.BehaviorTree.Start` if needed.
* Override `Abort` to reset field or any state when the parent condition changed..
* Action has Node `gameObject` field with `UniBT.BehaviorTree` attached.
* Private [SerializeField] field and public field can be set on Behavior Tree editor window.

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
* Create C# Script and extends `UniBT.Conditional`
* Override `IsUpdatable` and return result(true/false). when `IsUpdatable` returns update child.
* Override `OnAwake` called by `UniBT.BehaviorTree.Awake` if needed.
* Override `OnStart` called by `UniBT.BehaviorTree.Start` if needed.
* Conditional Node has `gameObject` field with `UniBT.BehaviorTree` attached.
* Private [SerializeField] field and public field can be set on Behavior Tree editor window.

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
* Create C# Script and extends `UniBT.Composite`
* Override `OnUpdate` and return status(Success/Running/Failure).
* Override `OnAwake` called by `UniBT.BehaviorTree.Awake` if needed.
* Override `OnStart` called by `UniBT.BehaviorTree.Start` if needed.
* To abort the running node when the condition changed override `Abort`.
* Composite Node has `gameObject` field with `UniBT.BehaviorTree` attached.
* Private [SerializeField] field and public field can be set on Behavior Tree editor window.

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

