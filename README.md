# Command And Callback

A lightweight and reusable pooled command library for Unity. Its also my first Github repo, yay!  
And yes, the name is directly referenced by a game called 'Command and Conquer'.  
If you wanna know what a command is, you can search "C# Command Pattern" to learn more.  

## Installation

1. Open you Unity project
2. Select `Window` at your top corner screen
3. Then go to `Package Management`
4. Select `Package Manager`
5. Select `+` at the top-left corner of the package manager window
6. Select `Install package from git URL`
7. Paste this git URL `https://github.com/Castforn/CommandAndCallback.git`
8. Your done!

## Features

- Action Command – Encapsulate actions that execute without returning a result.
- Func Command – Encapsulate funcs that execute and return a result.
- Predicate Command - Encapsulate predicates that execute and returns a bool.
- Pooling - Commands have internal pooling support to reduce garbage collections.
- Controlled Instantiation - Commands cannot be instantiated directly by users because their constructors are private. Instead, they are created through dedicated factory methods, preventing accidental instantiation.
- 8 Generic Parameters Support - Commands support up to 8 generic parameters, except predicate. Since anything more than that is just bad code design in my opinion.

## API

These are all of the APIs the library can offer.

### Command.GetAction(), Command.GetFunc(), Command.GetPredicate()

Gets a command from its pool or creates one if the pool is empty.

```csharp
using Castforn.CommandAndCallback;
using UnityEngine;

public class Test : MonoBehaviour
{

    void Start()
    {
        // A command wrapping an action
        ActionCommand command1 = Command.GetAction(MyMethod1);


        // Another one but has a parameter
        ActionCommand<int> command2 = Command.GetAction<int>(MyMethod2);


        // This time its a func
        FuncCommand<Test, int, MonoBehaviour> command3 = Command.GetFunc<Test, int, MonoBehaviour>(MyMethod3);


        // And lastly, a predicate
        PredicateCommand<MonoBehaviour> command4 = Command.GetPredicate<MonoBehaviour>(MyMethod4);


        // You can also pass arguments when you get the command too
        FuncCommand<Test, int, MonoBehaviour> command5 = Command.GetFunc(MyMethod3, this, 0);
    }


    void MyMethod1()
    {
        Debug.Log("My Method 1");
    }

    void MyMethod2(int number)
    {
        Debug.Log("My Method 2: " + number);
    }

    MonoBehaviour MyMethod3(Test test, int id)
    {
        Debug.Log("My Method 3: " + id + " : " + test);
        return this;
    }

    bool MyMethod4(MonoBehaviour mono)
    {
        Debug.Log("My Method 4: " + mono.name);
        return mono;
    }

}
```

### Invoke()

Invokes the command with either its stored arguements or the one you provided.

```csharp
using Castforn.CommandAndCallback;
using UnityEngine;

public class Test : MonoBehaviour
{

    ActionCommand<Test, double> actionCommand;
    FuncCommand<bool, MonoBehaviour> funcCommand;


    void Awake()
    {
        // This command was not applied with arguments in its parameters, so arguments will be set as default
        actionCommand = Command.GetAction<Test, double>(ActionMethod);


        // But this command does
        funcCommand = Command.GetFunc(FuncMethod, true);
    }


    void Start()
    {
        // You can change the arguments stored inside the command by doing this
        actionCommand.Arg1 = this;
        actionCommand.Arg2 = 1.05;


        // Or this
        actionCommand.SetArgs(null, 88.6);


        // You can then invoke it using its stored arguements
        actionCommand.Invoke();


        // Or the ones you provided
        actionCommand.Invoke(this, 101.002);


        // This same goes to func and predicate as well
        MonoBehaviour result = funcCommand.Invoke();
        result = funcCommand.Invoke(false);
    }


    void ActionMethod(Test test, double accurateNumber)
    {
        Debug.Log("Action Method: " + test + " " + accurateNumber);
    }

    MonoBehaviour FuncMethod(bool boolean)
    {
        Debug.Log("Func Method: " + boolean);
        return this;
    }

}
```

### Release()

Releases the command back to its pool. Its also important to discard the reference after the command was released.

