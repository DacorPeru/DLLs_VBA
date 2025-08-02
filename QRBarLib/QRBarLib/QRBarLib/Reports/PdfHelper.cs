using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace QRBarLib.Reports
{
    /// <summary>
    /// Clase para generar PDFs con etiquetas, códigos QR y códigos de barras.
    /// </summary>
    public class PdfHelper
    {
        /// <summary>
        /// Genera un PDF con etiquetas personalizadas.
        /// </summary>
        /// <param name="outputPath">Ruta donde se guardará el PDF.</param>
        /// <param name="labels">Lista de etiquetas a generar.</param>
        /// <param name="pageSize">Tamaño de página (Ej. A4, LETTER).</param>
        public void GenerateLabelsPdf(
            string outputPath,
            List<LabelItem> labels,
            PageSizeType pageSize = PageSizeType.A4)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("La ruta de salida no puede estar vacía.", nameof(outputPath));

            if (labels == null || labels.Count == 0)
                throw new ArgumentException("Debe haber al menos una etiqueta.", nameof(labels));

            // Definir tamaño de página
            iTextSharp.text.Rectangle pageRect = GetPageSize(pageSize);

            using (FileStream fs = new FileStream(outputPath, FileMode.Create))
            using (Document doc = new Document(pageRect))
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                foreach (var label in labels)
                {
                    PdfPTable table = new PdfPTable(1);
                    table.WidthPercentage = 100;

                    // Texto del label
                    PdfPCell textCell = new PdfPCell(new Phrase(label.Text,
                        new iTextSharp.text.Font(
                            iTextSharp.text.Font.FontFamily.HELVETICA,
                            12,
                            iTextSharp.text.Font.BOLD)))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        PaddingBottom = 10f
                    };
                    table.AddCell(textCell);

                    // Imagen si existe
                    if (label.ImageBytes != null)
                    {
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(label.ImageBytes);
                        img.Alignment = Element.ALIGN_CENTER;
                        img.ScaleAbsolute(label.ImageWidth, label.ImageHeight);

                        PdfPCell imgCell = new PdfPCell(img)
                        {
                            Border = Rectangle.NO_BORDER,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        table.AddCell(imgCell);
                    }

                    doc.Add(table);
                    doc.NewPage(); // Cada etiqueta en una página
                }

                doc.Close();
            }
        }

        /// <summary>
        /// Obtiene el tamaño de página según el tipo.
        /// </summary>
        private iTextSharp.text.Rectangle GetPageSize(PageSizeType pageSize)
        {
            switch (pageSize)
            {
                case PageSizeType.LETTER:
                    return PageSize.LETTER;
                case PageSizeType.A5:
                    return PageSize.A5;
                case PageSizeType.A6:
                    return PageSize.A6;
                default:
                    return PageSize.A4;
            }
        }
    }

    /// <summary>
    /// Representa una etiqueta con texto y/o imagen.
    /// </summary>
    public class LabelItem
    {
        public string Text { get; set; }
        public byte[] ImageBytes { get; set; }
        public float ImageWidth { get; set; } = 100f;
        public float ImageHeight { get; set; } = 100f;
    }

    /// <summary>
    /// Tipos de tamaños de página admitidos.
    /// </summary>
    public enum PageSizeType
    {
        A4,
        LETTER,
        A5,
        A6
    }
}
