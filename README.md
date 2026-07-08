# 🔄 Turn-Based Network Synchronisation Framework
[![Unity 6 Ready](https://img.shields.io/badge/Unity-6.5+-blue.svg?style=flat-square&logo=unity)](https://unity.com/)
[![Render Pipeline](https://img.shields.io/badge/Render%20Pipeline-URP%202D-green.svg?style=flat-square)](#)
[![Architecture](https://img.shields.io/badge/Architecture-SOAP%20%2B%20Command-orange.svg?style=flat-square)](#)

A high-performance, studio-grade architecture sample simulating asynchronous turn-based multiplayer network state reconciliation. Built specifically to demonstrate scalable engineering patterns required for complex digital tabletop and strategy game states.

---

## 🛠️ Core Architectural Pillars

### 1. Command Pattern Deserialization
Every discrete gameplay action (e.g., token placement, tile modification) is treated as a serializable command object. This isolates game logic from the network layer and allows actions to be buffered, validated, or safely replayed forward during synchronisation delays.

### 2. Zero-Allocation Mobile Optimisation
To maintain strict execution constraints matching production mobile hardware guidelines, heavy payload operations like JSON string reflection in runtime loops are completely bypassed. Transmission paths run on lightweight, primitive `struct` snapshots.

### 3. Decoupled Scriptable Architecture
State machinery transitions are completely isolated using **SOAP (ScriptableObject Architecture Pattern)** variables, ensuring full visual and state decoupling to mitigate high-friction dependency chains.

---

## 📁 Repository Structure

```text
Assets/
└── 📂 Core/
    └── 📂 TurnNetworkSystem/
        ├── 📄 IGameCommand.cs          # Command Interfaces & Concrete Data Packets
        └── 📄 TurnNetworkManager.cs    # Native Main-Loop Coroutine State Manager

💻 Technical Blueprint
Direct Main-Loop Simulation Matrix
C#
// Low-allocation pure data packet footprint ready for network stream buffers
[Serializable]
public struct TokenNetworkPacket
{
    public int PlayerId;
    public int TargetGridIndex;
    public int TokenValue;
}

The state pipeline processes immediate client-side simulation (local prediction for zero input latency) before shifting cleanly into an asynchronous Coroutine buffer to seamlessly simulate connection drops, penalty handshakes, and strict network validation.

⚡ Unity 6 & Performance Toolset Workflow
Unity Muse Integration: System logic and command expansions leverage Muse-assisted workflows for clean edge-case assertions.

Profile Analyser Verification: Core data loops are audited directly via the Unity Profile Analyser to verify zero Garbage Collection (GC) spikes during heavy simulation routines.

Universal Render Pipeline (URP): Constructed natively within a clean URP 2D matrix to align with top-tier mobile graphics optimisation workflows.

Developed as a high-fidelity technical practice framework modelled after enterprise synchronisation pipelines.
