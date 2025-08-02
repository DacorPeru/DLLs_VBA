using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SmartCodeLib.Reports
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class LabelTemplateHelper
    {
        /// <summary>
        /// Estructura para definir una plantilla de etiquetas.
        /// </summary>
        public class LabelTemplate
        {
            public string Name { get; set; } = "Default";
            public double PageWidthMm { get; set; } = 210;   // Ancho hoja (A4 por defecto)
            public double PageHeightMm { get; set; } = 297;  // Alto hoja
            public double LabelWidthMm { get; set; } = 50;   // Ancho etiqueta
            public double LabelHeightMm { get; set; } = 30;  // Alto etiqueta
            public int Columns { get; set; } = 3;
            public int Rows { get; set; } = 10;
            public double MarginLeftMm { get; set; } = 5;
            public double MarginTopMm { get; set; } = 5;
            public double HorizontalSpacingMm { get; set; } = 2;
            public double VerticalSpacingMm { get; set; } = 2;
        }

        private Dictionary<string, LabelTemplate> templates;

        public LabelTemplateHelper()
        {
            templates = new Dictionary<string, LabelTemplate>
            {
                { "Default", new LabelTemplate() },
                { "A4_3x10", new LabelTemplate
                    {
                        Name = "A4_3x10",
                        PageWidthMm = 210,
                        PageHeightMm = 297,
                        LabelWidthMm = 63.5,
                        LabelHeightMm = 38.1,
                        Columns = 3,
                        Rows = 10,
                        MarginLeftMm = 7,
                        MarginTopMm = 12.7,
                        HorizontalSpacingMm = 2.5,
                        VerticalSpacingMm = 0
                    }
                },
                { "A4_2x7", new LabelTemplate
                    {
                        Name = "A4_2x7",
                        PageWidthMm = 210,
                        PageHeightMm = 297,
                        LabelWidthMm = 99.1,
                        LabelHeightMm = 67.7,
                        Columns = 2,
                        Rows = 7,
                        MarginLeftMm = 4.2,
                        MarginTopMm = 8.4,
                        HorizontalSpacingMm = 3,
                        VerticalSpacingMm = 0
                    }
                }
            };
        }

        /// <summary>
        /// Obtiene la lista de nombres de plantillas disponibles.
        /// </summary>
        public string[] GetAvailableTemplates()
        {
            string[] keys = new string[templates.Count];
            templates.Keys.CopyTo(keys, 0);
            return keys;
        }

        /// <summary>
        /// Obtiene una plantilla por nombre.
        /// </summary>
        public LabelTemplate GetTemplate(string name)
        {
            return templates.ContainsKey(name) ? templates[name] : templates["Default"];
        }

        /// <summary>
        /// Agrega o reemplaza una plantilla personalizada.
        /// </summary>
        public void AddOrUpdateTemplate(string name, double pageWidthMm, double pageHeightMm,
                                        double labelWidthMm, double labelHeightMm,
                                        int columns, int rows,
                                        double marginLeftMm, double marginTopMm,
                                        double horizontalSpacingMm, double verticalSpacingMm)
        {
            templates[name] = new LabelTemplate
            {
                Name = name,
                PageWidthMm = pageWidthMm,
                PageHeightMm = pageHeightMm,
                LabelWidthMm = labelWidthMm,
                LabelHeightMm = labelHeightMm,
                Columns = columns,
                Rows = rows,
                MarginLeftMm = marginLeftMm,
                MarginTopMm = marginTopMm,
                HorizontalSpacingMm = horizontalSpacingMm,
                VerticalSpacingMm = verticalSpacingMm
            };
        }
    }
}
