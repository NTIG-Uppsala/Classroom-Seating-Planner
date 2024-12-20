# Coding Standards

## C#

#### Naming

-   Classes: `PascalCase`.
-   Methods: `PascalCase`.
-   Functions: `camelCase`.
-   Namespaces: `Pascal_Snake_Case`.
-   Variables: `camelCase`.
-   Properties: `camelCase`.
-   File names: `PascalCase`.
-   Folder names: `Pascal-Kebab-Case`.

-   Methods that are triggered by elements should be named with the element name followed by an underscore and the action. For example `ButtonTop_Click`.
-   Test methods should be named by their purpose followed by `Test`, for example `SeatingTest` as opposed to `TestSeating`. This is to make them more legible in the test explorer.

#### Syntax

-   Prefer explicit type declarations instead of `var`.
-   Public properties should be referenced with their full class reference to decrease ambiguity. For example `Utils.FileHandler.dataFolderPath` as opposed to `dataFolderPath`.
-   Local properties should be accessed with `this`. For example `this.classListFromFile` instead of `classListFromFile`.

## XAML

#### Naming

-   Element names: `PascalCase`.

-   Element names should be descriptive of their purpose and not their specific type. For example, a `ListBox` that displays a list of students' names should be named `StudentListElement` rather than `ListBoxStudentList`. The `Element` part is added for the sake of clarity in the `C#` code.

## General

#### Naming

-   Code language: `English`.
-   Comment language: `English`.
-   Documentation language: `English`.
-   Git branches: `kebab-case`.
-   File names: `kebab-case`.
-   Default case if not specified: `camelCase`.
-   Prefer longer and more descriptive variable names over short, abbreviated, unclear ones. Exceptions can be made for common abbreviations like `i` for index and reserved names such as `event` where `e` could be used instead.
-   Variable names should describe their purpose and not their type. For example, `studentCount` over `returnInt`.

#### Syntax

-   Indent size: `4 spaces`.
-   Double quotes over single quotes for strings. `"` > `'`.

#### Motivations

-   Longer variable names can helpful by describing their purpose. With modern IDEs, typing long variable names is not a problem and modern screens are large enough to display them. Given that, it isn't always the best way to go but that's why we have comments. In any case, avoid one-letter variable names or really short abbreviations since they usually only lead to confusion.

---

[Back to README.](../README.md)
