using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TwoWaySliderSkia.Controls
{
    public class TwoWaySlider : SKCanvasView
    {
        #region bindable props
        public static BindableProperty UpperBoundProperty = BindableProperty.Create(
            nameof(UpperBound),
            typeof(float),
            typeof(TwoWaySlider),
            100f
            );

        public float UpperBound
        {
            get { return (float)GetValue(UpperBoundProperty); }
            set { SetValue(UpperBoundProperty, value); }
        }
        public static BindableProperty LowerBoundProperty = BindableProperty.Create(
            nameof(LowerBound),
            typeof(float),
            typeof(TwoWaySlider),
            0f
            );
        public float LowerBound
        {
            get { return (float)GetValue(LowerBoundProperty); }
            set { SetValue(LowerBoundProperty, value); }
        }
        #endregion

        #region props
        SKPoint _touchPoint, pointStart, pointEnd;
        SKPaint paintRed = new SKPaint() { StrokeWidth = 15, Color = SKColors.Red, IsStroke = true };
        SKPaint paintBlack = new SKPaint() { StrokeWidth = 20, Color = SKColors.Black, IsStroke = true };
        SKPaint paintGray = new SKPaint() { StrokeWidth = 18, Color = SKColors.LightGray, IsStroke = true };
        SKPaint paintThumbText = new SKPaint { SubpixelText = true, Color = Color.Blue.ToSKColor(),
            IsLinearText = true, TextSize = 50, FakeBoldText = true };
        SKPaint paintThumb = new SKPaint { Style = SKPaintStyle.Fill, Color = Color.Yellow.ToSKColor()};
        bool isStart, dontChange;
        float positionY, touchedPositionY, upperBoundPixel, lowerBoundPixel;
        long touchId;
        #endregion

        public TwoWaySlider()
        {
            EnableTouchEvents = true;
            SetValues();
        }
        private async Task SetValues()
        {
            //TODO: delay and wait for initialize and CanvasSize
            await Task.Delay(100);
            upperBoundPixel = (UpperBound * CanvasSize.Width) / 100;
            lowerBoundPixel = (LowerBound * CanvasSize.Width) / 100;
            pointStart = new SKPoint(lowerBoundPixel, 150);
            pointEnd = new SKPoint(upperBoundPixel, 150);
            positionY = touchedPositionY = 175;
            InvalidateSurface();
        }

        protected override void OnTouch(SKTouchEventArgs e)
        {
            if (e.Location == default) return;

            if (e.ActionType == SKTouchAction.Entered
                || e.ActionType == SKTouchAction.Pressed
                || e.ActionType == SKTouchAction.Moved)
            {
                if (e.Location.X < lowerBoundPixel || e.Location.X > upperBoundPixel) return;
                _touchPoint = e.Location;
                touchedPositionY = 120;
                touchId = e.Id;
                var diffStart = Math.Abs(_touchPoint.X - pointStart.X);
                var diffEnd = Math.Abs(_touchPoint.X - pointEnd.X);

                if (diffStart > 50 && diffEnd > 50) return;

                if (diffStart < 50)
                    isStart = true;
                
                if (diffEnd < 50)
                    isStart = false;
                
                InvalidateSurface();
            }
            else if (e.ActionType == SKTouchAction.Released)
            {
                touchedPositionY = positionY;
                if (e.Location.X < lowerBoundPixel || e.Location.X > upperBoundPixel) return;
                InvalidateSurface();
            }
            e.Handled = true;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();

            canvas.DrawLine(new SKPoint(0, 150), new SKPoint(CanvasSize.Width, 150), paintBlack);
            canvas.DrawLine(new SKPoint(lowerBoundPixel, 150), new SKPoint(upperBoundPixel, 150), paintGray);

            if (_touchPoint != null && _touchPoint != default)
            {
                dontChange = false;
                if (isStart == false && _touchPoint.X <= pointStart.X)
                    dontChange = true;
                canvas.DrawCircle(_touchPoint, 10, new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = Color.Red.ToSKColor()
                });

                if (isStart)
                {
                    pointStart.X = _touchPoint.X;
                }
                else if (!isStart && !dontChange)
                {
                    pointEnd.X = _touchPoint.X;
                }
            }
            canvas.DrawRoundRect(pointStart.X - 100, pointStart.Y - 50, 100, 80, 30, 30, paintThumb);
            canvas.DrawRoundRect(pointEnd.X, pointEnd.Y - 50, 100, 80, 30, 30, paintThumb);
            
            canvas.DrawLine(pointStart, pointEnd, paintRed);
            
            canvas.DrawText(((int)Math.Round(((pointStart.X / CanvasSize.Width) * 100))).ToString().ToString(),
                new SKPoint(pointStart.X - 100, isStart ? touchedPositionY : positionY), paintThumbText);

            canvas.DrawText(((int)Math.Round(((pointEnd.X / CanvasSize.Width) * 100))).ToString(),
                new SKPoint(pointEnd.X, isStart ? positionY : touchedPositionY), paintThumbText);
        }
    }
}
