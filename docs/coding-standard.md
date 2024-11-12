# Coding Standards

## C#

#### Naming

-   Classes: `PascalCase`.
-   Methods: `PascalCase`.
-   Functions: `camelCase`.
-   Variables: `camelCase`.
-   File names: `PascalCase`.

-   Methods that are triggered by elements should be named with the element name followed by an underscore and the action. For example `ButtonTop_Click`. 
-   Test methods should be named by their purpose followed by `Test`, for example `SeatingTest` as opposed to `TestSeating`. This is to make them more legible in the test explorer.

#### Syntax

-   Prefer explicit type declarations instead of `var`.
-   Public properties should be declared with classes for clarity. For example `Utils.FileHandler.dataFolderPath` as opposed to `dataFolderPath`.
-   Private properties should be declared with `this`. For example `this.classListFromFile` instead of `classListFromFile`.

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
-   No one-letter variable names unless the full name is reserved in the language. For example: `event` might be reserved, so use `e`.
-   Long and clear variable names are preferred over short and unclear ones.

#### Syntax

-   Indent size: `4 spaces`.
-   Double quotes over single quotes. `"` > `'`.

#### Motivations

-   Longer variable names can helpful by describing their purpose. With modern IDEs, typing long variable names is not a problem and modern screens are large enough to display them. Given that, it isn't always the best way to go but that's why we have comments. In any case, avoid one-letter variable names (`index` > `i`) or really short abbreviations since they usually only lead to confusion.
-   Single quotes should only be used when required like in nested strings but never as the default. Double quotes are the standard in most programming languages and should be used as the default. This is to avoid confusion with single quotes in English text and general readability.
-   Default case is `camelCase` simply because it is good to have a default case. It is the most common case in programming and is readable enough.

---

[Back to README.](../README.md)
