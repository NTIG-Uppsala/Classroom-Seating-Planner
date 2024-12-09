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
        if (!target) return 0;

        // Get all the possible distances between the target and every classroom element
        const allDistances = references.classroomElements
            .filter((cell) => cell !== target) // Remove self as a candidate
            .filter((cell) => cell.cellType === "table") // Only consider tables
            .map((cell) => {
                return Math.sqrt((cell.x - target.x) ** 2 + (cell.y - target.y) ** 2);
            });

        const maxDistance = Math.max(...allDistances);
        const minDistance = Math.min(...allDistances);

        const actualDistance = Math.sqrt((target.x - source.x) ** 2 + (target.y - source.y) ** 2);

        // Between 0 and 1
        const relativeDistance = (actualDistance - minDistance) / (maxDistance - minDistance);

        switch (nearOrFar) {
            case "near": {
                return (1 - relativeDistance) * priority;
            }
            case "far": {
                return relativeDistance * priority;
            }
            default: {
                return 0; // In case the arguments are somehow messed up
            }
        }
    },
    adjacent: (source, target, yesOrNo, priority, references) => {
        if (!target) return 0;

        // Check if source is adjacent to target
        const xDiff = Math.abs(source.x - target.x);
        const yDiff = Math.abs(source.y - target.y);
        const isAdjacent = (xDiff === 1 && yDiff === 0) || (xDiff === 0 && yDiff === 1);

        switch (yesOrNo) {
            case "yes" && isAdjacent: {
                return priority;
            }
            case "no" && !isAdjacent: {
                return priority;
            }
            default: {
                return 0;
            }
        }
    },
};

const getStudentListFromFile = () => {
    const students = fs
        .readFileSync("./constraints-experiments/viggos-data/names.txt", "utf-8")
        .split("\n")
        .map((row) => row.trim())
        .filter(Boolean) // Remove empty lines
        .map((row) => {
            const [name, constraints] = row.split(":");
            const student = { name: name.trim() };

            if (constraints?.trim()) {
                student.constraints = interpretStudentConstraints(name.trim(), constraints);
            }

            return student;
        })
        .filter((student) => student.name); // Remove nameless students

    return students;
};

const getLayoutFromFile = () => {
    // A layout is a list of strings where each string represents a row in the classroom
    return fs.readFileSync("./constraints-experiments/viggos-data/classroom.txt", "utf-8").split("\n");
};

const getWhiteboardCover = (classroomElements) => {
    const whiteboardCells = classroomElements.filter((element) => element.cellType === "whiteboard");

    // Average position of the whiteboards which is really what we want
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

const getClassroomElementsFromLayout = (layout) => {
    const classroomElements = [];

    // Lookup table for the different cell types
    const cellTypes = {
        B: "table",
        T: "whiteboard",
        D: "door",
        F: "window",
    };

    // Parse layout character by character and add them to the classroomElements list
    layout.forEach((row, y) => {
        row.split("").forEach((cell, x) => {
            if (cellTypes[cell]) {
                classroomElements.push({ x, y, cellType: cellTypes[cell] });
            }
        });
    });

    // Add the whiteboard cover as a separate element to find more seamlessly later
    if (classroomElements.some((element) => element.cellType === "whiteboard")) {
        classroomElements.push(getWhiteboardCover(classroomElements));
    }

    return classroomElements;
};

const fancyDraw = (classroomElements, students) => {
    // Cool colors ░▒▓█
    const palette = {
        whiteboard: "T",
        window: "F",
        door: "D",
        tableOccupied: "a-z",
        tableEmpty: "▒",
        floor: "░",
    };

    const iconPool = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t"];

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
            const letter = studentLookup[table.student.name];
            const icon = table.student.constraints ? "▣" : "▢";
            const priority = table.student.constraints ? table.student.constraints.reduce((sum, constraint) => sum + constraint.priority, 0) : null;

            console.log(`${letter} ${icon}: ${table.student.name} ${priority ? "(" + priority + ")" : "(-)"}`);
        });

    console.log("");
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

                // Check if this student is the target or caller of the constraint
                //  to call the constraint function with the correct source and target
                else {
                    // Student is the caller in the constraint arguments
                    if (student.name === constraint.arguments[0]) {
                        args[1] =
                            tables
                                .filter((table) => {
                                    return table.student && table.student.name === constraint.arguments[2];
                                })
                                .at(0) || null; // If the target student is not placed, pass null
                    }
                    // Student is the target in the constraint arguments
                    else if (student.name === constraint.arguments[2]) {
                        args[1] =
                            tables
                                .filter((table) => {
                                    return table.student && table.student.name === constraint.arguments[0];
                                })
                                .at(0) || null; // If the target student is not placed, pass null
                    }
                }

                // Call the relevant constraint function
                score += constraintFunctions[constraint.type](...args, constraint.priority, { classroomElements });
            });

            table.score = score;

            return table;
        })
        .sort((a, b) => b.score - a.score);

    // Take the best scored tables. The amount is based on the student's constraints' summed priorities
    const prioritySum = student.constraints.reduce((sum, constraint) => sum + constraint.priority, 0);
    const bestTables = rankedTables.slice(0, Math.ceil(rankedTables.length * ((0.85 ** prioritySum / 0.85) * 0.3))); // Viggo does not approve - TODO - maybe configure to allow for more randomness??

    // Pick a random table from the best tables and place the student there
    const randomTable = bestTables[Math.floor(Math.random() * bestTables.length)];
    randomTable.student = student;

    // Main wants to know the score of the table
    return randomTable.score;
};