```csharp
using Castforn.CommandAndCallback;
using UnityEngine;

public class Test : MonoBehaviour
{

    FuncCommand<bool> command;

    void Awake()
    {
        command = Command.GetFunc(AnotherFunc);
    }


    void Start()
    {
        // This calls AnotherFunc() because the command is still active.
        bool result = command.Invoke();


        // But once you release the command back to its pool
        command.Release();


        // It will no longer call AnotherFunc() and instead outputs a default result
        result = command.Invoke();


        // I recommend discarding their reference after they've been released to prevent unnecessary problems
        command = null;


        // You can also look at HasReleased if the command has already been released or not yet
        if (command.HasReleased) Debug.Log("Has");
        else Debug.Log("Has not");


        // But thats not a safe way to identify that the command you released will stay that way
        // Because that command you've just released might be reused again by another one, like for example
        ActionCommand anotherCommand = Command.GetAction(AnotherAction);


        // You released it, and the command is returned to its Stack<> pool collection
        anotherCommand.Release();


        // But you didn't discarded the reference and attempts to get another command
        ActionCommand andAnotherCommand = Command.GetAction(AnotherAction);


        // HasReleased of andAnotherCommand is false, thats expected
        if (!andAnotherCommand.HasReleased) Debug.Log("Not released");


        // But HasReleased of anotherCommand is also false, despite you releasing it just now
        if (!anotherCommand.HasReleased) Debug.Log("Not released too???");


        // Its because anotherCommand and andAnotherCommand are the same object being reused
        // This limitation is expected in pooling, i can make it more safter, but that would mean more overhead and work
        // For a command library designed for Unity where optimization is important, i prefer this approach
    }


    bool AnotherFunc()
    {
        Debug.Log("Another one?");
        return true;
    }

    void AnotherAction()
    {
        Debug.Log("And another one?");
    }

}
```

### Execute()

Invokes the command and releases it afterward.

```csharp
using Castforn.CommandAndCallback;
using UnityEngine;

public class Test : MonoBehaviour
{

    ActionCommand<bool> command;


    void Start()
    {
        command = Command.GetAction(ThisHasToBeTheLast, false);


        // The command is executed and released after
        command.Execute();


        // Remember to discard the reference after releasing, its good practice for object pooling in general
        command = null;


        // You get one again
        command = Command.GetAction<bool>(ThisHasToBeTheLast);


        // And this time, you execute it with the arguments you passed
        command.Execute(true);


        // Then do the same thing again
        command = null;
    }


    void ThisHasToBeTheLast(bool probably)
    {
        Debug.Log("It should be " + probably);
    }

}
```

## Read Only Commands
`ActionCommand` and `FuncCommand` has a read-only variant (`ReadOnlyActionCommand` and `ReadOnlyFuncCommand`) that prevents you from being able to change arguments. `PredicateCommand` doesn't also have this.

```csharp
using Castforn.CommandAndCallback;
using UnityEngine;

public class Test : MonoBehaviour
{

    void Start()
    {
        ActionCommand<float, bool> command = Command.GetAction<float, bool>(UhHuh);


        // You can read and change this command's arguments
        if (command.Arg1 == 0) Debug.Log("Yippee!");

        command.Arg1 = 1.75f;
        command.Arg2 = false;

        command.SetArgs(2.2f, true);


        // But when converting to ReadOnlyActionCommand, you can only read its arguments
        ReadOnlyActionCommand<float, bool> readOnlyCommand = command;

        if (readOnlyCommand.Arg1 == 0) Debug.Log("Can still read!");


        // This will cause a compiler error
        readOnlyCommand.Arg2 = false;
    }


    void UhHuh(float uh, bool huh)
    {
        Debug.Log(uh + " " + huh);
    }

}
```


## Base Commands

`BaseActionCommand` is the base class for `ActionCommand` while `BaseFuncCommand` is the base class for `FuncCommand`. `PredicateCommand` however doesn't have one. The limit of these base commands is that their incapable of setting arguments or invoking them using the arguments you provided. However, they serve as the common base for all of their respective commands, regardless of how many arguments they take. They are are most useful when you want to store commands with different parameters into a single collection like for example.

