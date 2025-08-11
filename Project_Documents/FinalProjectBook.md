# **Project Name:** Shadow And Eye

**Developers:** Or Bar-Califa and Daniel Tselon Fradkin  
**Animators:** Naor Masri and Majd Francis  
**Mentor:** Ella Luna Pleasance  
**Department:** Department of Computer Science, Sapir Academic College

---

## Table of Contents

1.  [Introduction](#introduction)
2.  [Technical Infrastructure](#technical-infrastructure)
3.  [Existing State/Market Analysis](#existing-statemarket-analysis)
4.  [Characterization and Design](#characterization-and-design)
5.  [Use Cases/System Scenarios](#use-casessystem-scenarios)
6.  [Implementation](#implementation)
7.  [Technical Details](#technical-details)
8.  [Summary and Expansions](#summary-and-expansions)
9.  [Bibliography](#bibliography)
10. [Appendices](#appendices)

---

## Introduction

The game was developed from our desire to create a game that would use the ability of computer games to tell a story and foster a sense of capability and the ability to succeed despite difficulties. Our goal is to create a game that could help people dealing with mental health challenges, such as addiction or depression, to receive tools and improve their situation, all while creating a game that we would enjoy and love to play, combining our favorite mechanics from childhood games and games we play today.

### Genre

The game is a **Metroidvania**, a subgenre of action-adventure games that combines role-playing elements with a focus on non-linear exploration and ability-gated progression. True to the genre, players will navigate a large, interconnected world, with access to new areas being progressively unlocked as new abilities are acquired.

In "Shadow and Eye," this core Metroidvania loop is driven by the **suit system**. By defeating specific enemies, players acquire new suits, each of which grants a unique set of abilities. These abilities are essential for overcoming environmental obstacles, solving puzzles, and accessing previously unreachable parts of the map. This design encourages exploration and experimentation, as players must leverage their growing arsenal of suits to fully uncover the game's world. The gameplay also emphasizes **precise input**, rewarding skillful play in both combat and platforming.

### Synopsis

In a sick and unbalanced world, we play as Ado, an isolated shadow creature who embarks on a journey after an encounter with the Eye. The mission is to return the Eye to its rightful place, acquiring powerful suits from the enemies defeated along the way.

The **suit system** is the central pillar of the gameplay. It is not just a system for acquiring new abilities but the core mechanic that enables **dynamic playstyles and non-linear exploration**. Players are encouraged to experiment with different suits to overcome challenges, discover hidden paths, and tailor their combat approach to different enemies.

### Psychological Foundation

The game's narrative and themes are inspired by the analytical psychology of Carl Jung. Jung's theory of the human psyche, which divides the mind into various personalities and archetypes residing in the conscious and subconscious, serves as a foundational concept for the game's world and character development.

### Project Goals

- **Create a Compelling Single-Player Experience:** Our primary goal is to deliver a polished and engaging single-player adventure that will captivate players with its story, atmosphere, and gameplay.
- **Implement a Unique Character Transformation Mechanic:** The transformation system is the central pillar of our design. We aim to create a seamless and intuitive system that allows for strategic depth in both combat and exploration.
- **Develop Challenging Enemy AI:** Using the A\* Pathfinding Project, we will create intelligent and responsive enemies that provide a fair but challenging experience for players.
- **Build a Modular and Extensible Codebase:** We are committed to writing clean, well-documented, and modular code that will allow for easy maintenance and future expansion.

### Target Audience

"Shadow and Eye" is designed for players who enjoy action-adventure games, platformers, and puzzle-solvers. With its challenging gameplay and dark, atmospheric themes, we anticipate that the game will appeal to players aged 13 and up.

---

## Technical Infrastructure

This section outlines the core technologies and tools used to develop "Shadow and Eye".

### Programming

- **Unity:** The game is developed using Unity, a powerful cross-platform game engine. Its integrated development environment provides a comprehensive suite of tools for building, debugging, and deploying the game. The engine supplies fundamental physics, animation management, and allows for the use of external libraries and custom code to control and influence game objects, their movement, and their animations based on environmental events and player inputs.
- **C#:** An object-oriented language that allows for the creation and management of various objects. It is the primary programming language for development in Unity. All game logic, character behaviors, and system mechanics are written in C#.
- **JetBrains Rider:** The JetBrains IDE provides a development environment for writing code for games and .NET applications. We used it for writing our C# scripts.
- **A\* Pathfinding Project:** An open-source library that generates a graph based on the given map with possible points for movement and provides scripts to use the generated graph for navigation. We used this library to provide a movement path towards the player or a starting point for flying enemies.
- **Cinemachine:** A Unity library that, instead of scripting camera movements, allows for the definition of rules and targets for focus. It automatically calculates the camera's movement in real-time and enables the setup of multiple virtual cameras and a "brain" component for management and switching.
- **Universal Render Pipeline (URP):** This system processes the information in a scene into a visual image. It is based on Unity's older rendering system but is designed to be faster and more compatible with various hardware types, allowing for the definition and design of lighting in the project.

### Art/Visuals

- **Animations:** All character and environmental animations are created and managed within Unity's built-in animation system, allowing for precise control over timing and visual effects.
- **Art Style:** The game features a **modern, flat, minimalist, and abstract** art style, which contributes to its unique atmospheric aesthetic. The animations and player movement are designed to feel **snappy and fluid**.
- **Krita:** An open-source painting program used for concept art and preparing animations.
- **Krita Spritesheet Generator:** A plugin that exports animations to a spritesheet.

---

## Existing State/Market Analysis

#### Industry Context

The indie game market has experienced explosive growth over the past decade, fueled by accessible development tools and digital distribution platforms like Steam, Itch.io, and the Nintendo eShop. Within this thriving ecosystem, 2D action-adventure and Metroidvania games have carved out a significant niche, celebrated for their intricate level design, challenging gameplay, and compelling narratives. Titles like "Hollow Knight" and "Ori and the Blind Forest" have achieved massive commercial success and critical acclaim, demonstrating a strong and sustained appetite for high-quality, story-driven 2D experiences.

#### Comparative Analysis

"Shadow and Eye" enters a competitive landscape but distinguishes itself through its unique character transformation mechanic. The following table compares its core features with two of the most influential titles in the genre:

| Feature                                | Hollow Knight                                                                              | Ori and the Blind Forest                                                     | Shadow and Eye                                                                                                                  |
| :------------------------------------- | :----------------------------------------------------------------------------------------- | :--------------------------------------------------------------------------- | :------------------------------------------------------------------------------------------------------------------------------ |
| **Combat System**                      | Fast-paced, precise melee combat with a focus on dodging and parrying.                     | Fluid, ability-based combat with a mix of ranged and close-quarters attacks. | Hybrid system combining basic attacks with form-specific abilities.                                                             |
| **Platforming**                        | Challenging and technical, requiring mastery of core movement abilities.                   | Highly fluid and acrobatic, with an emphasis on chaining moves together.     | Puzzle-oriented platforming that requires switching between forms to navigate.                                                  |
| **Character Transformation/Abilities** | Abilities are acquired progressively, enhancing a single core moveset.                     | New abilities are learned to expand movement and combat options.             | **Core mechanic:** Player transforms into distinct forms (Duri and Ira), each with a unique moveset for combat and exploration. |
| **Art Style**                          | Hand-drawn 2D with a dark, gothic aesthetic.                                               | Lush, painterly art style with vibrant, atmospheric environments.            | **Modern, flat, and minimalist** with a dark, atmospheric tone.                                                                 |
| **Story-driven Narrative**             | Minimalist, lore-rich storytelling discovered through exploration and environmental clues. | Explicit, emotional narrative delivered through cutscenes and voice-over.    | A central narrative focused on the symbiotic relationship between Ado and the Eye, driving player progression.                  |

The key differentiator for "Shadow and Eye" is its **character transformation system**. While other games in the genre allow players to acquire new abilities, our game is built around the strategic choice of switching between entirely different character forms. This mechanic is not just an enhancement but the central pillar of the gameplay, deeply integrated into combat, platforming, and puzzle-solving, offering a unique and dynamic experience that sets it apart from its contemporaries.

---

## Characterization and Design

### System Goals

- **Fluid and Responsive Gameplay:** Deliver a seamless and reactive gameplay experience, ensuring that player actions are immediately and smoothly translated on-screen.
- **Meaningful Player Choices:** Create a system where decisions regarding abilities and transformations have a significant and tangible impact on gameplay and progression.
- **Effective State Management:** Implement a robust system to manage various game states, such as combat, dialogue, and exploration, to ensure smooth transitions and logical game flow.
- **Modular Quest and Story System:** Develop a flexible and scalable system for managing quests and story elements, allowing for easy expansion and modification.

### Design Choices

- **Art Style:** The unique art style was born from the need to abstract the characters to achieve more **'snappy and fluid'** animations that are not bound by perfect anatomy and fine details. We took inspiration from games like _Hollow Knight_, _Ori and the Blind Forest_, and _Rain World_, combining a dark aesthetic with solid, bold colors. This allows for clear character definition and a final look that is both eye-catching and thematically appropriate.
- **Character Design:** We decided the player would control a shadow, an indistinct character, to allow players to feel that the character itself is less important and that they can see themselves in that role.
- **Genre Choice:** The choice of the platformer genre was made with the idea of 'climbing' from the subconscious to the conscious mind. We felt this style would uniquely suit and enhance the existing thematic elements.

### Character Design

- **Ado (The Protagonist):** Ado is the main character, a being of shadow who serves as the player's avatar.
- **The Eye:** A symbiotic entity that attaches to Ado, granting transformative powers and serving as a key narrative element.
- **Transformations (Duri and Ira):** Ado can transform into different forms, each with unique visual designs and abilities that the player will use to overcome challenges.

---

## Use Cases/System Scenarios

This section outlines typical user stories and system processes that illustrate the core gameplay mechanics of "Shadow and Eye."

### Scenario 1: Combat and Transformation

- **User Story:** "As a player, I encounter an enemy I can't defeat in my base form, so I transform into 'Duri' to use its heavy attack and break the enemy's shield."
- **System Process:**
  1.  The player, controlling Ado, engages an enemy with a shield that deflects standard attacks.
  2.  The player presses the transformation button, which momentarily pauses the game and brings up a radial transformation wheel.
  3.  Using the mouse or controller stick, the player selects the 'Duri' form from the wheel.
  4.  The game resumes, and an animation sequence shows Ado transforming into Duri. Ado's sprite is replaced with Duri's, and the player's ability set is updated to match the new form.
  5.  The player uses Duri's specific heavy attack, which has shield-breaking properties.
  6.  The attack animation plays, the enemy's shield shatters, and the enemy becomes vulnerable to further damage, allowing the player to defeat it.

### Scenario 2: Exploration and Puzzles

- **User Story:** "As a player, I reach a high ledge I can't jump to, so I transform into 'Ira' to use its dash ability to cross the gap."
- **System Process:**
  1.  The player navigates a level and encounters a ledge that is too high or distant to be reached with Ado's standard jump.
  2.  Recognizing the environmental puzzle, the player opens the transformation wheel and selects the 'Ira' form, which is designed for agility.
  3.  Ado's sprite and abilities are switched to Ira's, which includes a mid-air dash.
  4.  The player performs a standard jump towards the ledge.
  5.  At the apex of the jump, the player presses the dash button.
  6.  Ira performs a quick horizontal dash in mid-air, covering the remaining distance and landing successfully on the previously unreachable platform.

### Scenario 3: Acquiring a New Suit

- **User Story:** "As a player, I defeated a powerful enemy, and it dropped a new suit that I can now equip."
- **System Process:**
  1.  The player defeats a specific enemy, reducing its health to zero.
  2.  The enemy's `OnDeath()` method is triggered.
  3.  A suit pickup object is instantiated and dropped in the game world.
  4.  The player collides with the pickup object.
  5.  The `OnTriggerEnter2D` method on the `SuitPickup` script is called.
  6.  The `PlayerController`'s `EquipSuit()` method is called, adding the new suit to the player's available transformations and equipping it.

---

## Implementation

This section details the technical architecture of the game's core systems, outlining the design and functionality of key components.

- **Game State Manager:** Built with a singleton pattern, this manager addresses the need to manage the game's various states (e.g., main menu, playing, paused).
- **UI Manager:** Responsible for managing all graphical UI elements, such as menus, tutorial text, and scene transitions.
- **Beacon:** A system for broadcasting events between different game components, allowing for decoupled communication.
- **Enemy State Machine:** Controls the behavior of enemies using a finite state machine, managing states like idle, patrol, chase, and attack.
- **Player Controller:** Handles all player-related actions, including movement, jumping, and attacking.
- **Level Manager:** Manages the loading and unloading of game levels and scenes.
- **Save System:** Responsible for saving and loading the player's progress.
- **Suit System (Transformation):** The suit system is designed to be the heart of the player's progression. It manages not only the visual transformation of the character but also the complete overhaul of the player's abilities, stats, and interaction with the game world. This system is what allows for the game's non-linear exploration, as different suits grant access to previously unreachable areas.

---

## Technical Details

This section provides a deeper look into the specific algorithms and data structures that are foundational to the game's mechanics.

### A\* Pathfinding Algorithm

The A\* (pronounced "A-star") algorithm is a widely used pathfinding algorithm known for its efficiency and accuracy. Its core concept is to find the shortest path between two points by considering both the actual distance from the start point and an estimated (heuristic) distance to the end point, often using the Manhattan distance as a heuristic. This combination allows it to make intelligent decisions, prioritizing paths that are likely to be optimal without exploring every possible route.

In "Shadow and Eye," we utilize the **A\* Pathfinding Project**, a powerful third-party library for Unity. This library implements the A\* algorithm to enable sophisticated enemy navigation. Enemies can:

- **Navigate Complex Environments:** Traverse the game world, moving around walls, platforms, and other obstacles.
- **Pursue the Player:** Dynamically calculate the most efficient route to the player's current location, even as the player moves.
- **Avoid Obstacles:** Update their paths in real-time to react to dynamic obstacles or changes in the level geometry.

### State Machine for AI and Game States

A Finite State Machine (FSM) is a computational model used to design systems that can be in one of a finite number of states at any given time. The system transitions from one state to another in response to specific inputs or conditions. This model is ideal for managing game logic in a clean and organized manner.

- **Enemy AI:** We use FSMs to control the behavior of enemies. Each enemy has a set of states, such as:

  - `Idle`: The enemy is stationary or follows a simple patrol route.
  - `Patrol`: The enemy moves along a predefined path.
  - `Chase`: The enemy has detected the player and is actively pursuing them.
  - `Attack`: The enemy is within range and is performing an attack.
    Transitions between these states are triggered by events like the player entering an enemy's line of sight or moving into attack range.

- **Game State Management:** A similar FSM is used to manage the overall game flow. This includes states like `MainMenu`, `Playing`, `Paused`, and `GameOver`, ensuring that the game behaves correctly during each phase (e.g., freezing gameplay when paused).

### Data Structures for Player Abilities

To manage the player's diverse abilities, especially across the different transformations (Duri and Ira), we use a dictionary-like data structure (such as C#'s `Dictionary<TKey, TValue>`). This structure maps an ability identifier (e.g., a string like `"Duri_Heavy_Attack"` or an enum) to the corresponding ability object or function.

This approach offers several advantages:

- **Efficiency:** It provides a fast and efficient way to look up and activate abilities based on the player's current form. When the player transforms, the system can quickly access the set of abilities associated with that form.
- **Scalability:** New abilities or even new transformations can be added with minimal changes to the core system. We simply add new entries to the data structure.
- **Organization:** It keeps the ability-related code clean and decoupled, as each ability's logic is encapsulated and can be accessed through a unified interface.
- **Ability Activation:** Each suit has a unique set of abilities that are managed by the suit system. When a form is active, its corresponding abilities are enabled, allowing the player to use them to overcome specific challenges.

---

## Summary and Expansions

#### Project Summary

"Shadow and Eye" is a 2D action-adventure game featuring a unique character transformation mechanic. The project has successfully reached a stage where the core systems for gameplay are fully implemented. This includes responsive player controls, a dynamic combat system, intelligent enemy AI, and the versatile transformation system that allows the player to switch between different forms with unique abilities.

#### Future Expansions

With the foundational systems in place, "Shadow and Eye" is well-positioned for future growth. Potential expansions could include:

- **More Transformations:** Introducing new forms for Ado to discover throughout the game world, each offering a distinct set of abilities and strategic options.
- **Expanded World:** Designing and building new levels, regions, and secret areas to encourage exploration and expand the game's scope.
- **Deeper Story:** Enriching the narrative by adding more quests, non-player characters (NPCs), and lore to build a more immersive and engaging world.
- **Boss Battles:** Creating complex, multi-stage boss encounters that challenge players to master the game's mechanics and use their transformations strategically.

---

## Bibliography

- **A\* Pathfinding Project:** A powerful and flexible pathfinding system for Unity. [https://arongranberg.com/astar/](https://arongranberg.com/astar/)
- **Unity Engine Documentation:** The official documentation for the Unity game engine. [https://docs.unity.com/](https://docs.unity.com/)
- "A\* Pathfinding Project Tutorials"
- "Coyote Time Implementation Tutorials"
- "Cinemachine Tutorials"

---

## Appendices

### Game Page

[Link to the game](https://[YourAccount].itch.io/[YourGame])

- "Carl Jung's book on his theory"
- "Project Poster"
