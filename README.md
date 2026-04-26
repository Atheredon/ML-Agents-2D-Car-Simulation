Unity 2D Autonomous Racing AI
This project demonstrates the development of an autonomous racing agent in a 2D environment using Deep Reinforcement Learning. Built with Unity and the ML-Agents Toolkit, the agent learns to navigate complex tracks, avoid obstacles, and optimize lap times through trial and error.

Key Features
PPO (Proximal Policy Optimization): Utilizes state-of-the-art reinforcement learning algorithms for stable and efficient training.
Curriculum Learning: The agent is trained through progressively difficult stages, from simple tracks to complex layouts with obstacles.Raycast 
Perception: Implements a multi-sensor raycast system providing the agent with a 360-degree awareness of its surroundings.
Event-Driven Reward System: A modular C# event system for handling rewards based on checkpoints and collisions.

System Architecture
The project follows the standard ML-Agents architecture, establishing a high-performance communication link between the Unity simulation and the Python-based training environment.
Unity Environment: Handles physics (Box2D), rendering, and sensor data collection.
Communicator (gRPC): Facilitates real-time data exchange between the C# simulation and the Python API.
Python Trainer: Executes the PPO algorithm and updates the Neural Network weights.

The Learning Loop
The training process follows the standard Reinforcement Learning cycle:
Observation: The agent receives raycast distances and velocity data.
Action: The neural network outputs continuous values for acceleration and steering.
Reward: The environment provides feedback based on the agent's performance.

Reward Function
The agent's behavior is shaped by a multi-objective reward function designed to maximize speed while ensuring safety:
R_total = w_1 * R_checkpoint + w_2 * R_velocity - w_3 * R_collision - w_4 * R_time
Checkpoint Reward: Significant positive reward for passing markers in the correct order.
Velocity Reward: Small positive reward proportional to speed to encourage faster lap times.
Collision Penalty: Large negative reward for hitting track walls or obstacles.
Time Penalty: Small negative reward per step to prevent idling and encourage efficiency.

Tech Stack
Game Engine: Unity 2022.3 LTS
AI Framework: Unity ML-Agents Toolkit
Programming: C#, Python
ML Libraries: PyTorch, NumPy
Environment Management: Python venv

How to Run
Clone the repo: git clone https://github.com/yourusername/Unity-2D-Autonomous-Racing-AI.git
Unity Setup: Open the project in Unity Hub (2022.3+). Load the TrainingScene.
Python Setup: 
    python -m venv venv
    source venv/bin/activate  # or venv\Scripts\activate on Windows
    pip install mlagents
Inference: To see the trained model in action, press Play in Unity. The agent will use the provided .onnx model.