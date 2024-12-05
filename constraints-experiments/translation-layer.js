// # Names.txt
// A: nära tavlan
// # or
// A: NearWhiteboard
// # or
// A: CloseTo(whiteboard)
// # or
// A: CloseTo(B)

// B: CloseTo(Whiteboard)
// # or
// B: Distance(near, Whiteboard)
const studentz = [
    {
        name: "A",
        constraints: [{ type: "distance", arguments: ["near", "B"], priority: 10 }],
    },
];

const fs = require("node:fs");

const lookup = {
    nära: { type: "distance", arguments: ["near", undefined] },
};

const students = fs
    .readFileSync("./viggos-data/names.txt", "utf-8")
    .split("\n")
    .map((row) => row.trim())
    .map((row) => {
        const [rawName, rawConstraints] = row.split(":");

        Object.keys(lookup).forEach((key) => {
            if (rawConstraints.includes(key)) {
                
            }
        });

        return returnObject;
    })
    .filter(Boolean);

// students.forEach((student) => {
//     console.log(student.name, "\n   Constraints:", JSON.stringify(student.constraints));
// });