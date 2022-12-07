using System;
using OpenTK;
using TucanEngine.Common.EventTranslation;
using TucanEngine.Rendering;

namespace TucanEngine.Gui
{
    public class Slider : Image2D
    {
        private const float ThumbScaleFactor = 0.1f;
        
        private float minValue;
        private float maxValue;
        private float currentValue;
        
        private float thumbTranslationUnit;
        private Image2D thumb;

        private MouseMovingEvent movingEvent;
        
        public Slider(float min, float max, GuiSkin skin) : base(skin.GetBoxTexture(), true) {
            minValue = min;
            maxValue = max;
            currentValue = minValue;
            var guiManager = GuiManager.GetCurrentManagerInstance();
            thumb = guiManager.Image(guiManager.GetSkin().GetThumbTexture(), true);
            thumb.SetParent(this);
            thumb.AddDragEvent(args => {
                AddValue(args.XDelta);
                movingEvent?.Invoke(args);
            });
            RecalculateBounds();
        }

        private void RecalculateBounds() {
            currentValue = minValue;
            thumb.LocalSpaceLocation = Vector3.Zero;
            thumb.LocalSpaceScale = Vector3.One;
            thumb.WorldSpaceScale = WorldSpaceScale.ScaleBy(ThumbScaleFactor, Axis.X);
            thumbTranslationUnit = (WorldSpaceScale.X - thumb.WorldSpaceScale.X) / (maxValue - minValue);
        }

        public override void OnScaling() {
            RecalculateBounds();
        }

        public void SetValue(float value) {
            currentValue = MathHelper.Clamp(value, minValue, maxValue);
            thumb.LocalSpaceLocation = thumb.LocalSpaceLocation
                .SetUnit(thumbTranslationUnit * (currentValue - minValue), Axis.X);
        }
        
        public void AddValue(float value) {
            SetValue(currentValue + value);
        }
        
        public void SetMovingEvent(MouseMovingEvent action) {
            movingEvent = action;
        }
        
        public float GetValue() {
            return currentValue;
        }
    }
}