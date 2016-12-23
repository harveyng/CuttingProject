using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CutGLib;

namespace CutTestCSharp
{
  class Program
  {
    private static string LinearTurn(bool aRotated)
    {
      return aRotated ? "; Turned" : string.Empty;
    }

    // This routine outputs the linear results 
    private static void OutputLinearResults_by_Parts(CutEngine aCalculator)
    {
      int StockNo;
      Console.Write("\n");
      Console.Write("OUTPUT PARTS RESULTS\n");
      Console.Write("Used {0} linear stocks\n", aCalculator.UsedLinearStockCount);
      // Output parts locations
      double Length = 0, X = 0, angleStart, angleEnd;
      bool rotated, isAngle = aCalculator.CrossSection > 0;
      string id;
      Console.Write("\nPart Count={0}\n", aCalculator.PartCount);
      for (int iPart = 0; iPart < aCalculator.PartCount; iPart++)
      {
        // Get sizes and location of the part with index Iter
        // in case of incomplete optimization the function returns FALSE
        if (isAngle)
        {
          if (aCalculator.GetResultLinearPart(iPart, out StockNo, out Length, out angleStart, out angleEnd, out X, out rotated, out id))
          {
            Console.Write("Part ID={0};  stock={1};  X={2};  Length={3}; AngleStart={4}; AngleEnd={5}{6}\n",
                          id, StockNo, X, Length, angleStart, angleEnd, LinearTurn(rotated));
          }
          else Console.Write("Source piece {0} was not placed\n", iPart);
        }
        else
        {
          if (aCalculator.GetResultLinearPart(iPart, out StockNo, out Length, out X, out id))
          {
            Console.Write("Part ID={0};  stock={1};  X={2};  Length={3}\n", id, StockNo, X, Length);
          }
          else Console.Write("Source piece {0} was not placed\n", iPart);
        }
      }
      Console.Write("\n");
    }

    // This routine outputs the linear results by stock layouts
    private static void OutputLinearResults_by_Layout(CutEngine aCalculator)
    {
      int StockIndex,StockCount,iPart,iLayout,partCount,partIndex,tmp,iStock;
      double partLength,X,StockLength,angleStart,angleEnd;
      bool rotated, StockActive;
      bool isAngle = aCalculator.CrossSection > 0;
      Console.Write("\n");
      Console.Write("OUTPUT LAYOUT RESULTS\n");
      Console.Write("Used {0} linear stocks\n", aCalculator.UsedLinearStockCount);
      Console.Write("Created {0} different layouts\n", aCalculator.LayoutCount);
      // Iterate by each layout and output information about each layout,
      // such as number and length of used stocks and part indices cut from the stocks
      for (iLayout = 0; iLayout < aCalculator.LayoutCount; iLayout++)
      {
        aCalculator.GetLayoutInfo(iLayout, out StockIndex, out StockCount);
        // StockIndex is global index of the first stock used in the layout iLayout
        // StockCount is quantity of stocks of the same length as StockIndex used for this layout
        if (StockCount > 0)
        {
          Console.Write("Layout={0}:  Start Stock={1};  Count of Stock={2}\n", iLayout, StockIndex, StockCount);
          // Output information about each stock, such as stock Length
          for (iStock = StockIndex; iStock < StockIndex + StockCount; iStock++)
          {
            if (aCalculator.GetLinearStockInfo(iStock, out StockLength, out StockActive))
            {
              Console.Write("Stock={0}:  Length={1}\n", iStock, StockLength);
              // Output the information about parts cut from this stock
              // First we get quantity of parts cut from the stock
              partCount = aCalculator.GetPartCountOnStock(iStock);
              // Iterate by parts and get indices of cut parts
              for (iPart = 0; iPart < partCount; iPart++)
              {
                // Get global part index of iPart cut from the current stock
                partIndex = aCalculator.GetPartIndexOnStock(iStock, iPart);
                // Get length and location of the part
                // X - coordinate on the stock where the part beggins.
                if (isAngle)
                {
                  aCalculator.GetResultLinearPart(partIndex, out tmp, out partLength, out angleStart, out angleEnd, out X, out rotated);
                  // Output the part information
                  Console.Write("Part= {0}:  X={1};  Length={2}; AngleStart={3}; AngleEnd={4}{5}\n",
                                partIndex, X, partLength, angleStart, angleEnd, LinearTurn(rotated));

                }
                else
                {
                  aCalculator.GetResultLinearPart(partIndex, out tmp, out partLength, out X);
                  // Output the part information
                  Console.Write("Part= {0}:  X={1};  Length={2}\n", partIndex, X, partLength);
                }
              }
            }
          }
        }
      }
      Console.Write("\n");
    }


