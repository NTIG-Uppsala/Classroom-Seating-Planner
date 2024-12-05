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



const layout = getLayout();
const tables = getTables(layout);
const whiteboard = getWhiteboard(layout);
const students = getStudentList();
addWhiteboardDistToTables(tables, whiteboard);

const constraints = {
    nearWhiteboard: {
        type: "static",
        baseWeight: 1,
        get: (students, tables) => {
            return // weight or null
        },
    },
    notNextTo: {
        type: "dynamic",
        baseWeight: 1,
        get: (students, tables) => {
            return // weight or null
        },
    },
    notFacing: {
        type: "dynamic",
        baseWeight: 1,
        get: (students, tables) => {
            return // weight or null
        },
    },
}

// Do all static constraints for tables
tables.forEach((table) => {
    constraints
        .filter((constraint) => constraint.type === "static")
        .forEach((constraint) => {
            
        });
});

// // Students with more constraints are placed first
// students.sort((a, b) => {
//     if (!a.constraints) return 1;
//     if (!b.constraints) return -1;

//     return b.constraints.length - a.constraints.length;
// });

// students.forEach((student) => {
//     // Try every table
// });













// const addWeightMapToStudents = (students, layout) => {
//     const rowCount = layout.length;
//     const colCount = layout.reduce((max, row) => Math.max(max, row.length), 0);

//     students.forEach((student) => {
//         if (student.constraints) {
//             student.weightMap = Array.from({ length: rowCount }, () => Array(colCount).fill(0));
//         } else {
//             student.weightMap = null;
//         }
//     });
// };

// const fancyDraw = (students) => {
//     // Fancy output
//     students.forEach((student) => {
//         if (!student.weightMap) return;

//         console.log("-------------------");
//         console.log(student.name);

//         const maxWeight = Math.max(...student.weightMap.flat());
//         const colors = ["░", "▒", "▓", "█", "█"];
//         const getWeightIndex = (weight) => {
//             return Math.floor(weight / maxWeight * (colors.length - 1)) || 0;
//         };

//         student.weightMap.forEach((row) => {
//             let fancyRow = "";

//             row.forEach((weight) => {
//                 fancyRow += colors[getWeightIndex(weight)];
//             });

//             console.log(fancyRow);
//         });
//     });
// };


// const layout = getLayout();
// const tables = getTables(layout);
// const whiteboard = getWhiteboard(layout);
// const students = getStudentList();
// addWeightMapToStudents(students, layout);
// addWhiteboardDistToTables(tables, whiteboard);



// const setWeightsForStaticConstraints = () => {
//     const baseWeight = 1;
//     const minDist = Math.min(...tables.map((table) => table.distanceToWhiteboard));
//     const maxDist = Math.max(...tables.map((table) => table.distanceToWhiteboard));

//     students.forEach((student) => {
//         if (!student.weightMap) return;
//         tables.forEach((table) => {
//             student.weightMap[table.y][table.x] = baseWeight * (1 - (table.distanceToWhiteboard - minDist) / (maxDist - minDist));
//         });
//     });
// };

// setWeightsForStaticConstraints();


// // Sort students by their max weight on their weightMap
// // so students with the most restrictions are placed first
// const studentsByMaxWeight = students.sort((a, b) => {
//     if (!a.weightMap) return 1;
//     if (!b.weightMap) return -1

//     const aMax = Math.max(...a.weightMap.flat()) || 0;
//     const bMax = Math.max(...b.weightMap.flat()) || 0;

//     return bMax - aMax;
// });

// fancyDraw(studentsByMaxWeight);
