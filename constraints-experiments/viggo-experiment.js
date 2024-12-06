const { log } = require("node:console");
const fs = require("node:fs");

const interpretConstraints = (name, rawConstraints) => {
    const lookupTable = {
        nära: { type: "distance", arguments: [name, "near", undefined], priority: 1 },
        intenära: { type: "distance", arguments: [name, "far", undefined], priority: 1 },
        långtfrån: { type: "distance", arguments: [name, "far", undefined], priority: 1 },
        bredvid: { type: "adjacent", arguments: [name, true, undefined], priority: 1 },
        intebredvid: { type: "adjacent", arguments: [name, false, undefined], priority: 1 },
    };

    const aliases = {
        tavlan: "whiteboardCover",
        tavla: "whiteboardCover",
        whiteboard: "whiteboardCover",
        whiteboards: "whiteboardCover",
        svartatavlan: "whiteboardCover",
        klösbrädan: "whiteboardCover",
        dörren: "door",
        dörr: "door",
        fönstret: "window",
        fönster: "window",
        vindöga: "window",
    };

    const returnConstraints = [];

    const constraintList = rawConstraints.split("/").map((constraint) => constraint.trim());

    constraintList.forEach((constraint) => {
        // Match first part of string with lookupTable
        Object.keys(lookupTable).forEach((key) => {
            if (constraint.replace(/\s/g, "").toLowerCase().startsWith(key)) {
                const returnConstraint = lookupTable[key];

                let target = constraint
                    .replace(key, "")
                    .replace(/\(.*\)/, "")
                    .trim();

                if (aliases[target.replace(/\s/g, "")]) {
                    target = aliases[target];
                }
                returnConstraint.arguments[2] = target;

                const match = constraint.match(/\(([^)]+)\)/);
                returnConstraint.priority = match ? parseFloat(match[1]) : 1;

                returnConstraints.push(returnConstraint);
            }
        });
    });

    return returnConstraints || null;
};

const getStudentList = () => {
    const students = fs
        .readFileSync("./constraints-experiments/viggos-data/names.txt", "utf-8")
        .split("\n")
        .map((row) => row.trim())
        .filter(Boolean)
        .map((row) => {
            const [name, constraints] = row.split(":");
            const returnObject = { name: name.trim() };

            if (constraints?.trim()) {
                returnObject.constraints = interpretConstraints(name.trim(), constraints);
            }

            return returnObject;
        })
        .filter(Boolean);

    return students;
};

const getLayout = () => {
    const layout = fs.readFileSync("./constraints-experiments/viggos-data/classroom.txt", "utf-8").split("\n");

    return layout;
};

const getWhiteboardCover = (classroomElements) => {
    const whiteboardCells = classroomElements.filter((element) => element.cellType === "whiteboard");

    // Avg position of the whiteboards which is really what we want
    const whiteboardPosAvg = whiteboardCells.reduce(
        (accumulator, current) => {
            accumulator.x += current.x;
            accumulator.y += current.y;
            return accumulator;
        },
        { x: 0, y: 0, cellType: "whiteboardCover" }
    );

    whiteboardPosAvg.x /= whiteboardCells.length;
    whiteboardPosAvg.y /= whiteboardCells.length;

    return whiteboardPosAvg;
};

const getClassroomElements = (layout) => {
    const classroomElements = [];

    layout.forEach((row, y) => {
        row.split("").forEach((cell, x) => {
            if (cell === "B") {
                classroomElements.push({ x, y, cellType: "table", student: null });
            } else if (cell === "T") {
                classroomElements.push({ x, y, cellType: "whiteboard" });
            } else if (cell === "D") {
                classroomElements.push({ x, y, cellType: "door" });
            } else if (cell === "F") {
                classroomElements.push({ x, y, cellType: "window" });
            }
        });
    });

    classroomElements.push(getWhiteboardCover(classroomElements));

    return classroomElements;
};