```csharp
using Castforn.CommandAndCallback;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    List<BaseActionCommand> actionCommandList = new();


    void Start()
    {
        // Here's a command with 2 parameters
        ActionCommand<float, Test> command1 = Command.GetAction<float, Test>(GTA);


        // Here's another command with 3 parameters
        ActionCommand<double, MonoBehaviour, byte> command2 = Command.GetAction<double, MonoBehaviour, byte>(IfYour, double.MinValue, this, byte.MaxValue);


        // Yet despite their differences, they both get added to the list.
        actionCommandList.Add(command1);
        actionCommandList.Add(command2);


        // You can even invoke them using their stored arguments
        foreach (BaseActionCommand command in actionCommandList)
        {
            command.Execute();
        }


        // Just remember to discard their references after you released them.
        actionCommandList.Clear();
    }


    void GTA(float six, Test inHere)
    {
        Debug.Log("but i only have " + six + " dollars " + inHere);
        
    }

    void IfYour(double coinless, MonoBehaviour just, byte proclaim)
    {
        Debug.Log("that your " + coinless);
    }

}
```

Another one is `Command`. This is base class for `BaseActionCommand` and `BaseFuncCommand`.  

It only has the bool `HasReleased` and the method `Release()`. You can't invoke it since it doesn't know what type of delegate its handling, so you'll barely use it at all. However you can use it as a way to release all commands in a collection for example.

```csharp
using Castforn.CommandAndCallback;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    List<Command> commands = new();


    void Start()
    {
        ActionCommand command1 = Command.GetAction(ImTiredBoss);

        FuncCommand<Test> command2 = Command.GetFunc(SameHere);

        PredicateCommand<int> command3 = Command.GetPredicate<int>(GuessThis);

        BaseActionCommand command4 = Command.GetAction(() => { Debug.Log("Lambda enclosure"); });

        BaseFuncCommand<MonoBehaviour> command5 = Command.GetFunc(FinallyDone);


        // All of those commands can be stored into the list
        commands.Add(command1);
        commands.Add(command2);
        commands.Add(command3);
        commands.Add(command4);
        commands.Add(command5);


        // But can only be released. Invoking is not available on this base class
        foreach (var command in commands)
        {
            command.Release();
        }

        commands.Clear();
    }


    void ImTiredBoss()
    {
        Debug.Log("He's tired boss");
    }

    Test SameHere()
    {
        return null;
    }

    bool GuessThis(int isIt)
    {
        return isIt == 0;
    }

    MonoBehaviour FinallyDone()
    {
        return this;
    }

}
```

## Representations

And lastly, all non-base commands (`ActionCommand`, `FuncCommand`, and `PredicateCommand`) are a representation of the delegate they encapsulates. Meaning they have some specific traits that might otherwise have been easy to overlook. Here are some examples.

```csharp
using Castforn.CommandAndCallback;
using System;
using UnityEngine;

public class Test : MonoBehaviour
{

    void Start()
    {
        // You wrap ImOut into a command
        ActionCommand command = Command.GetAction(ImOut);


        // And store the method into an action delegate
        Action method = ImOut;


        // Despite the command and method being different objects, they are still treated as the same because the command encapsulates ImOut
        if (command == method) Debug.Log("They're practically the same");


        // However, once you release the command, you also release its encapsulation of the method
        command.Release();


        // Meaning they are no longer the same
        if (command != method) Debug.Log("They're no longer the same");


        // Releasing a command is practically the same as setting an action to null
        method = null;
        command = null;


        // So its dangerous to release a command at the wrong time like being used as a key in a dictionary
        // Because releasing the command also causes its hashcode to turn into 0 despite the command object itself remains active


        command = Command.GetAction(ImOut);


        // Lastly, you can also explicitly turn the command back to its original delegate
        // When you turn the command back to its original delegate, you also release the command object back to its pool as a side effect
        method = (Action)command;


        // HasReleased is true here because the command was released when you converted it back to its original delegate
        if (command.HasReleased) Debug.Log("Yep, it released");


        // You can also implicitly turn the delegate into a command. Its the same as Command.GetAction() but more convenient
        command = method;
    }


    void ImOut()
    {
        Debug.Log("Welp, there he goes");
    }

}
```

## Thanks For Scrolling
And that's all my little command library is capable of, im still not that skilled for now, but i'll make sure that i become better in time, so that i can make a good game with the knowledge i'll amass in the future.
