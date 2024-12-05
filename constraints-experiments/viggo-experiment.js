const fs = require("node:fs");

const getStudentList = () => {
    const students = fs.readFileSync("./viggos-data/names.txt", "utf-8")
        .split("\n")
        .map((row) => row.trim())
        .map((row) => {
            const [name, constraints] = row.split(":");
            const returnObject = { name: name.trim() };

            if (constraints?.trim()) {
                returnObject.constraints = constraints.split("/").map((constraint) => constraint.trim()).filter(Boolean);
            }

            return returnObject;
        })
        .filter(Boolean);

    // console.log(students);
    return students;
};

const getLayout = () => {
    const layout = fs.readFileSync("./viggos-data/classroom.txt", "utf-8").split("\n");

    // console.log(layout);
    return layout;
};

const getTables = (layout) => {
    const tableCells = [];

    layout.forEach((row, y) => {
        row.split("").forEach((cell, x) => {
            if (cell === "B") {
                tableCells.push({ x, y });
            }
        });
    });

    // console.log(tableCells);
    return tableCells;
};

const getWhiteboardCells = (layout) => {
    const whiteboardCells = [];

    layout.forEach((row, y) => {
        row.split("").forEach((cell, x) => {
            if (cell === "T") {
                whiteboardCells.push({ x, y });
            }
        });
    });

    // console.log(whiteboardCells);
    return whiteboardCells;
};

const getWhiteboard = (layout) => {
    const whiteboardCells = [];

    layout.forEach((row, y) => {
        row.split("").forEach((cell, x) => {
            if (cell === "T") {
                whiteboardCells.push({ x, y });
            }
        });
    });

    // Avg position of the whiteboards which is really what we want
    const whiteboardPosAvg = whiteboardCells.reduce((accumulator, current) => {
        accumulator.x += current.x;
        accumulator.y += current.y;
        return accumulator;
    }, { x: 0, y: 0 });

    whiteboardPosAvg.x /= whiteboardCells.length;
    whiteboardPosAvg.y /= whiteboardCells.length;

    // console.log(whiteboardCells);
    // console.log(whiteboardPosAvg);
    return whiteboardPosAvg;
};

// Helpers
const addWhiteboardDistToTables = (tableCells, whiteboard) => {
    // Each table knows their distance to the whiteboard
    tableCells.forEach((table) => {
        table.distanceToWhiteboard = Math.sqrt((table.x - whiteboard.x) ** 2 + (table.y - whiteboard.y) ** 2);
    });
};

const fancyDraw = (tables) => {

    const palette = {
        whiteboard: "T",
        tableWithStudent: "█",
        tableWithStudentNoConstraints: "▓",
        tableWithoutStudent: "▒",
        floor: "░",
    }

    const maxX = Math.max(...tables.map((table) => table.x));
    const maxY = Math.max(...tables.map((table) => table.y));

    const map = Array.from({ length: maxY + 1 }, () => Array(maxX + 1).fill(palette.floor));

    tables.forEach((table) => {
        if (table.student) {
            if (table.student.constraints) {
                map[table.y][table.x] = palette.tableWithStudent;
            } else {
                map[table.y][table.x] = palette.tableWithStudentNoConstraints;
            }
        } else {
            map[table.y][table.x] = palette.tableWithoutStudent;
        }
    });

    getWhiteboardCells(layout).forEach((whiteboard) => {
        map[whiteboard.y][whiteboard.x] = palette.whiteboard;
    });

    console.log("----------------------");
    console.log(`${palette.whiteboard} Whiteboard.`);
    console.log(`${palette.tableWithStudent} Table with student.`);
    console.log(`${palette.tableWithStudentNoConstraints} Table with unconstrained student.`);
    console.log(`${palette.tableWithoutStudent} Table without student.`);
    console.log(`${palette.floor} Floor.`);
    console.log("");

    map.forEach((row) => {
        console.log(row.join(""));
    });

    console.log("----------------------");
};

const startTime = Date.now();

const layout = getLayout();
const tables = getTables(layout);
const whiteboard = getWhiteboard(layout);
const students = getStudentList();
addWhiteboardDistToTables(tables, whiteboard);

const constraints = {
    distance: (where, whom) => {
        return; // weight or null or 0
    },
    // NearWhiteboard: {
    //     baseWeight: 1,
    //     get: (student, table) => {
    //         const maxDist = Math.max(...tables.map((table) => table.distanceToWhiteboard));
    //         const minDist = Math.min(...tables.map((table) => table.distanceToWhiteboard));

    //         const distToWhiteboard = table.distanceToWhiteboard;

    //         const weight = (maxDist - distToWhiteboard) / (maxDist - minDist);
    //         return weight;
    //     },
    // },
    // NotNextTo: {
    //     baseWeight: 1,
    //     get: (student, table) => {
    //         return null; // weight or null
    //     },
    // },
    // NextTo: {
    //     baseWeight: 1,
    //     get: (student, table) => {
    //         return null; // weight or null
    //     },
    // },
}

// Students with more constraints are placed first
students.sort((a, b) => {
    if (!a.constraints) return 1;
    if (!b.constraints) return -1;

    return b.constraints.length - a.constraints.length;
});

// Place the students with constraints
students.forEach((student) => {
    if (!student.constraints) return;

    // Try every table
    let bestTable = null;
    tables.reduce((bestWeight, table) => {
        // Skip already used tables
        if (table.student) return bestWeight;

        let weight = 0;

        // Try every constraint to get a students overall preference for a table
        student.constraints.forEach((constraint) => {
            weight += constraints[constraint]?.get(student, table) || 0; // ? in case the constraint is not defined
        });

        if (weight > bestWeight) {
            bestWeight = weight;
            bestTable = table;
        }

        return bestWeight;
    }, 0);

    if (bestTable) {
        bestTable.student = student;
    }
});

// Place the students without constraints
students.forEach((student) => {
    if (student.constraints) return;

    const table = tables.find((table) => !table.student);

    if (table) {
        table.student = student;
    }
});

fancyDraw(tables);

console.log("Time:", Date.now() - startTime, "ms");








// # Names.txt
// A: nära tavlan
// # or
// A: NearWhiteboard
// # or
// A: CloseTo(whiteboard)
// # or
// A: CloseTo(B)

// # (siffra) är viktighet
// N: långt från tavlan (10)
// N: inte bredvid A (99999)

// B: CloseTo(Whiteboard)
// # or
// B: Distance(near, Whiteboard)
// const studentz = [
//     {
//         name: "A",
//         constraints: [{ type: "distance", arguments: ["near", "B"], priority: 10 }],
//     },
// // ];

// const fs = require("node:fs");

// const lookup = {
//     nära: { type: "distance", arguments: ["near", undefined], priority: undefined },
//     långtfrån: { type: "distance", arguments: ["far", undefined], priority: undefined },
//     bredvid: { type: "adjacent", arguments: [true, undefined], priority: undefined },
//     intebredvid: { type: "adjacent", arguments: [false, undefined], priority: undefined },
// };

// const students = fs
//     .readFileSync("./viggos-data/names.txt", "utf-8")
//     .split("\n")
//     .map((row) => row.trim())
//     .map((row) => {
//         const [rawName, rawConstraints] = row.split(":");

//         Object.keys(lookup).forEach((key) => {
//             if (rawConstraints.includes(key)) {
                
//             }
//         });

//         return returnObject;
//     })
//     .filter(Boolean);

// students.forEach((student) => {
//     console.log(student.name, "\n   Constraints:", JSON.stringify(student.constraints));
// });