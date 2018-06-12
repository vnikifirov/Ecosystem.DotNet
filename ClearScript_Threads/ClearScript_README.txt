If you're using a single instance of V8ScriptEngine, you can execute JavaScript code on any thread, but not on multiple threads simultaneously.

ClearScript supports asynchronous callbacks into script code, so as long as your scripts don't do any waiting or perform other long-running operations, you should be able to take full advantage of .NET's threading capabilities.