    /*
      This example demonstrates how to cut a linear stock (log/beam/wire) with size of 10 feet.
      Let say we need to cut 9 pieces of 3 feet, 3 pieces of 5 feet and 2 pieces of 7 feet.
    */
    private static void LinearOneSheetSize()
    {
      Console.Write("\n===============================================================\n");
      // First we create a new instance of the cut engine
      CutEngine Calculator = new CutEngine();
      // Add 7 linear stocks of 10 feet
      Calculator.AddLinearStock(10.0, 7);
      // Add linear pieces we have to cut from the stock:
      Calculator.AddLinearPart(3.0, 9, "Part 1"); // 9 pieces of 3 feet
      Calculator.AddLinearPart(5.0, 3, "Part 2"); // 3 pieces of 5 feet
      Calculator.AddLinearPart(7.0, 2, "Part 3"); // 2 pieces of 7 feet
      // Run the calculation:
      string result = Calculator.ExecuteLinear();
      if (result == "")
      {
        OutputLinearResults_by_Parts(Calculator);
        OutputLinearResults_by_Layout(Calculator);
      }
      else
      {
        Console.Write("%S\n", result);
      }
    }

    /*
      This example demonstrates how to cut a linear stock (log/beam/wire) with different sizes.
      Let say we need to cut 6 pieces of 11 feet, 8 pieces of 9 feet, 12 pieces of 7 feet
      and 4 pieces of 16 feet.
      There are 10 stocks of 20 feet, 5 stocks of 31 feet and 5 of 34 feet.
    */
    private static void LinearMultipleSheetSize()
    {
      Console.Write("\n===============================================================\n");
      // First we create a new instance of the cut engine
      CutEngine Calculator = new CutEngine();
      // Add 10 linear stocks of 20 feet
      Calculator.AddLinearStock(20.0, 6);
      // Add 5 linear stocks of 31 feet
      Calculator.AddLinearStock(31.0, 5);
      // Add 5 linear stocks of 34 feet
      Calculator.AddLinearStock(34.0, 5);
      // Add linear pieces we have to cut from the stock:
      Calculator.AddLinearPart(11.0, 6); // 6 pieces of 11 feet
      Calculator.AddLinearPart(9.0, 8); // 8 pieces of 9 feet
      Calculator.AddLinearPart(7.0, 12); // 12 pieces of 7 feet
      Calculator.AddLinearPart(16.0, 4); // 4 pieces of 16 feet
      // Run the calculation:
      string result = Calculator.ExecuteLinear();
      if (result == "")
      {
        OutputLinearResults_by_Parts(Calculator);
        OutputLinearResults_by_Layout(Calculator);
      }
      else
      {
        Console.Write("%S\n", result);
      }
    }

    // This example demonstrates how to cut a linear stock (log/beam/wire) with different angles.
    private static void LinearAngleCuts()
    {
      Console.Write("\n===============================================================\n");
      // First we create a new instance of the cut engine
      CutEngine Calculator = new CutEngine();
      Calculator.AddLinearStock(25.0, 10);
      // Add linear pieces we have to cut from the stock:
      Calculator.AddLinearPart(11, 2, 135, 60);
      Calculator.AddLinearPart(5, 3, 45, 45);
      Calculator.AddLinearPart(5, 4, 45, 135);
      // Set the stock cross section size
      Calculator.CrossSection = 1;
      Calculator.LinearExactAngle = false;
      Calculator.LinearAllowRotate = true;
      // Run the calculation:
      string result = Calculator.ExecuteLinear();
      if (result == "")
      {
        OutputLinearResults_by_Parts(Calculator);
        OutputLinearResults_by_Layout(Calculator);
      }
      else
      {
        Console.Write("%S\n", result);
      }
    }


