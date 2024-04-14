"# Hotkey Hook for .NET Framework" 
This DLL is used to set Windows system hook that detects keypress.
It is made in Visual Studio 2022, with C#

# A. Hook Scopes
There are 2 types of hook: global and local:

Global hooks can trigger even if application/thread is <u>not</u> focused.

Local hooks only trigger when application/thread is focused.

# B. Supported Hotkeys
Only the following keys can be set as hotkey:

F1 to F12

In case the list here is not updated, see Class_SupportedKeys file

# C. How to Use
Note that all public functions are static

## 1. Classes
This DLL consists of 3 classes:

Class_HookManager for high level purpose such as setting hotkey and unsetting hotkey,

Class_Hook to access individual hook,

Class_SupportedKeys that handles keys that are supported by this DLL.

## 2. Class_Hook.HookedFunction
This is a delegate for the function that will be run when hotkey is pressed.
This delegate takes List<object> as parameter, and returns void.
Function that is to be hooked must be in the same format as this delegate.

Example:
```C#
void exampleBadFunction() {}
void exampleGoodFunction(List<object> inObjects) {}
```
Hotkey cannot be set to run exampleBadFunction() because it is doesn't have the same paramter as the delegate.
On the other hand, exampleGoodFunction() has the correct format.

## 3. Setting Hook/Hotkey
There are 2 functions that can be used to set hook -- 1 for each hook scope. Both have the same parameters.
```C#
static bool SetHotkeyGlobal(Enum_SupportedKeys hotkey, Class_Hook.HookedFunction hookedFunction) {}
static bool SetHotkeyLocal(Enum_SupportedKeys hotkey, Class_Hook.HookedFunction hookedFunction) {}
```
hotkey is the Enum for hotkey. See the aforementioned <u>Supported Hotkeys</u> section.

hookedFunction is the function that will run when hotkey is pressed.

Return false if fail to set hotkey. Return true if success.

### a. Converting Hotkey String to Enum
A function has been provided to convert hotkey string to the hotkey enum -- for example, converting string "F3" to Enum_SupportedKeys.F3
```C#
static Enum_SupportedKeys GetEnumNameFromString(string inStr) {}
```

### b. Setting Arguments of Hooked Functions
<u>After</u> a hook has been created, to set the values of the arguments for hooked function, you can get a hook by its hotkey.
```C#
static Class_Hook GetHookByHotkey(Enum_SupportedKeys inHotkey) {}
```

The arguments can then be accessed through the retrieved hook.
```C#
void main()
{
    Enum_SupportedKeys myHotkey = Enum_SupportedKeys.F3;
    Class_Hook myHook = Class_Hook.GetHookByHotkey(myHotkey);
    myHook.Args = new List<object>()
    {
        ...
    }
}
```

If for some reason, you need to access all hooks, you can call this list:
```C#
static List<Class_Hook> Class_Hook.MyHooks;
```

## 4. Unsetting Hook/Hotkey
In order to not waste any resource, make sure to delete unused hotkey with:
```C#
static void UnsetHotkey(Enum_SupportedKeys inHotkey) {}
static void UnsetAllHotkeys() {}
```
It is highly recommended to unset all hotkeys when closing the application.

# D. Known Issue
Default functions of the hotkeys are still triggered.

Example:

Hotkey = F1

By default, F1 opens help page

When pressing F1, the hotkey for your application will run, but it will still open the help page

# E. Non-default .NET Framework References
PresentationCore

WindowsBase