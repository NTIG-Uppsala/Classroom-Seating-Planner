filePath = "/documents/bordsplaceringsgeneratorn/classroomLayout.txt"

//-------------- File content starts ----------------

   TTTT

BB BB BB BB BB

BBBB       BBB
      BBBB

 BB BB  BB BB

B BB BB  BB

//--------------- File content ends -----------------

public static ClassroomLayoutData InterpretClassroomLayoutString(string classroomLayoutString)
{
    ClassroomLayoutData returnObject = new();
    
    // We later find the biggest column width to set the column count
    List<int> xCoordinates = [];

    int rowIndex = 0;
    classroomLayoutString.Split("\n").ToList().ForEach((string row) =>
    {
        // Get every character in the row as a seperate char to iterate over
        int columnIndex = 0;
        row.ToList().ForEach((char letter) => // TODO - letter->cell/character?
        {
            if (letter.Equals('T'))
            {
                returnObject.whiteboardCells.Add(new cells.WhiteboardCell(columnIndex, rowIndex));
            }
            else if (letter.Equals('B'))
            {
                returnObject.tableCells.Add(new cells.TableCell(columnIndex, rowIndex));
            }

            xCoordinates.Add(columnIndex);
            columnIndex++;
        });

        rowIndex++;
    });

    int layoutWidth = xCoordinates.Max();
    int yCoordinate = rowIndex - 1;

    returnObject.rowCount = yCoordinate;
    returnObject.columnCount = layoutWidth;

    return returnObject;
}

public static ClassroomLayoutData GetClassroomLayoutDataFromFile()
{
    string classroomLayoutString = File.ReadAllText(FileHandler.filePath);
    Trace.WriteLine(classroomLayoutString);
    return InterpretClassroomLayoutString(classroomLayoutString);
}