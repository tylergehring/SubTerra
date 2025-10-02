SubTerra drops you into the depths of a dark, sprawling cave system where every step forward is a gamble. Armed with grit, wits, and the resources you scavenge—like precious TNT—you’ll blast through walls, uncover hidden passages, and push deeper into the unknown. But danger lurks in every shadow: collapsing tunnels, dwindling supplies, and the mystery of what lies ahead. Will you uncover the glittering treasure buried in the heart of the cave, or will Subterra claim you as its next lost explorer?

## Game Overview

Subterra is an adventure game that challenges players to navigate through treacherous caves while managing resources and facing various dangers.

## Technology
This project is developed using C# and Unity 6000.2.3f1.

## Getting Started

### Cloning the Repository

To get started, clone the repository from GitHub:

```bash
git clone https://github.com/tylergehring/SubTerra.git
cd SubTerra
```

### Development Workflow

1. **Install Unity**: Make sure you have Unity 6000.2.3f1 installed.
2. **Open the Project**: Launch Unity Hub and open the cloned `SubTerra` folder as a Unity project.
3. **Pull Latest Changes**: Before starting work, always pull the latest changes:
   ```bash
   git pull origin main
   ```
4. **Make Changes**: Develop your feature or fix in Unity. Save and test your changes.
5. **Commit Frequently**: Make small, focused commits to minimize merge conflicts:
   ```bash
   git add .
   git commit -m "Describe your changes"
   ```
6. **Pull Before Pushing**: Always pull again before pushing to integrate others' changes:
   ```bash
   git pull origin main
   # Resolve any conflicts if they occur
   git push origin main
   ```
7. **Communicate**: Let your team know when you're pushing significant changes.

**Tips:**
- Commit your changes frequently in small, logical chunks.
- Always pull before starting work and before pushing changes.
- When conflicts occur, communicate with the team member whose changes conflict with yours.
- Use the `/Docs` folder for documentation and design notes.
- Consider using `git stash` to temporarily save changes when pulling:
  ```bash
  # Stash your changes including untracked files
  git stash -u
  # Pull latest changes
  git pull origin main
  # Apply your stashed changes
  git stash pop
  ```

## Local Development Folders

You may create a `.devfolder` in the project root for local testing, experiments, or temporary files. This folder is listed in `.gitignore` and will not be pushed to the repository, so you can safely use it for personal development without affecting the shared codebase.
