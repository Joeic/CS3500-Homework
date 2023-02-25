
using Newtonsoft.Json;

public class test {
    static void Main(string[] args)
    {
        Dictionary<string, SS.Spreadsheet.Cell> cellMaps = new();
        cellMaps.Add("A1", new SS.Spreadsheet.Cell("20"));
        //cellMaps.Add("A2", new SS.Spreadsheet.Cell("10"));
        string Jsonfile = JsonConvert.SerializeObject(cellMaps, Formatting.Indented);
        Console.WriteLine(Jsonfile);
        Console.WriteLine("12");
        Console.WriteLine("12");
        Console.WriteLine("12");
        Console.WriteLine("11111");
    }

}       