# DotnetAsm
Ever had an argue with colleague about performance of a certain method? Then you have probably heard about https://sharplab.io/. While being excellent at compiling methods without tiered JIT, it has some drawbacks:
- PGO mode is not supported
- No possibility to use tiered compilation
- Method calls are represented as raw hex address instead of an actual method name
- It requires internet connection (yeah I know, in the current year you have internet almost everywhere)

There is a great extension for Visual Studio called [Disasmo](https://github.com/EgorBo/Disasmo) which inspired me for making this web-based extension that does not require Visual Studio.

# Prerequisites
- .NET 7 RC1 SDK or higher

# :warning: **Do not use external user input**
Code that you write in the editor is executed on your machine. Make sure you don't run anything that can corrupt your system!

# How to use it
1. Run the `DotnetAsm.Api.exe` executable file
2. Open http://localhost:4040/ page in your browser
3. Write your method that you want to test
4. Call it inside the for loop right before the `Thread.Sleep` call
5. Type your method name inside the input on the top left corner of page
6. Click `GENERATE ASM` button

# FAQ
**Q:** Why there is a `for` loop with `Thread.Sleep` inside?

**A:** In order to perform a so-called [tiered compilation](https://github.com/dotnet/runtime/blob/main/docs/design/coreclr/jit/ryujit-tutorial.md#execution-environment--external-interface), method needs to be executed several times so it can hit a call counting threshold to be recompiled from Tier-0 to Tier-1. And total execution time needs to be delayed to see the results.
