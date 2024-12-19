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

            Trace.Write("\n");
            Trace.WriteLine($"Seating {student.name} {student.constraints.Select(constraint => constraint.priority).Sum()}");
            if (student.name.Equals("B-bredvid-A"))
            {
                Trace.WriteLine(student.constraints[0].arguments[1]);
            }
            //Trace.WriteLine($"")
            Trace.Write("\n");

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

            // Take the best scored tables. The amount is based on the student's constraints' summed priorities
            //int prioritySum = student.constraints.Select(constraint => constraint.priority).Sum();
            //List<Cells.TableCell> bestTables = rankedTables.Take((int)Math.Ceiling(rankedTables.Count * (Math.Pow(0.85, prioritySum - 1) * 0.3))).ToList();

            // 50,20,1,0,0,0,0,0,0,0,0,0,0,0

            //Ifall två lika bra platser finns, ska båda finnas med i best tables
            // 500, 500, 30, 30, 23,12,0
            // [500, 504], [30, 30], [23], [12], [0]
            // [504], [500], [30, 30], [23], [12], [0]
            // [1], [0,0,0,0]

            //groupedTables = []

            //for (int i = 0; i < rankedTables.Count; i++){
            //    

            // Group scores in goodness.
            // If two scores are within 5% of eachother, group them.
            // Select the best group and pick a random one.

            double currentHighScore = rankedTables[0].score;
            //List<Cells.TableCell> currentGroup = [rankedTables[0]];
            List<List<Cells.TableCell>> groupedTables = [[]];

            // The threshold is 5% of the highest score
            //int prioritySum = student.constraints.Select(constraint => constraint.priority).Sum();
            //double threshold = currentHighScore * Math.Pow(0.85, prioritySum - 1) * 0.3;
            double threshold = currentHighScore * 0.1;
            
            int tableGroupIndex = 0;
            rankedTables.ForEach(table =>
            {
                if (groupedTables[tableGroupIndex].Count.Equals(0))
                {
                    groupedTables[tableGroupIndex].Add(table);

                }
                else if (groupedTables[tableGroupIndex][0].score - table.score <= threshold)
                {
                    groupedTables[tableGroupIndex].Add(table);
                }
                else
                {
                    tableGroupIndex++;
                    groupedTables.Add([table]);
                }   
            });

            //if (student.name.Equals("A-bredvid-B") || student.name.Equals("B-bredvid-A")){
            Trace.Write("\n");
            Trace.WriteLine($"Seating {student.name} {student.constraints.Select(constraint => constraint.priority).Sum()}");
            if (student.name.Equals("B-bredvid-A"))
            {
                Trace.WriteLine(student.constraints[0].arguments[1]);
            }
            //Trace.WriteLine($"")
            Trace.Write("\n");
            groupedTables.ForEach(tableGroup =>
            {
                Trace.Write($"Table group: {groupedTables.IndexOf(tableGroup)} | ");
                tableGroup.ForEach(table =>
                {
                    Trace.Write($"{(float)table.score}, ");
                });
                Trace.Write("\n");
            });
            //}
            //rankedTables.ForEach(Cells.TableCell table =>
            //{
            //    if (currentHighScore - table.score > threshold)
            //    {
            //        return;
            //    }

            //    currentGroup.Add(table);
            //});

            // Group items that are within 5% of the highest score
            //for (int i = 1; i < rankedTables.Count; i++)
            //{
            //    if (currentHighScore - rankedTables[i].score > threshold)
            //    {
            //        break;
            //    }

            //    currentGroup.Add(rankedTables[i]);
            //}

            // Select a random item from the top group
            Cells.TableCell randomTableAmongTheBest = groupedTables[0][random.Next(groupedTables[0].Count)];

            randomTableAmongTheBest.student = student;

            return randomTableAmongTheBest.score;

            //List<Cells.TableCell> bestTables = rankedTables.Take((int)Math.Ceiling(rankedTables.Count * (Math.Pow(0.85, prioritySum - 1) * 0.3))).ToList();

            //// Pick a random table from the best tables and place the student there
            //randomTable = bestTables[random.Next(bestTables.Count)];
            //randomTable.student = student;

            //// Main wants to know the score of the table
            //return randomTable.score;
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

            //static void DEBUG_CONSTRAINT(ConstraintsHandler.Constraint? constraint) // TODO - Remove
            //{
            //    if (constraint != null)
            //    {

            //        Trace.WriteLine($"{constraint?.arguments[0]} | {constraint?.type} | {constraint?.arguments[1]} | {constraint?.arguments[2]} | {constraint?.priority}");
            //    }
            //}

            static List<ConstraintsHandler.Student> sortStudentsByPriority(List<ConstraintsHandler.Student> students)
            {
                //Trace.WriteLine("\nSorting students by constraint priority\n"); // TODO - remove
                return students.OrderByDescending(student =>
                {
                    //Trace.WriteLine($"Student: {student.name}"); // TODO - remove
                    return student.constraints?.Select(constraint =>
                    {
                        //Trace.WriteLine(constraint.priority); // TODO - remove
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
                            //string DEBUG_TARGET = "null"; // TODO - remove
                            //if (constraint.arguments[0].Equals(student.name)) // TODO - remove
                            //{ // TODO - remove
                            //    DEBUG_TARGET = constraint.arguments[2]; // TODO - remove
                            //} // TODO - remove
                            //else if (constraint.arguments[2].Equals(student.name)) // TODO - remove
                            //{ // TODO - remove
                            //    DEBUG_TARGET = constraint.arguments[0]; // TODO - remove
                            //} // TODO - remove

                            //Trace.WriteLine($"{student.name} has constraint {constraint.type} | {constraint.arguments[1]} | {DEBUG_TARGET} | {constraint.priority}"); // TODO - remove

                            // Create an empty list for constraints if it does not already exist // TODO - remove
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
            //constraints.ForEach(constraint => // TODO - remove
            //{                                 // TODO - remove
            //    DEBUG_CONSTRAINT(constraint); // TODO - remove
            //});                               // TODO - remove

            // Make sure all constraints where a student is involved are assigned to the student
            students = nullifyAllStudentConstraints(students); // Reset all constraints so that we can reassign them
            //Trace.WriteLine("\nTRIED NULLIFYING CONSTRAITNS\n");                             // TODO - remove
            //students.ForEach(student =>                                                      // TODO - remove
            //{                                                                                // TODO - remove
            //    if (student.constraints != null)                                             // TODO - remove
            //    {                                                                            // TODO - remove
            //        //Trace.WriteLine($"\n{student.name} constraints\n");                    // TODO - remove
            //        student.constraints.ForEach(constraint => DEBUG_CONSTRAINT(constraint)); // TODO - remove

            //    }                             // TODO - remove
            //});                               // TODO - remove
            //constraints.ForEach(constraint => // TODO - remove
            //{ // TODO - remove
            //    DEBUG_CONSTRAINT(constraint); // TODO - remove
            //}); // TODO - remove

            //Trace.WriteLine("\nTRYING TO ASSIGN ALL CONSTRAINTS\n"); // TODO - remove
            students = assignAllConstraints(students, constraints);

            // Sort students to seat the pickiest students first
            students = sortStudentsByPriority(students);
            //Trace.WriteLine("\nSorting students by constraint priority\n"); // TODO - remove

            students.ForEach(student =>
                {
                    //Trace.WriteLine($"{student.name} {student.constraints?.Select(constraint => constraint.priority).Sum() ?? 0}");
                });

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
