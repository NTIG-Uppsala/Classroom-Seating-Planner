### Sektion 1

-   [ ] i projektet: x, y -> gridX, gridY & centerX, centerY -> x, y
-   [ ] Fixa Cellklass så att undantag omhändertas i respektive underklass
-   [ ] Fixa kommentar i utils xamlhandler parsestringtoobject: "," -> "|"
    -   [ ] "|" -> ";"?

### Fixas med implementation
-   [ ] Fixa namn i Cell.cs på cellElement
-   [ ] Cell.cs -> hur ska cellen läggas till i lista? nu är wAcc, eftersom den lägger till sig själv. (classroomLayoutManager.tableElements.Add(cellElement) -> classroomLayoutManager.classroomElements.Add(cellElement))

### Användarinput

-   [ ] Lägg till ett sätt att kommentera ut rader i datafilerna (t.ex. #)
-   [ ] Kommentera layoutfilen med instruktioner på hur man gör en layout
-   [ ] Kommentera namnlistan med instruktioner på hur man lägger in elever och constraint

### Senare

-   [ ] Ändra namn i testlistan i MoreStudentsThanTablesTest
-   [ ] Förklara CabinetWClass - "CabinetWClass is the class name for the file explorer window"
-   [ ] min-höjd på bord whack. Kapar namn.
-   [ ] ClassroomLayoutManager/Handler? mappar? (döpa om?)
-   [ ] Cell, set column, row, columnspan och rowspan behöver inte omvandlas till ints då de redan är ints

-   [ ] Popup andvändarvänlighet
    -   [ ] Man kan få flera popups samtidigt vilket inte är nice. Kanske skapa ett bannersystem inom någon framtid?
    -   [ ] Se över formuleringen i "det finns fler elever än bord". Bord/platser är svårläst. Platser är mer korrekt och kanske tydligare.
    -   [ ] Se över popupmeddelandena allmänt med deras formuleringar.

-   [ ] Dokumentation
    -   [ ] File system interactions borde kanske byta namn (t.ex. hard coded data).
    -   [ ] File system interactions borde nog struktureras om. Ex, en rubrik för klasslistab med underpunkter för vilken variabel i vilken file för varje ställe.

-   [ ] .Equals() i HandleAllDataFileIssues (konsekventhet)
-   [ ] Directory eller folder? (konsekventhet)
-   [ ] testVariable -> testingVariable?? (konsekventhet)
-   [ ] Utilsmapp/namespace
-   [ ] AnyPopupWindowContainsText/NoPopupWindowContainsText - omfaktorisera, true/false?
-   [ ] Test för layoutfil som testar vad som händer om man har skumma grejer (tex en 4, ett q eller ett ö)
    -   [ ] Eller fixa funktionalitet som hanterar andra tecken som blanksteg (i.e. säger att fil med bara "a" är tom)

### Senare senare dvs är en del av senare men inte lika viktigt

-   [ ] Fixa responsivitet i XAML istället för C#?
-   [ ] xPopupName-variabler -> xWindowTitle
-   [ ] Filinläsning case-sensitive? (B: b, T: t)
-   [ ] Lägg till caseSensitive i Options i Utils.XAMLHandler?
-   [ ] Ändra properties till PascalCase?
-   [ ] Lägg till funktionalitet i AnyPopupWindowContainsTest som kollar att det bara finns en popup
-

---

[Back to README.](README.md)