    // This rotine outputs the results for 2D cutting optimization
    private static void OutputSheetResults_by_Parts(CutEngine aCalculator)
    {
      int StockNo, iCut, iPart;
      long CutsCount;
      double Width, Height, X1 = 0, Y1 = 0, X2 = 0, Y2 = 0;
      bool active;
      string id;
      Console.Write("\n");
      Console.Write("OUTPUT CUTS RESULTS\n");
      Console.Write("Used {0} sheets\n", aCalculator.UsedStockCount);
      // Output guilltoine cuts for each sheet
      for (StockNo = 0; StockNo < aCalculator.StockCount; StockNo++)
      {
        aCalculator.GetStockInfo(StockNo, out Width, out Height, out active);
        // Sheet was not used during calculation
        if (!active)
        {
          Console.Write("Sheet={0} was not used.\n", StockNo); 
          continue;
        }
        Console.Write("Sheet={0}: Width={1} Height={2}\n", StockNo, Width, Height);
        // First output any trim cuts for the sheet StockNo
        CutsCount = aCalculator.GetStockTrimCutCount(StockNo);
        for (iCut = 0; iCut < CutsCount; iCut++)
        {
          aCalculator.GetStockTrimCut(StockNo, iCut, out X1, out Y1, out X2, out Y2);
          Console.Write("Trim  Cut={0}:  X1={1};  Y1={2};  X2={3};  Y2={4}\n", iCut, X1, Y1, X2, Y2);
        }
        // Now output any actual cuts for the sheet StockNo
        CutsCount = aCalculator.GetStockCutCount(StockNo);
        for (iCut = 0; iCut < CutsCount; iCut++)
        {
          aCalculator.GetStockCut(StockNo, iCut, out X1, out Y1, out X2, out Y2);
          Console.Write("Cut={0}:  X1={1};  Y1={2};  X2={3};  Y2={4}\n", iCut, X1, Y1, X2, Y2);
        }
      }

      // Get parts locations
      double W = 0, H = 0, X = 0, Y = 0;
      bool Rotated;
      Console.Write("\n");
      Console.Write("OUTPUT PARTS RESULTS\n");
      Console.Write("\nPart Count={0}\n", aCalculator.PartCount);
      for (iPart = 0; iPart < aCalculator.PartCount; iPart++)
      {
        // Get sizes and location of the source part with index Iter
        // in case of incomplete optimization the part can be unplaced
        // and the function returns FALSE.
        if (aCalculator.GetResultPart(iPart, out StockNo, out W, out H, out X, out Y, out Rotated, out id))
        {
          Console.Write("Part ID={0};  sheet={1};  X={2};  Y={3};  Width={4};  Height={5}\n",
                        id, StockNo, X, Y, W, H);
        }
        else Console.Write("Part {0} was not placed\n", iPart);
      }
      Console.Write("\n");
    }

