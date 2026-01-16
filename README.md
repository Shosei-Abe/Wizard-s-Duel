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
* **NavMesh & AI Tools:** Intelligent pathfinding and obstacle avoidance.
* **Particle Systems:** Visual effects for Fireballs, Shields, and Teleportation.

## 🛠️ How to Play (Mac)
1.  Download the `.zip` file and unzip it.
2.  **Important:** Due to Mac security settings, you must right-click the app to open it.
    * **Right-click** (or Control+Click) `WizardsDuel.app`.
    * Select **Open**.
    * Click **Open** again in the pop-up window.
3.  Enjoy the duel!

---
*Created by Shosei Abe*
