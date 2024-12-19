using System.Windows.Controls;
using System.Diagnostics;
using System.Security.Permissions;
using Extension_Methods;

namespace Classroom_Seating_Planner.Src
{
    internal class SeatingHandler
    {
        public static double SeatStudent(ConstraintsHandler.Student student, List<Cells.Cell> classroomElements)
        {
            List<Cells.TableCell> tables = classroomElements.OfType<Cells.TableCell>().ToList();
            List<Cells.TableCell> availableTables = tables.Where(table => table.student == null).ToList().Shuffle(); // Suffle to get some variation

            // Silent error when there aren't enough tables - the program will warn the user when the file is read
            if (availableTables.Count.Equals(0)) return 0;

            Random random = new();
            Cells.TableCell randomTable = availableTables[random.Next(availableTables.Count)];

            // Randomly seat students if they don't have any constraints
            if (student.constraints == null)
            {
                randomTable.student = student;

                return 0;
            }

            // Wipe all tables scores since they are persistent
            availableTables.ForEach(table => table.score = 0);

            // Sirt the tables by how well they meet the constraints
            List<Cells.TableCell> rankedTables = availableTables
                .Select(table =>
                {
                    double score = 0;

                    // Try every constraint to get a students overall preference for a table
                    student.constraints.ForEach(constraint =>
                    {
                        double callConstraintFunction(Cells.Cell? target)
                        {
                            // Get function reference depending on the constraint type
                            Func<Cells.Cell, Cells.Cell, string, int, Dictionary<string, object>, double> function = ConstraintFunctions.functions[constraint.type];

                            // Define reference scopes that the constraint functions may need
                            Dictionary<string, object> references = new() { { "classroomElements", classroomElements }, { "caller", constraint.arguments[0] } }; // TODO - remove caller

                            // Arguments: source, target, constraint specific parameter, priority, references to things beyond the constraint's scope
                            return function(table, target, constraint.arguments[1], constraint.priority, references);
                        }

                        // These are taken from the interpreted constraint and are strings
                        string caller = constraint.arguments[0];
                        string? recipient = constraint.arguments[2];

                        // If more classroom elements are added, add them here as well 
                        List<string> classroomElementNames = ["whiteboardCover"];

                        // If recipient is a classroom element, set it as the target
                        if (classroomElementNames.Contains(recipient))
                        {
                            Cells.Cell? target = classroomElements.Where(element =>
                            {
                                return element.cellType.Equals(recipient);
                            }).FirstOrDefault();

                            score += callConstraintFunction(target);
                        }

                        // Else, the target must be student

                        // Depending on if this student is the caller or recipient in the constraint,
                        //  set the target of the constraint function to the other student's table
                        else if (student.name.Equals(caller))
                        {
                            // Try to find the table of the targeted student by looking for their name amongst the tables
                            Cells.TableCell? targetStudentTable = tables.Where(targetTable =>
                            {
                                if (targetTable.student == null) return false;
                                return (bool)targetTable.student?.name.Equals(recipient);// If a student is defined, we know that the student has a name
                            }).FirstOrDefault();

                            Cells.TableCell? target = targetStudentTable ?? null; // Target may not be seated yet

                            score += callConstraintFunction(target);
                        }
                        else if (student.name.Equals(recipient))
                        {
                            // Try to find the table of the targeted student by looking for their name among the tables
                            Cells.TableCell? targetStudentTable = tables.Where(targetTable =>
                            {
                                if (targetTable.student == null) return false;
                                return (bool)targetTable.student?.name.Equals(caller); // If a student is defined, we know that the student has a name
                            }).FirstOrDefault();

                            Cells.TableCell? target = targetStudentTable ?? null; // Target may not be seated yet

                            score += callConstraintFunction(target);
                        }
                    });

                    table.score = score;

                    return table;
                }).OrderByDescending(table => table.score).ToList();

            // OLD STUFFZ
            // Take the best scored tables. The amount is based on the student's constraints' summed priorities
            //int prioritySum = student.constraints.Select(constraint => constraint.priority).Sum();
            //List<Cells.TableCell> bestTables = rankedTables.Take((int)Math.Ceiling(rankedTables.Count * (Math.Pow(0.85, prioritySum - 1) * 0.3))).ToList();

            static void PRINT<T>(List<T> things)
            {
                Trace.Write("\n");

                int i = 0;
                things.ForEach(thing =>
                {
                    Trace.Write(thing);

                    if (i < things.Count - 1 && !(thing?.ToString() ?? "").Contains(':')) Trace.Write(", ");
                    else Trace.WriteLine(" ");

                    i++;
                });
            }

            // Group scores that are close in value
            double currentHighScore = rankedTables[0].score;
            List<List<Cells.TableCell>> groupedTables = [[]];

            // How close the scores can be to be considered in the same group
            //int prioritySum = student.constraints.Select(constraint => constraint.priority).Sum();
            //double threshold = currentHighScore * Math.Pow(0.85, prioritySum - 1) * 0.3;
            double threshold = currentHighScore * 0.1;

            int tableGroupIndex = 0;
            rankedTables.ForEach(table =>
            {
                // If group is empty, add table to it
                if (groupedTables[tableGroupIndex].SequenceEqual([]))
                {
                    groupedTables[tableGroupIndex].Add(table);
                    return;
                }

                // If score is within the threshold, add to group
                if (groupedTables[tableGroupIndex].First().score - table.score <= threshold) // .First() to avoid getting a stair step effect in the groups
                {
                    groupedTables[tableGroupIndex].Add(table);
                    return;
                }

                // Make new group
                tableGroupIndex++;
                groupedTables.Add([table]);
            });

            // Select a random item from the top group
            Cells.TableCell randomTableAmongTheBest = groupedTables[0][random.Next(groupedTables[0].Count)];

            randomTableAmongTheBest.student = student;

            return randomTableAmongTheBest.score;
        }