const fancyDraw = (classroomElements, students) => {
    // ░▒▓█
    const palette = {
        whiteboard: "T",
        window: "F",
        door: "D",
        tableOccupied: "a-z",
        tableEmpty: "▒",
        floor: "░",
    };
    const iconPool = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l"];
    const studentLookup = {};
    students.forEach((student, index) => {
        studentLookup[student.name] = iconPool[index];
    });

    const tables = classroomElements.filter((element) => element.cellType === "table");
    const whiteboards = classroomElements.filter((element) => element.cellType === "whiteboard");
    const windows = classroomElements.filter((element) => element.cellType === "window");
    const doors = classroomElements.filter((element) => element.cellType === "door");

    const maxX = Math.max(...classroomElements.map((cells) => cells.x));
    const maxY = Math.max(...classroomElements.map((cells) => cells.y));

    const grid = Array.from({ length: maxY + 1 }, () => Array(maxX + 1).fill(palette.floor));

    // Populate grid
    tables.forEach((table, index) => {
        if (table.student) {
            grid[table.y][table.x] = studentLookup[table.student.name];
        } else {
            grid[table.y][table.x] = palette.tableEmpty;
        }
    });
    whiteboards.forEach((whiteboard) => {
        grid[whiteboard.y][whiteboard.x] = palette.whiteboard;
    });
    doors.forEach((door) => {
        grid[door.y][door.x] = palette.door;
    });
    windows.forEach((window) => {
        grid[window.y][window.x] = palette.window;
    });

    console.line = () => console.log("------------------------------------");

    console.line();

    // Legend
    Object.keys(palette).forEach((key) => {
        console.log(`${key}: ${palette[key]}`);
    });
    console.log("");
    console.log("Students:");
    tables
        .filter((table) => table.student)
        .sort((a, b) => {
            const aLetter = studentLookup[a.student.name];
            const bLetter = studentLookup[b.student.name];

            return aLetter.localeCompare(bLetter);
        })
        .forEach((table, index) => {
            console.log(`${studentLookup[table.student.name]}: ${table.student.name}`);
        });

    console.log("");

    // Draw entire grid
    grid.forEach((row) => {
        console.log(row.join(""));
    });

    console.log("");
    console.line();
};

const seatStudent = (student, classroomElements) => {
    if (!student.constraints) return 0;

    const constraintFunctions = {
        distance: (table, target, nearOrFar, priority) => {
            if (!target) return 0; // if target is a student and is not placed

            // Get distance of every classroom element compared to the tableObj
            const distances = classroomElements.map((cell) => {
                return Math.sqrt((cell.x - target.x) ** 2 + (cell.y - target.y) ** 2);
            });

            const maxDistance = Math.max(...distances);
            const minDistance = Math.min(...distances);

            const actualDistance = Math.sqrt((target.x - table.x) ** 2 + (target.y - table.y) ** 2);

            const scaledDist = (actualDistance - minDistance) / (maxDistance - minDistance);

            switch (nearOrFar) {
                case "near": {
                    return (1 - scaledDist) * priority;
                }
                case "far": {
                    return scaledDist * priority;
                }
                default: {
                    return 0;
                }
            }
        },
        adjacent: (table, target, yesOrNo, priority) => {
            if (!target) return 0; // if target is a student and is not placed

            // Check if caller is adjacent to target
            const xDiff = Math.abs(table.x - target.x);
            const yDiff = Math.abs(table.y - target.y);
            const isAdjacent = (xDiff === 1 && yDiff === 0) || (xDiff === 0 && yDiff === 1);
            
            if (isAdjacent === yesOrNo) {
                return priority;
            } else {
                return 0;
            }
        },
    };

    const tables = classroomElements.filter((element) => element.cellType === "table");
    const availableTables = tables.filter((table) => !table.student);

    // Sort the tables by how well they meet the constraints
    const rankedTables = availableTables
        .map((table) => {
            let score = 0;

            // Try every constraint to get a students overall preference for a table - CHECK might be necessary to send args in order depending on the args of the constraint
            student.constraints.forEach((constraint) => {
                // [caller, target, constraintArgument]
                const args = [table, null, constraint.arguments[1]];

                // Get the object reference of the target classroom element
                if (constraint.arguments[2] === "whiteboardCover") {
                    args[1] = getWhiteboardCover(classroomElements);
                    
                } else if (constraint.arguments[2] === "door") {
                    args[1] = classroomElements.filter((element) => element.cellType === "door").at(0);

                } else if (constraint.arguments[2] === "window") {
                    args[1] = classroomElements.filter((element) => element.cellType === "window").at(0);

                } else {
                    // Logic to check if the current student is the target or caller of the constraint
                    if (student.name === constraint.arguments[0]) {
                        const target =
                            tables
                                .filter((table) => {
                                    return table.student && table.student.name === constraint.arguments[2];
                                })
                                .at(0) || null;

                        args[1] = target;
                        
                    } else if (student.name === constraint.arguments[2]) {
                        const target =
                            tables
                                .filter((table) => {
                                    return table.student && table.student.name === constraint.arguments[0];
                                })
                                .at(0) || null;

                        args[1] = target;
                    }
                }

                score += constraintFunctions[constraint.type](...args, constraint.priority);
            });

            table.score = score;

            return table;
        })
        .sort((a, b) => b.score - a.score);

    // Take the top 30% of the tables
    const topTables = rankedTables.slice(0, Math.floor(rankedTables.length * 0.3));// Change this to consider priority. prel idea: use 0.9 to the power of priority

    // Make table completely random if score is 0
    const randomTable = topTables[Math.floor(Math.random() * topTables.length)];

    randomTable.student = student;

    // Main wants to know the score of the table
    return randomTable.score;
};

