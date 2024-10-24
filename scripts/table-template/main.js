const fs = require("node:fs");
const path = require("node:path");

const column = 6;
const row = 3;
const count = column * row;
let output = "";
const template =
    `

<Grid x:Name="{{TABLENAME}}" Grid.Row="{{ROW}}" Grid.Column="{{COLUMN}}">
    <Grid.RowDefinitions>
        <RowDefinition Height="2*"/>
        <RowDefinition Height="4*"/>
    </Grid.RowDefinitions>

    <Border BorderBrush="#FF8F8F8F" BorderThickness="1" Margin="2,2,2,2" Grid.RowSpan="2"/>

    <Label Content="{{TABLETEXT}}" HorizontalAlignment="Center" VerticalAlignment="Center"/>

    <Grid Grid.Row="2" Margin="2,0,2,2">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="1*"/>
            <ColumnDefinition Width="1*"/>
        </Grid.ColumnDefinitions>

        <Border BorderBrush="#FF8F8F8F" BorderThickness="1">
            <TextBlock x:Name="{{SEATNAME1}}" Padding="1,1,1,1" TextWrapping="WrapWithOverflow"></TextBlock>
        </Border>
        <Border BorderBrush="#FF8F8F8F" BorderThickness="1" Grid.Column="1">
            <TextBlock x:Name="{{SEATNAME2}}" Padding="1,1,1,1" TextWrapping="WrapWithOverflow"></TextBlock>
        </Border>
    </Grid>
</Grid>
`


for (let index = 0; index < count; index++) {
    let thisTable = template;

    thisTable = thisTable.replace("{{TABLENAME}}", `Table${index + 1}`);
    thisTable = thisTable.replace("{{TABLETEXT}}", `Bord ${index + 1}`);

    thisTable = thisTable.replace("{{ROW}}", `${Math.floor(index / column + 1)}`);
    thisTable = thisTable.replace("{{COLUMN}}", `${index % column}`);

    thisTable = thisTable.replace("{{SEATNAME1}}", `Seat${index * 2 + 1}`);
    thisTable = thisTable.replace("{{SEATNAME2}}", `Seat${index * 2 + 2}`);

    output += thisTable;
}

const outputPath = path.join(__dirname, "output.xaml");
fs.writeFileSync(outputPath, output);
console.log(`Done. Result written to "${outputPath}"`);