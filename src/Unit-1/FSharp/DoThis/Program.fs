open System
open Akka.FSharp
open Akka.Actor
open WinTail

[<EntryPoint>]
let main argv = 
    // initialize an actor system
    let myActorSystem = System.create "MyActorSystem" (Configuration.load ())
    
    // writer actor
    let consoleWriterActor = spawn myActorSystem "consoleWriterActor" (actorOf Actors.consoleWriterActor) 

    // new actor to validate messages
    let validationActor = spawn myActorSystem "validationActor" (actorOf2 (Actors.validationActor consoleWriterActor))
    
    // reader actor
    let consoleReaderActor = spawn myActorSystem "consoleReaderActor" (actorOf2 (Actors.consoleReaderActor validationActor))

    // tell the consoleReader actor to begin
    consoleReaderActor <! Start

    myActorSystem.WhenTerminated.Wait ()
    0