        // TODO - either remove score (as return of seatstudent aswell) or implement multiple runs of this function
        // TODO - This is somehow treated as a void function in main 
        public static double GenerateSeatingArrangement(List<ConstraintsHandler.Student> students, List<Cells.Cell> classroomElements)
        {
            static List<ConstraintsHandler.Constraint> getAllConstraints(List<ConstraintsHandler.Student> students)
            {
                return students
                    .Where(students => students.constraints != null)
                    .SelectMany(student => student.constraints) // It's fine. This can't be null at this point
                    .ToList();
            }

            static List<ConstraintsHandler.Student> sortStudentsByPriority(List<ConstraintsHandler.Student> students)
            {
                return students.OrderByDescending(student =>
                {
                    return student.constraints?.Select(constraint =>
                    {
                        return constraint.priority;

                    }).Sum() ?? 0;
                }).ToList();
            }

            static List<ConstraintsHandler.Student> nullifyAllStudentConstraints(List<ConstraintsHandler.Student> students)
            {
                return students.Select(student =>
                {
                    student.constraints = null;
                    return student;
                }).ToList();
            }

            // Takes a list of all the constraints and give every student a copy the each constraint involving them
            static List<ConstraintsHandler.Student> assignAllConstraints(List<ConstraintsHandler.Student> students, List<ConstraintsHandler.Constraint> constraints)
            {
                List<ConstraintsHandler.Student> localListOfStudents = students.Select(student => student).ToList();

                constraints.ForEach(constraint =>
                {
                    localListOfStudents = localListOfStudents.Select(student =>
                    {
                        // Check if student is involved in the constraint, either as the caller or the recipient
                        if (constraint.arguments[2] == null)
                        {
                            return student;
                        }

                        if (constraint.arguments[0].Equals(student.name) || constraint.arguments[2].Equals(student.name))
                        {
                            // Create an empty list for constraints if it does not already exist 
                            student.constraints ??= [];

                            student.constraints.Add(constraint);
                        }
                        return student;
                    }).ToList();
                });

                return localListOfStudents;
            }

            // Get all constraints as a list
            List<ConstraintsHandler.Constraint> constraints = getAllConstraints(students);

            // Make sure all constraints where a student is involved are assigned to the student
            students = nullifyAllStudentConstraints(students); // Reset all constraints so that we can reassign them

            students = assignAllConstraints(students, constraints);

            // Sort students to seat the pickiest students first
            students = sortStudentsByPriority(students);

            double seatingArrangementScore = 0;

            // Seat every student one at a time
            students.ForEach((student) =>
                {
                    seatingArrangementScore += SeatStudent(student, classroomElements);
                });

            return seatingArrangementScore;
        }

        public static void Populate(List<TextBlock> tableElements, List<string> classList)
        {
            // Ensure we don't exceed the number of available tables
            int numberOfStudentsToBePlaced = Math.Min(classList.Count, tableElements.Count);

            // Update the tables with the new order
            for (int index = 0; index < numberOfStudentsToBePlaced; index++)
            {
                // Assign the student name to the corresponding table
                tableElements[index].Text = classList[index];
            }

            // If there are more tables than students, clear the remaining tables
            for (int index = numberOfStudentsToBePlaced; index < tableElements.Count; index++)
            {
                // Clear the table if it's not occupied
                tableElements[index].Text = string.Empty;
            }
        }
    }
}
