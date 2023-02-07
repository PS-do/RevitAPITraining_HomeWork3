using Autodesk.Revit.ApplicationServices;
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

namespace RevitAPITraining_task3_3
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
//Создайте параметр проекта экземпляра «Длина с запасом» с типом данных «Длина» для труб.
//Создайте приложение, которое позволяет выбрать трубы, а также
//записывает их длину в метрах, увеличенную на коэффициент 1.1, в созданный
//параметр.
            var categorySet = new CategorySet();
            categorySet.Insert(Category.GetCategory(doc, BuiltInCategory.OST_PipeCurves));
            using (Transaction ts = new Transaction(doc, "Add parameter"))
            {
                ts.Start();
                CreateShareParameter(uiapp.Application, doc, "Длина с запасом", categorySet, BuiltInParameterGroup.PG_GEOMETRY, true);
                ts.Commit();
            }


            IList<Reference> selectedElementRefList = uidoc.Selection.PickObjects(ObjectType.Element, new PipeFilter(), "Выберите трубы");
           
            foreach (var selectedElement in selectedElementRefList)
            {                 
                Pipe oPipe = doc.GetElement(selectedElement) as Pipe;
                if (oPipe != null)
                {
                    double pipeLength = UnitUtils.ConvertFromInternalUnits(
                            oPipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble(),
                            UnitTypeId.Meters);
                    TaskDialog.Show("!", "pipeLength=" + pipeLength.ToString());
                    double newPipeLength = pipeLength * 1.1;
                    TaskDialog.Show("!", "newPipeLength=" + newPipeLength.ToString());
                    using (Transaction ts = new Transaction(doc, "Set parametrs"))
                    {
                        ts.Start();
                        //TaskDialog.Show("!","1");
                        Parameter edParameter = oPipe.LookupParameter("Длина с запасом");
                        //TaskDialog.Show("!", "2");
                        edParameter.Set(UnitUtils.ConvertToInternalUnits(pipeLength * 1.1,UnitTypeId.Meters));
                        //TaskDialog.Show("!", UnitUtils.ConvertToInternalUnits(pipeLength * 1.1, UnitTypeId.Millimeters).ToString());
                        ts.Commit();
                    }
                }
            }            
            return Result.Succeeded;
        }

        private void CreateShareParameter(Application application, Document doc,
           string parameterName, CategorySet categorySet,
           BuiltInParameterGroup builtInParameterGroup, bool isInstance)
        {
            DefinitionFile definitionFile = application.OpenSharedParameterFile();
            if (definitionFile == null)
            {
                TaskDialog.Show("Ошибка!", "Не найден файл общих параметров");
                return;
            }
            Definition definition = definitionFile.Groups
                .SelectMany(group => group.Definitions)
                .FirstOrDefault(def => def.Name.Equals(parameterName));
            if (definition == null)
            {
                TaskDialog.Show("Ощибка", "Не найден указанный параметр");
                return;
            }
            Binding binding = application.Create.NewTypeBinding(categorySet);
            if (isInstance)
                binding = application.Create.NewInstanceBinding(categorySet);
            BindingMap map = doc.ParameterBindings;
            map.Insert(definition, binding, builtInParameterGroup);
        }

    }
}
