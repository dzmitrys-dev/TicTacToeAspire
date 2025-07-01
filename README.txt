# Distributed Tic-Tac-Toe with .NET Aspire and YARP

This project is a solution to the "Distributed Tic-Tac-Toe Microservices" home assignment, enhanced with an API Gateway and thread-safe services. It implements a fully automated game of Tic-Tac-Toe using a microservices architecture orchestrated by .NET Aspire.

## Final Architecture

The system is composed of four primary services that work together to simulate a game:

-   **`ApiGateway`**: The central entry point for all incoming requests, built with **YARP (Yet Another Reverse Proxy)**. It routes traffic to the appropriate backend service, simplifying the network topology and securing the backend.
-   **`GameEngineService`**: A .NET Web API that manages the core game logic, including board state, move validation, and determining game outcomes. It now includes a **locking mechanism** to handle concurrent move requests safely.
-   **`GameSessionService`**: A .NET Web API responsible for managing game sessions. It contains an automated player that generates random moves and communicates with the `GameEngineService` through the `ApiGateway`.
-   **`WebApp`**: A server-side Blazor application that provides a real-time visual representation of the game. It communicates exclusively with the `ApiGateway`.
-   **`TicTacToeAspire.AppHost`**: The .NET Aspire orchestrator project that launches, configures, and wires together all services for local development and testing.

## Prerequisites

To build and run this application, you will need the following tools installed:
-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (with the **ASP.NET and web development** workload, including the **.NET Aspire SDK**)
-   [Docker Desktop](https://www.docker.com/products/docker-desktop/) (must be running in the background)

## How to Run the Application

1.  **Clone the Repository**:
    ```bash
    git clone [https://github.com/dzmitrys-dev/TicTacToeAspire.git](https://github.com/dzmitrys-dev/TicTacToeAspire.git)
    cd TicTacToeAspire
    ```
2.  **Open the Solution**: Open the `TicTacToeAspire.sln` file in Visual Studio 2022.
3.  **Set Startup Project**: Ensure that `TicTacToeAspire.AppHost` is set as the startup project.
4.  **Run the Application**: Press **F5** or click the "Start" button in Visual Studio.

This will perform the following actions:
-   Restore all NuGet packages.
-   Build all projects in the solution.
-   Launch the .NET Aspire dashboard in your browser, where you can view logs, traces, and environment variables for all services.
-   Launch the Blazor `WebApp` in a separate browser tab.
-   Click the **"Start New Simulation"** button in the web app to begin a game.

## How to Run the Tests

The solution includes a dedicated test project (`TicTacToeAspire.Tests`) for end-to-end integration testing. The tests validate the entire system, including the API Gateway and concurrency management.

1.  **Open the Test Explorer**: In Visual Studio, go to **Test** > **Test Explorer**.
2.  **Build the Solution**: Build the solution by pressing **Ctrl + Shift + B** to ensure the tests are discovered.
3.  **Run the Tests**: In the Test Explorer window, click the **"Run All Tests"** button (the double green play icon).

The test suite will launch the entire distributed application in-memory, run the simulations, and verify that the system behaves correctly under normal and concurrent conditions.

## Discussion on Implemented & Potential Improvements

### Implemented Improvements

1.  **.Net Apire**: A unified entry point. This decouples the front-end from the backend services, allowing them to evolve independently. It also centralizes routing logic and provides a single point for implementing cross-cutting concerns like authentication, rate limiting, and caching in the future.
2.  **Concurrency Management**: The `GameEngineService` is now thread-safe. By implementing a per-game locking mechanism, we prevent race conditions that could occur if multiple move requests for the same game were processed simultaneously. This ensures data integrity and system stability under concurrent load.

### Potential Future Enhancements

1.  **Real-Time UI Updates with SignalR**: The current front-end polls for updates every second. A more efficient approach would be to use **SignalR**. The `GameSessionService` could host a SignalR hub and push game state changes to the Blazor client in real-time, eliminating polling and reducing network latency.
2.  **Data Persistence**: The services currently use in-memory storage, meaning all data is lost on restart. Migrating to a persistent database (e.g., **PostgreSQL** or **SQLite**, managed as another containerized resource in the Aspire `AppHost`) would allow game history to be stored and analyzed permanently.
3.  **Containerization for Production**: While .NET Aspire manages container orchestration for development, the next step would be to create `Dockerfile`s for each service. This would prepare the application for deployment to a production container environment like **Azure Container Apps** or **Kubernetes**.