# What is this Repository
A small C# Game Engine using [SFML.NET](https://www.sfml-dev.org/download/sfml.net/) created for my own learning of making games.
There are also some small games or otherwise testing grounds to test the extensiveness of my engine's implementation as well as drive developments and improvements to the engine itself.

Coroutines were particularly fun for me to implement.

## How do I run this?
Visual Studio and .NET 8.0

Ensure implicit usings is enabled.

## Why is this Repository?
For fun and learning.

This project is not intended to be used for serious development by anyone, but I might use it for a Ludum Dare if I ever feel adventurous enough and foolish enough not to just use Unity.

## How is this Repository?
Not too bad, and yourself?

## Why use SFML?
At time of writing, I'm not comfortable enough with graphics programming (OpenGL, especially not Vulkan) to
bother with writing it from the ground up. This Game Engine was created as a way to understand and appreciate what larger engines like Unity and Godot are doing and why they might have made certain design decisions.

This is also why there are 3 different implementations of GameObject, 2 of which are not referenced in the most actively used since I found the shortcomings in those designs early on.

SFML handles all of the low level basics as well as some of the higher level components that I may attempt to write myself once I am confident in my abilities to replace them with something of my own. Since most of that space is in OS level API land, I will also probably move to C++ at that point since I would have to write my own wrappers or rely on whatever wrappers are available and deal with the joy that is copious C# DLL loading otherwise.

## Why are there 3 different implementations of GameObject anyway?
In short, strongly typed externally defined state is not something C# does well and loosely typed externally defined state becomes like using a dynamic language and I prefer having tools which can tell me when I have made a typo. 

(These are the base classes defined in packages BaseEngine.GameObjects.ExternalState.StateDict and BaseEngine.GameObjects.ExternalState.Stateful respectively)

The latter was my first implementation attempt and the former was the attempted strongly-typed fix before I realized that I just didn't like the design of either very much, not that they are wrong or necessarily even bad in theory, but that they ended up feeling better expressed in my final inheritance-based design - despite my initial attempts to rely more on interfaces and composition.

As a matter of fact, much of the final - and now mostly used design initially started as interfaces with default implementations akin to mixins before moving them to abstract classes once I realized there was more implementation which needed to be shared before finally moving to concrete classes once I realized that at a base level, GameObjects have so much implementation already in them that the GameObject base class itself is enough for a reasonable template for what might be needed in of itself: such as a GameObject which simply has a Collider, or some SFML Drawable objects to draw and otherwise no behavior, etc.

Unfortunately those iterations won't be found in the commit history as I had already changed it to its current form when I decided to begin committing this code to GitHub.
