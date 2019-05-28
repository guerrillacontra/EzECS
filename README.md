# EzECS

An Easy to use Entity Component System (ECS) for the Unity3D game engine.

An ECS is a useful way of organising your game logic code so that data/logic is separated
allowing you to iterate faster, refactor with less friction while increasing the amount of
re-use over your objects.

This flexibility is far superior to Unity's "GameObject model" while also providing greater performance
for real-time processing.

# How does it work?

1. Add the EzSpace script to a GameObject. This space will contain your entities and run a bunch of systems on them.
2. Add some EzSystem's to the EzSpace object. These systems will be prioritized to run in a specific order.
3. Create a GameObject and add it underneath the space in the hierarchy.
4. Add the EzEntity script to your GameObject. This GO is now an Entity.
2. Add some EzComponent's to your GameObject. These will automatically be registered to the entity at runtime.
3. Play
4. The systems will pick up all the relevent components and cache them effeciently, allowing you to listen, iterate and react.

# Design Principles

* Works with the "Unity way" of doing things, not against. Moving an Entity out of its Space in the hierarchy
  will automatically unregistered it from the space. Move it back in, re-registered. All sorts of things like that.
  
* A balance between high performing and easy to use design. All "Family" nodes are store contiguously in arrays
  for fast access however, for ease of use this is not fully Data Oriented Designed (DOD).

* Not forcing developers to work in a specific way. This is a light weight framework that allows you to optionally
  use a simple ECS without getting in your way.
  
* No singletons/globals. 

* Inheritance only uses for creating Components and Systems - nothing else. It is a flat hierarchy of inheritance.


# How to use

Extend the "EzSystem" script to create your own systems and add them to a GameObject that has an EzSpace.

Use the "EzFamilyCache<T>" to create caches that will cache component families at runtime on the fly inside your systems.

Extend the "EzComponent" script to create your own components and add them to a GameObject that has an EzEntity.
The Entity should be a child of an EzSpace.



# Example

There is a TestScene with some basic components/systems for you to play with. It shows you how to effectively handle collision detection
on child nodes within your entity and also how to correctly lay out your entity so that its data/presentation is seperated.


# Note

This is still in development and is not designed to compete with Unity's own "burst compiled" ECS.

The Unity ECS system is always changing and screwing me over while being fairly complex to work with.

Sure it is very fast which is great, but for game-logic, you don't really need it to be that fast when GPU/Physics
are what slow down your game anyway.
