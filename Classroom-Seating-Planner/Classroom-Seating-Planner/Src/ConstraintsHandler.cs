using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Classroom_Seating_Planner.Src
{
    public class ConstraintsHandler
    {
        // TODO - remove. should be deprecated. 
        public static double GetDistanceBetweenCells(Classroom_Seating_Planner.Cells.Cell cell1, Classroom_Seating_Planner.Cells.Cell cell2)
        {
            // Get the horizontal and vertical distance between the two cells
            double horizontalDistance = Math.Abs(cell1.x - cell2.x);
            double verticalDistance = Math.Abs(cell1.y - cell2.y);

            // Use the Pythagorean theorem to calculate the distance between the two cells
            return Math.Sqrt(Math.Pow(horizontalDistance, 2) + Math.Pow(verticalDistance, 2));
        }


        public struct Constraint
        {
            public string type;
            public List<string?> arguments;
            public int priority;
        }

        public struct Student
        {
            public string name;
            public List<Constraint>? constraints;
        }

        public static List<Constraint>? InterpretStudentConstraints(string studentName, string rawConstraints)
        {
            Dictionary<string, Constraint> functionLookupTable = new() {
                { "nära",         new Constraint { type = "distance", arguments = [studentName, "near", null], priority = 1 }},
                { "intenära",     new Constraint { type = "distance", arguments = [studentName, "far",  null], priority = 1 }},
                { "inte nära",    new Constraint { type = "distance", arguments = [studentName, "far",  null], priority = 1 }},
                { "långtfrån",    new Constraint { type = "distance", arguments = [studentName, "far",  null], priority = 1 }},
                { "långt från",   new Constraint { type = "distance", arguments = [studentName, "far",  null], priority = 1 }},
                { "bredvid",      new Constraint { type = "adjacent", arguments = [studentName, "yes",  null], priority = 1 }},
                { "intebredvid",  new Constraint { type = "adjacent", arguments = [studentName, "no",   null], priority = 1 }},
                { "inte bredvid", new Constraint { type = "adjacent", arguments = [studentName, "no",   null], priority = 1 }},
            };

            Dictionary<string, string> recipientLookupTable = new() {
                {"tavlan",       "whiteboardCover"},
                {"tavla",        "whiteboardCover"},
                {"whiteboard",   "whiteboardCover"},
                {"whiteboards",  "whiteboardCover"},
                {"svartatavlan", "whiteboardCover"},
                {"klösbrädan",   "whiteboardCover"},
                {"dörren",       "door"},
                {"dörr",         "door"},
                {"fönstret",     "window"},
                {"fönster",      "window"},
                {"vindöga",      "window"},
            };

            List<Constraint> interpretedConstraints = [];

            List<string> rawConstraintsList = rawConstraints.Split('/').Select(rawConstraint => rawConstraint.Trim()).ToList();

            rawConstraintsList.ForEach(rawConstraint =>
            { // TODO - Whitespace remover Regex.Replace(input, @"\s", "")
                string trimmedConstraint = rawConstraint.ToLower();
                string? functionName = functionLookupTable.Keys
                    .ToList()
                    .Where(functionName => trimmedConstraint.StartsWith(functionName))
                    .FirstOrDefault();

                if (functionName == null) return;

                Constraint interpretedConstraint = functionLookupTable[functionName];

                // Isolate the recipient string
                string recipient = Regex.Replace(rawConstraint, @"\(.*\) ", "") // Remove priority (N)
                    .Replace(functionName, "") // TODO - whitespace remover, this is linked to the whitespace remover mentioned above
                    .Trim();

                // Look if the recipent is in the recipientLookupTable
                if (recipientLookupTable.TryGetValue(Regex.Replace(recipient, @"\s", "").ToLower(), out string? value))
                {
                    recipient = value;
                }
                interpretedConstraint.arguments[2] = recipient;

                // Find the priority of the constraint
                // Priority is the number insite the parenthesis (N)
                System.Text.RegularExpressions.Match match = Regex.Match(rawConstraint, @"\(([^)]+)\)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int intValue))
                {
                    interpretedConstraint.priority = intValue; // Override default priority value
                }

                interpretedConstraints.Add(interpretedConstraint);
            });

            return interpretedConstraints;
        }
    }
}
