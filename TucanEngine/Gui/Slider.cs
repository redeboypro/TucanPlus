using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Input;
using TucanEngine.Common.EventTranslation;
using TucanEngine.Rendering;

namespace TucanEngine.Gui
{
    public class Slider : Image2D
    {
        private const float ThumbScaleFactor = 0.1f;
        private const float BaseScaleFactor = 2.0f;
        private const float ThresholdFactor = 0.25f;
        
        private readonly float minValue;
        private readonly float maxValue;
        private float currentValue;
        
        private float thumbTranslationUnit;
        private readonly Image2D thumb;

        private readonly Orientation orientation;
        private Action valueChangingEvent;

        private int additionValueSign = 1;

        public Slider(float min, float max, Orientation orientation, GuiSkin skin) : base(skin.GetBoxTexture(), true) {
            minValue = min;
            maxValue = max;
            currentValue = minValue;
            this.orientation = orientation;
            var guiManager = GuiManager.GetCurrentManagerInstance();
            thumb = guiManager.Image(guiManager.GetSkin().GetThumbTexture(), true);
            thumb.SetParent(this);
            thumb.AddDragEvent(args => {
                AddValue(orientation == Orientation.Horizontal ? args.XDelta * ThresholdFactor :
                    orientation == Orientation.Vertical ? args.YDelta * -additionValueSign * ThresholdFactor :
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null));
                valueChangingEvent?.Invoke();
            });
            RecalculateBounds();
        }

        private void RecalculateBounds() {
            currentValue = minValue;
            thumb.LocalSpaceScale = Vector3.One;
            thumb.WorldSpaceScale = WorldSpaceScale.ScaleBy(ThumbScaleFactor, (Axis) (int)orientation);
            thumbTranslationUnit = BaseScaleFactor / (maxValue - minValue);
        }

        public override void OnScaling() {
            RecalculateBounds();
            SetThumbStartLocation();
        }
        
        public override void OnRotating() {
            RecalculateBounds();
            SetThumbStartLocation();
            additionValueSign = Sign(MathHelper.RadiansToDegrees(GetRotationAngle()));
        }

        private void SetThumbStartLocation() {
            thumb.LocalSpaceLocation = -Vector3.UnitX;
        }
        
        private void SetThumbLocationByValue() {
            thumb.LocalSpaceLocation = -Vector3.UnitX + Vector3.Zero
                .SetUnit(thumbTranslationUnit * (currentValue - minValue), Axis.X);
        }

        private static int Sign(float value) {
            value = Math.Sign(value);
            if (value is 0) value = 1;
            return (int)value;
        }

        public void SetValue(float value) {
            currentValue = MathHelper.Clamp(value, minValue, maxValue);
            SetThumbLocationByValue();
            valueChangingEvent?.Invoke();
        }
        
        public void AddValue(float value) {
            SetValue(currentValue + value);
        }
        
        public void SetValueChangingEvent(Action action) {
            valueChangingEvent = action;
        }
        
        public float GetValue() {
            return currentValue;
        }

        public Image2D GetThumb() {
            return thumb;
        }
    }
}