// PdfHelper.cs
// Requiere: Install-Package iTextSharp (v5.x) en el proyecto
using System;
using System.Data;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Proyecto_BDII
{
    public static class PdfHelper
    {
        public static byte[] GenerarFacturaPdf(DataRow factura, DataTable detalles)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 50f, 50f, 60f, 40f);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Fuentes
                Font fTitulo = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD, new BaseColor(0, 123, 255));
                Font fNormal = new Font(Font.FontFamily.HELVETICA, 11, Font.NORMAL, BaseColor.BLACK);
                Font fBold = new Font(Font.FontFamily.HELVETICA, 11, Font.BOLD, BaseColor.BLACK);
                Font fThTotal = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
                Font fTotal = new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD, new BaseColor(40, 167, 69));

                // Título
                Paragraph titulo = new Paragraph("FACTURA - iStore Tienda de Celulares", fTitulo);
                titulo.Alignment = Element.ALIGN_CENTER;
                titulo.SpacingAfter = 6f;
                doc.Add(titulo);

                // Línea separadora
                doc.Add(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, new BaseColor(0, 123, 255), Element.ALIGN_CENTER, -1)));
                doc.Add(new Paragraph(" "));

                // Info factura
                PdfPTable tInfo = new PdfPTable(2) { WidthPercentage = 100 };
                tInfo.SetWidths(new float[] { 1, 2 });
                tInfo.DefaultCell.Border = Rectangle.NO_BORDER;
                tInfo.DefaultCell.PaddingBottom = 4f;

                Action<string, string> addRow = (lbl, val) => {
                    tInfo.AddCell(new PdfPCell(new Phrase(lbl, fBold)) { Border = Rectangle.NO_BORDER, PaddingBottom = 4f });
                    tInfo.AddCell(new PdfPCell(new Phrase(val ?? "", fNormal)) { Border = Rectangle.NO_BORDER, PaddingBottom = 4f });
                };

                addRow("N° Factura:", factura["numero_factura"].ToString());
                addRow("Fecha:", Convert.ToDateTime(factura["fecha_emision"]).ToString("dd/MM/yyyy HH:mm"));
                addRow("Cliente:", factura["razon_social"].ToString());
                addRow("C.I./NIT:", factura["nit_ci_cliente"].ToString());
                addRow("Método de pago:", factura["metodo_pago"].ToString());
                addRow("Dirección envío:", factura["direccion_envio"].ToString());
                addRow("Proveedor envío:", factura["proveedor_envio"].ToString());
                doc.Add(tInfo);
                doc.Add(new Paragraph(" "));

                // Tabla de productos
                PdfPTable tDet = new PdfPTable(4) { WidthPercentage = 100, SpacingBefore = 10f };
                tDet.SetWidths(new float[] { 3, 1, 1.5f, 1.5f });

                BaseColor azulClaro = new BaseColor(232, 244, 255);
                BaseColor azul = new BaseColor(0, 123, 255);

                string[] headers = { "Producto", "Cant.", "Precio Unit.", "Subtotal" };
                foreach (string h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h, fThTotal))
                    {
                        BackgroundColor = azul,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 7f
                    };
                    tDet.AddCell(cell);
                }

                bool altRow = false;
                foreach (DataRow d in detalles.Rows)
                {
                    BaseColor bg = altRow ? azulClaro : BaseColor.WHITE;
                    altRow = !altRow;

                    tDet.AddCell(new PdfPCell(new Phrase(d["marca"] + " " + d["modelo"], fNormal)) { BackgroundColor = bg, Padding = 6f });
                    tDet.AddCell(new PdfPCell(new Phrase(d["cantidad"].ToString(), fNormal)) { BackgroundColor = bg, HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6f });
                    tDet.AddCell(new PdfPCell(new Phrase("Bs. " + string.Format("{0:N2}", d["precio_unitario"]), fNormal)) { BackgroundColor = bg, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 6f });
                    tDet.AddCell(new PdfPCell(new Phrase("Bs. " + string.Format("{0:N2}", d["subtotal"]), fNormal)) { BackgroundColor = bg, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 6f });
                }
                doc.Add(tDet);
                doc.Add(new Paragraph(" "));

                // Total
                Paragraph pTotal = new Paragraph("TOTAL: Bs. " + string.Format("{0:N2}", factura["monto_total"]), fTotal);
                pTotal.Alignment = Element.ALIGN_RIGHT;
                doc.Add(pTotal);

                doc.Close();
                return ms.ToArray();
            }
        }
    }
}