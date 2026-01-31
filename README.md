# 🧩 PuzzleSolver

![CI](https://github.com/Alex-Fix/puzzle-solver/actions/workflows/release.yml/badge.svg)

**PuzzleSolver** is a cross-platform, high-performance .NET console application and library for solving different kinds of puzzles using multiple algorithms.

It is built with a **modular architecture**, modern **.NET 10 features**, and best practices around **dependency injection, validation, testing, and CI/CD**. The project is designed to scale both in **supported puzzle types** and **solving strategies**.

---

## ✨ Features

* ⚙️ Modular, extensible architecture
* 🚀 High-performance solving algorithms
* 🧠 Multiple algorithm strategies (Beam Search)
* 🌍 Cross-platform (Windows, macOS, Linux)
* 🖥️ Clean CLI powered by **Spectre.Console**
* 🎭 Browser automation via **Microsoft Playwright**
* 🧪 Strong test coverage (unit, functional, integration)
* 📦 Self-contained published binaries
* 🔁 Fully automated CI/CD & GitHub Releases

---

## 🧱 Project Structure

```text
src/
├── PuzzleSolver.Core          # Core domain models, algorithms, solvers
├── PuzzleSolver.Automation    # Browser automation (Playwright-based)
├── PuzzleSolver.Cli           # CLI entry point and commands

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

| Option              | Default      | Description                                      |
| ------------------- | ------------ | ------------------------------------------------ |
| `-a, --algorithm`   | `BeamSearch` | Solving algorithm (`BeamSearch`, `AStar`, `BFS`) |
| `-b, --beamwidth`   | `500`        | Beam width (Beam Search only)                    |
| `-H, --headless`    | `false`      | Run browser in headless mode                     |
| `-m, --movedelayms` | `600`        | Delay (ms) between moves during playback         |
| `-h, --help`        | —            | Show help                                        |

---

## 🧠 Algorithms

Currently implemented:

* **Beam Search** (default)

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
