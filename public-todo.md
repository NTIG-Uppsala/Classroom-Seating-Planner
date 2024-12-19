### Sektion 1

-   [x] i projektet: x, y -> gridX, gridY & centerX, centerY -> x, y
-   [x] Fixa Cellklass så att undantag omhändertas i respektive underklass
-   [x] Fixa kommentar i utils xamlhandler parsestringtoobject: "," -> "|"
    -   [x] "|" -> ";"?

### Fixas med implementation

-   [ ] Fixa namn i Cell.cs på cellElement
-   [ ] Cell.cs -> hur ska cellen läggas till i lista? nu är wAcc, eftersom den lägger till sig själv. (classroomLayoutManager.tableElements.Add(cellElement) -> classroomLayoutManager.classroomElements.Add(cellElement))

### Användarinput

-   [ ] Lägg till ett sätt att kommentera ut rader i datafilerna (t.ex. #)
    -   [ ] Lägg in i file-system-interactions.md att det står mer info i filerna
-   [ ] Kommentera layoutfilen med instruktioner på hur man gör en layout
-   [ ] Kommentera namnlistan med instruktioner på hur man lägger in elever och constraint

### Senare

-   [x] Skapa testnamnlista för MoreStudentsThanTablesTest
    -   [x] Skapa en programmatiskt? Namn1, Namn2 osv.?
-   [x] Förklara CabinetWClass - "CabinetWClass is the class name for the file explorer window"
-   [x] Fixa responsiviteten så att det funkar som innan, men kanske löst smidigare
    -   [x] Fixa responsivitet i XAML istället för C#?
    -   [x] min-höjd på bord whack. Kapar namn.
    -   [x] Fil: DynamicClassroomZicer.cs???
-   [x] ClassroomLayoutManager/Handler? mappar? (döpa om?)
    -   [x] Tog bort handlern och löste det i xaml istället
-   [x] Cell, set column, row, columnspan och rowspan behöver inte omvandlas till ints då de redan är ints

-   [ ] Popup andvändarvänlighet
    -   [ ] Man kan få flera popups samtidigt vilket inte är nice. Kanske skapa ett bannersystem inom någon framtid?
    -   [ ] Se över formuleringen i "det finns fler elever än bord". Bord/platser är svårläst. Platser är mer korrekt och kanske tydligare.
    -   [ ] Se över popupmeddelandena allmänt med deras formuleringar.

-   [ ] Dokumentation
    -   [x] File system interactions borde kanske byta namn (t.ex. hard coded data).
    -   [x] File system interactions borde nog struktureras om. Ex, en rubrik för klasslistab med underpunkter för vilken variabel i vilken file för varje ställe.
    -   [x] Dela upp i olika filer
    -   [ ] Manuella tester (tex responsivitet)

-   [x] .Equals() i HandleAllDataFileIssues (konsekventhet)
    -   [x] Skippa null iom varning om att det är null
-   [ ] testingVariable / testCaseVariable??
    -   [x] testVariable -> testingVariable?? (konsekventhet)
    -   [ ] -> defaultTestingVariable && testCaseVariable??
-   [x] Utilsmapp/namespace
-   [ ] AnyPopupWindowContainsText/NoPopupWindowContainsText - omfaktorisera, true/false?
-   [ ] Vad händer om man har annat än T och B i layoutfilen?
    -   [ ] De blir golv. Ska man testa för det?
    -   [ ] Hur ska det hanteras?
        -   [ ] Ska det varna?
        -   [ ] Ska den antaga bord?
        -   [ ] Case sensitive?
        -   [ ] Ska det bli golv?
            -   [ ] Viggo och Ac tycker att det ska bli det så layouten i filen matchar programmet bättre.

### Senare senare dvs är en del av senare men inte lika viktigt

-   [ ] xPopupName-variabler -> xWindowTitle
-   [ ] Filinläsning case-sensitive? (B: b, T: t)
-   [ ] Lägg till frivillig option i GetAllElementsBy\* i XAMLHandler för case sensitivity?
-   [ ] Ändra properties till PascalCase?
    -   [ ] V: Nej?
    -   [ ] J: Jo?
-   [ ] Lägg till funktionalitet i AnyPopupWindowContainsTest som kollar att det bara finns en popup
-   [ ] ??(gridX. gridY -> gridPosColumn, gridPosRow / gridColumn, gridRow / gridColumnPos, gridRowPos)??
-   [ ] datafiler och hårdkodad data:
    -   [ ] Gör objekt med datafilernas namn istället för separata variabler
        Idé 1:
        ```cs
        string dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), FileHandler.dataFolderName);
        Dictionary? dataFiles = {
            classList = {
                fileName: "klasslista.txt",
                filePath: Path.Combine(dataFolderPath, dataFiles.classList.fileName)
            },
            classroomLayout = {
                fileName: "bordskarta.txt",
                filePath: Path.Combine(dataFolderPath, dataFiles.classroomLayout.fileName)
            },
            constraints = {
                fileName: "constraints.txt",
                filePath: Path.Combine(dataFolderPath, dataFiles.constraints.fileName)
            }, // OBS! Exempel
        };
        ```
        Idé 2:
        ```js
        const filePaths = {
            base: "%USERPROFILE%\\Documents\\",
            files: {
                classList: "klasslista.txt",
                classroomLayout: "bordskarta.txt",
                constraints: "constraints.txt",
            }
            getPath: (file) => {
                return this.base + this.files[file];
            }
        }
        ```
    -   [ ] Bryta ut file paths hårdkodad data till separat fil?
-   [ ] Omfaktorisera UpdateClassroomLayoutSize
-   [ ] Fundera på om responsivitet kan förbättras - just nu kan lite mer än en rad text synas om programfönstret inte går att göra tillräckligt litet
-   [ ] Directory eller folder? (konsekventhet)
    -   [x] V och T tycker att det inte är så viktigt och att det för det mesta bara förekommer i testDataDirectory.cs och den kan få leva i sin egen värld. De är ju trots allt synonymer.
    -   [ ] Vi återkommer
-   [ ] Mer avancerade namnurval i testerna
    -   [ ] några tusen för och efternamn som komnineras slumpmässigt
    -   [ ] putta ibland in ett förnamn som mellannamn
-   [ ] make constraint functions whitespace agnostic.
-   [ ] test för att man inte behöver starta om programmet
-   [ ] funtoinLookupTable should reference the actual constraint functions instead of a string that we need to look up.

---

[Back to README.](README.md)
