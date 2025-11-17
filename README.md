# SurviveGA - evolutionary simulation through genetic algorithms

SurviveGA is a research project developed as part of the MSc Computer Games Technology program at Abertay University in Dundee, within the class MAT501 – Applied Mathematics and Artificial Intelligence.

Project Description
SurviveGA is an evolutionary simulation platform that models the emergence of survival behaviors through genetic algorithms and natural selection principles. The system simulates a population of autonomous agents that must learn to navigate complex environments containing beneficial resources, hazardous zones, and dangerous obstacles—all without explicit programming of survival strategies.

Core Concept
At its foundation, SurviveGA explores how simple organisms can evolve sophisticated survival behaviors over multiple generations through the mechanisms of:

Selection pressure from environmental hazards and resource availability

Genetic crossover combining successful traits from parent agents

Random mutation introducing behavioral diversity

Elitism preserving the most successful adaptations

Unlike traditional pathfinding or navigation systems, agents are not given explicit instructions on how to survive. Instead, each agent possesses a unique genetic code—a sequence of movement directions—that determines its behavior throughout its lifetime. Agents that successfully locate food while avoiding dangers achieve higher fitness scores, making them more likely to pass their genetic traits to subsequent generations.

Evolutionary Mechanics
The simulation operates on generational cycles where:

A population of agents executes behaviors encoded in their DNA

Fitness scores accumulate based on interactions with the environment

High-performing agents are selected as parents for the next generation

Offspring inherit genetic material through two-point crossover

Random mutations introduce novel behaviors and prevent local optima

The cycle repeats, allowing complex survival strategies to emerge

Research Applications
SurviveGA serves as a testbed for studying:

Emergent behavior arising from simple rules and selection pressure

Adaptive strategies in multi-objective environments (approach food, avoid danger)

Population dynamics and the balance between exploration and exploitation

Evolutionary convergence toward optimal survival behaviors

The role of mutation rates in maintaining population diversity

This simulation provides insights into fundamental questions about evolution, learning, and adaptation—demonstrating how complexity can emerge from simplicity through iterative selection over time.
