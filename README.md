# 🧩 PuzzleSolver

![CI](https://github.com/Alex-Fix/puzzle-solver/actions/workflows/release.yml/badge.svg)

**PuzzleSolver** is a cross-platform, high-performance .NET console application and library for solving different kinds of puzzles using multiple algorithms.

It is built with a **modular architecture**, modern **.NET 10 features**, and best practices around **dependency injection, validation, testing, and CI/CD**. The project is designed to scale both in **supported puzzle types** and **solving strategies**.

---

## ✨ Features

* ⚙️ Modular, extensible architecture
* 🚀 High-performance solving algorithms
* 🧠 Multiple algorithm strategies (Beam Search, AStar, Bfs, Dfs)
* 🌍 Cross-platform (Windows, macOS, Linux)
* 🖥️ Clean CLI powered by **Spectre.Console**
* 🎭 Browser automation via **Microsoft Playwright**
* 🧪 Strong test coverage (unit, functional, integration)
* 📦 Self-contained published binaries
* 📊 Built-in benchmark project for performance analysis
* 🔁 Fully automated CI/CD & GitHub Releases

---

## 🧱 Project Structure

```text
src/
├── PuzzleSolver.Core          # Core domain models, algorithms, solvers
├── PuzzleSolver.Automation    # Browser automation (Playwright-based)
├── PuzzleSolver.Cli           # CLI entry point and commands
├── PuzzleSolver.Benchmarks    # Benchmarks for algorithms defined in Core

tests/
├── PuzzleSolver.Core.UnitTests
├── PuzzleSolver.Cli.FunctionalTests
└── PuzzleSolver.Automation.IntegrationTests
```

### Key Modules

* **PuzzleSolver.Core**

    * Puzzle state models
    * Solver abstractions
    * Algorithm implementations
* **PuzzleSolver.Automation**

    * Puzzle extraction via browser automation
    * Playwright-based automation factories
* **PuzzleSolver.Cli**

    * Command-line interface
    * Argument parsing & validation
    * Exit codes and exception handling
* **PuzzleSolver.Benchmarks**

    * Benchmark harness
    * Solver performance tests
    * Memory and GC profiling

---

## 🧑‍💻 Requirements

* **.NET SDK 10.0**
* **PowerShell 7+** (needed for Playwright installation)

    * [Install PowerShell](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
* Supported OS:

    * Windows (x64)
    * macOS (arm64)
    * Linux (x64)

> Published binaries are **self-contained** — .NET runtime not required to run.

---

## 🏗️ First-Time Development Setup

After cloning the repository, perform the following steps from the **root of the project**:

```bash
# Build the solution
dotnet build

# Install Playwright dependencies for automation
pwsh artifacts/bin/PuzzleSolver.Cli/debug/playwright.ps1 install --with-deps chromium
```

---

## 🏃‍♂️ Running the Application During Development

You can run the application directly from source using `dotnet run`:

```bash
dotnet run --project src/PuzzleSolver.Cli/PuzzleSolver.Cli.csproj -- ballsort https://en.grandgames.net/ballsort_classic/id477125
```

This will execute the **Ball Sort solver** on the provided URL using default options.

---

## 🚀 Installation & Platform Setup

### macOS

```bash
# Make all scripts and executables runnable
chmod -R +x .

# Allow execution of downloaded files
xattr -dr com.apple.quarantine .
```

### Linux

```bash
# Make all scripts and executables runnable
chmod -R +x .
```

> Windows usually does not require these steps.

---

### Playwright Setup

Before using any automation commands (BallSort, etc.), install Playwright dependencies:

```powershell
pwsh playwright.ps1 install --with-deps chromium
```

> Run this **from the root of the project** or from the `PuzzleSolver.Cli` folder where `playwright.ps1` is located.

---

## 🖥️ CLI Usage

### Global Help

```bash
puzzle-solver --help
```

```text
USAGE:
    puzzle-solver [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    ballsort <url>    Solve a Ball Sort puzzle from given Url
```

### Ball Sort Solver

Solve a Ball Sort puzzle directly from a URL.

```bash
puzzle-solver ballsort <url> [OPTIONS]
```

#### Arguments

| Argument | Description                 |
| -------- | --------------------------- |
| `url`    | URL of the Ball Sort puzzle |

Example:

```text
https://en.grandgames.net/ballsort_classic/id477125
```

#### Options

| Option              | Default      | Description                                             |
| ------------------- | ------------ |---------------------------------------------------------|
| `-a, --algorithm`   | `BeamSearch` | Solving algorithm (`BeamSearch`, `AStar`, `Bfs`, `Dfs`) |
| `-b, --beamwidth`   | `500`        | Beam width (Beam Search only)                           |
| `-H, --headless`    | `false`      | Run browser in headless mode                            |
| `-m, --movedelayms` | `600`        | Delay (ms) between moves during playback                |
| `-h, --help`        | —            | Show help                                               |

#### 🎥 Demo video
https://private-user-images.githubusercontent.com/58109972/543418799-27017f66-2f23-4dc6-b778-bd6edeb1ffef.mp4?jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3Njk5NDk0OTMsIm5iZiI6MTc2OTk0OTE5MywicGF0aCI6Ii81ODEwOTk3Mi81NDM0MTg3OTktMjcwMTdmNjYtMmYyMy00ZGM2LWI3NzgtYmQ2ZWRlYjFmZmVmLm1wND9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFWQ09EWUxTQTUzUFFLNFpBJTJGMjAyNjAyMDElMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjYwMjAxVDEyMzMxM1omWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPTMzNTQ0NzUxY2ZmMjMwMDY5ZWQ4MTVmMjM5M2ZlYTA5OTRmNGVlZjA5MDc0ODJiYzJkNTQyYzIwYmE0NDQzMmYmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0In0.hH6BNaIlIavZQzPeNngKaP7tEsHWnCzV9FHnm7ufOhQ

---

## 🧠 Algorithms

Currently implemented:

* **Beam Search** (default)
* **AStar**
* **Bfs**
* **Dfs**

The architecture allows easy addition of new algorithms by implementing solver interfaces in `PuzzleSolver.Core`.

---

## 🎭 Browser Automation

PuzzleSolver uses **Microsoft Playwright** to:

* Load puzzle pages
* Extract initial puzzle state
* Replay solution steps visually

---

## 🧪 Testing

The project includes:

* **Unit tests** — core algorithms & state logic
* **Functional tests** — CLI behavior
* **Integration tests** — Playwright automation

Run all tests:

```bash
dotnet test -c Release
```

---

## 🏁 Running Benchmarks

Benchmarks are in PuzzleSolver.Benchmarks:

Run all benchmarks

```bash
dotnet run --project src/PuzzleSolver.Benchmarks/PuzzleSolver.Benchmarks.csproj -c Release
```

### BallSort Solver Performance

| Method           | Mean          | Error       | StdDev      | Ratio     | RatioSD | Gen0      | Gen1      | Gen2     | Allocated   | Alloc Ratio |
|----------------- |--------------:|------------:|------------:|----------:|--------:|----------:|----------:|---------:|------------:|------------:|
| Solve_BeamSearch |      3.285 us |   0.0145 us |   0.0113 us |      1.00 |    0.00 |    1.4534 |    0.0381 |        - |     8.92 KB |        1.00 |
| Solve_BeamSearch |  6,334.846 us | 156.2174 us | 121.9643 us |  1,928.49 |   36.24 |  562.5000 |  242.1875 |  93.7500 |  3411.58 KB |      382.38 |
| Solve_BeamSearch |  6,698.393 us |  22.2290 us |  17.3550 us |  2,039.16 |    8.43 |  562.5000 |  281.2500 |  93.7500 |     3563 KB |      399.36 |
| Solve_AStar      |      1.384 us |   0.0047 us |   0.0036 us |      0.42 |    0.00 |    0.4292 |    0.0019 |        - |     2.64 KB |        0.30 |
| Solve_AStar      |     17.014 us |   0.1309 us |   0.1022 us |      5.18 |    0.03 |    4.6082 |    0.3052 |        - |     28.3 KB |        3.17 |
| Solve_AStar      |     34.962 us |   0.2211 us |   0.1463 us |     10.64 |    0.06 |    7.3242 |    0.6714 |        - |    45.02 KB |        5.05 |
| Solve_Bfs        |      2.589 us |   0.0048 us |   0.0032 us |      0.79 |    0.00 |    0.8011 |    0.0076 |        - |     4.92 KB |        0.55 |
| Solve_Bfs        | 20,617.613 us |  81.7495 us |  63.8246 us |  6,276.53 |   27.90 | 1687.5000 |  718.7500 | 281.2500 |  10718.7 KB |    1,201.40 |
| Solve_Bfs        | 42,331.286 us | 318.4200 us | 248.6014 us | 12,886.73 |   84.26 | 3250.0000 | 1166.6667 | 583.3333 | 20082.25 KB |    2,250.90 |
| Solve_Dfs        |      1.123 us |   0.0051 us |   0.0040 us |      0.34 |    0.00 |    0.4215 |    0.0019 |        - |     2.59 KB |        0.29 |
| Solve_Dfs        |    191.279 us |   0.9089 us |   0.7096 us |     58.23 |    0.28 |   39.0625 |   12.9395 |        - |   240.45 KB |       26.95 |
| Solve_Dfs        |    117.233 us |   0.7651 us |   0.5973 us |     35.69 |    0.21 |   26.2451 |    6.8359 |        - |   161.56 KB |       18.11 |


---

## 🔁 CI/CD

PuzzleSolver uses **GitHub Actions** for automated testing, publishing, and releases:

* **Trigger:** Push **tags** matching `vX.Y`
* **Jobs:**

    1. **Test:** Runs on `ubuntu-latest`

        * Clones repo, installs .NET, builds solution
        * Installs Playwright dependencies (`chromium`) via PowerShell
        * Runs unit, functional, and integration tests
    2. **Publish:** Runs on Linux, macOS, Windows

        * Builds **self-contained binaries** for each platform
        * Renames executables (`puzzle-solver` or `puzzle-solver.exe`)
        * Uploads artifacts for release
    3. **Release:** Runs on `ubuntu-latest`

        * Downloads published artifacts
        * Creates `.zip` packages for each platform
        * Uploads to GitHub Releases
        * Generates release notes automatically

> Versioning is **driven by Git tags**. CI ensures that each release is reproducible, tested, and cross-platform.

---

## 📦 Dependencies

Key technologies used:

* **.NET 10**
* **Spectre.Console.Cli** — CLI framework
* **Microsoft Playwright** — browser automation
* **xUnit** + **FluentAssertions** — testing
* **Central Package Management** via `Directory.Packages.props`

---

## 🛣️ Roadmap

* Add more puzzle types
* More heuristic strategies
* Parallel solving
* Export solutions to files
* Interactive / TUI modes

---

## 👤 About the Author

**Alex Papish** — senior software engineer and creator of **PuzzleSolver**.

* GitHub: [Alex-Fix](https://github.com/Alex-Fix)
* This project reflects modern .NET 10 best practices, test-driven development, and cross-platform