const main = (students, classroomElements, debug = false) => {
    let seatingArrangementScore = 0;
    seatingArrangementScore = 0;

    const getAllConstraints = (students) => {
        return students
            .filter((students) => students.constraints)
            .map((student) => student.constraints.sort((a, b) => b.priority - a.priority)) // Sort a students constraints by priority
            .flat()
            .filter(Boolean);
    };

    const sortStudentsByPriority = (students) => {
        students.sort((a, b) => {
            if (!a.constraints) return 1;
            if (!b.constraints) return -1;

            // Compare their sum of priorities - TODO add priorities where the student is the target for the constraint :
            // Problem is that the weight of these constraints would then be inflated (x2)
            const aPriority = a.constraints.reduce((sum, constraint) => sum + constraint.priority, 0);
            const bPriority = b.constraints.reduce((sum, constraint) => sum + constraint.priority, 0);

            return bPriority - aPriority;
        });
    };

    const removeStudentConstraints = (students) => {
        students = students.map((student) => {
            student.constraints = null;
            return student;
        });
    };

    const assignAllConstraints = (constraints, students) => {
        constraints.forEach((constraint) => {
            students = students.map((student) => {
                if (constraint.arguments[0] === student.name || constraint.arguments[2] === student.name) {
                    if (!student.constraints) {
                        student.constraints = [];
                    }
                    student.constraints.push(constraint);
                }
                return student;
            });
        });
    };

    const DEBUG_logAllStudents = (message = null) => {
        if (!debug) {
            return;
        }

        if (message) console.log(message);

        students.forEach((student) => {
            console.log(
                student.name,
                student.constraints?.reduce((sum, constraint) => sum + constraint?.priority || 0, 0)
            );
        });
    };

    DEBUG_logAllStudents("\nBefore sorting\n");

    // Get all constraints sorted by the students sum of priorities
    const constraints = getAllConstraints(students);

    removeStudentConstraints(students);

    assignAllConstraints(constraints, students);

    sortStudentsByPriority(students);

    DEBUG_logAllStudents("\nAfter sorting\n");

    students.forEach((student) => {
        seatingArrangementScore += seatStudent(student, classroomElements);
    });

    fancyDraw(classroomElements, students);

    // console.log(`Seating arrangement score: ${seatingArrangementScore.toFixed(2)}`);
    return seatingArrangementScore;
};

const startTime = Date.now();

const scoresList = [];
let totalScore = 0;

const iterations = 1;

for (let i = 0; i < iterations; i++) {
    const layout = getLayout();
    const classroomElements = getClassroomElements(layout);
    const students = getStudentList();

    score = main(students, classroomElements, (debug = false));
    scoresList.push(score);
    totalScore += score;
}

console.log("-----------------------------");
console.log("Average score:", (scoresList.reduce((sum, curr) => (sum += curr), 0) / iterations).toFixed(2));
console.log("Max score:", Math.max(...scoresList).toFixed(2));

console.log("Time:", Date.now() - startTime, "ms");
console.log("");

/*
asd















*/