using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining_task3_4
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Создайте параметр проекта экземпляра «Наименование» с типом данных «Текст» для труб.
            //Создайте приложение, которое для всех труб в модели записывает значение в
            //созданный параметр в следующую формате «Труба НАРУЖНЫЙ_ДИАМЕТР / ВНУТРЕННИЙ_ДИАМЕТР»,
            //где НАРУЖНЫЙ_ДИАМЕТР и ВНУТРЕННИЙ_ДИАМЕТР соответствующие диаметры трубы в
            //миллиметрах.
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<Pipe> pipes = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType()
                .Cast<Pipe>()
                .ToList();
            foreach (Pipe pipe in pipes)
            {
                if (pipe != null)
                {
                    double OUTER_DIAMETER = UnitUtils.ConvertFromInternalUnits(//НАРУЖНЫЙ_ДИАМЕТР
                            pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble(),
                            UnitTypeId.Millimeters);
                    double INNER_DIAM_PARAM = UnitUtils.ConvertFromInternalUnits(// ВНУТРЕННИЙ_ДИАМЕТР
                            pipe.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM).AsDouble(),
                            UnitTypeId.Millimeters);
                    string paramStr = "Труба " + OUTER_DIAMETER.ToString(".#") + "/" + INNER_DIAM_PARAM.ToString(".#");
                    using (Transaction ts = new Transaction(doc, "Set parametrs"))
                    {
                        ts.Start();
                        pipe.LookupParameter("Наименование").Set(paramStr);
                        ts.Commit();
                    }
                }
            }
            return Result.Succeeded;
        }
    }
}
