# Coding Standards

## General

#### Naming
-   Code language: _English_.
-   Comment language: _English_.
-   Documentation language: _English_.
-   Git branches: _kebab-case_.
-   File names: _kebab-case_.
-   Default case if not specified: _camelCase_.
-   No one-letter variable names.
-   Prefer longer, clearer variable names over short, unclear ones.

#### Syntax
-   Indent size: _4 spaces_.
-   Double quotes over single quotes. `"` > `'`.

#### Motivations
-   Longer variable names can helpful by describing their purpose. With modern IDEs, typing long variable names is not a problem and modern screens are large enough to display them. Given that, it isn't always the best way to go but that's why we have comments. In any case, avoid one-letter variable names (`index` > `i`) or really short abbreviations since they usually only lead to confusion.
-   Single quotes are not quotes and should only be used when required like in nested strings but never as the default. Double quotes are the standard in most programming languages and should be used as the default. This is to avoid confusion with single quotes in English text and general readability.
-   Default case is camelCase simply because it is good to have a default case. It is the most common case in programming and is readable enough.

## C#

#### Naming
-   Classes: _PascalCase_.
-   Methods: _PascalCase_.
-   Functions: _camelCase_.
-   Variables: _camelCase_.
-   File names: _PascalCase_.

-   Methods that are triggered by elements should be named with the element name followed by the action. For example _ButtonTop\_Click_.

#### Syntax
-   Prefer explicit type declarations instead of _var_.

## XAML

#### Naming
-   Element names: _PascalCase_.

-   Element names should be descriptive of their purpose and not their type. For example, a `ListBox` that displays a list of students' names should be named `StudentList` rather than `ListBoxStudentList`. 

---

[Back to README.](../README.md)