const main = (students, classroomElements, options) => {
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

            // Compare the students by the sum of their constraints' priorities
            const aPriority = a.constraints.reduce((sum, constraint) => sum + constraint.priority, 0);
            const bPriority = b.constraints.reduce((sum, constraint) => sum + constraint.priority, 0);

            return bPriority - aPriority;
        });
    };

    const nullifyAllStudentConstraints = (students) => {
        students = students.map((student) => {
            student.constraints = null;
            return student;
        });
    };

    // Takes a list of all the constraints and give every student a copy the each constraint involving them
    const assignAllConstraints = (constraints, students) => {
        constraints.forEach((constraint) => {
            students = students.map((student) => {
                // Check if student is involved in the constraint, either as the caller or the target
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

    // Function for cleaner debugging
    const IF_DEBUG_logAllStudents = (message = null) => {
        if (!options.debug) {
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

    IF_DEBUG_logAllStudents("\nBefore sorting\n");

    // THIS IS WHERE THE MAGIC HAPPENS

    // Get all constraints sorted by the students sum of priorities
    const constraints = getAllConstraints(students);

    // Make sure all constraints where a student is involved are assigned to the student
    nullifyAllStudentConstraints(students); // Reset all constraints so that we can reassign them
    assignAllConstraints(constraints, students);

    // Sort students to seat the pickiest students first
    sortStudentsByPriority(students);

    IF_DEBUG_logAllStudents("\nAfter sorting\n");

    let seatingArrangementScore = 0;

    // Seat every student one at a time
    students.forEach((student) => {
        seatingArrangementScore += seatStudent(student, classroomElements);
    });

    // Draw the classroom
    if (options.fancyDraw) {
        fancyDraw(classroomElements, students);
    }

    return seatingArrangementScore;
};

const startTime = Date.now();

const layouts = [];
const scoresList = [];
const iterations = 10000;

// console.clear();

for (let i = 0; i < iterations; i++) {
    const layout = getLayoutFromFile();
    const classroomElements = getClassroomElementsFromLayout(layout);
    const students = getStudentListFromFile();

    const score = main(students, classroomElements, { debug: false, fancyDraw: i >= iterations - 1 }); // Show output on the last iteration(s)
    // const score = main(students, classroomElements, { debug: false, fancyDraw: false }); // Show output on the last iteration(s)

    layouts.push({ score, classroomElements });
    scoresList.push(score);
}

const timeTaken = Date.now() - startTime;

// Draw the best layout
const bestLayout = layouts.sort((a, b) => b.score - a.score)[0];
// fancyDraw(bestLayout.classroomElements, getStudentListFromFile());

console.log("Iterations:", iterations);
console.log("Avg score:", parseFloat((scoresList.reduce((sum, curr) => (sum += curr), 0) / iterations).toFixed(2)));
console.log("Max score:", parseFloat(Math.max(...scoresList).toFixed(2)));
console.log("Min score:", parseFloat(Math.min(...scoresList).toFixed(2)));
console.log("Time taken:", timeTaken, "ms");
console.log("\n------------------------------------\n");

// TODO - maybe consider average or mew score of students instead of total score of layout
// TODO - toy around with increasing the amount of randomness in the pickNG