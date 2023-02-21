open System
open Akka.FSharp
open Akka.Actor
open WinTail

[<EntryPoint>]
let main argv = 
    //SupervisionStrategy used by tailCoordinatorActor
    let strategy () = Strategy.OneForOne((fun ex ->
        match ex with
        | :? ArithmeticException  -> Directive.Resume
        | :? NotSupportedException -> Directive.Stop
        | _ -> Directive.Restart), 10, TimeSpan.FromSeconds(30.))

    // initialize an actor system
    let myActorSystem = System.create "MyActorSystem" (Configuration.load ())
    
    // writer actor
    let consoleWriterActor = spawn myActorSystem "consoleWriterActor" (actorOf Actors.consoleWriterActor) 

    let tailCoordinatorActor = spawnOpt myActorSystem "tailCoordinatorActor" (actorOf2 Actors.tailCoordinatorActor) [ SpawnOption.SupervisorStrategy(strategy ()) ]
    // pass tailCoordinatorActor to fileValidatorActorProps (just adding one extra arg)
    let fileValidatorActor = spawn myActorSystem "fileValidatorActor" (actorOf2 (Actors.fileValidatorActor consoleWriterActor tailCoordinatorActor))
    
    // reader actor
    let consoleReaderActor = spawn myActorSystem "consoleReaderActor" (actorOf2 (Actors.consoleReaderActor fileValidatorActor))

    // tell the consoleReader actor to begin
    consoleReaderActor <! Start

    myActorSystem.WhenTerminated.Wait ()
    0
