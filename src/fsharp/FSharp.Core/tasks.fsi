// TaskBuilder.fs - TPL task computation expressions for F#
//
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Control

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
    open System
    open System.Runtime.CompilerServices
    open System.Threading.Tasks
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.CompilerServices
    open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections

    [<Struct; NoComparison; NoEquality>]
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    [<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly", 1204, IsHidden=true)>]
    /// This is used by the compiler as a template for creating state machine structs
    type TaskStateMachine<'T> =

        /// Holds the final result of the state machine
        [<DefaultValue(false)>]
        val mutable Result : 'T

        /// When statically compiled, holds the continuation goto-label further execution of the state machine
        [<DefaultValue(false)>]
        val mutable ResumptionPoint : int

        /// When interpreted, holds the continuation for the further execution of the state machine
        [<DefaultValue(false)>]
        val mutable ResumptionFunc : TaskMachineFunc<'T>

        /// When interpreted, holds the awaiter used to suspend of the state machine
        [<DefaultValue(false)>]
        val mutable Awaiter : ICriticalNotifyCompletion

        [<DefaultValue(false)>]
        val mutable MethodBuilder : AsyncTaskMethodBuilder<'T>

        interface IAsyncStateMachine

    and
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        [<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly", 1204, IsHidden=true)>]
        /// Contains the `task` computation expression builder.
        TaskMachineFunc<'TOverall> = delegate of byref<TaskStateMachine<'TOverall>> -> bool

    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    [<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly", 1204, IsHidden=true)>]
    [<ResumableCode>]
    type TaskCode<'TOverall, 'T> = delegate of byref<TaskStateMachine<'TOverall>> -> bool 

    [<Class>]
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    type TaskBuilder =
    
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline Combine: 
            task1: TaskCode<'TOverall, unit> * 
            task2: TaskCode<'TOverall, 'T> 
                -> TaskCode<'TOverall, 'T>
    
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline Delay: 
            f: (unit -> TaskCode<'TOverall, 'T>) 
                -> TaskCode<'TOverall, 'T>
    
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline For: 
           sequence: seq<'T> * 
           body: ('T -> TaskCode<'TOverall, unit>) 
               -> TaskCode<'TOverall, unit>
    
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline Return: value: 'T -> TaskCode<'T, 'T>
    
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline Run: code: TaskCode<'T, 'T> -> Task<'T>
    
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline TryFinally: 
           body: TaskCode<'TOverall, 'T> * 
           compensation: (unit -> unit) 
              -> TaskCode<'TOverall, 'T>
    
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline TryWith: 
            body: TaskCode<'TOverall, 'T> * 
            catch: (exn -> TaskCode<'TOverall, 'T>) 
              -> TaskCode<'TOverall, 'T>
    
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline Using:
            resource: 'Resource *
            body: ('Resource -> TaskCode<'TOverall, 'T>)
              -> TaskCode<'TOverall, 'T> when 'Resource :> IDisposable
    
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline While:
            condition: (unit -> bool) *
            body: TaskCode<'TOverall, unit>
              -> TaskCode<'TOverall, unit>
    
        [<DefaultValue>]
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline Zero: unit -> TaskCode<'TOverall, unit>

        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline ReturnFrom: task: Task<'T> -> TaskCode<'T, 'T>

        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member RunDynamic: task: TaskCode<'T, 'T> -> Task<'T>
    
        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member CombineDynamic: task1: TaskCode<'TOverall, unit> * task2: TaskCode<'TOverall, 'T> -> TaskCode<'TOverall, 'T>
    
        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member WhileDynamic: condition: (unit -> bool) * body: TaskCode<'TOverall, unit> -> TaskCode<'TOverall, unit>
    
        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member TryFinallyDynamic: body: TaskCode<'TOverall, 'T> * compensation: (unit -> unit) -> TaskCode<'TOverall, 'T>
    
        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member TryWithDynamic: body: TaskCode<'TOverall, 'T> * handler: (exn -> TaskCode<'TOverall, 'T>) -> TaskCode<'TOverall, 'T>
    
        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member ReturnFromDynamic: task: Task<'T> -> TaskCode<'T, 'T>

    /// Contains the `task` computation expression builder.
    [<AutoOpen>]
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    module TaskBuilder = 

        /// Builds a task using computation expression syntax
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        val task : TaskBuilder

    /// Contains extension methods allowing the `task` computation expression builder
    /// binding to tasks in a way that is sensitive to the current scheduling context.
    /// This module is automatically opened.
    [<AutoOpen>]
    module ContextSensitiveTasks = 

        /// Provides evidence that various types can be used in bind and return constructs in task computation expressions
        [<Sealed; NoComparison; NoEquality>]
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        type TaskWitnesses =
                interface IPriority1
                interface IPriority2
                interface IPriority3

                /// Provides evidence that task-like types can be used in 'bind' in a task computation expression
                
                [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
                static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter, 'TOverall > :
                    priority: IPriority2 *
                    task: ^TaskLike *
                    continuation: ( ^TResult1 -> TaskCode<'TOverall, 'TResult2>)
                        -> TaskCode<'TOverall, 'TResult2>
                                                    when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                                    and ^Awaiter :> ICriticalNotifyCompletion
                                                    and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                                    and ^Awaiter: (member GetResult:  unit ->  ^TResult1) 

                /// Provides evidence that tasks can be used in 'bind' in a task computation expression
                [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
                static member inline CanBind:
                    priority: IPriority1 *
                    task: Task<'TResult1> *
                    continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)
                        -> TaskCode<'TOverall, 'TResult2>

                /// Provides evidence that F# Async computations can be used in 'bind' in a task computation expression
                [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
                static member inline CanBind:
                    priority: IPriority1 *
                    computation: Async<'TResult1> *
                    continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)
                        -> TaskCode<'TOverall, 'TResult2>

                /// Provides evidence that task-like types can be used in 'return' in a task workflow
                
                [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
                static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T> : 
                    priority: IPriority2 *
                    task: ^TaskLike
                        -> TaskCode< ^T, ^T > 
                                                    when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                                    and ^Awaiter :> ICriticalNotifyCompletion
                                                    and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                                    and ^Awaiter: (member GetResult: unit ->  ^T)

                /// Provides evidence that F# Async computations can be used in 'return' in a task computation expression
                [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
                static member inline CanReturnFrom:
                    priority: IPriority1 *
                    task: Task<'T>
                        -> TaskCode<'T, 'T>

                /// Provides evidence that F# Async computations can be used in 'return' in a task computation expression
                [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
                static member inline CanReturnFrom:
                    priority: IPriority1 *
                    computation: Async<'T>
                        -> TaskCode<'T, 'T>

                /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
                [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
                static member inline CanBindDynamic< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter, 'TOverall > :
                    priority: IPriority2 *
                    task: ^TaskLike *
                    continuation: ( ^TResult1 -> TaskCode<'TOverall, 'TResult2>)
                        -> TaskCode<'TOverall, 'TResult2>
                                                    when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                                    and ^Awaiter :> ICriticalNotifyCompletion
                                                    and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                                    and ^Awaiter: (member GetResult:  unit ->  ^TResult1) 

                /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
                [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
                static member inline CanBindDynamic:
                    priority: IPriority1 *
                    task: Task<'TResult1> *
                    continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)
                        -> TaskCode<'TOverall, 'TResult2>

                /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
                [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
                static member inline CanReturnFromDynamic< ^TaskLike, ^Awaiter, ^T> :
                    priority: IPriority2 *
                    task: ^TaskLike
                        -> TaskCode< ^T, ^T > 
                                                    when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                                    and ^Awaiter :> ICriticalNotifyCompletion
                                                    and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                                    and ^Awaiter: (member GetResult: unit ->  ^T)

                /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
                [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
                static member CanReturnFromDynamic:
                    priority: IPriority1 *
                    task: Task<'T>
                        -> TaskCode<'T, 'T>

        [<AutoOpen>]
        module TaskHelpers = 

            type TaskBuilder with 
                /// Provides the ability to bind to a variety of tasks, using context-sensitive semantics
                [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
                member inline Bind< ^TaskLike, ^TResult1, 'TResult2, 'TOverall
                                        when (TaskWitnesses or  ^TaskLike): (static member CanBind: TaskWitnesses * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>)> :
                                    task: ^TaskLike * 
                                    continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>)
                                        -> TaskCode<'TOverall, 'TResult2>        

                /// Provides the ability to return results from a variety of tasks, using context-sensitive semantics
                [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
                member inline ReturnFrom: task: ^TaskLike -> TaskCode< 'T, 'T >
                    when (TaskWitnesses or  ^TaskLike): (static member CanReturnFrom: TaskWitnesses * ^TaskLike -> TaskCode<'T, 'T>)


(*
    /// Contains extension methods allowing the `task` computation expression builder
    /// to bind to tasks in a way that doesn't reschedule work in the current scheduling context.
    /// This is not suitable when tasks must interact with user interface controls on the same
    /// thread as its caller.
    ///
    /// This module is automatically opened.
    module ContextInsensitiveTasks = 

        /// Provides evidence that various types can be used in bind and return constructs in task computation expressions
        [<Sealed; NoComparison; NoEquality>]
        type TaskWitnesses =
            interface IPriority1
            interface IPriority2
            interface IPriority3

            /// Provides evidence that task-like computations can be used in 'bind' in a task computation expression
            
            static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter, 'TOverall > : priority: IPriority3 * task: ^TaskLike * continuation: ( ^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>
                when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                and ^Awaiter :> ICriticalNotifyCompletion
                and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                and ^Awaiter: (member GetResult:  unit ->  ^TResult1)

            /// Provides evidence that task-like computations can be used in 'bind' in a task computation expression
            
            static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaitable, ^Awaiter, 'TOverall > : priority: IPriority2 * task: ^TaskLike * continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>
                when  ^TaskLike: (member ConfigureAwait: bool ->  ^Awaitable)
                and ^Awaitable: (member GetAwaiter:  unit ->  ^Awaiter)
                and ^Awaiter :> ICriticalNotifyCompletion
                and ^Awaiter: (member get_IsCompleted: unit -> bool)
                and ^Awaiter: (member GetResult: unit ->  ^TResult1)

            /// Provides evidence that tasks can be used in 'bind' in a task computation expression
            
            static member inline CanBind: priority: IPriority1 * task: Task<'TResult1> * continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>

            /// Provides evidence that F# async computations can be used in 'bind' in a task computation expression
            
            static member inline CanBind: priority: IPriority1 * computation: Async<'TResult1> * continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>

            /// Provides evidence that types following the "awaitable" pattern can be used in 'return!' in a task computation expression
            
            static member inline CanReturnFrom< ^Awaitable, ^Awaiter, ^T> : IPriority2 * task: ^Awaitable -> TaskStep< ^T, ^T>
                                                    when  ^Awaitable: (member GetAwaiter: unit ->  ^Awaiter)
                                                    and ^Awaiter :> ICriticalNotifyCompletion
                                                    and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                                    and ^Awaiter: (member GetResult: unit -> ^T)

            /// Provides evidence that types following the task-like pattern can be used in 'return!' in a task computation expression
            
            static member inline CanReturnFrom< ^TaskLike, ^Awaitable, ^Awaiter, ^T
                                                    when ^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)
                                                    and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                    and ^Awaiter :> ICriticalNotifyCompletion 
                                                    and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                    and ^Awaiter : (member GetResult : unit -> ^T) > : IPriority1 * configurableTaskLike: ^TaskLike -> TaskStep< ^T, ^T>

            /// Provides evidence that F# async computations can be used in 'return!' in a task computation expression
            
            static member inline CanReturnFrom: IPriority1 * computation: Async<'T> -> TaskStep<'T, 'T>

        type TaskBuilder with

            /// Provides the ability to bind to a variety of tasks, using context-sensitive semantics
            
            member inline Bind : task: ^TaskLike * continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>
                when (TaskWitnesses or  ^TaskLike): (static member CanBind: TaskWitnesses * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>)

            /// Provides the ability to return results from a variety of tasks, using context-sensitive semantics
            
            member inline ReturnFrom: a: ^TaskLike -> TaskStep< 'T, 'T >
                when (TaskWitnesses or  ^TaskLike): (static member CanReturnFrom: TaskWitnesses * ^TaskLike -> TaskStep<'T, 'T>)
*)
#endif
