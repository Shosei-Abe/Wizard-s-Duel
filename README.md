# Wizard's Duel

A 1-vs-1, 3D action game set in a magical arena. The player takes on the role of a wizard and fights against an AI-controlled enemy wizard using three types of magic: Attack, Defense, and Movement.

## 🔗 Project Documentation
* **Game Plan (Design Doc):** [View Google Doc](https://docs.google.com/document/d/1MWKwiUYaMSsBkOXo4Xk7uJaSHKdmiE0sGGDAyP-EhW4/edit?usp=sharing)
* **Task Management:** [Trello Board](https://trello.com/b/Tn7xgch2/wizards-duel)

## 🎮 Controls (Keyboard Only)
| Action | Key | Description |
| :--- | :---: | :--- |
| **Move** | `W` `A` `S` `D` | Move character / Orbit when locked-on |
| **Camera** | `Q` / `E` | Rotate camera left/right (Manual) |
| **Lock-On** | `Tab` | Toggle auto-lock-on to the enemy |
| **Attack (Fireball)** | `F` | Shoot a fireball (Consumes MP) |
| **Defense (Shield)** | `R` (Hold) | Deploy a magic shield (Consumes MP) |
| **Teleport** | `Space` | Short-range teleport to evade (Consumes MP) |

## 🧠 AI Logic Implementation
The Enemy AI utilizes **NavMesh** for movement and makes autonomous decisions based on the player's actions:
* **Chase:** Moves to maintain optimal distance using NavMesh Agents.
* **Attack:** Fires projectiles when the player is vulnerable.
* **Human-like Imperfection:** Includes "Reaction Time" and "Mistake Chance" to simulate a realistic opponent, rather than a perfect computer.

## ✨ Technical Highlights
This project implements features beyond the basic curriculum:
1.  **NavMesh & AI Tools:** Intelligent pathfinding and obstacle avoidance.
2.  **Particle Systems:** Visual effects for Fireballs, Shields, and Teleportation.

## 🛠️ How to Run
1.  Clone this repository.
2.  Open the project with **Unity 6 (6000.0.25f1)** or later.
3.  Open `Scenes/rpgpp_lt_scene_1.0`.
4.  Press Play!

---
*Created by Shosei Abe*
