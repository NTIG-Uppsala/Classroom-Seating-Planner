const fs = require("node:fs");

const interpretStudentConstraints = (name, rawConstraints) => {
    const functionLookupTable = {
        nära: { type: "distance", arguments: [name, "near", undefined], priority: 1 },
        intenära: { type: "distance", arguments: [name, "far", undefined], priority: 1 },
        långtfrån: { type: "distance", arguments: [name, "far", undefined], priority: 1 },
        bredvid: { type: "adjacent", arguments: [name, true, undefined], priority: 1 },
        intebredvid: { type: "adjacent", arguments: [name, false, undefined], priority: 1 },
    };

    const targetLookupTable = {
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

    const interpretedConstraints = [];

    const constraintList = rawConstraints.split("/").map((constraint) => constraint.trim());

    constraintList.forEach((rawConstraint) => {
        // Find the function name from the functionLookupTable
        const trimmedConstraint = rawConstraint.replace(/\s/g, "").toLowerCase();
        const functionName = Object.keys(functionLookupTable)
            .filter((functionName) => trimmedConstraint.startsWith(functionName))
            .at(0);

        // Create the template constraint object
        const interpretedConstraint = functionLookupTable[functionName];

        if (!interpretedConstraint) return;

        // Isolate the target string
        let target = rawConstraint
            .replace(functionName, "")
            .replace(/\(.*\)/, "") // Remove priority (N)
            .trim();

        // Clean up the target string using lookup table
        if (targetLookupTable[target.replace(/\s/g, "").toLowerCase()]) {
            target = targetLookupTable[target];
        }
        interpretedConstraint.arguments[2] = target;

        // Find the priority of the constraint
        // Priority is the number inside the parenthesis (N)
        const priority = parseFloat(rawConstraint.match(/\(([^)]+)\)/)?.[1] || 1); // Default priority is 1
        interpretedConstraint.priority = priority;

        interpretedConstraints.push(interpretedConstraint);
    });

    return interpretedConstraints || null; // Return null if no constraints are assigned
};

const constraintFunctions = {
    distance: (source, target, nearOrFar, priority, references) => {
        // If target is a student and is not placed
        if (!target) return 0;

        // Get distance of every classroom element compared to the source object
        const distances = references.classroomElements.map((cell) => {
            return Math.sqrt((cell.x - target.x) ** 2 + (cell.y - target.y) ** 2);
        });

        const maxDistance = Math.max(...distances);
        const minDistance = Math.min(...distances);

        const actualDistance = Math.sqrt((target.x - source.x) ** 2 + (target.y - source.y) ** 2);

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
    adjacent: (source, target, yesOrNo, priority, references) => {
        if (!target) return 0; // If target is a student and is not placed

        // Check if caller is adjacent to target
        const xDiff = Math.abs(source.x - target.x);
        const yDiff = Math.abs(source.y - target.y);
        const isAdjacent = (xDiff === 1 && yDiff === 0) || (xDiff === 0 && yDiff === 1);

        if (isAdjacent === yesOrNo) {
            return priority;
        } else {
            return 0;
        }
    },
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
                returnObject.constraints = interpretStudentConstraints(name.trim(), constraints);
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
    console.log("Students: (▣: constrained), (▢: unconstrained)");
    tables
        .filter((table) => table.student)
        .sort((a, b) => {
            const aLetter = studentLookup[a.student.name];
            const bLetter = studentLookup[b.student.name];

            return aLetter.localeCompare(bLetter);
        })
        .forEach((table, index) => {
            console.log(`${studentLookup[table.student.name]} ${table.student.constraints ? "▣" : "▢"}: ${table.student.name}`);
        });

    console.log("");

    // Draw entire grid
    grid.forEach((row) => {
        console.log(row.join(""));
    });

    console.log("");
    console.line();
    console.log("");
};

const seatStudent = (student, classroomElements) => {
    const tables = classroomElements.filter((element) => element.cellType === "table");
    const availableTables = tables.filter((table) => !table.student);

    // If student has no constraints, place them randomly
    if (!student.constraints) {
        const randomTable = availableTables[Math.floor(Math.random() * availableTables.length)];

        randomTable.student = student;

        return 0;
    }

    // Wipe all tables scores since they are persistent
    tables.forEach((table) => {
        table.score = 0;
    });

    // Sort the tables by how well they meet the constraints
    const rankedTables = availableTables
        .map((table) => {
            let score = 0;

            // Try every constraint to get a students overall preference for a table - CHECK might be necessary to send args in order depending on the args of the constraint
            student.constraints.forEach((constraint) => {
                // [source, target, constraintArgument]
                const args = [table, null, constraint.arguments[1]];

                const classroomElementsNames = ["whiteboardCover", "door", "window"];

                // If target is a classroom element
                if (classroomElementsNames.includes(constraint.arguments[2])) {
                    args[1] = classroomElements.filter((element) => element.cellType === constraint.arguments[2]).at(0);
                }

                // Check if the current student is the target or caller of the constraint
                // Assign target of constraint function accordingly - TODO - why?
                else {
                    // Student is the caller in the constraint arguments
                    if (student.name === constraint.arguments[0]) {
                        args[1] =
                            tables
                                .filter((table) => {
                                    return table.student && table.student.name === constraint.arguments[2];
                                })
                                .at(0) || null; // If the target is not placed, pass null
                    }
                    // Student is the target in the constraint arguments
                    else if (student.name === constraint.arguments[2]) {
                        args[1] =
                            tables
                                .filter((table) => {
                                    return table.student && table.student.name === constraint.arguments[0];
                                })
                                .at(0) || null; // If the target is not placed, pass null
                    }
                }

                score += constraintFunctions[constraint.type](...args, constraint.priority, { classroomElements });
            });

            table.score = score;

            return table;
        })
        .sort((a, b) => b.score - a.score);

    // Make a list of the best tables based on the priority of the constraints
    const prioritySum = student.constraints.reduce((sum, constraint) => sum + constraint.priority, 0);
    const topTables = rankedTables.slice(0, Math.ceil(rankedTables.length * (0.85 ** prioritySum / 2.83333333333334)));

    const randomTable = topTables[Math.floor(Math.random() * topTables.length)];
    randomTable.student = student;

    // Main wants to know the score of the table
    return randomTable.score;
};

const main = (students, classroomElements, debug = false) => {
    let seatingArrangementScore = 0;

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

    return seatingArrangementScore;
};

const startTime = Date.now();

const scoresList = [];
const iterations = 10;

for (let i = 0; i < iterations; i++) {
    const layout = getLayout();
    const classroomElements = getClassroomElements(layout);
    const students = getStudentList();

    score = main(students, classroomElements, (debug = false));
    scoresList.push(score);
}

console.log("Average score:", (scoresList.reduce((sum, curr) => (sum += curr), 0) / iterations).toFixed(2));
console.log("Max score:", Math.max(...scoresList).toFixed(2));
console.log("Time:", Date.now() - startTime, "ms");
console.log("\n------------------------------------\n");
