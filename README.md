AI-Example
===

A simple XNA game that AI players compete in, intended as a simple learning exercise for artificial intelligence.
This was a term project I did for an artificial intelligence course.

It is a super simplified real time strategy game. Each team has a set of units, which gather resources and
talk to each other. They use these resources to activate radio towers or build new units. When they confront
enemies, they will attack. The behaviors are meant to be fairly customizable and it is easy to create new ones.

One feature I had planned was a sort of evolutionary mechanic. Teams would be given random parameters at the start
of each game. The winning team would get to keep their parameters into the next game, and ideally, the team with
the strongest traits would become dominant. I don't know what this would ultimately acoomplish, but it sounds
cool to me :D

The units are driven by a list of behaviors. Behaviors at the beginning of the list (index 0,1,2, etc.) have low
priority. The last behavior in the list has the highest priority, and if it is fired, will override all others.
Each behavior defines the conditions under which it will fire. Each behavior also provides a method which is called
when the behavior starts, and a method which is called every frame to update the behavior.

The units also pass messages to each other, and their "radio range" can be increased by being inside the radius of
a radio tower. They alert each other of nearby resources or enemies. Radio towers start off inactive, but units
can activate them by depositing resources into them.