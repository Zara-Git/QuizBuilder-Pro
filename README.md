# QuizBuilder-Pro

A modern WPF quiz builder and player app — create, edit, and play multiple-choice quizzes with JSON storage.

## Features
- Play mode with random questions and live score
- Create new quizzes (3 answers per question)
- Edit existing quizzes (add / update / delete)
- Async JSON storage under `%LocalAppData%\QuizGame\quizzes.json`

## Build & Run
1. Open `QuizGame.sln` in Visual Studio 2022
2. Restore NuGet packages (Newtonsoft.Json)
3. Start (F5)

"@ | Out-File -Encoding UTF8 README.md

git add README.md
git commit -m "Add README"
git push
