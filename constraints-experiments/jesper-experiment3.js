// List of students
const students = [
    {
        name: "A",
        constraints: [{ type: "closeTo", target: "whiteboard", importance: 10 }],
    },
    {
        name: "B",
        constraints: [{ type: "closeTo", target: "window", importance: 5 }],
    },
    {
        name: "C",
        constraints: [{ type: "closeTo", target: "door", importance: 5 }],
    },
    {
        name: "D",
        constraints: [{ type: "farFrom", target: "E", importance: 10 }],
    },
    {
        name: "E",
        constraints: [
            { type: "closeTo", target: "C", importance: 5 }, // Denna kollas aldrig pga hur closeTo är implementerad
            { type: "notNextTo", target: "F", importance: 10 },
        ],
    },
    { name: "F" },
    { name: "G" },
    { name: "H" },
    { name: "I" },
    { name: "J" },
];

// // List of tables
// const tables = [
//     {name: "1", occupant: " ", x: 0, y: 0,},
//     {name: "2", occupant: " ", x: 1, y: 0,},
//     {name: "3", occupant: " ", x: 2, y: 0,},
//     {name: "4", occupant: " ", x: 0, y: 1,},
//     {name: "5", occupant: " ", x: 1, y: 1,},
//     {name: "6", occupant: " ", x: 2, y: 1,},
//     {name: "7", occupant: " ", x: 0, y: 2,},
//     {name: "8", occupant: " ", x: 1, y: 2,},
//     {name: "9", occupant: " ", x: 2, y: 2,},
// ]

// const availableTables = tables.filter(table => table.occupant === " ");

// console.log("Available tables: ", availableTables);

// ----------------------------------------
// Lista med studenter
// let students = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"];

// Kopiera listan med tillgängliga studenter
let availableStudents = [...students];

// Lista med bord
const tables = [
    { x: 0, y: 0, occupant: null },
    { x: 1, y: 0, occupant: null },
    { x: 2, y: 0, occupant: null },
    { x: 0, y: 1, occupant: null },
    { x: 1, y: 1, occupant: null },
    { x: 2, y: 1, occupant: null },
    { x: 0, y: 2, occupant: null },
    { x: 1, y: 2, occupant: null },
    { x: 2, y: 2, occupant: null },
];

// Filtrera tillgängliga bord
let availableTables = tables.filter((table) => !table.occupant);

// Begränsningar med olika typer och nivåer
// let constraints = [
//     { element: "A", constraint: "closeTo", constraintType: "distance", constraintElement: "whiteboard", importance: 10 },
//     { element: "B", constraint: "closeTo", constraintType: "distance", constraintElement: "window", importance: 5 },
//     { element: "C", constraint: "closeTo", constraintType: "distance", constraintElement: "door", importance: 5 },
//     { element: "D", constraint: "farFrom", constraintType: "distance", constraintElement: "E", importance: 10 },
//     { element: "E", constraint: "closeTo", constraintType: "distance", constraintElement: "C", importance: 5 },
//     { element: "E", constraint: "notNextTo", constraintType: "boolean", constraintElement: "F", importance: 10 },
// ];
const constraints = students
    .map((student) => {
        student.constraints.map((constraint) => {
            return constraint;
        });
    })
    .flat();