    // This rotine outputs the results for 2D cutting optimization by layouts
    private static void OutputSheetResults_by_Layout(CutEngine aCalculator)
    {
      int sheetIndex,StockCount,iPart,iLayout,partCount,partIndex,tmp,iSheet;
      double width,height,X,Y,W,H;
      bool rotated,sheetActive;
      string Txt;
      Console.Write("\n");
      Console.Write("OUTPUT LAYOUT RESULTS\n");
      Console.Write("Used {0} sheets\n", aCalculator.UsedStockCount);
      Console.Write("Created {0} different layouts\n", aCalculator.LayoutCount);
      // Iterate by each layout and output information about each layout,
      // such as number and length of used stocks and part indices cut from the stocks
      for (iLayout = 0; iLayout < aCalculator.LayoutCount; iLayout++)
      {
        aCalculator.GetLayoutInfo(iLayout, out sheetIndex, out StockCount);
        // sheetIndex is global index of the first sheet used in the layout iLayout
        // StockCount is quantity of sheets of the same size as sheetIndex used for this layout
        if (StockCount > 0)
        {
          Console.Write("Layout={0}:  Start Sheet={1};  Count of Sheet={2}\n", iLayout, sheetIndex, StockCount);
          // Output information about each stock, such as stock Length
          for (iSheet = sheetIndex; iSheet < sheetIndex + StockCount; iSheet++)
          {
            if (aCalculator.GetStockInfo(iSheet, out width, out height, out sheetActive))
            {
              Console.Write("Sheet={0}:  Width={1}; Height={2}\n", iSheet, width, height);
              // Output the information about parts cut from this sheet
              // First we get quantity of parts cut from the sheet
              partCount = aCalculator.GetPartCountOnStock(iSheet);
              // Iterate by parts and get indices of cut parts
              for (iPart = 0; iPart < partCount; iPart++)
              {
                // Get global part index of iPart cut from the current sheet
                partIndex = aCalculator.GetPartIndexOnStock(iSheet, iPart);
                // Get sizes and location of the source part with index partIndex
                if (aCalculator.GetResultPart(partIndex, out tmp, out W, out H, out X, out Y, out rotated))
                {
                  // Output the coordinates
                  if (rotated) Txt = "Yes";
                  else Txt = "No";
                  Console.Write("Part={0}; sheet={1}; Width={2}; Height={3}; X={4}; Y={5}; Rotated={6}\n",
                                partIndex, iSheet, W, H, X, Y, Txt);
 
                }
              }
            }
          }
        }
      }
      Console.Write("\n");
    }

    // Outputs the layout information.
    private static void OuputLayoutInfo(CutEngine aCalculator)
    {
      int StockNo, StockCount;

      Console.Write("Used {0} sheets\n", aCalculator.UsedStockCount);
      Console.Write("Created {0} different layouts\n", aCalculator.LayoutCount);
      for (int iLayout = 0; iLayout < aCalculator.LayoutCount; iLayout++)
      {
        aCalculator.GetLayoutInfo(iLayout, out StockNo, out StockCount);
        if (StockCount > 0)
        {
          Console.Write("Layout={0}:  Start Sheet={1};  Count of Sheets={2}\n", iLayout, StockNo, StockCount);
        }
      }
    }

    /*
      This example demonstrates how to cut a 2D rectangular sheet/panels with size of 2400x2000 mm.
      Let say we need to cut 9 parts of 640x420 mm, 14 parts of 150x720 mm and 12 parts of 1000x420 mm. 
      In addition the 14 parts of 150x720 mm cannot be rotated.
    */
    private static void SheetOneSheetSize()
    {
      Console.Write("\n===============================================================\n");
      // First we create a new instance of the cut engine
      CutEngine Calculator = new CutEngine();
      // Add 5 sheets of 2400x2000 mm
      Calculator.AddStock(1000, 1500, 5);
      // Add parts we have to cut from the sheet:
      Calculator.AddPart(640, 420, 3, true, "640x420mm"); // 9 parts of 640x420 mm
      Calculator.AddPart(150, 720, 5, false, "150x720mm"); // 14 non-rotatable parts of 150x720 mm
      Calculator.AddPart(1000, 420, 2, true, "1000x420mm"); // 12 parts of 1000x420 mm
      // Run the calculation:
      string result = Calculator.Execute();
      if (result == "")
      {
        OuputLayoutInfo(Calculator);
        OutputSheetResults_by_Parts(Calculator);
        OutputSheetResults_by_Layout(Calculator);
        // Save image file for the stock 0 with name "SheetOne.png" close to executable
        try
        {
            for(int i=0; i < Calculator.UsedStockCount; i++)
            {
                string imageFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
                imageFile = Path.Combine(Path.GetDirectoryName(imageFile), "Sheet"+i+".png");
                // if the location of the executable file is read-only then we'll get exception here
                Calculator.CreateStockImage(i, imageFile, 1500);
            }
          //string imageFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
          //imageFile = Path.Combine(Path.GetDirectoryName(imageFile), "SheetOne.png");
          // if the location of the executable file is read-only then we'll get exception here
          //Calculator.CreateStockImage(0, imageFile, 1500);
        }
        catch
        {}
      }
      else
      {
        Console.Write("%S\n", result);
      }
    }

