using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining_task3_1
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;


            IList<Reference> selectedElementRefList = uidoc.Selection.PickObjects(ObjectType.Face, new WallFilter(), "Выберите стены по граням");
            double sumVolume = 0;
            foreach (var selectedElement in selectedElementRefList)
            {
                Wall oWall = doc.GetElement(selectedElement) as Wall;
                if (oWall != null)
                {
                sumVolume += UnitUtils.ConvertFromInternalUnits(
                        oWall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble()
                        , UnitTypeId.CubicMeters);
                }
            }
            TaskDialog.Show("Объм стен", $"Суммарный объём выбранных стен: {sumVolume} м³");
            return Result.Succeeded;
        }
    }
}
