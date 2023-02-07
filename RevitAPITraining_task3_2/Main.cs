using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining_task3_2
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Создайте приложение, которое выводит общую длину выбранных труб.
            IList<Reference> selectedElementRefList = uidoc.Selection.PickObjects(ObjectType.Element, new PipeFilter(), "Выберите трубы");
            double sumLength = 0;
            foreach (var selectedElement in selectedElementRefList)
            {
                Pipe oPipe = doc.GetElement(selectedElement) as Pipe;
                if (oPipe != null)
                {
                    sumLength += UnitUtils.ConvertFromInternalUnits(
                            oPipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble()
                            , UnitTypeId.Meters);
                }
            }
            TaskDialog.Show("Длина труб", $"Суммарная длина выбранных труб: {sumLength} м");
            return Result.Succeeded;
        }
    }
}
