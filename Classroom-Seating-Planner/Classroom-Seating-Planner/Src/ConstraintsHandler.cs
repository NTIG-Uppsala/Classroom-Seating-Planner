using FlaUI.Core.WindowsAPI;
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
        public struct Constraint()
        {
            public string type;
            public string caller;
            public string? recipient;
            public string argument;
            public int priority = 1;
            public int priorityMultiplier;
        }

        public struct Student
        {
            public string name;
            public List<Constraint>? constraints;
        }

        public static List<Constraint>? InterpretStudentConstraints(string studentName, string rawConstraints)
        {
            Dictionary<string, Constraint> functionLookupTable = new() { 
                { "nära",         new Constraint { type="distance", caller=studentName, recipient=null, argument="near", priorityMultiplier=1 }},
                { "intenära",     new Constraint { type="distance", caller=studentName, recipient=null, argument="far",  priorityMultiplier=1 }},
                { "långtfrån",    new Constraint { type="distance", caller=studentName, recipient=null, argument="far",  priorityMultiplier=1 }},
                { "långtifrån",   new Constraint { type="distance", caller=studentName, recipient=null, argument="far",  priorityMultiplier=1 }},
                { "bredvid",      new Constraint { type="adjacent", caller=studentName, recipient=null, argument="yes",  priorityMultiplier=3 }},
                { "intebredvid",  new Constraint { type="adjacent", caller=studentName, recipient=null, argument="no",   priorityMultiplier=3 }},
            };

            Dictionary<string, string> recipientLookupTable = new() {
                {"tavlan",       "whiteboardCover"},
                {"tavla",        "whiteboardCover"},
                {"whiteboard",   "whiteboardCover"},
                {"whiteboards",  "whiteboardCover"},
                {"svartatavlan", "whiteboardCover"},
                {"klösbrädan",   "whiteboardCover"},
                // {"dörren",       "door"},
                // {"dörr",         "door"},
                // {"fönstret",     "window"},
                // {"fönster",      "window"},
                // {"vindöga",      "window"},
            };

            List<Constraint> interpretedConstraints = [];

            List<string> rawConstraintsList = rawConstraints.Split('/').Select(rawConstraint => rawConstraint.Trim()).ToList();

            rawConstraintsList.ForEach(rawConstraint =>
            {
                // Function Name
                string noWhitespaceConstraint = Regex.Replace(rawConstraint, @"\s", "").ToLower();
                string? functionName = functionLookupTable.Keys
                    .ToList()
                    .Where(functionName => noWhitespaceConstraint.StartsWith(functionName))
                    .FirstOrDefault();

                if (functionName == null) return; // If we can't find a function, ignore the constraint

                Constraint interpretedConstraint = functionLookupTable[functionName];


                // Priority
                System.Text.RegularExpressions.Match match = Regex.Match(rawConstraint, @"\(([^)]+)\)"); // Priority is the optional number inside the parenthesis (N)
                if (match.Success && int.TryParse(match.Groups[1].Value, out int readPriority))
                {
                    interpretedConstraint.priority = readPriority;
                }
                interpretedConstraint.priority *= interpretedConstraint.priorityMultiplier;


                // Recipient
                // Function names may contain whitespace in the raw constraint. So to isolate the recipient, we need to find where the function name ends
                int functionNameIndex = 0;
                int readIndex = 0;
                while (functionNameIndex < functionName.Length && readIndex < rawConstraint.Length)
                {
                    if (functionName[functionNameIndex].Equals(rawConstraint[readIndex]))
                    {
                        // If they match, carry on reading
                        functionNameIndex++;
                        readIndex++;
                    }
                    else
                    {
                        // If they don't match, look ahead in the raw constraint
                        readIndex++;
                    }
                }

                string constraintWithoutPriority = Regex.Replace(rawConstraint, @"\(.*\)", ""); // Remove priority (N)
                string recipient = constraintWithoutPriority.Substring(readIndex).Trim(); // Remove the function name from the constraint string

                // If the recipient is in the lookup table, replace it with the value from the table
                string possibleRecipient = Regex.Replace(recipient, @"\s", "").ToLower();
                interpretedConstraint.recipient = recipientLookupTable.ContainsKey(possibleRecipient) ? recipientLookupTable[possibleRecipient] : recipient;


                interpretedConstraints.Add(interpretedConstraint);
            });

            if (interpretedConstraints.Count.Equals(0)) interpretedConstraints = null;

            return interpretedConstraints;
        }
    }

    public class ConstraintFunctions
    {
        // TODO - change interpret func to define the function as an actual function reference
        // Look up for the constraint functions since they are stored as strings
        public static readonly Dictionary<string, Func<Cells.Cell, Cells.Cell, string, int, Dictionary<string, object>, double>> functions = new()
        {
            { "distance", Src.ConstraintFunctions.DistanceFunction },
            { "adjacent", Src.ConstraintFunctions.AdjacentFunction }
        };

        public static double DistanceFunction(Cells.Cell source, Cells.Cell target, string nearOrFar, int priority, Dictionary<string, object> references)
        {
            if (target == null) return 0;

            // Required references need to be defined
            if (!references.TryGetValue("classroomElements", out object? reference))
            {
                Trace.WriteLine("WARNING: Missing references in ConstraintsHandler DistanceFunction");
                return 0;
            }

            List<Cells.Cell> classroomElements = (List<Cells.Cell>)references["classroomElements"];

            // Get all the possible distances between the target and every classroom element
            List<double> allDistances = classroomElements
                .Where(cell => cell != target) // Remove target since we do not want the distance to our self
                .Where(cell => cell.cellType.Equals("table")) // We're only interrested in tables
                .Select(cell => Math.Sqrt(Math.Pow(cell.x - target.x, 2) + Math.Pow(cell.y - target.y, 2)))
                .ToList();

            // In case there are no tables
            if (allDistances.Count.Equals(0)) return 0;

            double maxPossibleDistance = allDistances.Max();
            double minPossibleDistance = allDistances.Min();

            double distanceToTarget = Math.Sqrt(Math.Pow(target.x - source.x, 2) + Math.Pow(target.y - source.y, 2));

            // Scale the distance in relation to closest and furthest possible distances to a value between 0 and 1 to get a normalized value
            double normalizedScore = (distanceToTarget - minPossibleDistance) / (maxPossibleDistance - minPossibleDistance);

            return nearOrFar switch
            {
                "near" => (1 - normalizedScore) * priority,
                "far" => normalizedScore * priority,
                _ => 0 // In case the arguments are somehow messed up
            };
        }

        public static double AdjacentFunction(Cells.Cell source, Cells.Cell target, string yesOrNo, int priority, Dictionary<string, object> references)
        {
            if (target == null) return 0;

            // Check if source is adjacent to target
            float xDiff = Math.Abs(source.x - target.x);
            float yDiff = Math.Abs(source.y - target.y);
            bool isAdjacent = (xDiff.Equals(1) && yDiff.Equals(0)) || (xDiff.Equals(0) && yDiff.Equals(1));

            return yesOrNo switch
            {
                "yes" when isAdjacent => priority,
                "no" when !isAdjacent => priority,
                _ => (double)0,// In case the requirements of the constraint are not met
            };
        }
    }
}