    /*
      This example demonstrates how to cut a 2D rectangular sheet/panels with different sizes.
      All parts cannot be rotated.
    */
    private static void SheetMultipleSheetSize()
    {
      Console.Write("\n===============================================================\n");
      // First we create a new instance of the cut engine
      CutEngine Calculator = new CutEngine();
      Calculator.AddStock(2000, 2400, 5); // 5 sheets of 2000x24000
      //Calculator.AddStock(1800, 2000, 5); // 5 sheets of 1800x2000
      //Calculator.AddStock(1200, 1600, 10); // 10 sheets of 1200x1600
      // Add parts we have to cut from the sheets:
      Calculator.AddPart(650, 450, 7, false); // 36 non-rotatable parts of 650x450 mm
      Calculator.AddPart(650, 732, 5, false); // 24 non-rotatable parts of 650x732 mm
//       Calculator.AddPart(500, 430, 24, false); // 24 non-rotatable parts of 500x430 mm
//       Calculator.AddPart(163, 422, 36, false); // 36 non-rotatable parts of 163x422 mm
//       Calculator.AddPart(444, 363, 36, false); // 36 non-rotatable parts of 444x363 mm
//       Calculator.AddPart(104, 362, 36, false); // 36 non-rotatable parts of 104x362 mm
      // Run the calculation:
      string result = Calculator.Execute();
      if (result == "")
      {
        OutputSheetResults_by_Parts(Calculator);
        OutputSheetResults_by_Layout(Calculator);
      }
      else
      {
        Console.Write("%S\n", result);
      }
    }

    /*
      This example demonstrates how to cut a 2D rectangular sheet/panels with or without
      layout optimization.
      If the parameter aMinimizeLayout is TRUE then calculation uses 9 sheets and 4 different layouts.
      If the parameter aMinimizeLayout is FALSE then calculation uses 8 sheets and 6 different layouts.
    */
    private static void SheetLayoutSheetSize(bool aMinimizeLayout)
    {
      Console.Write("\n===============================================================\n");
      // First we create a new instance of the cut engine
      CutEngine Calculator = new CutEngine();
      Calculator.AddStock(2000, 2000, 20); // 20 sheets of 2000x2000 mm.
      // Add parts we have to cut from the sheets:
      Calculator.AddPart(650, 550, 12); // 12 parts of 650x550 mm
      Calculator.AddPart(650, 732, 20); // 20 parts of 650x732 mm
      Calculator.AddPart(500, 430, 21); // 21 parts of 500x430 mm
      Calculator.AddPart(163, 422, 32); // 32 parts of 163x422 mm
      Calculator.AddPart(444, 363, 30); // 30 parts of 444x363 mm
      Calculator.AddPart(104, 362, 32); // 32 parts of 104x362 mm
      Calculator.AddPart(640, 200, 32); // 32 parts of 640x200 mm
      Calculator.UseLayoutMinimization = aMinimizeLayout;
      // Run the calculation:
      string result = Calculator.Execute();
      if (result == "")
      {
        OuputLayoutInfo(Calculator);
      }
      else
      {
        Console.Write("%S\n", result);
      }
    }


    static void Main(string[] args)
    {
      // 1D optimization examples
      //LinearOneSheetSize();
      //LinearMultipleSheetSize();
      //LinearAngleCuts();
      // 2D rectangular optimization examples
      SheetOneSheetSize();
      //SheetMultipleSheetSize();
      // 2D example with layouts 
      //SheetLayoutSheetSize(false);
      //SheetLayoutSheetSize(true);
    }
  }
}
