namespace WinTail

[<AutoOpen>]
module Messages =
    open Akka.Actor
    
    type Command =
        | Start
        | Continue
        | Message of string
        | Exit

    type ErrorType =
        | Null
        | Validation

    type InputResult =
        | InputSuccess of string
        | InputError of reason: string * errorType: ErrorType

    //Messages to start and stop observing file content for any changes
    type TailCommand =
    | StartTail of filePath: string * reporterActor: IActorRef  //File to observe, actor to display contents
    | StopTail of filePath: string                             

    type FileCommand =
    | FileWrite of fileName: string
    | FileError of fileName: string * reason: string
    | InitialRead of fileName: string * text: string