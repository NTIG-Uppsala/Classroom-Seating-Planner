// Grid:
// [
//     {
//         "centerX": 1,
//         "centerY": 0,
//         "cellType": "whiteboard"
//     },
//     {
//         "centerX": 2,
//         "centerY": 3,
//         "cellType": "table",
//         "student": null
//     }
// ]

// Students:
// [
//     {
//         "name": "John Doe",
//         "constraints": [
//             "closeToWhiteboard",
//         ],
//     },
// ]

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

const addWeightMapToStudents = (students, layout) => {
    const rowCount = layout.length;
    const colCount = layout.reduce((max, row) => Math.max(max, row.length), 0);

    students.forEach((student) => {
        if (student.constraints) {
            student.weightMap = Array.from({ length: rowCount }, () => Array(colCount).fill(0));
        } else {
            student.weightMap = null;
        }
    });
};


const layout = getLayout();
const tables = getTables(layout);
const whiteboard = getWhiteboard(layout);
const students = getStudentList();
addWeightMapToStudents(students, layout);


//                                         //
// Distance to whiteboard penalty thingies //
//                                         //

const calculateWhiteboardDistWeights = () => {
    addWhiteboardDistToTables(tables, whiteboard);

    const baseWeight = 1;
    const minDist = Math.min(...tables.map((table) => table.distanceToWhiteboard));
    const maxDist = Math.max(...tables.map((table) => table.distanceToWhiteboard));

    // Sort students with NearWhiteboard constraint first
    const [withConstraints, noConstraints] = students.reduce((acc, student) => {
        if (student.constraints?.includes("NearWhiteboard")) {
            acc[0].push(student);
        } else {
            acc[1].push(student);
        }
        return acc;
    }, [[], []]);

    const studentsNearWhiteboardFirst = [...withConstraints, ...noConstraints];
    const nearestTablesFirst = tables.sort((a, b) => a.distanceToWhiteboard - b.distanceToWhiteboard);

    studentsNearWhiteboardFirst.forEach((student) => {
        if (student.constraints?.includes("NearWhiteboard")) {
            nearestTablesFirst.forEach((table) => {
                student.weightMap[table.y][table.x] = baseWeight * (1 - (table.distanceToWhiteboard - minDist) / (maxDist - minDist));
            });
        }
    });

    studentsNearWhiteboardFirst.forEach((student) => {
        if (!student.weightMap) return;

        console.log("-------------------");
        console.log(student.name);

        const maxWeight = Math.max(...student.weightMap.flat());
        const colors = ["░", "▒", "▓", "█", "█"];
        const getWeightIndex = (weight) => {
            return Math.floor(weight / maxWeight * (colors.length - 1)) || 0;
        };

        student.weightMap.forEach((row) => {
            let fancyRow = "";

            row.forEach((weight) => {
                fancyRow += colors[getWeightIndex(weight)];
            });

            console.log(fancyRow);
        });
    });

    // Sort students by their max weight on their weightMap
    const sortedStudents = studentsNearWhiteboardFirst.sort((a, b) => {
        if (!a.weightMap) return 1;
        if (!b.weightMap) return -1

        const aMax = Math.max(...a.weightMap.flat()) || 0;
        const bMax = Math.max(...b.weightMap.flat()) || 0;

        return bMax - aMax;
    });

    console.log(sortedStudents);
};

calculateWhiteboardDistWeights();


