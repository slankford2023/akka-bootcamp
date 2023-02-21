﻿open System
open Akka.FSharp
open Akka.FSharp.Spawn
open Akka.Actor
open WinTail

let printInstructions () =
    Console.WriteLine "Write whatever you want into the console!"
    Console.Write "Some lines will appear as"
    Console.ForegroundColor <- ConsoleColor.Red
    Console.Write " red"
    Console.ResetColor ()
    Console.Write " and others will appear as"
    Console.ForegroundColor <- ConsoleColor.Green
    Console.Write " green! "
    Console.ResetColor ()
    Console.WriteLine ()
    Console.WriteLine ()
    Console.WriteLine "Type 'exit' to quit this application at any time.\n"

[<EntryPoint>]
let main argv = 
    // initialize an actor system
    let myActorSystem = System.create "MyActorSystem" (Configuration.load ())
        
    printInstructions ()
    
    // make your first actors using the 'spawn' function
    let consoleWriterActor = spawn myActorSystem "consoleWriterActor" (actorOf Actors.consoleWriterActor)  
    let consoleReaderActor = spawn myActorSystem "consoleReaderActor" (actorOf2 (Actors.consoleReaderActor consoleWriterActor))

    // tell the consoleReader actor to begin
    consoleReaderActor <! Actors.Start

    myActorSystem.WhenTerminated.Wait ()
    0
