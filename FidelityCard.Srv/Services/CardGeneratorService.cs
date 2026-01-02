using SkiaSharp;
using QRCoder;
using FidelityCard.Lib.Services;
using FidelityCard.Lib.Models;

namespace FidelityCard.Srv.Services;

public class CardGeneratorService : ICardGeneratorService
{
    public async Task<byte[]> GeneraCardDigitaleAsync(Fidelity fidelity, string? puntoVenditaNome = null)
    {
        return await Task.Run(() =>
        {
            var storeName = puntoVenditaNome ?? "Suns Fidelity Card";
            const int width = 800;
            const int height = 500;
            
            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            
            using (var paint = new SKPaint())
            {
                paint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    new SKPoint(width, 0),
                    new[] { SKColor.Parse("#105a12"), SKColor.Parse("#053e30") },
                    SKShaderTileMode.Clamp
                );
                canvas.DrawRect(0, 0, width, height, paint);
            }
            
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.White;
                paint.TextSize = 36;
                paint.IsAntialias = true;
                paint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold);
                canvas.DrawText("SUNS FIDELITY CARD", 40, 60, paint);
            }
            
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.White;
                paint.TextSize = 24;
                paint.IsAntialias = true;
                canvas.DrawText(storeName, 40, 100, paint);
            }
            
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.White;
                paint.TextSize = 28;
                paint.IsAntialias = true;
                paint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold);
                canvas.DrawText($"{fidelity.Nome} {fidelity.Cognome}", 40, 200, paint);
            }
            
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.White;
                paint.TextSize = 32;
                paint.IsAntialias = true;
                paint.Typeface = SKTypeface.FromFamilyName("Courier New", SKFontStyle.Bold);
                // Use CdFidelity if available, otherwise fallback or empty
                var code = fidelity.CdFidelity ?? "N/A";
                canvas.DrawText(code, 40, 260, paint);
            }
            
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.White;
                paint.TextSize = 24;
                paint.IsAntialias = true;
                // Assuming points are not readily available in Fidelity object or need to be passed. 
                // For now, omitting or putting 0 if not present.
                 canvas.DrawText($"Punti: 0", 40, 320, paint);
            }
            
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(fidelity.CdFidelity ?? "NO_CODE", QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(10);
            
            using (var qrImage = SKBitmap.Decode(qrBytes))
            {
                canvas.DrawBitmap(qrImage, new SKRect(550, 150, 750, 350));
            }
            
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        });
    }

    public async Task<byte[]> GeneraQRCodeAsync(string contenuto, int dimensione = 200)
    {
        return await Task.Run(() =>
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(contenuto, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(dimensione / 10); 
        });
    }
}
