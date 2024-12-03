-   [ ] Lägg till caseSensitive i Options i Utils.XAMLHandler?
-   [ ] Ändra properties till PascalCase?
-   [ ] Lägg till funktionalitet i AnyPopupWindowContainsTest som kollar att det bara finns en popup
-   [ ] Ändra namn i testlistan i MoreStudentsThanTablesTest
-   [ ] Utilsmapp/namespace
-   [ ] AnyPopupWindowContainsText/NoPopupWindowContainsText - omfaktorisera, true/false?
-   [ ] testVariable -> testingVariable
-   [ ] Test för layoutfil som testar vad som händer om man har skumma grejer
-   [ ] Eller fixa funktionalitet som hanterar andra tecken som blanksteg (i.e. säger att fil med bara "a" är tom)
-   [ ] Filinläsning case-sensitive?
-   [ ] xPopupName-variabler -> xWindowTitle
-   [ ] Directory eller folder?
-   [ ] Förklara CabinetWClass - "CabinetWClass is the class name for the file explorer window"
-   [ ] Fixa responsivitet i XAML istället för C#?
-   [ ] min-höjd på bord wacc?
-   [ ] ClassroomLayoutManager/Handler? mappar?
-   [ ] Använd Coordinates till constraints
-   [ ] Cell x och y -> Coordinate object? (Samma med centerX o centerY)
-   [ ] Fundera på kommentar för -1 när man beräknar centerX o centerY
-   [ ] Fixa Cellklass så att undantag omhändertas i respektive underklass
-   [ ] Fixa kommentar i utils xamlhandler parsestringtoobject: "," -> "|"
-   [ ] Cell, set column, row, columnspan och rowspan behöver inte omvandlas till ints då de redan är ints
-   [ ] Cell.cs -> hur ska cellen läggas till i lista? nu är wAcc, eftersom den lägger till sig själv.

Rita upp XAML template / exempel

// var label = new System.Windows.Controls.Label() {
// Content = "I exist",
// HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
// VerticalAlignment = System.Windows.VerticalAlignment.Center
// };
//
// System.Windows.Controls.Grid.SetRow(label, 4);
// System.Windows.Controls.Grid.SetColumn(label, 4);
//
// ClassroomElement.Children.Add(label);

---

[Back to README.](README.md)
